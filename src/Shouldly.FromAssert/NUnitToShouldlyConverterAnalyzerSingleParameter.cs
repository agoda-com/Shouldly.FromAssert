using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Shouldly.FromAssert
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NUnitToShouldlyConverterAnalyzerSingleParameter : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SHU002";

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
                listOfBooleanMethods.ContainsKey(memberAccess.Name.Identifier.ValueText) &&
                memberAccess.Expression is IdentifierNameSyntax identifierName &&
                identifierName.Identifier.ValueText == "Assert" &&
                invocationExpression.ArgumentList.Arguments.Count == 1)
            {
                var diagnostic = Diagnostic.Create(Rule, invocationExpression.Parent.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }
        internal static Dictionary<string, string> listOfBooleanMethods = new Dictionary<string,string>()
        {
            {"True","ShouldBeTrue"},
            {"False","ShouldBeFalse"},
            {"Null","ShouldBeNull"} ,
            {"NotNull","ShouldNotBeNull"},
            {"IsEmpty", "ShouldBeEmpty"}
        };
    }
}