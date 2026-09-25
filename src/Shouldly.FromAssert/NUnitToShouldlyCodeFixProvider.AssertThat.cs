using System.Collections.Generic;
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

            var statements = new List<StatementSyntax>();
            foreach (var link in links)
            {
                var single = invocation.WithArgumentList(
                    SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(new[] { actual, SyntaxFactory.Argument(link) })));
                var converted = ConvertLink(single, message, semanticModel, position);
                if (converted == null) return null;
                statements.Add(converted);
            }

            return statements;
        }

        private StatementSyntax ConvertLink(
            InvocationExpressionSyntax invocation,
            ExpressionSyntax message,
            SemanticModel semanticModel,
            int position)
        {
            if (!(ConvertToShouldly(invocation, semanticModel, position) is InvocationExpressionSyntax converted))
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
            var positional = call.AddArgumentListArguments(SyntaxFactory.Argument(message));
            if (Binds(positional, semanticModel, position)) return positional;

            var named = call.AddArgumentListArguments(
                SyntaxFactory.Argument(message).WithNameColon(SyntaxFactory.NameColon("customMessage")));
            return Binds(named, semanticModel, position) ? named : positional;
        }

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
                var root = ChainRoot(rest);
                if (root == null) return null;

                var and = FindInnermostAnd(rest);
                if (and == null)
                {
                    links.Add(rest);
                    return links;
                }

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
            var segments = FlattenConstraint(invocation.ArgumentList.Arguments[1].Expression);
            if (segments == null) return null;

            var shape = string.Join(".", segments.Select(s => s.Arguments == null ? s.Name : s.Name + "()"));
            var constraintArguments = segments[segments.Count - 1].Arguments?.Arguments
                                      ?? default(SeparatedSyntaxList<ArgumentSyntax>);
            var expected = constraintArguments.Count > 0 ? constraintArguments[0].Expression : null;

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
                    return Should(actual, "ShouldAllBe", ItemLambda(ItemEquals(expected, semanticModel, position)));
                case "Is.SameAs()":
                    return Should(actual, "ShouldBeSameAs", expected);
                case "Is.Not.SameAs()":
                    return Should(actual, "ShouldNotBeSameAs", expected);
                case "Is.All.Null":
                case "Has.All.Null":
                    return Should(actual, "ShouldAllBe", ItemLambda(SyntaxFactory.BinaryExpression(
                        SyntaxKind.EqualsExpression, Item(), Literal(SyntaxKind.NullLiteralExpression))));
                case "Is.All.True":
                case "Has.All.True":
                    return Should(actual, "ShouldAllBe", ItemLambda(Item()));
                case "Is.All.False":
                case "Has.All.False":
                    return Should(actual, "ShouldAllBe", ItemLambda(
                        SyntaxFactory.PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, Item())));
                case "Is.All.Matches()":
                case "Has.All.Matches()":
                    var allPredicate = AsPredicateExpression(expected);
                    return allPredicate == null ? null : Should(actual, "ShouldAllBe", allPredicate);
                case "Has.None.Matches()":
                    var nonePredicate = AsPredicateExpression(expected);
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
                default:
                    return null;
            }
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

        private static IdentifierNameSyntax Item() => SyntaxFactory.IdentifierName("item");

        private static SimpleLambdaExpressionSyntax ItemLambda(ExpressionSyntax body) =>
            SyntaxFactory.SimpleLambdaExpression(SyntaxFactory.Parameter(SyntaxFactory.Identifier("item")), body);

        // `==` keeps NUnit's numeric equality across types (1 == 1L); anything else compares with object.Equals
        // so reference types aren't silently switched to reference equality.
        private static ExpressionSyntax ItemEquals(ExpressionSyntax expected, SemanticModel semanticModel, int position)
        {
            var type = TypeOf(expected, semanticModel, position);
            var usesOperator = type == null ||
                               type.TypeKind == TypeKind.Enum ||
                               (type.SpecialType >= SpecialType.System_Boolean && type.SpecialType <= SpecialType.System_String);

            return usesOperator
                ? SyntaxFactory.BinaryExpression(SyntaxKind.EqualsExpression, Item(), expected)
                : (ExpressionSyntax) SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ObjectKeyword)),
                        SyntaxFactory.IdentifierName("Equals")),
                    SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(new[]
                    {
                        SyntaxFactory.Argument(Item()),
                        SyntaxFactory.Argument(expected)
                    })));
        }

        // Shouldly takes an Expression<Func<T, bool>>, so a statement-bodied lambda can't be passed through.
        private static ExpressionSyntax AsPredicateExpression(ExpressionSyntax predicate)
        {
            switch (predicate)
            {
                case LambdaExpressionSyntax lambda:
                    return lambda.Body is ExpressionSyntax ? lambda : null;
                case null:
                    return null;
                default:
                    return ItemLambda(SyntaxFactory.InvocationExpression(
                        predicate,
                        SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(Item())))));
            }
        }

        // Has.Count reads Count by reflection, which for an array is Length.
        private static ExpressionSyntax CountOf(ExpressionSyntax actual, SemanticModel semanticModel, int position)
        {
            foreach (var property in new[] { "Count", "Length" })
            {
                var access = SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    actual,
                    SyntaxFactory.IdentifierName(property));
                var symbol = semanticModel
                    .GetSpeculativeSymbolInfo(position, access, SpeculativeBindingOption.BindAsExpression)
                    .Symbol;
                if (symbol is IPropertySymbol) return access;
            }

            return null;
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
