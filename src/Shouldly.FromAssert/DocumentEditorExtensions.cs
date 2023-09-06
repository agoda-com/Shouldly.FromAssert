using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace Shouldly.FromAssert
{
    public static class DocumentEditorExtensions
    {
        public static DocumentEditor AddUsingDirective(this DocumentEditor editor, UsingDirectiveSyntax usingDirective)
        {

            // Get the syntax root of the document.
            var syntaxRoot = editor.OriginalRoot;

            // Find the compilation unit (the root node for the entire code file).
            var compilationUnit = syntaxRoot as CompilationUnitSyntax;

            if (compilationUnit != null)
            {
                // Check if the using directive already exists.
                var existingUsing = compilationUnit.Usings.FirstOrDefault(u => u.Name.ToString() == usingDirective.ToFullString());

                // Add the new using directive only if it doesn't exist already.
                if (existingUsing == null)
                {
                    compilationUnit = compilationUnit.AddUsings(usingDirective);

                    // Apply the updated compilation unit to the editor.
                    editor.ReplaceNode(syntaxRoot, compilationUnit);
                }
            }
            
            return editor;
        }
    }
}