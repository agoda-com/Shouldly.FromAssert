using System.Collections.Immutable;
using System.Linq;
using System.Threading;
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

        private static readonly ImmutableHashSet<string> NUnitAssertTypes = ImmutableHashSet.Create(
            "NUnit.Framework.Assert",
            "NUnit.Framework.StringAssert",
            "NUnit.Framework.CollectionAssert");

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;

            if (IsReported(invocation, context.SemanticModel, context.CancellationToken))
            {
                var diagnostic = Diagnostic.Create(Rule, invocation.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }

        internal static bool IsReported(InvocationExpressionSyntax invocation, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            var symbolInfo = semanticModel.GetSymbolInfo(invocation, cancellationToken);
            var method = symbolInfo.Symbol as IMethodSymbol
                         ?? symbolInfo.CandidateSymbols.OfType<IMethodSymbol>().FirstOrDefault();
            if (method == null) return false;

            var containingType = method.ContainingType?.ToDisplayString();
            if (containingType == null || !NUnitAssertTypes.Contains(containingType)) return false;

            // Only report Assert.Multiple when the fix can turn it into ShouldSatisfyAllConditions.
            if (AssertMultiple.IsAssertMultiple(invocation) && AssertMultiple.GetConditions(invocation) == null) return false;

            return true;
        }
    }
}