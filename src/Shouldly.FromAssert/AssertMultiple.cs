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
    /// <c>Assert.Multiple(async () => ...)</c> converts in two steps: its awaits are hoisted into locals,
    /// then the synchronous call that's left converts like any other.
    /// </summary>
    internal static class AssertMultiple
    {
        /// <summary>
        /// Matches <c>Assert.Multiple</c>, <c>NUnit.Framework.Assert.Multiple</c> and <c>Multiple</c> under
        /// <c>using static NUnit.Framework.Assert</c>. Only called on invocations the analyzer has already bound
        /// to an NUnit assert type, so the name alone identifies the method.
        /// </summary>
        public static bool IsAssertMultiple(InvocationExpressionSyntax invocation)
        {
            switch (invocation.Expression)
            {
                case MemberAccessExpressionSyntax memberAccess:
                    return memberAccess.Name.Identifier.Text == "Multiple";
                case IdentifierNameSyntax identifier:
                    return identifier.Identifier.Text == "Multiple";
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns the expression of each statement in the lambda, or null when the call can't be converted:
        /// an async lambda (no Action equivalent, see <see cref="GetAwaitsToHoist"/>), a body with anything other
        /// than expression statements (locals can't be split across lambdas), or a static context (no <c>this</c> receiver).
        /// </summary>
        public static IReadOnlyList<ExpressionSyntax> GetConditions(InvocationExpressionSyntax invocation)
        {
            var lambda = GetLambda(invocation);
            return lambda == null || IsAsync(lambda) || !HasThis(invocation) ? null : GetConditions(lambda);
        }

        /// <summary>
        /// For <c>Assert.Multiple(async () => { ... })</c>, returns the awaits to move into locals ahead of the call,
        /// which leaves a synchronous Assert.Multiple that converts as usual. Null when that can't be done: the call
        /// isn't a statement in a block of an async function, a condition is itself an await (nothing left to assert),
        /// or an await is only evaluated conditionally (<c>?.</c>, <c>??</c>, <c>&amp;&amp;</c>, <c>||</c>, <c>?:</c>),
        /// so hoisting it would run it when NUnit wouldn't.
        /// </summary>
        public static IReadOnlyList<AwaitExpressionSyntax> GetAwaitsToHoist(InvocationExpressionSyntax invocation)
        {
            var lambda = GetLambda(invocation);
            if (lambda == null || !IsAsync(lambda) || !HasThis(invocation) ||
                !(invocation.Parent is ExpressionStatementSyntax statement) ||
                !(statement.Parent is BlockSyntax) ||
                !IsInAsyncFunction(statement))
            {
                return null;
            }

            var conditions = GetConditions(lambda);
            if (conditions == null) return null;

            var awaits = new List<AwaitExpressionSyntax>();
            foreach (var condition in conditions)
            {
                // Awaits inside a nested lambda belong to that lambda, and an await inside an await is hoisted with it.
                foreach (var awaitExpression in condition
                             .DescendantNodesAndSelf(n => !(n is AnonymousFunctionExpressionSyntax) && !(n is AwaitExpressionSyntax))
                             .OfType<AwaitExpressionSyntax>())
                {
                    if (awaitExpression == condition || !IsAlwaysEvaluated(awaitExpression, condition)) return null;
                    awaits.Add(awaitExpression);
                }
            }

            return awaits;
        }

        private static ParenthesizedLambdaExpressionSyntax GetLambda(InvocationExpressionSyntax invocation) =>
            IsAssertMultiple(invocation) &&
            invocation.ArgumentList.Arguments.Count == 1 &&
            invocation.ArgumentList.Arguments[0].Expression is ParenthesizedLambdaExpressionSyntax lambda &&
            lambda.ParameterList.Parameters.Count == 0
                ? lambda
                : null;

        private static bool IsAsync(ParenthesizedLambdaExpressionSyntax lambda) =>
            lambda.AsyncKeyword.IsKind(SyntaxKind.AsyncKeyword);

        private static IReadOnlyList<ExpressionSyntax> GetConditions(ParenthesizedLambdaExpressionSyntax lambda)
        {
            if (lambda.ExpressionBody != null) return new[] { lambda.ExpressionBody };

            var statements = lambda.Block.Statements;
            if (statements.Count == 0 || !statements.All(s => s is ExpressionStatementSyntax)) return null;

            return statements.Cast<ExpressionStatementSyntax>().Select(s => s.Expression).ToList();
        }

        private static bool IsAlwaysEvaluated(SyntaxNode node, ExpressionSyntax condition)
        {
            for (; node != condition; node = node.Parent)
            {
                switch (node.Parent)
                {
                    case ConditionalAccessExpressionSyntax access when access.WhenNotNull == node:
                    case ConditionalExpressionSyntax conditional when conditional.Condition != node:
                    case BinaryExpressionSyntax binary when binary.Right == node &&
                                                            (binary.IsKind(SyntaxKind.CoalesceExpression) ||
                                                             binary.IsKind(SyntaxKind.LogicalAndExpression) ||
                                                             binary.IsKind(SyntaxKind.LogicalOrExpression)):
                    case SwitchExpressionArmSyntax _:
                        return false;
                }
            }

            return true;
        }

        private static bool IsInAsyncFunction(SyntaxNode node)
        {
            foreach (var ancestor in node.Ancestors())
            {
                switch (ancestor)
                {
                    case AnonymousFunctionExpressionSyntax function:
                        return function.AsyncKeyword.IsKind(SyntaxKind.AsyncKeyword);
                    case LocalFunctionStatementSyntax localFunction:
                        return localFunction.Modifiers.Any(SyntaxKind.AsyncKeyword);
                    case BaseMethodDeclarationSyntax method:
                        return method.Modifiers.Any(SyntaxKind.AsyncKeyword);
                    case MemberDeclarationSyntax _:
                        return false;
                }
            }

            return false;
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
