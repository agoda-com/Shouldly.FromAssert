using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Shouldly.FromAssert
{
    /// <summary>
    /// Fix-all that converts <c>Assert.Multiple</c> and the asserts inside it in a single pass.
    /// The batch fixer computes every diagnostic's fix against the original document and drops a fix whose edits
    /// overlap another's, so an <c>Assert.Multiple</c> and its inner asserts can't both be applied: one pass converted
    /// the inner asserts and left the <c>Assert.Multiple</c> for a second <c>dotnet format</c> run (#32).
    /// Here a diagnostic nested in another one's span is skipped, because the outer fix already converts what's inside
    /// it, and the rest are applied one after another from the end of the document. A fix only rewrites its own
    /// statement and what follows it, so the spans of the diagnostics before it still hold.
    /// </summary>
    internal sealed class NUnitToShouldlyFixAllProvider : DocumentBasedFixAllProvider
    {
        private readonly NUnitToShouldlyCodeFixProvider _codeFixProvider;

        public NUnitToShouldlyFixAllProvider(NUnitToShouldlyCodeFixProvider codeFixProvider)
        {
            _codeFixProvider = codeFixProvider;
        }

        protected override string GetFixAllTitle(FixAllContext fixAllContext) => "Convert all to Shouldly";

        protected override async Task<Document> FixAllAsync(FixAllContext fixAllContext, Document document, ImmutableArray<Diagnostic> diagnostics)
        {
            var spans = diagnostics.Select(d => d.Location.SourceSpan).ToList();
            var outermost = spans
                .Where(span => !spans.Any(other => other != span && other.Contains(span)))
                .Distinct()
                .OrderByDescending(span => span.Start);

            var changed = false;
            foreach (var span in outermost)
            {
                var tree = await document.GetSyntaxTreeAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
                var diagnostic = Diagnostic.Create(NUnitToShouldlyAnalyzer.Rule, Location.Create(tree, span));
                var fixedDocument = await FixAsync(document, diagnostic, fixAllContext.CancellationToken).ConfigureAwait(false);
                if (fixedDocument == null) continue;

                var text = await document.GetTextAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
                var fixedText = await fixedDocument.GetTextAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
                if (fixedText.ContentEquals(text)) continue;

                document = fixedDocument;
                changed = true;
            }

            return changed ? document : null;
        }

        // Through the code action rather than straight to the fix, so the result is formatted as the single fix's is.
        private async Task<Document> FixAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var action = _codeFixProvider.CreateCodeAction(document, diagnostic);
            var operations = await action.GetOperationsAsync(cancellationToken).ConfigureAwait(false);
            return operations.OfType<ApplyChangesOperation>().FirstOrDefault()?.ChangedSolution.GetDocument(document.Id);
        }
    }
}
