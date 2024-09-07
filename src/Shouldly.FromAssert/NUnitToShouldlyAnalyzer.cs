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
            true,
            helpLinkUri: "https://github.com/agoda-com/Shouldly.FromAssert/");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;
            string methodName = null;
            string assertClass = null;

            if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
            {
                methodName = memberAccess.Name.Identifier.Text;
                if (memberAccess.Expression is IdentifierNameSyntax identifier)
                {
                    assertClass = identifier.Identifier.Text;
                }
            }
            else if (invocation.Expression is IdentifierNameSyntax identifierName)
            {
                methodName = identifierName.Identifier.Text;
            }

            if (methodName == null) return;

            if (assertClass == "Assert" || assertClass == "StringAssert" || assertClass == "CollectionAssert" ||
                (assertClass == null && methodName.StartsWith("Assert")))
            {
                var diagnostic = Diagnostic.Create(Rule, invocation.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}