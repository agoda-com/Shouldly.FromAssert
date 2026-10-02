using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace Shouldly.FromAssert
{
    // Assert.That shapes the single-constraint switch can't handle on its own: a trailing message,
    // a bare bool, `.And.` chains, a conditional constraint, and constraints that need the semantic model.
    public partial class NUnitToShouldlyCodeFixProvider
    {
        private SyntaxNode ConvertAssertThat(SyntaxNode root, InvocationExpressionSyntax invocation, SemanticModel semanticModel)
        {
            if (!TryParseAssertThat(invocation, semanticModel, out var actual, out var constraint, out var message))
                return null;

            // A plain Assert.That(actual, constraint) goes through the switch like every other assert.
            if (constraint != null && message == null && !IsCompound(constraint))
                return null;

            var messageExpression = message == null ? null : MessageExpression(message, semanticModel);
            var statements = ConvertConstraint(invocation, actual, constraint, messageExpression, semanticModel, invocation.SpanStart);
            return statements == null ? null : ReplaceWithStatements(root, invocation, statements);
        }

        private static bool TryParseAssertThat(
            InvocationExpressionSyntax invocation,
            SemanticModel semanticModel,
            out ArgumentSyntax actual,
            out ExpressionSyntax constraint,
            out ArgumentSyntax message)
        {
            actual = null;
            constraint = null;
            message = null;

            if (!(invocation.Expression is MemberAccessExpressionSyntax memberAccess) ||
                memberAccess.Name.Identifier.Text != "That" ||
                !(memberAccess.Expression is IdentifierNameSyntax assertClass) ||
                assertClass.Identifier.Text != "Assert")
                return false;

            var arguments = invocation.ArgumentList.Arguments;
            if (arguments.Count == 0 || arguments.Count > 3) return false;

            actual = arguments[0];
            var rest = arguments.Skip(1).ToList();

            if (rest.Count > 0 && IsMessage(rest[rest.Count - 1], semanticModel))
            {
                message = rest[rest.Count - 1];
                rest.RemoveAt(rest.Count - 1);
            }

            // Anything left over is a format argument (params object[] args), which Shouldly has no equivalent for.
            if (rest.Count > 1) return false;

            if (rest.Count == 1)
            {
                if (IsMessage(rest[0], semanticModel)) return false;
                constraint = rest[0].Expression;
                return true;
            }

            // Assert.That(bool); Assert.That(Func<bool>) has no Shouldly equivalent.
            return semanticModel.GetTypeInfo(actual.Expression).Type?.SpecialType == SpecialType.System_Boolean;
        }

        private static bool IsMessage(ArgumentSyntax argument, SemanticModel semanticModel)
        {
            var name = argument.NameColon?.Name.Identifier.Text;
            if (name == "message" || name == "getExceptionMessage") return true;

            var typeInfo = semanticModel.GetTypeInfo(argument.Expression);
            if (typeInfo.Type?.SpecialType == SpecialType.System_String) return true;

            return typeInfo.ConvertedType is INamedTypeSymbol delegateType &&
                   delegateType.Name == "Func" &&
                   delegateType.ContainingNamespace?.ToDisplayString() == "System" &&
                   delegateType.TypeArguments.Length == 1 &&
                   delegateType.TypeArguments[0].SpecialType == SpecialType.System_String;
        }

        // Shouldly 4 marks its Func<string> customMessage overloads obsolete-as-error, so a lazy NUnit message is
        // evaluated up front: `() => Describe(x)` becomes `Describe(x)` and any other Func<string> is invoked.
        private static ExpressionSyntax MessageExpression(ArgumentSyntax message, SemanticModel semanticModel)
        {
            var expression = message.Expression;
            if (semanticModel.GetTypeInfo(expression).Type?.SpecialType == SpecialType.System_String)
                return expression.WithoutTrivia();

            if (expression is ParenthesizedLambdaExpressionSyntax lambda &&
                lambda.ParameterList.Parameters.Count == 0 &&
                lambda.Body is ExpressionSyntax body)
                return body.WithoutTrivia();

            return SyntaxFactory.InvocationExpression(expression.WithoutTrivia());
        }

        private static bool IsCompound(ExpressionSyntax constraint)
        {
            constraint = Unparenthesize(constraint);
            return constraint is ConditionalExpressionSyntax || FindInnermostAnd(constraint) != null;
        }

        private List<StatementSyntax> ConvertConstraint(
            InvocationExpressionSyntax invocation,
            ArgumentSyntax actual,
            ExpressionSyntax constraint,
            ExpressionSyntax message,
            SemanticModel semanticModel,
            int position)
        {
            if (constraint == null)
            {
                // Assert.That(bool) is Assert.IsTrue(bool).
                var memberAccess = (MemberAccessExpressionSyntax) invocation.Expression;
                var isTrue = invocation
                    .WithExpression(memberAccess.WithName(SyntaxFactory.IdentifierName("IsTrue")))
                    .WithArgumentList(SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(actual)));
                var converted = ConvertLink(isTrue, message, semanticModel, position);
                return converted == null ? null : new List<StatementSyntax> { converted };
            }

            constraint = Unparenthesize(constraint);

            if (constraint is ConditionalExpressionSyntax conditional)
            {
                var whenTrue = ConvertConstraint(invocation, actual, conditional.WhenTrue, message, semanticModel, position);
                var whenFalse = ConvertConstraint(invocation, actual, conditional.WhenFalse, message, semanticModel, position);
                if (whenTrue == null || whenFalse == null) return null;

                return new List<StatementSyntax>
                {
                    SyntaxFactory.IfStatement(
                        conditional.Condition.WithoutTrivia(),
                        SyntaxFactory.Block(whenTrue),
                        SyntaxFactory.ElseClause(SyntaxFactory.Block(whenFalse)))
                };
            }

            var links = SplitAndChain(constraint);
            if (links == null) return null;

            // Shouldly's ShouldNotBeNullOrEmpty only takes a string, so on anything else the two halves stay separate asserts.
            var notNullOrEmpty = IsString(actual.Expression, semanticModel, position) &&
                                 links.Any(link => ConstraintShape(link) == "Is.Not.Null") &&
                                 links.Any(link => ConstraintShape(link) == "Is.Not.Empty");

            var statements = new List<StatementSyntax>();
            foreach (var link in links)
            {
                if (notNullOrEmpty && (ConstraintShape(link) == "Is.Not.Null" || ConstraintShape(link) == "Is.Not.Empty"))
                {
                    if (statements.Any(IsShouldNotBeNullOrEmpty)) continue;

                    var call = Should(ParenthesiseIfNeeded(actual.Expression.WithoutTrivia()), "ShouldNotBeNullOrEmpty");
                    if (message != null) call = AppendMessage(call, message, semanticModel, position);
                    statements.Add(SyntaxFactory.ExpressionStatement(call));
                    continue;
                }

                var single = invocation.WithArgumentList(
                    SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(new[] { actual, SyntaxFactory.Argument(link) })));
                var converted = ConvertLink(single, message, semanticModel, position);
                if (converted == null) return null;
                statements.Add(converted);
            }

            return statements;
        }

        private static bool IsShouldNotBeNullOrEmpty(StatementSyntax statement) =>
            statement is ExpressionStatementSyntax expressionStatement &&
            expressionStatement.Expression is InvocationExpressionSyntax invocation &&
            invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
            memberAccess.Name.Identifier.Text == "ShouldNotBeNullOrEmpty";

        private StatementSyntax ConvertLink(
            InvocationExpressionSyntax invocation,
            ExpressionSyntax message,
            SemanticModel semanticModel,
            int position)
        {
            if (!(PrepareReceiver(ConvertToShouldly(invocation, semanticModel, position), semanticModel, position) is InvocationExpressionSyntax converted))
                return null;

            if (message != null)
            {
                converted = AppendMessage(converted, message, semanticModel, position);
            }

            return SyntaxFactory.ExpressionStatement(converted.WithoutTrivia());
        }

        // Every Shouldly assertion takes the message as `customMessage`, but the string
        // overloads put `Case caseSensitivity` first, so fall back to a named argument when positional doesn't bind.
        private static InvocationExpressionSyntax AppendMessage(
            InvocationExpressionSyntax call,
            ExpressionSyntax message,
            SemanticModel semanticModel,
            int position)
        {
            // The sequence ShouldBe puts `bool ignoreOrder` before the message, so a positional message binds to the
            // object overload or to nothing at all (#31).
            if (IsSequenceShouldBe(call, semanticModel, position))
            {
                var sequence = call.AddArgumentListArguments(
                    SyntaxFactory.Argument(Literal(SyntaxKind.FalseLiteralExpression))
                        .WithNameColon(SyntaxFactory.NameColon("ignoreOrder")),
                    SyntaxFactory.Argument(message).WithNameColon(SyntaxFactory.NameColon("customMessage")));
                if (Binds(sequence, semanticModel, position)) return sequence;
            }

            var positional = call.AddArgumentListArguments(SyntaxFactory.Argument(message));
            if (Binds(positional, semanticModel, position)) return positional;

            var named = call.AddArgumentListArguments(
                SyntaxFactory.Argument(message).WithNameColon(SyntaxFactory.NameColon("customMessage")));
            return Binds(named, semanticModel, position) ? named : positional;
        }

        private static bool IsSequenceShouldBe(InvocationExpressionSyntax call, SemanticModel semanticModel, int position) =>
            call.Expression is MemberAccessExpressionSyntax memberAccess &&
            memberAccess.Name.Identifier.Text == "ShouldBe" &&
            call.ArgumentList.Arguments.Count == 1 &&
            ElementType(TypeOf(memberAccess.Expression, semanticModel, position)) != null;

        private static bool Binds(ExpressionSyntax expression, SemanticModel semanticModel, int position) =>
            semanticModel.GetSpeculativeSymbolInfo(position, expression, SpeculativeBindingOption.BindAsExpression)
                .Symbol != null;

        // `Does.Contain("a").And.Contain("b")` becomes [`Does.Contain("a")`, `Does.Contain("b")`].
        private static List<ExpressionSyntax> SplitAndChain(ExpressionSyntax constraint)
        {
            var links = new List<ExpressionSyntax>();
            var rest = constraint;

            while (true)
            {
                var and = FindInnermostAnd(rest);
                if (and == null)
                {
                    links.Add(rest);
                    return links;
                }

                var root = ChainRoot(rest);
                if (root == null) return null;

                links.Add(and.Expression);
                rest = rest.ReplaceNode(and, SyntaxFactory.IdentifierName(root.Identifier.Text));
            }
        }

        private static IdentifierNameSyntax ChainRoot(ExpressionSyntax constraint)
        {
            var node = constraint;
            while (true)
            {
                switch (node)
                {
                    case InvocationExpressionSyntax invocation:
                        node = invocation.Expression;
                        break;
                    case MemberAccessExpressionSyntax memberAccess:
                        // And binds tighter than Or, so a chain with Or can't be split into separate asserts.
                        if (memberAccess.Name.Identifier.Text == "Or") return null;
                        node = memberAccess.Expression;
                        break;
                    case IdentifierNameSyntax identifier:
                        return identifier;
                    default:
                        return null;
                }
            }
        }

        private static MemberAccessExpressionSyntax FindInnermostAnd(ExpressionSyntax constraint)
        {
            MemberAccessExpressionSyntax innermost = null;
            var node = constraint;
            while (true)
            {
                switch (node)
                {
                    case InvocationExpressionSyntax invocation:
                        node = invocation.Expression;
                        break;
                    case MemberAccessExpressionSyntax memberAccess:
                        if (memberAccess.Name.Identifier.Text == "And") innermost = memberAccess;
                        node = memberAccess.Expression;
                        break;
                    default:
                        return innermost;
                }
            }
        }

        private static ExpressionSyntax Unparenthesize(ExpressionSyntax expression)
        {
            while (expression is ParenthesizedExpressionSyntax parenthesized)
            {
                expression = parenthesized.Expression;
            }

            return expression;
        }

        private static SyntaxNode ReplaceWithStatements(SyntaxNode root, InvocationExpressionSyntax invocation, List<StatementSyntax> statements)
        {
            if (!(invocation.Parent is ExpressionStatementSyntax original))
            {
                // e.g. an expression-bodied lambda: only a single call can stand in for the invocation.
                return statements.Count == 1 && statements[0] is ExpressionStatementSyntax single
                    ? root.ReplaceNode(invocation, single.Expression.WithTriviaFrom(invocation))
                    : null;
            }

            var leading = original.GetLeadingTrivia();
            var lastEndOfLine = leading.ToList().FindLastIndex(t => t.IsKind(SyntaxKind.EndOfLineTrivia));
            var indentation = SyntaxFactory.TriviaList(leading.Skip(lastEndOfLine + 1));

            var replacements = statements
                .Select((statement, i) =>
                {
                    var placed = statement
                        .WithLeadingTrivia(i == 0 ? leading : indentation)
                        .WithTrailingTrivia(original.GetTrailingTrivia());
                    return placed is ExpressionStatementSyntax ? placed : placed.WithAdditionalAnnotations(Formatter.Annotation);
                })
                .ToList();

            if (replacements.Count == 1) return root.ReplaceNode(original, replacements[0]);

            switch (original.Parent)
            {
                case BlockSyntax _:
                case SwitchSectionSyntax _:
                    return root.ReplaceNode(original, replacements);
                case GlobalStatementSyntax _:
                    return null;
                default:
                    // An embedded statement such as `if (x) Assert.That(...)` needs braces to hold several asserts.
                    return root.ReplaceNode(
                        original,
                        SyntaxFactory.Block(replacements).WithTriviaFrom(original).WithAdditionalAnnotations(Formatter.Annotation));
            }
        }

        // Constraints the switch doesn't cover, matched on the shape of the NUnit chain, e.g. "Has.Count.EqualTo()".
        private static ExpressionSyntax ConvertAdditionalThatConstraint(
            InvocationExpressionSyntax invocation,
            SemanticModel semanticModel,
            int position)
        {
            var actual = invocation.ArgumentList.Arguments[0].Expression;
            var segments = FlattenConstraint(Unparenthesize(invocation.ArgumentList.Arguments[1].Expression));
            if (segments == null) return null;

            var shape = Shape(segments);
            var constraintArguments = segments[segments.Count - 1].Arguments?.Arguments
                                      ?? default(SeparatedSyntaxList<ArgumentSyntax>);
            var expected = constraintArguments.Count > 0 ? constraintArguments[0].Expression : null;
            var item = ItemName(expected, semanticModel, position);

            if (shape != "Does.Contain()" && shape.Replace(".Or.Contain()", "") == "Does.Contain()")
                return ContainsAny(actual, segments, semanticModel, position);

            switch (shape)
            {
                case "Is.Null":
                    return Should(actual, "ShouldBeNull");
                case "Is.Not.Null":
                    return Should(actual, "ShouldNotBeNull");
                case "Is.True":
                    return IsBoolean(actual, semanticModel, position)
                        ? Should(actual, "ShouldBeTrue")
                        : Should(actual, "ShouldBe", Literal(SyntaxKind.TrueLiteralExpression));
                case "Is.False":
                    return IsBoolean(actual, semanticModel, position)
                        ? Should(actual, "ShouldBeFalse")
                        : Should(actual, "ShouldBe", Literal(SyntaxKind.FalseLiteralExpression));
                case "Is.Not.True":
                    return Should(actual, "ShouldNotBe", Literal(SyntaxKind.TrueLiteralExpression));
                case "Is.Not.False":
                    return Should(actual, "ShouldNotBe", Literal(SyntaxKind.FalseLiteralExpression));
                case "Is.Zero":
                    return Should(actual, "ShouldBe", Zero());
                case "Is.Not.Zero":
                    return Should(actual, "ShouldNotBe", Zero());
                case "Has.Count.EqualTo()":
                    var count = CountOf(actual, semanticModel, position);
                    return count == null ? null : Should(count, "ShouldBe", expected);
                case "Does.Not.Contain()":
                    // NUnit compares strings case-sensitively; Shouldly's string overload defaults to Case.Insensitive.
                    return IsString(actual, semanticModel, position)
                        ? Should(actual, "ShouldNotContain", expected, EnumValue("Case", "Sensitive"))
                        : Should(actual, "ShouldNotContain", expected);
                case "Is.EquivalentTo()":
                    return ShouldWithArguments(
                        actual,
                        "ShouldBe",
                        SyntaxFactory.Argument(expected),
                        SyntaxFactory.Argument(Literal(SyntaxKind.TrueLiteralExpression))
                            .WithNameColon(SyntaxFactory.NameColon("ignoreOrder")));
                case "Is.All.EqualTo()":
                case "Has.All.EqualTo()":
                    return Should(actual, "ShouldAllBe", ItemLambda(item, ItemEquals(item, expected, semanticModel, position)));
                case "Is.SameAs()":
                    return Should(actual, "ShouldBeSameAs", expected);
                case "Is.Not.SameAs()":
                    return Should(actual, "ShouldNotBeSameAs", expected);
                case "Is.All.Null":
                case "Has.All.Null":
                    return Should(actual, "ShouldAllBe", ItemLambda(item, SyntaxFactory.BinaryExpression(
                        SyntaxKind.EqualsExpression, item, Literal(SyntaxKind.NullLiteralExpression))));
                case "Is.All.True":
                case "Has.All.True":
                    return Should(actual, "ShouldAllBe", ItemLambda(item, item));
                case "Is.All.False":
                case "Has.All.False":
                    return Should(actual, "ShouldAllBe", ItemLambda(
                        item, SyntaxFactory.PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, item)));
                case "Is.All.Matches()":
                case "Has.All.Matches()":
                    var allPredicate = AsPredicateExpression(item, expected);
                    return allPredicate == null ? null : Should(actual, "ShouldAllBe", allPredicate);
                case "Has.None.Matches()":
                    var nonePredicate = AsPredicateExpression(item, expected);
                    return nonePredicate == null ? null : Should(actual, "ShouldNotContain", nonePredicate);
                case "Is.InRange()" when constraintArguments.Count == 2:
                    return Should(actual, "ShouldBeInRange", expected, constraintArguments[1].Expression);
                case "Is.Ordered":
                case "Is.Ordered.Ascending":
                    return Should(actual, "ShouldBeInOrder");
                case "Is.Ordered.Descending":
                    return Should(actual, "ShouldBeInOrder", EnumValue("SortDirection", "Descending"));
                case "Does.Match()":
                    return Should(actual, "ShouldMatch", expected);
                case "Does.Not.Match()":
                    return Should(actual, "ShouldNotMatch", expected);
                // Shouldly only has IReadOnlyDictionary overloads from 4.3.0, so check the call binds.
                case "Does.ContainKey()":
                    return BindsOrNull(Should(ParenthesiseIfNeeded(actual), "ShouldContainKey", expected), semanticModel, position);
                case "Does.Not.ContainKey()":
                    return BindsOrNull(Should(ParenthesiseIfNeeded(actual), "ShouldNotContainKey", expected), semanticModel, position);
                case "Has.Some.EqualTo()":
                    return Should(actual, "ShouldContain", expected);
                case "Has.None.EqualTo()":
                    return Should(actual, "ShouldNotContain", expected);
                case "Has.Length.EqualTo()":
                    var length = PropertyOf(actual, "Length", semanticModel, position);
                    return length == null ? null : Should(length, "ShouldBe", expected);
                case "Is.SubsetOf()":
                    return Should(actual, "ShouldBeSubsetOf", expected);
                case "Is.All.Contain()":
                case "Has.All.Contain()":
                case "Is.All.Contains()":
                case "Has.All.Contains()":
                case "Is.All.StartsWith()":
                case "Has.All.StartsWith()":
                case "Is.All.StartWith()":
                case "Has.All.StartWith()":
                case "Is.All.EndsWith()":
                case "Has.All.EndsWith()":
                case "Is.All.EndWith()":
                case "Has.All.EndWith()":
                    return AnyOrAllStrings(actual, "ShouldAllBe", segments[segments.Count - 1].Name, expected, item, semanticModel, position);
                case "Has.Some.Contain()":
                case "Has.Some.Contains()":
                case "Has.Some.StartsWith()":
                case "Has.Some.StartWith()":
                case "Has.Some.EndsWith()":
                case "Has.Some.EndWith()":
                    return AnyOrAllStrings(actual, "ShouldContain", segments[segments.Count - 1].Name, expected, item, semanticModel, position);
                case "Has.None.Contain()":
                case "Has.None.Contains()":
                case "Has.None.StartsWith()":
                case "Has.None.StartWith()":
                case "Has.None.EndsWith()":
                case "Has.None.EndWith()":
                    return AnyOrAllStrings(actual, "ShouldNotContain", segments[segments.Count - 1].Name, expected, item, semanticModel, position);
                case "Is.EqualTo().Within()":
                    return EqualWithin(actual, segments[1].Arguments, expected, semanticModel, position);
                case "Is.TypeOf()":
                    return TypeAssertion(actual, "ShouldBeOfType", invocation.ArgumentList.Arguments[1].Expression);
                case "Is.Not.TypeOf()":
                    return TypeAssertion(actual, "ShouldNotBeOfType", invocation.ArgumentList.Arguments[1].Expression);
                case "Is.InstanceOf()":
                    return TypeAssertion(actual, "ShouldBeAssignableTo", invocation.ArgumentList.Arguments[1].Expression);
                case "Is.Not.InstanceOf()":
                    return TypeAssertion(actual, "ShouldNotBeAssignableTo", invocation.ArgumentList.Arguments[1].Expression);
                default:
                    return null;
            }
        }

        // Is.TypeOf<T>() and Is.TypeOf(typeof(T)) both become actual.ShouldBeOfType<T>(); likewise for the other type
        // constraints. A Type that isn't a typeof expression (e.g. a variable) is left for a hand conversion.
        private static ExpressionSyntax TypeAssertion(ExpressionSyntax actual, string method, ExpressionSyntax constraint)
        {
            if (!(Unparenthesize(constraint) is InvocationExpressionSyntax typeConstraint) ||
                !(typeConstraint.Expression is MemberAccessExpressionSyntax memberAccess))
                return null;

            TypeSyntax type;
            if (memberAccess.Name is GenericNameSyntax genericName &&
                genericName.TypeArgumentList.Arguments.Count == 1 &&
                typeConstraint.ArgumentList.Arguments.Count == 0)
                type = genericName.TypeArgumentList.Arguments[0];
            else if (memberAccess.Name is IdentifierNameSyntax &&
                     typeConstraint.ArgumentList.Arguments.Count == 1 &&
                     typeConstraint.ArgumentList.Arguments[0].Expression is TypeOfExpressionSyntax typeOf)
                type = typeOf.Type;
            else
                return null;

            return SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    actual,
                    SyntaxFactory.GenericName(SyntaxFactory.Identifier(method))
                        .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(SyntaxFactory.SingletonSeparatedList(type)))),
                SyntaxFactory.ArgumentList());
        }

        // Is.EqualTo(x).Within(tolerance) maps onto Shouldly's tolerance overloads (double, float, decimal, TimeSpan,
        // DateTime, DateTimeOffset). Those don't take nullables, so a nullable actual asserts non-null first and a
        // nullable expected is unwrapped. Anything that still doesn't bind (e.g. ints) is left to convert by hand.
        private static ExpressionSyntax EqualWithin(
            ExpressionSyntax actual,
            ArgumentListSyntax equalTo,
            ExpressionSyntax tolerance,
            SemanticModel semanticModel,
            int position)
        {
            if (equalTo.Arguments.Count != 1 || tolerance == null) return null;

            var receiver = ParenthesiseIfNeeded(actual);
            if (IsNullableValueType(actual, semanticModel, position))
                receiver = Should(receiver, "ShouldNotBeNull");

            var expected = equalTo.Arguments[0].Expression;
            if (IsNullableValueType(expected, semanticModel, position))
                expected = SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    ParenthesiseIfNeeded(expected),
                    SyntaxFactory.IdentifierName("Value"));

            return BindsOrNull(Should(receiver, "ShouldBe", expected, tolerance), semanticModel, position);
        }

        private static ExpressionSyntax BindsOrNull(ExpressionSyntax call, SemanticModel semanticModel, int position) =>
            Binds(call, semanticModel, position) ? call : null;

        // Does.Contain(a).Or.Contain(b) on a string: `new[] { a, b }.ShouldContain(item => actual.Contains(item))`.
        // string.Contains(string) is ordinal, as NUnit is. The predicate is an expression tree, so the actual value
        // must be something an expression tree can hold, and a maybe-null string is left alone rather than
        // dereferenced inside it.
        private static ExpressionSyntax ContainsAny(
            ExpressionSyntax actual,
            List<(string Name, ArgumentListSyntax Arguments)> segments,
            SemanticModel semanticModel,
            int position)
        {
            var typeInfo = semanticModel.GetSpeculativeTypeInfo(position, actual, SpeculativeBindingOption.BindAsExpression);
            if (typeInfo.Type?.SpecialType != SpecialType.System_String ||
                typeInfo.Nullability.FlowState == NullableFlowState.MaybeNull ||
                !IsExpressionTreeSafe(actual))
                return null;

            var alternatives = segments.Where(s => s.Name == "Contain").Select(s => s.Arguments).ToList();
            if (alternatives.Any(a => a.Arguments.Count != 1)) return null;

            var item = ItemName(actual, semanticModel, position);
            var array = SyntaxFactory.ImplicitArrayCreationExpression(
                SyntaxFactory.InitializerExpression(
                    SyntaxKind.ArrayInitializerExpression,
                    SyntaxFactory.SeparatedList(alternatives.Select(a => a.Arguments[0].Expression.WithoutTrivia()))));

            return Should(array, "ShouldContain", ItemLambda(item, Should(ParenthesiseIfNeeded(actual), "Contains", item)));
        }

        // Expression trees can't hold await, ?., lambdas, assignments, patterns and the like, so only plain
        // names, member and element access, calls and literals are captured into one.
        private static bool IsExpressionTreeSafe(ExpressionSyntax expression) =>
            expression != null &&
            expression.DescendantNodesAndSelf().All(node =>
                node is IdentifierNameSyntax ||
                node is GenericNameSyntax ||
                node is TypeArgumentListSyntax ||
                node is PredefinedTypeSyntax ||
                node is MemberAccessExpressionSyntax ||
                node is ElementAccessExpressionSyntax ||
                node is BracketedArgumentListSyntax ||
                node is InvocationExpressionSyntax ||
                node is ArgumentListSyntax ||
                node is ArgumentSyntax argument && argument.RefKindKeyword.IsKind(SyntaxKind.None) ||
                node is LiteralExpressionSyntax ||
                node is ThisExpressionSyntax ||
                node is ParenthesizedExpressionSyntax);

        // `Is.All.StartsWith(x)` and friends: a string predicate on each element, captured into an expression tree.
        private static bool AllStrings(ExpressionSyntax actual, ExpressionSyntax expected, SemanticModel semanticModel, int position) =>
            IsStringSequence(actual, semanticModel, position) &&
            IsExpressionTreeSafe(expected) &&
            IsString(expected, semanticModel, position);

        // Has.Some/Has.None/Has.All/Is.All over a string sequence with a substring constraint, e.g.
        // Has.None.Contains(x) -> actual.ShouldNotContain(item => item.Contains(x)). string.Contains(string) is already
        // ordinal, but string.StartsWith(string) alone is culture-sensitive (CA1310), so the comparison is spelled out.
        private static ExpressionSyntax AnyOrAllStrings(
            ExpressionSyntax actual,
            string method,
            string constraint,
            ExpressionSyntax expected,
            IdentifierNameSyntax item,
            SemanticModel semanticModel,
            int position)
        {
            if (!AllStrings(actual, expected, semanticModel, position)) return null;

            ExpressionSyntax body;
            switch (constraint)
            {
                case "Contain":
                case "Contains":
                    body = Should(item, "Contains", expected);
                    break;
                case "StartWith":
                case "StartsWith":
                    body = Should(item, "StartsWith", expected, OrdinalComparison(semanticModel, position));
                    break;
                default:
                    body = Should(item, "EndsWith", expected, OrdinalComparison(semanticModel, position));
                    break;
            }

            return Should(actual, method, ItemLambda(item, body));
        }

        // `StringComparison.Ordinal`, qualified when the file doesn't import System.
        private static ExpressionSyntax OrdinalComparison(SemanticModel semanticModel, int position)
        {
            var shortForm = EnumValue("StringComparison", "Ordinal");
            var type = semanticModel.GetSpeculativeTypeInfo(position, shortForm.Expression, SpeculativeBindingOption.BindAsTypeOrNamespace).Type;
            return type?.ToDisplayString() == "System.StringComparison"
                ? shortForm
                : SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName("System"),
                        SyntaxFactory.IdentifierName("StringComparison")),
                    SyntaxFactory.IdentifierName("Ordinal"));
        }

        // A sequence of non-nullable strings, so `item.Contains(x)` is a substring match on each, as NUnit's is.
        private static bool IsStringSequence(ExpressionSyntax expression, SemanticModel semanticModel, int position)
        {
            var element = ElementType(TypeOf(expression, semanticModel, position));
            return element?.SpecialType == SpecialType.System_String &&
                   element.NullableAnnotation != NullableAnnotation.Annotated;
        }

        // T for an IEnumerable<T> other than string; null otherwise.
        private static ITypeSymbol ElementType(ITypeSymbol type)
        {
            if (type == null || type.SpecialType == SpecialType.System_String) return null;

            var sequence = type.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T
                ? (INamedTypeSymbol) type
                : type.AllInterfaces.FirstOrDefault(i =>
                    i.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T);
            return sequence?.TypeArguments[0];
        }

        private static bool IsNullableValueType(ExpressionSyntax expression, SemanticModel semanticModel, int position) =>
            TypeOf(expression, semanticModel, position)?.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;

        // `Has.All.Matches<int>(p)` is "Has.All.Matches()".
        private static string Shape(List<(string Name, ArgumentListSyntax Arguments)> segments) =>
            string.Join(".", segments.Select(s => s.Arguments == null ? s.Name : s.Name + "()"));

        private static string ConstraintShape(ExpressionSyntax constraint)
        {
            var segments = FlattenConstraint(Unparenthesize(constraint));
            return segments == null ? null : Shape(segments);
        }

        // `Has.All.Matches<int>(p)` becomes [("Has", null), ("All", null), ("Matches", (p))].
        private static List<(string Name, ArgumentListSyntax Arguments)> FlattenConstraint(ExpressionSyntax constraint)
        {
            var segments = new List<(string Name, ArgumentListSyntax Arguments)>();
            ArgumentListSyntax pendingArguments = null;
            var node = constraint;

            while (true)
            {
                switch (node)
                {
                    case InvocationExpressionSyntax invocation when pendingArguments == null:
                        pendingArguments = invocation.ArgumentList;
                        node = invocation.Expression;
                        break;
                    case MemberAccessExpressionSyntax memberAccess:
                        segments.Add((memberAccess.Name.Identifier.Text, pendingArguments));
                        pendingArguments = null;
                        node = memberAccess.Expression;
                        break;
                    case IdentifierNameSyntax identifier when pendingArguments == null:
                        segments.Add((identifier.Identifier.Text, null));
                        segments.Reverse();
                        return segments;
                    default:
                        return null;
                }
            }
        }

        private static InvocationExpressionSyntax Should(ExpressionSyntax receiver, string method, params ExpressionSyntax[] arguments) =>
            ShouldWithArguments(receiver, method, arguments.Select(SyntaxFactory.Argument).ToArray());

        private static InvocationExpressionSyntax ShouldWithArguments(ExpressionSyntax receiver, string method, params ArgumentSyntax[] arguments) =>
            SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    receiver,
                    SyntaxFactory.IdentifierName(method)),
                SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments)));

        private static LiteralExpressionSyntax Literal(SyntaxKind kind) => SyntaxFactory.LiteralExpression(kind);

        private static LiteralExpressionSyntax Zero() =>
            SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(0));

        private static MemberAccessExpressionSyntax EnumValue(string type, string member) =>
            SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.IdentifierName(type),
                SyntaxFactory.IdentifierName(member));

        // The lambda parameter must not capture a name the expected value refers to:
        // `Has.All.EqualTo(item)` would otherwise become the tautology `item => object.Equals(item, item)`.
        private static IdentifierNameSyntax ItemName(ExpressionSyntax expected, SemanticModel semanticModel, int position)
        {
            var referenced = expected?.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>()
                                 .Select(identifier => identifier.Identifier.Text)
                                 .ToImmutableHashSet()
                             ?? ImmutableHashSet<string>.Empty;

            for (var suffix = 0; ; suffix++)
            {
                var name = suffix == 0 ? "item" : "item" + suffix;
                if (!referenced.Contains(name) && semanticModel.LookupSymbols(position, name: name).IsEmpty)
                    return SyntaxFactory.IdentifierName(name);
            }
        }

        private static SimpleLambdaExpressionSyntax ItemLambda(IdentifierNameSyntax item, ExpressionSyntax body) =>
            SyntaxFactory.SimpleLambdaExpression(SyntaxFactory.Parameter(item.Identifier), body);

        // `==` keeps NUnit's numeric equality across types (1 == 1L); anything else compares with object.Equals
        // so reference types aren't silently switched to reference equality.
        private static ExpressionSyntax ItemEquals(IdentifierNameSyntax item, ExpressionSyntax expected, SemanticModel semanticModel, int position)
        {
            var type = TypeOf(expected, semanticModel, position);
            var usesOperator = type == null ||
                               type.TypeKind == TypeKind.Enum ||
                               (type.SpecialType >= SpecialType.System_Boolean && type.SpecialType <= SpecialType.System_String);

            return usesOperator
                ? SyntaxFactory.BinaryExpression(SyntaxKind.EqualsExpression, item, expected)
                : (ExpressionSyntax) SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ObjectKeyword)),
                        SyntaxFactory.IdentifierName("Equals")),
                    SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(new[]
                    {
                        SyntaxFactory.Argument(item),
                        SyntaxFactory.Argument(expected)
                    })));
        }

        // Shouldly takes an Expression<Func<T, bool>>, so a statement-bodied lambda can't be passed through.
        private static ExpressionSyntax AsPredicateExpression(IdentifierNameSyntax item, ExpressionSyntax predicate)
        {
            switch (predicate)
            {
                case LambdaExpressionSyntax lambda:
                    return lambda.Body is ExpressionSyntax ? lambda : null;
                case null:
                    return null;
                default:
                    return ItemLambda(item, SyntaxFactory.InvocationExpression(
                        predicate,
                        SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(item)))));
            }
        }

        // Has.Count reads Count by reflection, which for an array is Length.
        private static ExpressionSyntax CountOf(ExpressionSyntax actual, SemanticModel semanticModel, int position) =>
            PropertyOf(actual, "Count", semanticModel, position) ?? PropertyOf(actual, "Length", semanticModel, position);

        private static ExpressionSyntax PropertyOf(ExpressionSyntax actual, string property, SemanticModel semanticModel, int position)
        {
            var access = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                ParenthesiseIfNeeded(actual),
                SyntaxFactory.IdentifierName(property));
            return semanticModel.GetSpeculativeSymbolInfo(position, access, SpeculativeBindingOption.BindAsExpression)
                .Symbol is IPropertySymbol
                ? access
                : null;
        }

        private static bool IsBoolean(ExpressionSyntax expression, SemanticModel semanticModel, int position) =>
            TypeOf(expression, semanticModel, position)?.SpecialType == SpecialType.System_Boolean;

        private static bool IsString(ExpressionSyntax expression, SemanticModel semanticModel, int position) =>
            TypeOf(expression, semanticModel, position)?.SpecialType == SpecialType.System_String;

        // The nodes being converted are rebuilt copies that aren't in the document's tree, so bind them speculatively.
        private static ITypeSymbol TypeOf(ExpressionSyntax expression, SemanticModel semanticModel, int position) =>
            semanticModel.GetSpeculativeTypeInfo(position, expression, SpeculativeBindingOption.BindAsExpression).Type;
    }
}
