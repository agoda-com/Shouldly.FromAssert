using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Shouldly.FromAssert
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NUnitToShouldlyCodeFixProvider)), Shared]
    public class NUnitToShouldlyCodeFixProvider : CodeFixProvider
    {
        private const string Title = "Convert to Shouldly";

        public sealed override ImmutableArray<string> FixableDiagnosticIds =>
            ImmutableArray.Create(NUnitToShouldlyAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                if (diagnostic.Id != NUnitToShouldlyAnalyzer.DiagnosticId) continue;

                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: Title,
                        createChangedDocument: c => ConvertToShouldlyAsync(context.Document, diagnostic, c),
                        equivalenceKey: Title),
                    diagnostic);
            }
        }


        private async Task<Document> ConvertToShouldlyAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var invocation = root.FindNode(diagnosticSpan) as InvocationExpressionSyntax;

            if (invocation == null) return document;

            var newInvocation = ConvertToShouldly(invocation);

            if (newInvocation != null)
            {
                root = root.ReplaceNode(invocation, newInvocation);
                return document.WithSyntaxRoot(root);
            }

            return document;
        }

        private ExpressionSyntax ConvertToShouldly(InvocationExpressionSyntax invocation)
        {
            string methodName = null;
            if (invocation.Expression is MemberAccessExpressionSyntax memberAccessExpSyn)
            {
                methodName = memberAccessExpSyn.Name.Identifier.Text;
            }
            else if (invocation.Expression is IdentifierNameSyntax identifier)
            {
                methodName = identifier.Identifier.Text;
            }

            if (methodName == null) return null;

            string assertClass = null;
            if (invocation.Expression is MemberAccessExpressionSyntax memberAccessForClass)
            {
                if (memberAccessForClass.Expression is IdentifierNameSyntax identifierForClass)
                {
                    assertClass = identifierForClass.Identifier.Text;
                }
            }

            var arguments = invocation.ArgumentList.Arguments;

            switch (methodName)
            {
                case "DoesNotContain" when assertClass == "CollectionAssert":
                    return SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                arguments[0].Expression,
                                SyntaxFactory.IdentifierName("ShouldNotContain")),
                            SyntaxFactory.ArgumentList(
                                SyntaxFactory.SingletonSeparatedList(
                                    SyntaxFactory.Argument(arguments[1].Expression)
                                )
                            )
                        )
                        .WithLeadingTrivia(invocation.GetLeadingTrivia());

                case "AreEqual" when assertClass == "CollectionAssert":
                    return SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                arguments[1].Expression,
                                SyntaxFactory.IdentifierName("ShouldBe")),
                            SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(arguments[0])))
                        .WithLeadingTrivia(invocation.GetLeadingTrivia());

                case "AreEquivalent" when assertClass == "CollectionAssert":
                    return SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                arguments[1].Expression,
                                SyntaxFactory.IdentifierName("ShouldBe")),
                            SyntaxFactory.ArgumentList(
                                SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                    new SyntaxNodeOrToken[]
                                    {
                                        arguments[0],
                                        SyntaxFactory.Token(SyntaxKind.CommaToken),
                                        SyntaxFactory.Argument(
                                            SyntaxFactory.NameColon("ignoreOrder"),
                                            SyntaxFactory.Token(SyntaxKind.None),
                                            SyntaxFactory.LiteralExpression(
                                                SyntaxKind.TrueLiteralExpression))
                                    })))
                        .WithLeadingTrivia(invocation.GetLeadingTrivia());

                case "AllItemsAreInstancesOfType" when assertClass == "CollectionAssert":
                    return SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                arguments[0].Expression,
                                SyntaxFactory.IdentifierName("ShouldAllBe")),
                            SyntaxFactory.ArgumentList(
                                SyntaxFactory.SingletonSeparatedList(
                                    SyntaxFactory.Argument(
                                        SyntaxFactory.SimpleLambdaExpression(
                                            SyntaxFactory.Parameter(
                                                SyntaxFactory.Identifier("item")),
                                            SyntaxFactory.BinaryExpression(
                                                SyntaxKind.IsExpression,
                                                SyntaxFactory.IdentifierName("item"),
                                                ((TypeOfExpressionSyntax) arguments[1].Expression).Type))))))
                        .WithLeadingTrivia(invocation.GetLeadingTrivia())
                        .WithTrailingTrivia(invocation.GetTrailingTrivia());

                case "AllItemsAreNotNull" when assertClass == "CollectionAssert":
                    return SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                arguments[0].Expression,
                                SyntaxFactory.IdentifierName("ShouldNotContain")),
                            SyntaxFactory.ArgumentList(
                                SyntaxFactory.SingletonSeparatedList(
                                    SyntaxFactory.Argument(
                                        SyntaxFactory.SimpleLambdaExpression(
                                            SyntaxFactory.Parameter(
                                                SyntaxFactory.Identifier("item")),
                                            SyntaxFactory.BinaryExpression(
                                                SyntaxKind.EqualsExpression,
                                                SyntaxFactory.IdentifierName("item"),
                                                SyntaxFactory.LiteralExpression(
                                                    SyntaxKind.NullLiteralExpression)))))))
                        .WithLeadingTrivia(invocation.GetLeadingTrivia());

                case "AllItemsAreUnique" when assertClass == "CollectionAssert":
                    return SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                arguments[0].Expression,
                                SyntaxFactory.IdentifierName("ShouldBeUnique")),
                            SyntaxFactory.ArgumentList())
                        .WithLeadingTrivia(invocation.GetLeadingTrivia());

                case "That" when assertClass == "Assert" &&
                                 arguments.Count == 2 &&
                                 arguments[1].Expression is InvocationExpressionSyntax inv &&
                                 inv.Expression is MemberAccessExpressionSyntax ma &&
                                 ma.Name.Identifier.Text == "EqualTo" &&
                                 ma.Expression is IdentifierNameSyntax ins &&
                                 ins.Identifier.Text == "Is":
                    return SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                arguments[0].Expression,
                                SyntaxFactory.IdentifierName("ShouldBe")),
                            SyntaxFactory.ArgumentList(
                                SyntaxFactory.SingletonSeparatedList(
                                    inv.ArgumentList.Arguments[0])))
                        .WithLeadingTrivia(invocation.GetLeadingTrivia());

                case "That" when assertClass == "Assert" &&
                                 arguments.Count == 2 &&
                                 arguments[1].Expression is InvocationExpressionSyntax inv &&
                                 inv.Expression is MemberAccessExpressionSyntax ma &&
                                 ma.Name.Identifier.Text == "EqualTo" &&
                                 ma.Expression is MemberAccessExpressionSyntax innerMa &&
                                 innerMa.Name.Identifier.Text == "Not" &&
                                 innerMa.Expression is IdentifierNameSyntax ins &&
                                 ins.Identifier.Text == "Is":
                    return SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                arguments[0].Expression,
                                SyntaxFactory.IdentifierName("ShouldNotBe")),
                            SyntaxFactory.ArgumentList(
                                SyntaxFactory.SingletonSeparatedList(
                                    inv.ArgumentList.Arguments[0])))
                        .WithLeadingTrivia(invocation.GetLeadingTrivia());

                case "IsNotNull" when assertClass == "Assert":
                    return SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                arguments[0].Expression,
                                SyntaxFactory.IdentifierName("ShouldNotBeNull")),
                            SyntaxFactory.ArgumentList())
                        .WithLeadingTrivia(invocation.GetLeadingTrivia());

                case "IsNull" when assertClass == "Assert":
                    return SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                arguments[0].Expression,
                                SyntaxFactory.IdentifierName("ShouldBeNull")),
                            SyntaxFactory.ArgumentList())
                        .WithLeadingTrivia(invocation.GetLeadingTrivia());

                case "IsTrue" when assertClass == "Assert":
                    if (arguments[0].Expression is BinaryExpressionSyntax binaryExpressionTrue)
                    {
                        return SyntaxFactory.InvocationExpression(
                                SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    SyntaxFactory.ParenthesizedExpression(binaryExpressionTrue),
                                    SyntaxFactory.IdentifierName("ShouldBeTrue")),
                                SyntaxFactory.ArgumentList())
                            .WithLeadingTrivia(invocation.GetLeadingTrivia());
                    }

                    return SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                arguments[0].Expression,
                                SyntaxFactory.IdentifierName("ShouldBeTrue")),
                            SyntaxFactory.ArgumentList())
                        .WithLeadingTrivia(invocation.GetLeadingTrivia());

                case "IsFalse" when assertClass == "Assert":
                    if (arguments[0].Expression is BinaryExpressionSyntax binaryExpression)
                    {
                        return SyntaxFactory.InvocationExpression(
                                SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    SyntaxFactory.ParenthesizedExpression(binaryExpression),
                                    SyntaxFactory.IdentifierName("ShouldBeFalse")),
                                SyntaxFactory.ArgumentList())
                            .WithLeadingTrivia(invocation.GetLeadingTrivia());
                    }

                    return SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                arguments[0].Expression,
                                SyntaxFactory.IdentifierName("ShouldBeFalse")),
                            SyntaxFactory.ArgumentList())
                        .WithLeadingTrivia(invocation.GetLeadingTrivia());
                case "AreSame":
                    return SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                arguments[1].Expression,
                                SyntaxFactory.IdentifierName("ShouldBeSameAs")),
                            SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(arguments[0])))
                        .WithLeadingTrivia(invocation.GetLeadingTrivia());

                case "AreNotSame":
                    return SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                arguments[1].Expression,
                                SyntaxFactory.IdentifierName("ShouldNotBeSameAs")),
                            SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(arguments[0])))
                        .WithLeadingTrivia(invocation.GetLeadingTrivia());

                case "IsInstanceOf" when assertClass == "Assert":
                    if (invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
                        memberAccess.Name is GenericNameSyntax genericName)
                    {
                        return SyntaxFactory.InvocationExpression(
                                SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    arguments[0].Expression,
                                    SyntaxFactory.GenericName(
                                            SyntaxFactory.Identifier("ShouldBeOfType"))
                                        .WithTypeArgumentList(genericName.TypeArgumentList)),
                                SyntaxFactory.ArgumentList())
                            .WithLeadingTrivia(invocation.GetLeadingTrivia());
                    }

                    break;
                case "That" when arguments.Count == 2 && arguments[1].Expression is InvocationExpressionSyntax inv &&
                                 inv.Expression is MemberAccessExpressionSyntax ma &&
                                 ma.Name.Identifier.Text == "TypeOf":
                    return SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                arguments[0].Expression,
                                SyntaxFactory.GenericName(SyntaxFactory.Identifier("ShouldBeOfType"))
                                    .WithTypeArgumentList(
                                        SyntaxFactory.TypeArgumentList(
                                            SyntaxFactory.SingletonSeparatedList(
                                                ((TypeOfExpressionSyntax) inv.ArgumentList.Arguments[0].Expression)
                                                .Type)))),
                            SyntaxFactory.ArgumentList())
                        .WithLeadingTrivia(invocation.GetLeadingTrivia());
                case "IsNotInstanceOf" when assertClass == "Assert":
                    if (invocation.Expression is MemberAccessExpressionSyntax memberAccess1 &&
                        memberAccess1.Name is GenericNameSyntax genericName1)
                    {
                        return SyntaxFactory.InvocationExpression(
                                SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    arguments[0].Expression,
                                    SyntaxFactory.GenericName(
                                            SyntaxFactory.Identifier("ShouldNotBeOfType"))
                                        .WithTypeArgumentList(genericName1.TypeArgumentList)),
                                SyntaxFactory.ArgumentList())
                            .WithLeadingTrivia(invocation.GetLeadingTrivia());
                    }

                    break;

                case "That" when arguments.Count == 2 && arguments[1].Expression is InvocationExpressionSyntax inv &&
                                 inv.Expression is MemberAccessExpressionSyntax ma &&
                                 ma.Name.Identifier.Text == "Not" &&
                                 ma.Expression is MemberAccessExpressionSyntax innerMa &&
                                 innerMa.Name.Identifier.Text == "TypeOf":
                    if (inv.ArgumentList.Arguments.Count > 0 &&
                        inv.ArgumentList.Arguments[0].Expression is TypeOfExpressionSyntax typeOfExpr)
                    {
                        return SyntaxFactory.InvocationExpression(
                                SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    arguments[0].Expression,
                                    SyntaxFactory.GenericName(
                                            SyntaxFactory.Identifier("ShouldNotBeOfType"))
                                        .WithTypeArgumentList(
                                            SyntaxFactory.TypeArgumentList(
                                                SyntaxFactory.SingletonSeparatedList(typeOfExpr.Type)))),
                                SyntaxFactory.ArgumentList())
                            .WithLeadingTrivia(invocation.GetLeadingTrivia());
                    }

                    break;
                case "Contains" when assertClass == "StringAssert":
                case "That" when arguments.Count == 2 && arguments[1].Expression is InvocationExpressionSyntax inv &&
                                 inv.Expression is MemberAccessExpressionSyntax ma &&
                                 ma.Name.Identifier.Text == "Contains":
                    return SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                arguments[1].Expression,
                                SyntaxFactory.IdentifierName("ShouldContain")),
                            SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(arguments[0])))
                        .WithLeadingTrivia(invocation.GetLeadingTrivia());

                case "Contains":
                case "That" when arguments.Count == 2 && arguments[1].Expression is InvocationExpressionSyntax inv &&
                                 inv.Expression is MemberAccessExpressionSyntax ma &&
                                 ma.Name.Identifier.Text == "Contains":
                    return SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                arguments[1].Expression,
                                SyntaxFactory.IdentifierName("ShouldContain")),
                            SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(arguments[0])))
                        .WithLeadingTrivia(invocation.GetLeadingTrivia());

                case "DoesNotContain":
                case "That" when arguments.Count == 2 && arguments[1].Expression is InvocationExpressionSyntax inv &&
                                 inv.Expression is MemberAccessExpressionSyntax ma &&
                                 ma.Name.Identifier.Text == "Not" &&
                                 ma.Expression is MemberAccessExpressionSyntax innerMa &&
                                 innerMa.Name.Identifier.Text == "Contains":
                    return SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                arguments[1].Expression,
                                SyntaxFactory.IdentifierName("ShouldNotContain")),
                            SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(arguments[0])))
                        .WithLeadingTrivia(invocation.GetLeadingTrivia());
                case "That" when assertClass == "Assert" &&
                                 arguments.Count == 2 &&
                                 arguments[1].Expression is InvocationExpressionSyntax inv &&
                                 inv.Expression is MemberAccessExpressionSyntax ma &&
                                 ma.Name.Identifier.Text == "Contain" &&
                                 ma.Expression is IdentifierNameSyntax ins &&
                                 ins.Identifier.Text == "Does":
                    return SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                arguments[0].Expression,
                                SyntaxFactory.IdentifierName("ShouldContain")),
                            SyntaxFactory.ArgumentList(
                                SyntaxFactory.SingletonSeparatedList(
                                    inv.ArgumentList.Arguments[0])))
                        .WithLeadingTrivia(invocation.GetLeadingTrivia());
                case "That" when assertClass == "Assert" &&
                                 arguments.Count == 2 &&
                                 arguments[1].Expression is InvocationExpressionSyntax inv &&
                                 inv.Expression is MemberAccessExpressionSyntax ma &&
                                 ma.Name.Identifier.Text == "EndWith" &&
                                 ma.Expression is IdentifierNameSyntax ins &&
                                 ins.Identifier.Text == "Does":
                    return SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                arguments[0].Expression,
                                SyntaxFactory.IdentifierName("ShouldEndWith")),
                            SyntaxFactory.ArgumentList(
                                SyntaxFactory.SingletonSeparatedList(
                                    inv.ArgumentList.Arguments[0])))
                        .WithLeadingTrivia(invocation.GetLeadingTrivia());
                case "That" when assertClass == "Assert" &&
                                 arguments.Count == 2 &&
                                 arguments[1].Expression is InvocationExpressionSyntax inv &&
                                 inv.Expression is MemberAccessExpressionSyntax ma &&
                                 ma.Name.Identifier.Text == "StartWith" &&
                                 ma.Expression is IdentifierNameSyntax ins &&
                                 ins.Identifier.Text == "Does":
                    return SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                arguments[0].Expression,
                                SyntaxFactory.IdentifierName("ShouldStartWith")),
                            SyntaxFactory.ArgumentList(
                                SyntaxFactory.SingletonSeparatedList(
                                    inv.ArgumentList.Arguments[0])))
                        .WithLeadingTrivia(invocation.GetLeadingTrivia());
                case "IsEmpty":
                case "That" when arguments.Count == 2 && arguments[1].Expression is MemberAccessExpressionSyntax ma &&
                                 ma.Name.Identifier.Text == "Empty":
                    return SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                arguments[0].Expression,
                                SyntaxFactory.IdentifierName("ShouldBeEmpty")),
                            SyntaxFactory.ArgumentList())
                        .WithLeadingTrivia(invocation.GetLeadingTrivia());

                case "IsNotEmpty":
                    return SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                arguments[0].Expression,
                                SyntaxFactory.IdentifierName("ShouldNotBeEmpty")),
                            SyntaxFactory.ArgumentList())
                        .WithLeadingTrivia(invocation.GetLeadingTrivia());
                // For Has.Member
                case "That" when assertClass == "Assert" &&
                                 arguments.Count == 2 &&
                                 arguments[1].Expression is InvocationExpressionSyntax inv &&
                                 inv.Expression is MemberAccessExpressionSyntax ma &&
                                 ma.Name.Identifier.Text == "Member" &&
                                 ma.Expression is IdentifierNameSyntax ins &&
                                 ins.Identifier.Text == "Has":
                    return SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                arguments[0].Expression,
                                SyntaxFactory.IdentifierName("ShouldContain")),
                            SyntaxFactory.ArgumentList(
                                SyntaxFactory.SingletonSeparatedList(
                                    inv.ArgumentList.Arguments[0])))
                        .WithLeadingTrivia(invocation.GetLeadingTrivia());

                // For Has.No.Member
                case "That" when assertClass == "Assert" &&
                                 arguments.Count == 2 &&
                                 arguments[1].Expression is InvocationExpressionSyntax inv &&
                                 inv.Expression is MemberAccessExpressionSyntax ma &&
                                 ma.Name.Identifier.Text == "Member" &&
                                 ma.Expression is MemberAccessExpressionSyntax innerMa &&
                                 innerMa.Name.Identifier.Text == "No" &&
                                 innerMa.Expression is IdentifierNameSyntax ins &&
                                 ins.Identifier.Text == "Has":
                    return SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                arguments[0].Expression,
                                SyntaxFactory.IdentifierName("ShouldNotContain")),
                            SyntaxFactory.ArgumentList(
                                SyntaxFactory.SingletonSeparatedList(
                                    inv.ArgumentList.Arguments[0])))
                        .WithLeadingTrivia(invocation.GetLeadingTrivia());
                case "Greater":
                case "That" when arguments.Count == 2 && arguments[1].Expression is InvocationExpressionSyntax inv &&
                                 inv.Expression is MemberAccessExpressionSyntax ma &&
                                 ma.Name.Identifier.Text == "GreaterThan":
                    return SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                arguments[0].Expression,
                                SyntaxFactory.IdentifierName("ShouldBeGreaterThan")),
                            SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(arguments[1])))
                        .WithLeadingTrivia(invocation.GetLeadingTrivia());

                case "GreaterOrEqual":
                case "That" when arguments.Count == 2 && arguments[1].Expression is InvocationExpressionSyntax inv &&
                                 inv.Expression is MemberAccessExpressionSyntax ma &&
                                 ma.Name.Identifier.Text == "GreaterThanOrEqualTo":
                    return SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                arguments[0].Expression,
                                SyntaxFactory.IdentifierName("ShouldBeGreaterThanOrEqualTo")),
                            SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(arguments[1])))
                        .WithLeadingTrivia(invocation.GetLeadingTrivia());

                case "Less":
                case "That" when arguments.Count == 2 && arguments[1].Expression is InvocationExpressionSyntax inv &&
                                 inv.Expression is MemberAccessExpressionSyntax ma &&
                                 ma.Name.Identifier.Text == "LessThan":
                    return SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                arguments[0].Expression,
                                SyntaxFactory.IdentifierName("ShouldBeLessThan")),
                            SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(arguments[1])))
                        .WithLeadingTrivia(invocation.GetLeadingTrivia());

                case "LessOrEqual":
                case "That" when arguments.Count == 2 && arguments[1].Expression is InvocationExpressionSyntax inv &&
                                 inv.Expression is MemberAccessExpressionSyntax ma &&
                                 ma.Name.Identifier.Text == "LessThanOrEqualTo":
                    return SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                arguments[0].Expression,
                                SyntaxFactory.IdentifierName("ShouldBeLessThanOrEqualTo")),
                            SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(arguments[1])))
                        .WithLeadingTrivia(invocation.GetLeadingTrivia());

                case "IsNaN" when assertClass == "Assert":
                    return SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                SyntaxFactory.InvocationExpression(
                                        SyntaxFactory.MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            SyntaxFactory.PredefinedType(
                                                SyntaxFactory.Token(SyntaxKind.DoubleKeyword)),
                                            SyntaxFactory.IdentifierName("IsNaN")))
                                    .WithArgumentList(
                                        SyntaxFactory.ArgumentList(
                                            SyntaxFactory.SingletonSeparatedList(arguments[0]))),
                                SyntaxFactory.IdentifierName("ShouldBeTrue")),
                            SyntaxFactory.ArgumentList())
                        .WithLeadingTrivia(invocation.GetLeadingTrivia());

                case "StartsWith" when assertClass == "StringAssert":
                case "That" when arguments.Count == 2 && arguments[1].Expression is InvocationExpressionSyntax inv &&
                                 inv.Expression is MemberAccessExpressionSyntax ma &&
                                 ma.Name.Identifier.Text == "StartsWith":
                    return SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                arguments[1].Expression,
                                SyntaxFactory.IdentifierName("ShouldStartWith")),
                            SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(arguments[0])))
                        .WithLeadingTrivia(invocation.GetLeadingTrivia());

                case "EndsWith" when assertClass == "StringAssert":
                case "That" when arguments.Count == 2 && arguments[1].Expression is InvocationExpressionSyntax inv &&
                                 inv.Expression is MemberAccessExpressionSyntax ma &&
                                 ma.Name.Identifier.Text == "EndsWith":
                    return SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                arguments[1].Expression,
                                SyntaxFactory.IdentifierName("ShouldEndWith")),
                            SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(arguments[0])))
                        .WithLeadingTrivia(invocation.GetLeadingTrivia());


                case "Throws":
                    return SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                SyntaxFactory.IdentifierName("Should"),
                                SyntaxFactory.GenericName(
                                        SyntaxFactory.Identifier("Throw"))
                                    .WithTypeArgumentList(
                                        SyntaxFactory.TypeArgumentList(
                                            SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                SyntaxFactory.IdentifierName(
                                                    ((GenericNameSyntax) ((MemberAccessExpressionSyntax) invocation
                                                        .Expression).Name).TypeArgumentList.Arguments.First()
                                                    .ToFullString()))))),
                            SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(arguments[0])))
                        .WithLeadingTrivia(invocation.GetLeadingTrivia())
                        .WithTrailingTrivia(invocation.GetTrailingTrivia());

                case "DoesNotThrow":
                    return SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                SyntaxFactory.IdentifierName("Should"),
                                SyntaxFactory.IdentifierName("NotThrow")),
                            SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(arguments[0])))
                        .WithLeadingTrivia(invocation.GetLeadingTrivia())
                        .WithTrailingTrivia(invocation.GetTrailingTrivia());

                case "AreEqual" when assertClass == "Assert":
                    return SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                arguments[1].Expression,
                                SyntaxFactory.IdentifierName("ShouldBe")),
                            SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(arguments[0])))
                        .WithLeadingTrivia(invocation.GetLeadingTrivia());

                case "AreNotEqual" when assertClass == "Assert":
                    return SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                arguments[1].Expression,
                                SyntaxFactory.IdentifierName("ShouldNotBe")),
                            SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(arguments[0])))
                        .WithLeadingTrivia(invocation.GetLeadingTrivia());

                case "That" when arguments.Count == 2 && arguments[1].Expression is MemberAccessExpressionSyntax ma &&
                                 ma.Name.Identifier.Text == "Unique":
                    return SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                arguments[0].Expression,
                                SyntaxFactory.IdentifierName("ShouldBeUnique")),
                            SyntaxFactory.ArgumentList())
                        .WithLeadingTrivia(invocation.GetLeadingTrivia());

                case "That" when arguments.Count == 2 && arguments[1].Expression is InvocationExpressionSyntax inv &&
                                 inv.Expression is MemberAccessExpressionSyntax ma &&
                                 ma.Name.Identifier.Text == "StringContaining":
                    return SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                arguments[0].Expression,
                                SyntaxFactory.IdentifierName("ShouldContain")),
                            SyntaxFactory.ArgumentList(
                                SyntaxFactory.SingletonSeparatedList(inv.ArgumentList.Arguments[0])))
                        .WithLeadingTrivia(invocation.GetLeadingTrivia());

                case "That" when arguments.Count == 2 && arguments[1].Expression is InvocationExpressionSyntax inv &&
                                 inv.Expression is MemberAccessExpressionSyntax ma &&
                                 ma.Name.Identifier.Text == "StringStarting":
                    return SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            arguments[0].Expression,
                            SyntaxFactory.IdentifierName("ShouldStartWith")),
                        SyntaxFactory.ArgumentList(
                            SyntaxFactory.SingletonSeparatedList(inv.ArgumentList.Arguments[0])));

                case "That" when arguments.Count == 2 && arguments[1].Expression is InvocationExpressionSyntax inv &&
                                 inv.Expression is MemberAccessExpressionSyntax ma &&
                                 ma.Name.Identifier.Text == "StringEnding":
                    return SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            arguments[0].Expression,
                            SyntaxFactory.IdentifierName("ShouldEndWith")),
                        SyntaxFactory.ArgumentList(
                            SyntaxFactory.SingletonSeparatedList(inv.ArgumentList.Arguments[0])));

                default:
                    return null;
            }

            return null;
        }
    }
}