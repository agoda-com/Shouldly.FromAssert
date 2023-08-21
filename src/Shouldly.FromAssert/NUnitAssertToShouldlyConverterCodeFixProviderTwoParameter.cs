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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NUnitAssertToShouldlyConverterCodeFixProviderTwoParameter)), Shared]
    public class NUnitAssertToShouldlyConverterCodeFixProviderTwoParameter : CodeFixProvider
    {
        private const string Title = "Convert to Shouldly";
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(NUnitToShouldlyConverterAnalyzerTwoParameter.DiagnosticId);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics.First(x => x.Id == NUnitToShouldlyConverterAnalyzerTwoParameter.DiagnosticId);
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var expression = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MemberAccessExpressionSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: Title,
                    createChangedDocument: c => ConvertToShouldlyAsync(context.Document, expression, c),
                    equivalenceKey: Title),
                diagnostic);
        }

        private async Task<Document> ConvertToShouldlyAsync(Document document, MemberAccessExpressionSyntax expression, CancellationToken cancellationToken)
        {
            if (expression.Parent is InvocationExpressionSyntax invocation &&
                invocation.Expression is MemberAccessExpressionSyntax methodAccess &&
                NUnitToShouldlyConverterAnalyzerTwoParameter.ListOfTwoParameterMethods.ContainsKey(methodAccess.Name.Identifier.ValueText) &&
                methodAccess.Expression is IdentifierNameSyntax identifierName &&
                identifierName.Identifier.ValueText == "Assert" &&
                invocation.ArgumentList.Arguments.Count == 2)
            {
                var expected = invocation.ArgumentList.Arguments[0].Expression;
                var underTest = invocation.ArgumentList.Arguments[1].Expression;
                var should = NUnitToShouldlyConverterAnalyzerTwoParameter.ListOfTwoParameterMethods[methodAccess.Name.Identifier.ValueText];
                var newExpression = SyntaxFactory.ParseExpression($"{invocation.GetLeadingTrivia().ToFullString()}{underTest}.{should}({expected})");
                var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
                var newRoot = root.ReplaceNode(expression.Parent, newExpression);
                return document.WithSyntaxRoot(newRoot);
            }

            return document;
        }
    }
}