using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using NUnit.Framework;
using Shouldly.FromAssert;

namespace Shouldly.FromAssert.Tests
{
    public class NUnitToShouldlyConverterTestsThat
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


namespace TestNamespace
{
    public class TestClass
    {
        [Test]
        public void TestMethod()
        {
            var contestant = 1337;
            Assert.That(contestant, Is.EqualTo(1337));
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
            var contestant = 1337;
            contestant.ShouldBe(1337);
        }
    }
}";
            var codeFixTest = new CodeFixTest(test, expected,
                    CSharpAnalyzerVerifier<NUnitToShouldlyConverterAnalyzer, NUnitVerifier>
                        .Diagnostic(NUnitToShouldlyConverterAnalyzer.DiagnosticId)
                    .WithSpan(13, 13, 13, 55));
            
            await codeFixTest.RunAsync(CancellationToken.None);
            var a = codeFixTest.CompilerDiagnostics;
        }

        [Test]
        public async Task TestThrow()
        {
            var test = @"
using NUnit.Framework;
using System;

namespace TestNamespace
{
    public class TestClass
    {
        [Test]
        public void TestMethod()
        {
            var denominator = 1;
            Assert.Throws<NullReferenceException>(() =>
            {
                var y = 3000 / denominator;
            });
        }
    }
}";

            var expected = @"
using NUnit.Framework;
using System;
using Shouldly;

namespace TestNamespace
{
    public class TestClass
    {
        [Test]
        public void TestMethod()
        {
            var denominator = 1;
            Should.Throw<NullReferenceException>(() =>
            {
                var y = 3000 / denominator;
            });
        }
    }
}";
            var codeFixTest = new CodeFixTest(test, expected,
                CSharpAnalyzerVerifier<NUnitToShouldlyConverterAnalyzer, NUnitVerifier>
                    .Diagnostic(NUnitToShouldlyConverterAnalyzer.DiagnosticId)
                    .WithSpan(13, 13, 16, 16));

            await codeFixTest.RunAsync(CancellationToken.None);
            var a = codeFixTest.CompilerDiagnostics;
        }
    }
}