using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using NUnit.Framework;

namespace Shouldly.FromAssert.Tests;

public class NUnitToShouldlyConverterTestsAll
{
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
            ShouldlyAssertion = "greeting.ShouldContain(\"World\", Case.Sensitive);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 53
        }).SetName("StringAssert.Contains");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var greeting = \"Hello, World!\";",
            NUnitAssertion = "StringAssert.DoesNotContain(\"world\", greeting);",
            ShouldlyAssertion = "greeting.ShouldNotContain(\"world\", Case.Sensitive);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 59
        }).SetName("StringAssert.DoesNotContain");

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
            ShouldlyAssertion = "greeting.ShouldContain(\"World\", Case.Sensitive);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 57
        }).SetName("Assert.That with Does.Contain");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var greetings = new List<string> { \"Hello\", \"World\" };",
            NUnitAssertion = "Assert.That(greetings, Does.Contain(\"World\"));",
            ShouldlyAssertion = "greetings.ShouldContain(\"World\");",
            Line = 12,
            StartColumn = 13,
            EndColumn = 58
        }).SetName("Assert.That with Does.Contain on a collection");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "string? greeting = \"Hello, World!\";",
            NUnitAssertion = "Assert.That(greeting, Does.Contain(\"World\"));",
            ShouldlyAssertion = "greeting.ShouldContain(\"World\", Case.Sensitive);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 57
        }).SetName("Assert.That with Does.Contain on a nullable string");

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

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestant = 1337;",
            NUnitAssertion = "Assert.That(contestant, Is.EqualTo(1337), \"top caps the rows\");",
            ShouldlyAssertion = "contestant.ShouldBe(1337, \"top caps the rows\");",
            Line = 12,
            StartColumn = 13,
            EndColumn = 75
        }).SetName("Assert.That with a message");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestant = 1337;",
            NUnitAssertion = "Assert.That(contestant, Is.EqualTo(1337), message: \"top caps the rows\");",
            ShouldlyAssertion = "contestant.ShouldBe(1337, \"top caps the rows\");",
            Line = 12,
            StartColumn = 13,
            EndColumn = 84
        }).SetName("Assert.That with a named message");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "bool? contestant = false;",
            NUnitAssertion = "Assert.That(contestant, Is.Not.True, () => \"flag \" + contestant);",
            ShouldlyAssertion = "contestant.ShouldNotBe(true, \"flag \" + contestant);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 77
        }).SetName("Assert.That with a Func<string> message");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var greeting = \"Hello, World!\";",
            NUnitAssertion = "Assert.That(greeting, Does.StartWith(\"Hello\"), \"greets\");",
            ShouldlyAssertion = "greeting.ShouldStartWith(\"Hello\", customMessage: \"greets\");",
            Line = 12,
            StartColumn = 13,
            EndColumn = 69
        }).SetName("Assert.That with a message where the string overload needs it named");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestant = true;",
            NUnitAssertion = "Assert.That(contestant);",
            ShouldlyAssertion = "contestant.ShouldBeTrue();",
            Line = 12,
            StartColumn = 13,
            EndColumn = 36
        }).SetName("Assert.That with a bool");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestant = 1337;",
            NUnitAssertion = "Assert.That(contestant > 1000, \"too small\");",
            ShouldlyAssertion = "(contestant > 1000).ShouldBeTrue(\"too small\");",
            Line = 12,
            StartColumn = 13,
            EndColumn = 56
        }).SetName("Assert.That with a bool expression and message");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "string contestant = null;",
            NUnitAssertion = "Assert.That(contestant, Is.Null);",
            ShouldlyAssertion = "contestant.ShouldBeNull();",
            Line = 12,
            StartColumn = 13,
            EndColumn = 45
        }).SetName("Assert.That with Is.Null");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestant = \"1337\";",
            NUnitAssertion = "Assert.That(contestant, Is.Not.Null);",
            ShouldlyAssertion = "contestant.ShouldNotBeNull();",
            Line = 12,
            StartColumn = 13,
            EndColumn = 49
        }).SetName("Assert.That with Is.Not.Null");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestant = true;",
            NUnitAssertion = "Assert.That(contestant, Is.True);",
            ShouldlyAssertion = "contestant.ShouldBeTrue();",
            Line = 12,
            StartColumn = 13,
            EndColumn = 45
        }).SetName("Assert.That with Is.True");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "bool? contestant = true;",
            NUnitAssertion = "Assert.That(contestant, Is.True);",
            ShouldlyAssertion = "contestant.ShouldBe(true);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 45
        }).SetName("Assert.That with Is.True on a nullable bool");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestant = false;",
            NUnitAssertion = "Assert.That(contestant, Is.False);",
            ShouldlyAssertion = "contestant.ShouldBeFalse();",
            Line = 12,
            StartColumn = 13,
            EndColumn = 46
        }).SetName("Assert.That with Is.False");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestant = false;",
            NUnitAssertion = "Assert.That(contestant, Is.Not.True);",
            ShouldlyAssertion = "contestant.ShouldNotBe(true);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 49
        }).SetName("Assert.That with Is.Not.True");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestant = 0L;",
            NUnitAssertion = "Assert.That(contestant, Is.Zero);",
            ShouldlyAssertion = "contestant.ShouldBe(0);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 45
        }).SetName("Assert.That with Is.Zero");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestants = new List<int> { 1, 3, 3, 7 };",
            NUnitAssertion = "Assert.That(contestants, Has.Count.EqualTo(4));",
            ShouldlyAssertion = "contestants.Count.ShouldBe(4);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 59
        }).SetName("Assert.That with Has.Count.EqualTo");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestants = new[] { 1, 3, 3, 7 };",
            NUnitAssertion = "Assert.That(contestants, Has.Count.EqualTo(4));",
            ShouldlyAssertion = "contestants.Length.ShouldBe(4);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 59
        }).SetName("Assert.That with Has.Count.EqualTo on an array");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var greeting = \"Hello, World!\";",
            NUnitAssertion = "Assert.That(greeting, Does.Not.Contain(\"world\"));",
            ShouldlyAssertion = "greeting.ShouldNotContain(\"world\", Case.Sensitive);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 61
        }).SetName("Assert.That with Does.Not.Contain on a string");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestants = new List<int> { 1, 3, 3, 7 };",
            NUnitAssertion = "Assert.That(contestants, Does.Not.Contain(42));",
            ShouldlyAssertion = "contestants.ShouldNotContain(42);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 59
        }).SetName("Assert.That with Does.Not.Contain on a collection");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestants = new List<int> { 1, 3, 3, 7 };",
            NUnitAssertion = "Assert.That(contestants, Is.EquivalentTo(new[] { 7, 3, 3, 1 }));",
            ShouldlyAssertion = "contestants.ShouldBe(new[] { 7, 3, 3, 1 }, ignoreOrder: true);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 76
        }).SetName("Assert.That with Is.EquivalentTo");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestants = new List<int> { 7, 7 };",
            NUnitAssertion = "Assert.That(contestants, Is.All.EqualTo(7));",
            ShouldlyAssertion = "contestants.ShouldAllBe(item => item == 7);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 56
        }).SetName("Assert.That with Is.All.EqualTo");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var version = new Version(1, 0); var contestants = new List<Version> { new Version(1, 0) };",
            NUnitAssertion = "Assert.That(contestants, Has.All.EqualTo(version));",
            ShouldlyAssertion = "contestants.ShouldAllBe(item => object.Equals(item, version));",
            Line = 12,
            StartColumn = 13,
            EndColumn = 63
        }).SetName("Assert.That with Has.All.EqualTo on a reference type");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestant = new object(); var other = contestant;",
            NUnitAssertion = "Assert.That(contestant, Is.SameAs(other));",
            ShouldlyAssertion = "contestant.ShouldBeSameAs(other);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 54
        }).SetName("Assert.That with Is.SameAs");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestants = new List<string> { null };",
            NUnitAssertion = "Assert.That(contestants, Is.All.Null);",
            ShouldlyAssertion = "contestants.ShouldAllBe(item => item == null);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 50
        }).SetName("Assert.That with Is.All.Null");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestants = new List<int> { 1, 3 };",
            NUnitAssertion = "Assert.That(contestants, Has.All.Matches<int>(x => x > 0));",
            ShouldlyAssertion = "contestants.ShouldAllBe(x => x > 0);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 71
        }).SetName("Assert.That with Has.All.Matches");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestants = new List<int> { 1, 3 };",
            NUnitAssertion = "Assert.That(contestants, Has.None.Matches<int>(x => x < 0));",
            ShouldlyAssertion = "contestants.ShouldNotContain(x => x < 0);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 72
        }).SetName("Assert.That with Has.None.Matches");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestants = new List<int> { 1, 3 }; Predicate<int> positive = x => x > 0;",
            NUnitAssertion = "Assert.That(contestants, Has.All.Matches(positive));",
            ShouldlyAssertion = "contestants.ShouldAllBe(item => positive(item));",
            Line = 12,
            StartColumn = 13,
            EndColumn = 64
        }).SetName("Assert.That with Has.All.Matches on a predicate variable");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestant = 1337;",
            NUnitAssertion = "Assert.That(contestant, Is.InRange(1000, 2000));",
            ShouldlyAssertion = "contestant.ShouldBeInRange(1000, 2000);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 60
        }).SetName("Assert.That with Is.InRange");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestants = new List<int> { 7, 3, 1 };",
            NUnitAssertion = "Assert.That(contestants, Is.Ordered.Descending);",
            ShouldlyAssertion = "contestants.ShouldBeInOrder(SortDirection.Descending);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 60
        }).SetName("Assert.That with Is.Ordered.Descending");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestant = \"1337\";",
            NUnitAssertion = "Assert.That(contestant, Does.Match(\"^13\"));",
            ShouldlyAssertion = "contestant.ShouldMatch(\"^13\");",
            Line = 12,
            StartColumn = 13,
            EndColumn = 55
        }).SetName("Assert.That with Does.Match");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestants = new List<bool> { false };",
            NUnitAssertion = "Assert.That(contestants, Has.All.False);",
            ShouldlyAssertion = "contestants.ShouldAllBe(item => !item);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 52
        }).SetName("Assert.That with Has.All.False");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestant = new { MediaType = \"image/png\" };",
            NUnitAssertion = "Assert.That(contestant?.MediaType, Is.EqualTo(\"image/png\"));",
            ShouldlyAssertion = "(contestant?.MediaType).ShouldBe(\"image/png\");",
            Line = 12,
            StartColumn = 13,
            EndColumn = 72
        }).SetName("Assert.That with null-conditional receiver");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestant = 1337;",
            NUnitAssertion = "Assert.AreEqual(true, contestant != 0 && contestant > 1000);",
            ShouldlyAssertion = "(contestant != 0 && contestant > 1000).ShouldBe(true);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 72
        }).SetName("Assert.AreEqual with binary receiver");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestant = 1337;",
            NUnitAssertion = "Assert.IsTrue(contestant > 0 ? contestant < 2000 : false);",
            ShouldlyAssertion = "(contestant > 0 ? contestant < 2000 : false).ShouldBeTrue();",
            Line = 12,
            StartColumn = 13,
            EndColumn = 70
        }).SetName("Assert.IsTrue with conditional receiver");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestants = new List<int> { 1 };",
            NUnitAssertion = "Assert.That(contestants, Is.Not.Empty);",
            ShouldlyAssertion = "contestants.ShouldNotBeEmpty();",
            Line = 12,
            StartColumn = 13,
            EndColumn = 51
        }).SetName("Assert.That with Is.Not.Empty");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestant = 1337;",
            NUnitAssertion = "Assert.That(contestant, Is.GreaterThan(1000));",
            ShouldlyAssertion = "contestant.ShouldBeGreaterThan(1000);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 58
        }).SetName("Assert.That with Is.GreaterThan");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestant = 1337;",
            NUnitAssertion = "Assert.That(contestant, Is.GreaterThanOrEqualTo(1337));",
            ShouldlyAssertion = "contestant.ShouldBeGreaterThanOrEqualTo(1337);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 67
        }).SetName("Assert.That with Is.GreaterThanOrEqualTo");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestant = 0.9;",
            NUnitAssertion = "Assert.That(contestant, Is.LessThan(0.95));",
            ShouldlyAssertion = "contestant.ShouldBeLessThan(0.95);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 55
        }).SetName("Assert.That with Is.LessThan");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestant = 1337;",
            NUnitAssertion = "Assert.That(contestant, Is.LessThanOrEqualTo(1337));",
            ShouldlyAssertion = "contestant.ShouldBeLessThanOrEqualTo(1337);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 64
        }).SetName("Assert.That with Is.LessThanOrEqualTo");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestant = \"EBG\";",
            NUnitAssertion = "Assert.That(contestant, Has.Length.LessThanOrEqualTo(3));",
            ShouldlyAssertion = "contestant.Length.ShouldBeLessThanOrEqualTo(3);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 69
        }).SetName("Assert.That with Has.Length.LessThanOrEqualTo");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestant = \"EBG\";",
            NUnitAssertion = "Assert.That(contestant.Trim(), Has.Length.GreaterThan(0));",
            ShouldlyAssertion = "contestant.Trim().Length.ShouldBeGreaterThan(0);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 70
        }).SetName("Assert.That with Has.Length on invocation receiver");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestants = new List<int> { 1337 };",
            NUnitAssertion = "Assert.That(contestants, Has.Count.GreaterThanOrEqualTo(1));",
            ShouldlyAssertion = "contestants.Count.ShouldBeGreaterThanOrEqualTo(1);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 72
        }).SetName("Assert.That with Has.Count.GreaterThanOrEqualTo");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var contestant = \"EBG\";",
            NUnitAssertion = "Assert.That(contestant ?? \"\", Has.Length.LessThan(4));",
            ShouldlyAssertion = "(contestant ?? \"\").Length.ShouldBeLessThan(4);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 66
        }).SetName("Assert.That with Has.Length on binary receiver");
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

    [Test]
    public async Task AssertThatWithAndChain_SplitsIntoOneAssertPerLink()
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
            var path = ""/img/logo.png"";
            // the path is a png under root
            Assert.That(path, Does.StartWith(""/"").And.EndWith("".png""));
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
            var path = ""/img/logo.png"";
            // the path is a png under root
            path.ShouldStartWith(""/"");
            path.ShouldEndWith("".png"");
        }
    }
}";

        await new CodeFixTest(test, expected,
                CSharpAnalyzerVerifier<NUnitToShouldlyAnalyzer, NUnitVerifier>
                    .Diagnostic(NUnitToShouldlyAnalyzer.DiagnosticId)
                    .WithSpan(13, 13, 13, 71))
            .RunAsync(CancellationToken.None);
    }

    [Test]
    public async Task AssertThatWithAndChainOfStringContains_KeepsCaseSensitivity()
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
            var path = ""/img/logo.png"";
            Assert.That(path, Does.Contain(""img"").And.Contain(""logo""));
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
            var path = ""/img/logo.png"";
            path.ShouldContain(""img"", Case.Sensitive);
            path.ShouldContain(""logo"", Case.Sensitive);
        }
    }
}";

        await new CodeFixTest(test, expected,
                CSharpAnalyzerVerifier<NUnitToShouldlyAnalyzer, NUnitVerifier>
                    .Diagnostic(NUnitToShouldlyAnalyzer.DiagnosticId)
                    .WithSpan(12, 13, 12, 71))
            .RunAsync(CancellationToken.None);
    }

    [Test]
    public async Task AssertThatWithAndChainAndMessage_PassesTheMessageToEveryLink()
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
            var contestant = 1337;
            Assert.That(contestant, Is.Not.Zero.And.EqualTo(1337), ""leet"");
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
            contestant.ShouldNotBe(0, ""leet"");
            contestant.ShouldBe(1337, ""leet"");
        }
    }
}";

        await new CodeFixTest(test, expected,
                CSharpAnalyzerVerifier<NUnitToShouldlyAnalyzer, NUnitVerifier>
                    .Diagnostic(NUnitToShouldlyAnalyzer.DiagnosticId)
                    .WithSpan(12, 13, 12, 75))
            .RunAsync(CancellationToken.None);
    }

    [Test]
    public async Task AssertThatWithAndChainInEmbeddedStatement_WrapsInBlock()
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
            var contestants = new System.Collections.Generic.List<int> { 1, 3, 3, 7 };
            if (contestants.Count > 0)
                Assert.That(contestants, Has.Member(1).And.Member(7));
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
            var contestants = new System.Collections.Generic.List<int> { 1, 3, 3, 7 };
            if (contestants.Count > 0)
            {
                contestants.ShouldContain(1);
                contestants.ShouldContain(7);
            }
        }
    }
}";

        await new CodeFixTest(test, expected,
                CSharpAnalyzerVerifier<NUnitToShouldlyAnalyzer, NUnitVerifier>
                    .Diagnostic(NUnitToShouldlyAnalyzer.DiagnosticId)
                    .WithSpan(13, 17, 13, 70))
            .RunAsync(CancellationToken.None);
    }

    [Test]
    public async Task AssertThatWithConditionalConstraint_BecomesIfElse()
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
            var expected = true;
            var candidate = new object();
            var picked = candidate;
            Assert.That(picked, expected ? Is.SameAs(candidate) : Is.Null);
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
            var expected = true;
            var candidate = new object();
            var picked = candidate;
            if (expected)
            {
                picked.ShouldBeSameAs(candidate);
            }
            else
            {
                picked.ShouldBeNull();
            }
        }
    }
}";

        await new CodeFixTest(test, expected,
                CSharpAnalyzerVerifier<NUnitToShouldlyAnalyzer, NUnitVerifier>
                    .Diagnostic(NUnitToShouldlyAnalyzer.DiagnosticId)
                    .WithSpan(14, 13, 14, 75))
            .RunAsync(CancellationToken.None);
    }

    [Test]
    public async Task AssertThatWithMessageInLambda_ReplacesTheExpression()
    {
        var test = @"
using System;
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
            Action check = () => Assert.That(contestant, Is.Null, ""unset"");
            check();
        }
    }
}";

        var expected = @"
