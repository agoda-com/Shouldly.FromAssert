using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Shouldly.FromAssert
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NUnitToShouldlyAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SHU001";

        public static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            "Convert to Shouldly",
            "Convert to Shouldly format",
            "NUnit to Shouldly",
            DiagnosticSeverity.Warning,
            true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax) context.Node;
            var methodName = invocation.Expression switch
            {
                MemberAccessExpressionSyntax memberAccess => memberAccess.Name.Identifier.Text,
                IdentifierNameSyntax identifier => identifier.Identifier.Text,
                _ => null
            };

            if (methodName == null) return;

            var assertClass = invocation.Expression switch
            {
                MemberAccessExpressionSyntax memberAccess => memberAccess.Expression switch
                {
                    IdentifierNameSyntax identifier => identifier.Identifier.Text,
                    _ => null
                },
                _ => null
            };

            if (assertClass == "Assert" || assertClass == "StringAssert" || assertClass == "CollectionAssert" ||
                (assertClass == null && methodName.StartsWith("Assert")))
            {
                var diagnostic = Diagnostic.Create(Rule, invocation.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}