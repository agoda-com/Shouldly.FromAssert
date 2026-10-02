using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Shouldly.FromAssert.Tests.Infrastructure;

/// <summary>Runs the analyzer alone: only the diagnostics in the markup (or added to ExpectedDiagnostics) may be reported.</summary>
internal class AnalyzerOnlyTest : CSharpAnalyzerTest<NUnitToShouldlyAnalyzer, NUnitVerifier>
{
    public AnalyzerOnlyTest(string source)
    {
        TestCode = source;
        ReferenceAssemblies = CodeFixTest.References;
    }
}
