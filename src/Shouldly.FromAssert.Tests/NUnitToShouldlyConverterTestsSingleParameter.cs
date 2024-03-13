using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using NUnit.Framework;

namespace Shouldly.FromAssert.Tests;

public class NUnitToShouldlyConverterTestsSingleParameter
{
    private class CodeFixTest : CSharpCodeFixTest<NUnitToShouldlyConverterAnalyzer, NUnitAssertToShouldlyConverterCodeFixProvider, NUnitVerifier>
    {
        public CodeFixTest(
            string source,
            string fixedSource,
            params DiagnosticResult[] expected)
        {
            TestCode = source;
            FixedCode = fixedSource;
            ExpectedDiagnostics.AddRange(expected);
            ReferenceAssemblies = ReferenceAssemblies.Default
                .AddPackages(ImmutableArray.Create(
                        new PackageIdentity("Shouldly", "4.2.1"),
                        new PackageIdentity("NUnit", "3.13.3")
                    )
                );
        }
    }

    [Test]
    public async Task TestConversion()
    {
        var test = @"
using NUnit.Framework;
using Shouldly;

namespace TestNamespace
{
    public class TestClass
    {
        [Test]
        public void TestMethod()
        {
            string contestant = null;
            Assert.True(true);
            Assert.False(true);
            Assert.Null(contestant);
            Assert.NotNull(contestant);
            Assert.IsEmpty(contestant);
            Assert.IsTrue(true);
            Assert.IsFalse(true);
        }
    }
}";
        
        var expected = @"
using NUnit.Framework;
using Shouldly;

namespace TestNamespace
{
    public class TestClass
    {
        [Test]
        public void TestMethod()
        {
            string contestant = null;
            true.ShouldBeTrue();
            true.ShouldBeFalse();
            contestant.ShouldBeNull();
            contestant.ShouldNotBeNull();
            contestant.ShouldBeEmpty();
            true.ShouldBeTrue();
            true.ShouldBeFalse();
        }
    }
}";
        var codeFixTest = new CodeFixTest(test, expected,
            CSharpAnalyzerVerifier<NUnitToShouldlyConverterAnalyzer, NUnitVerifier>
                .Diagnostic(NUnitToShouldlyConverterAnalyzer.Rule)
                .WithSpan(13, 13, 13, 31),
            CSharpAnalyzerVerifier<NUnitToShouldlyConverterAnalyzer, NUnitVerifier>
                .Diagnostic(NUnitToShouldlyConverterAnalyzer.Rule)
                .WithSpan(14, 13, 14, 32),
            CSharpAnalyzerVerifier<NUnitToShouldlyConverterAnalyzer, NUnitVerifier>
                .Diagnostic(NUnitToShouldlyConverterAnalyzer.Rule)
                .WithSpan(15, 13, 15, 37),
            CSharpAnalyzerVerifier<NUnitToShouldlyConverterAnalyzer, NUnitVerifier>
                .Diagnostic(NUnitToShouldlyConverterAnalyzer.Rule)
                .WithSpan(16, 13, 16, 40),
            CSharpAnalyzerVerifier<NUnitToShouldlyConverterAnalyzer, NUnitVerifier>
                .Diagnostic(NUnitToShouldlyConverterAnalyzer.Rule)
                .WithSpan(17, 13, 17, 40),
            CSharpAnalyzerVerifier<NUnitToShouldlyConverterAnalyzer, NUnitVerifier>
                .Diagnostic(NUnitToShouldlyConverterAnalyzer.Rule)
                .WithSpan(18, 13, 18, 33),
            CSharpAnalyzerVerifier<NUnitToShouldlyConverterAnalyzer, NUnitVerifier>
                .Diagnostic(NUnitToShouldlyConverterAnalyzer.Rule)
                .WithSpan(19, 13, 19, 34));

        await codeFixTest.RunAsync(CancellationToken.None);
    }


    [Test]
    public async Task TestConversion2()
    {
        var test = @"
using NUnit.Framework;
using Shouldly;

namespace TestNamespace
{
    public class TestClass
    {
        [Test]
        public void TestMethod()
        {
            int i = 0;
            Assert.IsTrue(i == 0);
        }
    }
}";

        var expected = @"
using NUnit.Framework;
using Shouldly;

namespace TestNamespace
{
    public class TestClass
    {
        [Test]
        public void TestMethod()
        {
            int i = 0;
            (i == 0).ShouldBeTrue();
        }
    }
}";
        var codeFixTest = new CodeFixTest(test, expected,
            CSharpAnalyzerVerifier<NUnitToShouldlyConverterAnalyzer, NUnitVerifier>
                .Diagnostic(NUnitToShouldlyConverterAnalyzer.Rule)
                .WithSpan(13, 13, 13, 35));

        await codeFixTest.RunAsync(CancellationToken.None);
    }
}