using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Shouldly.FromAssert
{
    /// <summary>
    /// Shared rules for turning <c>Assert.Multiple(() => { ... })</c> into
    /// <c>this.ShouldSatisfyAllConditions(() => ..., () => ...)</c>.
    /// The analyzer only reports the forms the code fix can convert.
    /// </summary>
    internal static class AssertMultiple
    {
        public static bool IsAssertMultiple(InvocationExpressionSyntax invocation)
        {
            return invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
                   memberAccess.Name.Identifier.Text == "Multiple" &&
                   memberAccess.Expression is IdentifierNameSyntax identifier &&
                   identifier.Identifier.Text == "Assert";
        }

        /// <summary>
        /// Returns the expression of each statement in the lambda, or null when the call can't be converted:
        /// an async lambda (no Action equivalent), a body with anything other than expression statements
        /// (locals can't be split across lambdas), or a static context (no <c>this</c> receiver).
        /// </summary>
        public static IReadOnlyList<ExpressionSyntax> GetConditions(InvocationExpressionSyntax invocation)
        {
            if (!IsAssertMultiple(invocation) || invocation.ArgumentList.Arguments.Count != 1) return null;

            if (!(invocation.ArgumentList.Arguments[0].Expression is ParenthesizedLambdaExpressionSyntax lambda) ||
                lambda.ParameterList.Parameters.Count != 0 ||
                lambda.AsyncKeyword.IsKind(SyntaxKind.AsyncKeyword) ||
                !HasThis(invocation))
            {
                return null;
            }

            if (lambda.ExpressionBody != null) return new[] { lambda.ExpressionBody };

            var statements = lambda.Block.Statements;
            if (statements.Count == 0 || !statements.All(s => s is ExpressionStatementSyntax)) return null;

            return statements.Cast<ExpressionStatementSyntax>().Select(s => s.Expression).ToList();
        }

        private static bool HasThis(SyntaxNode node)
        {
            foreach (var ancestor in node.Ancestors())
            {
                switch (ancestor)
                {
                    case AnonymousFunctionExpressionSyntax function when function.Modifiers.Any(SyntaxKind.StaticKeyword):
                        return false;
                    case LocalFunctionStatementSyntax localFunction when localFunction.Modifiers.Any(SyntaxKind.StaticKeyword):
                        return false;
                    case BaseMethodDeclarationSyntax method:
                        return !method.Modifiers.Any(SyntaxKind.StaticKeyword);
                    case BasePropertyDeclarationSyntax property:
                        return !property.Modifiers.Any(SyntaxKind.StaticKeyword);
                    case MemberDeclarationSyntax _:
                        return false;
                }
            }

            return false;
        }
    }
}
