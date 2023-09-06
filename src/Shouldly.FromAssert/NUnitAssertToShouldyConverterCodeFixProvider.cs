using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace Shouldly.FromAssert
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NUnitAssertToShouldlyConverterCodeFixProvider)), Shared]
    public class NUnitAssertToShouldlyConverterCodeFixProvider : CodeFixProvider
    {
        private const string Title = "Convert to Shouldly";
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(NUnitToShouldlyConverterAnalyzer.DiagnosticId);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics.First(x => x.Id == NUnitToShouldlyConverterAnalyzer.DiagnosticId);
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: Title,
                    createChangedDocument: c => ConvertToShouldlyAsync(context, diagnostic, c),
                    equivalenceKey: Title),
                diagnostic);
        }

        private async Task<Document> ConvertToShouldlyAsync(CodeFixContext context, Diagnostic diagnostic,
            CancellationToken cancellationToken)
        {
            var editor = await DocumentEditor.CreateAsync(context.Document, cancellationToken);
            var shouldyNamespace = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Shouldly"));

            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var expression = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MemberAccessExpressionSyntax>().First();

            if (expression.Parent is InvocationExpressionSyntax invocation &&
                invocation.Expression is MemberAccessExpressionSyntax methodAccess &&
                methodAccess.Name.Identifier.ValueText == "That" &&
                methodAccess.Expression is IdentifierNameSyntax identifierName &&
                identifierName.Identifier.ValueText == "Assert" &&
                invocation.ArgumentList.Arguments.Count == 2)
            {
                var expectedValue = ((InvocationExpressionSyntax)invocation.ArgumentList.Arguments[1].Expression).ArgumentList.Arguments[0];
                var underTest = invocation.ArgumentList.Arguments[0].Expression;

                var newExpression = SyntaxFactory.ParseExpression($"{invocation.GetLeadingTrivia().ToFullString()}{underTest}.ShouldBe({expectedValue})");

                editor.ReplaceNode(expression.Parent, newExpression);
                editor.AddUsingDirective(shouldyNamespace);

                return editor.GetChangedDocument();
            }

            if (expression.Parent is InvocationExpressionSyntax invocation2 &&
                invocation2.Expression is MemberAccessExpressionSyntax methodAccess2 &&
                NUnitToShouldlyConverterAnalyzer.ListOfTwoParameterMethods.ContainsKey(methodAccess2.Name.Identifier.ValueText) &&
                methodAccess2.Expression is IdentifierNameSyntax identifierName2 &&
                identifierName2.Identifier.ValueText == "Assert" &&
                invocation2.ArgumentList.Arguments.Count == 2)
            {
                var expected = invocation2.ArgumentList.Arguments[0].Expression;
                var underTest = invocation2.ArgumentList.Arguments[1].Expression;
                var should = NUnitToShouldlyConverterAnalyzer.ListOfTwoParameterMethods[methodAccess2.Name.Identifier.ValueText];
                var newExpression = SyntaxFactory.ParseExpression($"{invocation2.GetLeadingTrivia().ToFullString()}{underTest}.{should}({expected})");

                editor.ReplaceNode(expression.Parent, newExpression);
                editor.AddUsingDirective(shouldyNamespace);
                
                return editor.GetChangedDocument();
            }

            if (expression.Parent is InvocationExpressionSyntax invocation1 &&
                invocation1.Expression is MemberAccessExpressionSyntax methodAccess1 &&
                NUnitToShouldlyConverterAnalyzer.ListOfSingleParameterMethods.ContainsKey(methodAccess1.Name.Identifier.ValueText) &&
                methodAccess1.Expression is IdentifierNameSyntax identifierName1 &&
                identifierName1.Identifier.ValueText == "Assert" &&
                invocation1.ArgumentList.Arguments.Count == 1)
            {
                var underTest = invocation1.ArgumentList.Arguments[0].Expression;
                var should = NUnitToShouldlyConverterAnalyzer.ListOfSingleParameterMethods[methodAccess1.Name.Identifier.ValueText];
                var newExpression = SyntaxFactory.ParseExpression($"{invocation1.GetLeadingTrivia().ToFullString()}{underTest}.{should}()");

                editor.ReplaceNode(expression.Parent, newExpression);
                editor.AddUsingDirective(shouldyNamespace);

                return editor.GetChangedDocument();
            }

            if (expression.Parent is InvocationExpressionSyntax invocationT &&
                invocationT.Expression is MemberAccessExpressionSyntax methodAccessT &&
                NUnitToShouldlyConverterAnalyzer.ListOfThrowsMethods.ContainsKey(methodAccessT.Name.Identifier.ValueText) &&
                methodAccessT.Expression is IdentifierNameSyntax identifierNameT &&
                identifierNameT.Identifier.ValueText == "Assert")
            {
                var underTest = ((InvocationExpressionSyntax)identifierNameT.Parent.Parent).ArgumentList.Arguments[0];
                var genericName = ((GenericNameSyntax)methodAccessT.Name).TypeArgumentList.Arguments[0];
                var thrower = NUnitToShouldlyConverterAnalyzer.ListOfThrowsMethods[methodAccessT.Name.Identifier.ValueText];
                var newExpression = SyntaxFactory.ParseExpression($"{identifierNameT.GetLeadingTrivia().ToFullString()}Should.{thrower}<{genericName}>({underTest})");

                editor.ReplaceNode(expression.Parent, newExpression);
                editor.AddUsingDirective(shouldyNamespace);

                return editor.GetChangedDocument();
            }
            return context.Document;
        }
        
    }
}
