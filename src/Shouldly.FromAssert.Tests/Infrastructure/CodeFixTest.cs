using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Shouldly.FromAssert.Tests.Infrastructure;

internal class CodeFixTest : CSharpCodeFixTest<NUnitToShouldlyAnalyzer, NUnitToShouldlyCodeFixProvider, NUnitVerifier>
{
    public CodeFixTest(
        string source,
        string fixedSource,
        params DiagnosticResult[] expected)
    {
        TestCode = source;
        FixedCode = fixedSource;
        ExpectedDiagnostics.AddRange(expected);

        ReferenceAssemblies = References;
    }

    public static readonly ReferenceAssemblies References = ReferenceAssemblies.Default
        .AddPackages(ImmutableArray.Create(
                new PackageIdentity("Shouldly", "4.2.1"),
                new PackageIdentity("NUnit", "3.14.0")
            )
        );
}
