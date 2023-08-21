using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Shouldly.FromAssert
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NUnitToShouldlyConverterAnalyzerTwoParameter : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SHU003";

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

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var invocationExpression = (InvocationExpressionSyntax)context.Node;

            if (invocationExpression.Expression is MemberAccessExpressionSyntax memberAccess &&
                ListOfTwoParameterMethods.ContainsKey(memberAccess.Name.Identifier.ValueText) &&
                memberAccess.Expression is IdentifierNameSyntax identifierName &&
                identifierName.Identifier.ValueText == "Assert" &&
                invocationExpression.ArgumentList.Arguments.Count == 2)
            {
                var diagnostic = Diagnostic.Create(Rule, invocationExpression.Parent.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }
        internal static Dictionary<string, string> ListOfTwoParameterMethods = new Dictionary<string, string>()
        {
            {"AreEqual","ShouldBe"},
            {"AreNotEqual","ShouldNotBe"},
            {"AreSame","ShouldBe"} ,
            {"AreNotSame","ShouldNotBe"},
            {"Contains", "ShouldContain"},
            {"IsInstanceOf", "ShouldBeOfType"},
            {"IsNotInstanceOf", "ShouldNotBeOfType"},
            {"IsAssignableFrom", "ShouldBeAssignableTo"}
        };
    }
}