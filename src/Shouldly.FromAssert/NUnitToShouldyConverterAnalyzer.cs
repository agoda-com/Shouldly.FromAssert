using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Shouldly.FromAssert
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NUnitToShouldlyConverterAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SHU001a";

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
                memberAccess.Name.Identifier.ValueText == "That" &&
                memberAccess.Expression is IdentifierNameSyntax identifierName &&
                identifierName.Identifier.ValueText == "Assert" &&
                invocationExpression.ArgumentList.Arguments.Count == 2)
            {
                var diagnostic = Diagnostic.Create(Rule, invocationExpression.Parent.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }

            if (invocationExpression.Expression is MemberAccessExpressionSyntax memberAccess2 &&
                ListOfTwoParameterMethods.ContainsKey(memberAccess2.Name.Identifier.ValueText) &&
                memberAccess2.Expression is IdentifierNameSyntax identifierName2 &&
                identifierName2.Identifier.ValueText == "Assert" &&
                invocationExpression.ArgumentList.Arguments.Count == 2)
            {
                var diagnostic = Diagnostic.Create(Rule, invocationExpression.Parent.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
            if (invocationExpression.Expression is MemberAccessExpressionSyntax memberAccess1 &&
                ListOfSingleParameterMethods.ContainsKey(memberAccess1.Name.Identifier.ValueText) &&
                memberAccess1.Expression is IdentifierNameSyntax identifierName1 &&
                identifierName1.Identifier.ValueText == "Assert" &&
                invocationExpression.ArgumentList.Arguments.Count == 1)
            {
                var diagnostic = Diagnostic.Create(Rule, invocationExpression.Parent.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }

            if (invocationExpression.Expression is MemberAccessExpressionSyntax memberAccessT &&
                ListOfThrowsMethods.ContainsKey(memberAccessT.Name.Identifier.ValueText) &&
                memberAccessT.Expression is IdentifierNameSyntax identifierNameT &&
                identifierNameT.Identifier.ValueText == "Assert")
            {
                var diagnostic = Diagnostic.Create(Rule, invocationExpression.Parent.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }
        internal static Dictionary<string, string> ListOfSingleParameterMethods = new Dictionary<string, string>()
        {
            {"True","ShouldBeTrue"},
            {"False","ShouldBeFalse"},
            {"IsTrue","ShouldBeTrue"},
            {"IsFalse","ShouldBeFalse"},
            {"Null","ShouldBeNull"} ,
            {"NotNull","ShouldNotBeNull"},
            {"IsEmpty", "ShouldBeEmpty"}
        };
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
        internal static Dictionary<string, string> ListOfThrowsMethods = new Dictionary<string, string>()
        {
            {"Throws", "Throw"},
            {"ThrowsAsync", "ThrowAsync"},
            {"DoesNotThrow","NotThrow"},
            {"DoesNotThrowAsync","NotThrowAsync"}
        };
    }
}