using System;
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
            Action check = () => contestant.ShouldBeNull(""unset"");
            check();
        }
    }
}";

        await new CodeFixTest(test, expected,
                CSharpAnalyzerVerifier<NUnitToShouldlyAnalyzer, NUnitVerifier>
                    .Diagnostic(NUnitToShouldlyAnalyzer.DiagnosticId)
                    .WithSpan(13, 34, 13, 75))
            .RunAsync(CancellationToken.None);
    }

    [Test]
    public async Task UnqualifiedMethodStartingWithAssert_IsNotFlagged()
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
            AssertNoUnionTypedEnum(1337, ""auction"");
        }

        private static void AssertNoUnionTypedEnum(int node, string path) { }
    }
}";

        await new CodeFixTest(test, test).RunAsync(CancellationToken.None);
    }

    [Test]
    public async Task UserDefinedAssertClass_IsNotFlagged()
    {
        var test = @"
namespace TestNamespace
{
    public static class Assert
    {
        public static void AreEqual(int expected, int actual) { }
    }

    public class TestClass
    {
        public void TestMethod()
        {
            Assert.AreEqual(1337, 1337);
        }
    }
}";

        await new CodeFixTest(test, test).RunAsync(CancellationToken.None);
    }

    [Test]
    public async Task UsingStaticNUnitAssert_IsFlagged()
    {
        var test = @"
using NUnit.Framework;
using static NUnit.Framework.Assert;
namespace TestNamespace
{
    public class TestClass
    {
        [Test]
        public void TestMethod()
        {
            That(1337, Is.EqualTo(1337));
        }
    }
}";

        var analyzerTest = new CSharpAnalyzerTest<NUnitToShouldlyAnalyzer, NUnitVerifier>
        {
            TestCode = test,
            ReferenceAssemblies = CodeFixTest.References
        };
        analyzerTest.ExpectedDiagnostics.Add(
            CSharpAnalyzerVerifier<NUnitToShouldlyAnalyzer, NUnitVerifier>
                .Diagnostic(NUnitToShouldlyAnalyzer.DiagnosticId)
                .WithSpan(11, 13, 11, 41));

        await analyzerTest.RunAsync(CancellationToken.None);
    }

    private static IEnumerable<TestCaseData> AssertMultipleTestCases()
    {
        yield return new TestCaseData(
            @"var contestant = 1337;
            var name = ""Joel"";
            [|Assert.Multiple(() =>
            {
                [|Assert.That(contestant, Is.EqualTo(1337))|];
                [|Assert.IsNotNull(name)|];
            })|];",
            @"var contestant = 1337;
            var name = ""Joel"";
            this.ShouldSatisfyAllConditions(
                () => contestant.ShouldBe(1337),
                () => name.ShouldNotBeNull());"
        ).SetName("Assert.Multiple converts each inner assertion into a condition");

        yield return new TestCaseData(
            @"var contestants = new List<int> { 1 };
            [|Assert.Multiple(() =>
            {
                [|Assert.That(contestants, Has.Exactly(1).Items)|];
                contestants.ShouldNotBeEmpty();
            })|];",
            @"var contestants = new List<int> { 1 };
            this.ShouldSatisfyAllConditions(
                () => [|Assert.That(contestants, Has.Exactly(1).Items)|],
                () => contestants.ShouldNotBeEmpty());"
        ).SetName("Assert.Multiple keeps statements it cannot convert");

        yield return new TestCaseData(
            @"var contestant = 1337;
            [|Assert.Multiple(() => [|Assert.AreEqual(1337, contestant)|])|];",
            @"var contestant = 1337;
            this.ShouldSatisfyAllConditions(
                () => contestant.ShouldBe(1337));"
        ).SetName("Assert.Multiple with an expression-bodied lambda");
    }

    [Test]
    [TestCaseSource(nameof(AssertMultipleTestCases))]
    public async Task TestAssertMultipleConversion(string before, string after)
    {
        var codeFixTest = new CodeFixTest(WrapInTestMethod(before), WrapInTestMethod(after));
        // Statements left as NUnit asserts are still reported after the fix.
        codeFixTest.FixedState.MarkupHandling = MarkupMode.Allow;

        await codeFixTest.RunAsync(CancellationToken.None);
    }

    private static IEnumerable<TestCaseData> UnconvertibleAssertMultipleTestCases()
    {
        yield return new TestCaseData(
            "public async Task TestMethod()",
            @"var contestant = 1337;
            Assert.Multiple(async () =>
            {
                await Task.Yield();
                [|Assert.That(contestant, Is.EqualTo(1337))|];
            });"
        ).SetName("Assert.Multiple with an async lambda is not reported");

        yield return new TestCaseData(
            "public void TestMethod()",
            @"Assert.Multiple(() =>
            {
                var contestant = 1337;
                [|Assert.That(contestant, Is.EqualTo(1337))|];
            });"
        ).SetName("Assert.Multiple declaring a local is not reported");

        yield return new TestCaseData(
            "public static void TestMethod()",
            @"var contestant = 1337;
            Assert.Multiple(() =>
            {
                [|Assert.That(contestant, Is.EqualTo(1337))|];
            });"
        ).SetName("Assert.Multiple in a static method is not reported");
    }

    [Test]
    [TestCaseSource(nameof(UnconvertibleAssertMultipleTestCases))]
    public async Task TestUnconvertibleAssertMultipleIsNotReported(string signature, string body)
    {
        var test = new CSharpAnalyzerTest<NUnitToShouldlyAnalyzer, NUnitVerifier>
        {
            TestCode = WrapInTestMethod(body, signature),
            ReferenceAssemblies = TestReferenceAssemblies
        };

        await test.RunAsync(CancellationToken.None);
    }

    private static string WrapInTestMethod(string body, string signature = "public void TestMethod()") => $@"
