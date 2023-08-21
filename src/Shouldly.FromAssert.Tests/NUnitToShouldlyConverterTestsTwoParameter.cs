using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using NUnit.Framework;

namespace Shouldly.FromAssert.Tests;

public class NUnitToShouldlyConverterTestsTwoParameter
{
    private class CodeFixTest : CSharpCodeFixTest<NUnitToShouldlyConverterAnalyzerTwoParameter, NUnitAssertToShouldlyConverterCodeFixProviderTwoParameter, NUnitVerifier>
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
            string underTest = null;
            Assert.AreEqual(underTest, contestant);
            Assert.AreNotEqual(underTest, contestant);
            Assert.AreSame(underTest, contestant);
            Assert.AreNotSame(underTest, contestant);
            Assert.Contains(contestant, new string[]{});
            Assert.IsInstanceOf(typeof(string), underTest);
            Assert.IsNotInstanceOf(typeof(string), underTest);
            Assert.IsAssignableFrom(typeof(string), underTest);
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
            string underTest = null;
            contestant.ShouldBe(underTest);
            contestant.ShouldNotBe(underTest);
            contestant.ShouldBe(underTest);
            contestant.ShouldNotBe(underTest);
            new string[]{}.ShouldContain(contestant);
            underTest.ShouldBeOfType(typeof(string));
            underTest.ShouldNotBeOfType(typeof(string));
            underTest.ShouldBeAssignableTo(typeof(string));
        }
    }
}";
        var codeFixTest = new CodeFixTest(test, expected,
            CSharpAnalyzerVerifier<NUnitToShouldlyConverterAnalyzerTwoParameter, NUnitVerifier>
                .Diagnostic(NUnitToShouldlyConverterAnalyzerTwoParameter.Rule)
                .WithSpan(21, 13, 21, 64),
            CSharpAnalyzerVerifier<NUnitToShouldlyConverterAnalyzerTwoParameter, NUnitVerifier>
                .Diagnostic(NUnitToShouldlyConverterAnalyzerTwoParameter.Rule)
                .WithSpan(14, 13, 14, 52),
            CSharpAnalyzerVerifier<NUnitToShouldlyConverterAnalyzerTwoParameter, NUnitVerifier>
                .Diagnostic(NUnitToShouldlyConverterAnalyzerTwoParameter.Rule)
                .WithSpan(15, 13, 15, 55),
            CSharpAnalyzerVerifier<NUnitToShouldlyConverterAnalyzerTwoParameter, NUnitVerifier>
                .Diagnostic(NUnitToShouldlyConverterAnalyzerTwoParameter.Rule)
                .WithSpan(16, 13, 16, 51),
            CSharpAnalyzerVerifier<NUnitToShouldlyConverterAnalyzerTwoParameter, NUnitVerifier>
                .Diagnostic(NUnitToShouldlyConverterAnalyzerTwoParameter.Rule)
                .WithSpan(17, 13, 17, 54),
            CSharpAnalyzerVerifier<NUnitToShouldlyConverterAnalyzerTwoParameter, NUnitVerifier>
                .Diagnostic(NUnitToShouldlyConverterAnalyzerTwoParameter.Rule)
                .WithSpan(18, 13, 18, 57),
            CSharpAnalyzerVerifier<NUnitToShouldlyConverterAnalyzerTwoParameter, NUnitVerifier>
                .Diagnostic(NUnitToShouldlyConverterAnalyzerTwoParameter.Rule)
                .WithSpan(19, 13, 19, 60),
            CSharpAnalyzerVerifier<NUnitToShouldlyConverterAnalyzerTwoParameter, NUnitVerifier>
                .Diagnostic(NUnitToShouldlyConverterAnalyzerTwoParameter.Rule)
                .WithSpan(20, 13, 20, 63));

        await codeFixTest.RunAsync(CancellationToken.None);
    }
}