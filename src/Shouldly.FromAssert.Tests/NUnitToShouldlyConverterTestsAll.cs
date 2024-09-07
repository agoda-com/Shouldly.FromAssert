using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;
using System.Collections.Immutable;
using Shouldly.FromAssert.NUnitToShouldlyConverter;

namespace Shouldly.FromAssert.Tests;

public class NUnitToShouldlyConverterTestsAll
{

    private class CodeFixTest : CSharpCodeFixTest<NUnitToShouldlyAnalyzer, NUnitToShouldlyCodeFixProvider, NUnitVerifier>
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
                        new PackageIdentity("NUnit", "3.14.0")
                    )
                );
        }
    }
    public class TestCase
    {
        public string NUnitAssertion { get; set; }
        public string ShouldlyAssertion { get; set; }
        public int Line { get; set; }
        public int StartColumn { get; set; }
        public int EndColumn { get; set; }
        public string SetupCode { get; set; }

    }
    private static IEnumerable<TestCaseData> TestCases()
    {
        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestant = 1337;",
            NUnitAssertion = "Assert.That(contestant, Is.EqualTo(1337));",
            ShouldlyAssertion = "contestant.ShouldBe(1337);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 54
        }).SetName("Assert.That with Is.EqualTo");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestant = 1337;",
            NUnitAssertion = "Assert.AreEqual(1337, contestant);",
            ShouldlyAssertion = "contestant.ShouldBe(1337);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 46
        }).SetName("Assert.AreEqual");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestant = 1337;",
            NUnitAssertion = "Assert.AreNotEqual(1336, contestant);",
            ShouldlyAssertion = "contestant.ShouldNotBe(1336);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 49
        }).SetName("Assert.AreNotEqual");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestant = 1337;",
            NUnitAssertion = "Assert.IsTrue(contestant > 1000);",
            ShouldlyAssertion = "(contestant > 1000).ShouldBeTrue();",
            Line = 12,
            StartColumn = 13,
            EndColumn = 45
        }).SetName("Assert.IsTrue");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestant = 1337;",
            NUnitAssertion = "Assert.IsFalse(contestant < 1000);",
            ShouldlyAssertion = "(contestant < 1000).ShouldBeFalse();",
            Line = 12,
            StartColumn = 13,
            EndColumn = 46
        }).SetName("Assert.IsFalse");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "string contestant = null;",
            NUnitAssertion = "Assert.IsNull(contestant);",
            ShouldlyAssertion = "contestant.ShouldBeNull();",
            Line = 12,
            StartColumn = 13,
            EndColumn = 38
        }).SetName("Assert.IsNull");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestant = \"1337\";",
            NUnitAssertion = "Assert.IsNotNull(contestant);",
            ShouldlyAssertion = "contestant.ShouldNotBeNull();",
            Line = 12,
            StartColumn = 13,
            EndColumn = 41
        }).SetName("Assert.IsNotNull");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var expected = new object(); var contestant = expected;",
            NUnitAssertion = "Assert.AreSame(expected, contestant);",
            ShouldlyAssertion = "contestant.ShouldBeSameAs(expected);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 49
        }).SetName("Assert.AreSame");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var expected = new object(); var contestant = new object();",
            NUnitAssertion = "Assert.AreNotSame(expected, contestant);",
            ShouldlyAssertion = "contestant.ShouldNotBeSameAs(expected);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 52
        }).SetName("Assert.AreNotSame");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestant = \"1337\";",
            NUnitAssertion = "Assert.IsInstanceOf<string>(contestant);",
            ShouldlyAssertion = "contestant.ShouldBeOfType<string>();",
            Line = 12,
            StartColumn = 13,
            EndColumn = 52
        }).SetName("Assert.IsInstanceOf");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestant = \"1337\";",
            NUnitAssertion = "Assert.IsNotInstanceOf<int>(contestant);",
            ShouldlyAssertion = "contestant.ShouldNotBeOfType<int>();",
            Line = 12,
            StartColumn = 13,
            EndColumn = 52
        }).SetName("Assert.IsNotInstanceOf");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestants = new List<int> { 1337, 2448, 3559 };",
            NUnitAssertion = "Assert.Contains(1337, contestants);",
            ShouldlyAssertion = "contestants.ShouldContain(1337);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 47
        }).SetName("CollectionAssert.Contains");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestants = new List<int> { 1337, 2448, 3559 };",
            NUnitAssertion = "CollectionAssert.DoesNotContain(contestants, 1336);",
            ShouldlyAssertion = "contestants.ShouldNotContain(1336);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 63
        }).SetName("CollectionAssert.DoesNotContain");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestants = new List<int>();",
            NUnitAssertion = "CollectionAssert.IsEmpty(contestants);",
            ShouldlyAssertion = "contestants.ShouldBeEmpty();",
            Line = 12,
            StartColumn = 13,
            EndColumn = 50
        }).SetName("CollectionAssert.IsEmpty");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestants = new List<int> { 1337 };",
            NUnitAssertion = "CollectionAssert.IsNotEmpty(contestants);",
            ShouldlyAssertion = "contestants.ShouldNotBeEmpty();",
            Line = 12,
            StartColumn = 13,
            EndColumn = 53
        }).SetName("CollectionAssert.IsNotEmpty");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestant = 1337;",
            NUnitAssertion = "Assert.Greater(contestant, 1000);",
            ShouldlyAssertion = "contestant.ShouldBeGreaterThan(1000);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 45
        }).SetName("Assert.Greater");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestant = 1337;",
            NUnitAssertion = "Assert.GreaterOrEqual(contestant, 1337);",
            ShouldlyAssertion = "contestant.ShouldBeGreaterThanOrEqualTo(1337);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 52
        }).SetName("Assert.GreaterOrEqual");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestant = 1337;",
            NUnitAssertion = "Assert.Less(contestant, 2000);",
            ShouldlyAssertion = "contestant.ShouldBeLessThan(2000);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 42
        }).SetName("Assert.Less");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestant = 1337;",
            NUnitAssertion = "Assert.LessOrEqual(contestant, 1337);",
            ShouldlyAssertion = "contestant.ShouldBeLessThanOrEqualTo(1337);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 49
        }).SetName("Assert.LessOrEqual");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestant = double.NaN;",
            NUnitAssertion = "Assert.IsNaN(contestant);",
            ShouldlyAssertion = "double.IsNaN(contestant).ShouldBeTrue();",
            Line = 12,
            StartColumn = 13,
            EndColumn = 37
        }).SetName("Assert.IsNaN");
        
        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var greeting = \"Hello, World!\";",
            NUnitAssertion = "StringAssert.StartsWith(\"Hello\", greeting);",
            ShouldlyAssertion = "greeting.ShouldStartWith(\"Hello\");",
            Line = 12,
            StartColumn = 13,
            EndColumn = 55
        }).SetName("StringAssert.StartsWith");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var greeting = \"Hello, World!\";",
            NUnitAssertion = "StringAssert.EndsWith(\"World!\", greeting);",
            ShouldlyAssertion = "greeting.ShouldEndWith(\"World!\");",
            Line = 12,
            StartColumn = 13,
            EndColumn = 54
        }).SetName("StringAssert.EndsWith");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var greeting = \"Hello, World!\";",
            NUnitAssertion = "StringAssert.Contains(\"World\", greeting);",
            ShouldlyAssertion = "greeting.ShouldContain(\"World\");",
            Line = 12,
            StartColumn = 13,
            EndColumn = 53
        }).SetName("StringAssert.Contains");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "void ThrowException() { throw new ArgumentException(); }",
            NUnitAssertion = "Assert.Throws<ArgumentException>(() => ThrowException());",
            ShouldlyAssertion = "Should.Throw<ArgumentException>(() => ThrowException());",
            Line = 12,
            StartColumn = 13,
            EndColumn = 69
        }).SetName("Assert.Throws");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "void DoNotThrow() { }",
            NUnitAssertion = "Assert.DoesNotThrow(() => DoNotThrow());",
            ShouldlyAssertion = "Should.NotThrow(() => DoNotThrow());",
            Line = 12,
            StartColumn = 13,
            EndColumn = 52
        }).SetName("Assert.DoesNotThrow");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var expected = new List<int> { 1, 2, 3 }; var actual = new List<int> { 1, 2, 3 };",
            NUnitAssertion = "CollectionAssert.AreEqual(expected, actual);",
            ShouldlyAssertion = "actual.ShouldBe(expected);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 56
        }).SetName("CollectionAssert.AreEqual");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var expected = new List<int> { 1, 2, 3 }; var actual = new List<int> { 3, 2, 1 };",
            NUnitAssertion = "CollectionAssert.AreEquivalent(expected, actual);",
            ShouldlyAssertion = "actual.ShouldBe(expected, ignoreOrder: true);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 61
        }).SetName("CollectionAssert.AreEquivalent");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var collection = new List<string> { \"a\", \"b\", \"c\" };",
            NUnitAssertion = "CollectionAssert.AllItemsAreInstancesOfType(collection, typeof(string));",
            ShouldlyAssertion = "collection.ShouldAllBe(item => item is string);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 84
        }).SetName("CollectionAssert.AllItemsAreInstancesOfType");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var collection = new List<string> { \"a\", \"b\", \"c\" };",
            NUnitAssertion = "CollectionAssert.AllItemsAreNotNull(collection);",
            ShouldlyAssertion = "collection.ShouldNotContain(item => item == null);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 60
        }).SetName("CollectionAssert.AllItemsAreNotNull");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var collection = new List<int> { 1, 2, 3 };",
            NUnitAssertion = "CollectionAssert.AllItemsAreUnique(collection);",
            ShouldlyAssertion = "collection.ShouldBeUnique();",
            Line = 12,
            StartColumn = 13,
            EndColumn = 59
        }).SetName("CollectionAssert.AllItemsAreUnique");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestant = 1337;",
            NUnitAssertion = "Assert.That(contestant, Is.Not.EqualTo(1336));",
            ShouldlyAssertion = "contestant.ShouldNotBe(1336);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 58
        }).SetName("Assert.That with Is.Not.EqualTo");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestants = new List<int> { 1337, 2448, 3559 };",
            NUnitAssertion = "Assert.That(contestants, Has.Member(1337));",
            ShouldlyAssertion = "contestants.ShouldContain(1337);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 55
        }).SetName("Assert.That with Has.Member");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestants = new List<int> { 1337, 2448, 3559 };",
            NUnitAssertion = "Assert.That(contestants, Has.No.Member(1336));",
            ShouldlyAssertion = "contestants.ShouldNotContain(1336);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 58
        }).SetName("Assert.That with Has.No.Member");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestants = new List<int> { 1337, 2448, 3559 };",
            NUnitAssertion = "Assert.That(contestants, Is.Unique);",
            ShouldlyAssertion = "contestants.ShouldBeUnique();",
            Line = 12,
            StartColumn = 13,
            EndColumn = 48
        }).SetName("Assert.That with Is.Unique");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var greeting = \"Hello, World!\";",
            NUnitAssertion = "Assert.That(greeting, Does.Contain(\"World\"));",
            ShouldlyAssertion = "greeting.ShouldContain(\"World\");",
            Line = 12,
            StartColumn = 13,
            EndColumn = 57
        }).SetName("Assert.That with Does.Contain");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var greeting = \"Hello, World!\";",
            NUnitAssertion = "Assert.That(greeting, Does.StartWith(\"Hello\"));",
            ShouldlyAssertion = "greeting.ShouldStartWith(\"Hello\");",
            Line = 12,
            StartColumn = 13,
            EndColumn = 59
        }).SetName("Assert.That with Does.StartWith");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var greeting = \"Hello, World!\";",
            NUnitAssertion = "Assert.That(greeting, Does.EndWith(\"World!\"));",
            ShouldlyAssertion = "greeting.ShouldEndWith(\"World!\");",
            Line = 12,
            StartColumn = 13,
            EndColumn = 58
        }).SetName("Assert.That with Does.EndWith");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestants = new List<int>();",
            NUnitAssertion = "Assert.That(contestants, Is.Empty);",
            ShouldlyAssertion = "contestants.ShouldBeEmpty();",
            Line = 12,
            StartColumn = 13,
            EndColumn = 47
        }).SetName("Assert.That with Is.Empty");
    }


    [Test]
    [TestCaseSource(nameof(TestCases))]
    public async Task TestConversion(TestCase testCase)
    {
        var test = $@"
using NUnit.Framework;using System.Collections.Generic;using System;
using Shouldly;
namespace TestNamespace
{{
    public class TestClass
    {{
        [Test]
        public void TestMethod()
        {{
            {testCase.SetupCode}
            {testCase.NUnitAssertion}
        }}
    }}
}}";

        var expected = $@"
using NUnit.Framework;using System.Collections.Generic;using System;
using Shouldly;
namespace TestNamespace
{{
    public class TestClass
    {{
        [Test]
        public void TestMethod()
        {{
            {testCase.SetupCode}
            {testCase.ShouldlyAssertion}
        }}
    }}
}}";

        var codeFixTest = new CodeFixTest(test, expected,
            CSharpAnalyzerVerifier<NUnitToShouldlyAnalyzer, NUnitVerifier>
                .Diagnostic(NUnitToShouldlyAnalyzer.DiagnosticId)
                .WithSpan(testCase.Line, testCase.StartColumn, testCase.Line, testCase.EndColumn));

        await codeFixTest.RunAsync(CancellationToken.None);
        var compilerDiagnostics = codeFixTest.CompilerDiagnostics;

        // Add any additional assertions here if needed
    }
}