using NUnit.Framework;using System.Collections.Generic;using System;using System.Threading.Tasks;
using Shouldly;
namespace TestNamespace
{{
    public class TestClass
    {{
        [Test]
        {signature}
        {{
            {body}
        }}
    }}
}}";

    private static readonly ReferenceAssemblies TestReferenceAssemblies = ReferenceAssemblies.Default
        .AddPackages(ImmutableArray.Create(
                new PackageIdentity("Shouldly", "4.2.1"),
                new PackageIdentity("NUnit", "3.14.0")
            )
        );

    [Test]
    public async Task AwaitReceiver_IsParenthesised()
    {
        var test = @"
using NUnit.Framework;using System.Threading.Tasks;
using Shouldly;
namespace TestNamespace
{
    public class TestClass
    {
        [Test]
        public async Task TestMethod()
        {
            Assert.That(await GetStatusAsync(), Is.EqualTo(""Ordered""));
        }

        private static Task<string> GetStatusAsync() => Task.FromResult(""Ordered"");
    }
}";

        var expected = @"
using NUnit.Framework;using System.Threading.Tasks;
using Shouldly;
namespace TestNamespace
{
    public class TestClass
    {
        [Test]
        public async Task TestMethod()
        {
            (await GetStatusAsync()).ShouldBe(""Ordered"");
        }

        private static Task<string> GetStatusAsync() => Task.FromResult(""Ordered"");
    }
}";

        await new CodeFixTest(test, expected,
                CSharpAnalyzerVerifier<NUnitToShouldlyAnalyzer, NUnitVerifier>
                    .Diagnostic(NUnitToShouldlyAnalyzer.DiagnosticId)
                    .WithSpan(11, 13, 11, 71))
            .RunAsync(CancellationToken.None);
    }

    private class
        CodeFixTest :CSharpCodeFixTest<NUnitToShouldlyAnalyzer, NUnitToShouldlyCodeFixProvider, NUnitVerifier>
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

    public class TestCase
    {
        public string NUnitAssertion { get; set; }
        public string ShouldlyAssertion { get; set; }
        public int Line { get; set; }
        public int StartColumn { get; set; }
        public int EndColumn { get; set; }
        public string SetupCode { get; set; }
    }
}