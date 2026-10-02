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
            SetupCode = "List<int> contestants = null; var fallback = new List<int> { 1, 3, 3, 7 };",
            NUnitAssertion = "Assert.That(contestants ?? fallback, Has.Count.EqualTo(4));",
            ShouldlyAssertion = "(contestants ?? fallback).Count.ShouldBe(4);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 71
        }).SetName("Assert.That with Has.Count.EqualTo on a coalesce expression");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "var item = new Version(1, 0); var contestants = new List<Version> { item };",
            NUnitAssertion = "Assert.That(contestants, Has.All.EqualTo(item));",
            ShouldlyAssertion = "contestants.ShouldAllBe(item1 => object.Equals(item1, item));",
            Line = 12,
            StartColumn = 13,
            EndColumn = 60
        }).SetName("Assert.That with Has.All.EqualTo does not shadow a local named item");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "string contestant = null;",
            NUnitAssertion = "Assert.That(contestant, (Is.Null));",
            ShouldlyAssertion = "contestant.ShouldBeNull();",
            Line = 12,
            StartColumn = 13,
            EndColumn = 47
        }).SetName("Assert.That with a parenthesised constraint");

        yield return new TestCaseData(new TestCase
        {
            SetupCode = "",
            NUnitAssertion = "Assert.That(\"1337\", Has.Length.GreaterThan(3));",
            ShouldlyAssertion = "\"1337\".Length.ShouldBeGreaterThan(3);",
            Line = 12,
            StartColumn = 13,
            EndColumn = 59
        }).SetName("Assert.That with Has.Length.GreaterThan on a literal is not parenthesised");

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

    [Test]
    public async Task UsingStaticMultipleInStaticMethod_IsNotFlagged()
    {
        var test = @"
using NUnit.Framework;
using static NUnit.Framework.Assert;
namespace TestNamespace
{
    public class TestClass
    {
        [Test]
        public static void TestMethod()
        {
            Multiple(() =>
            {
                [|Assert.That(1337, Is.EqualTo(1337))|];
            });
        }
    }
}";

        var analyzerTest = new CSharpAnalyzerTest<NUnitToShouldlyAnalyzer, NUnitVerifier>
        {
            TestCode = test,
            ReferenceAssemblies = CodeFixTest.References
        };

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

        yield return new TestCaseData(
            @"var contestant = 1337;
            [|Assert.Multiple(() =>
            {
                [|Assert.That(contestant, Is.EqualTo(1337), ""leet"")|];
                [|Assert.That(contestant > 1000)|];
                [|Assert.That(contestant, Is.GreaterThan(1000).And.LessThan(2000))|];
            })|];",
            @"var contestant = 1337;
            this.ShouldSatisfyAllConditions(
                () => contestant.ShouldBe(1337, ""leet""),
                () => (contestant > 1000).ShouldBeTrue(),
                () => contestant.ShouldBeGreaterThan(1000),
                () => contestant.ShouldBeLessThan(2000));"
        ).SetName("Assert.Multiple converts inner asserts with a message, a bare bool and an And chain");

        yield return new TestCaseData(
            @"var contestant = 1337;
            [|NUnit.Framework.Assert.Multiple(() =>
            {
                [|Assert.That(contestant, Is.EqualTo(1337))|];
            })|];",
            @"var contestant = 1337;
            this.ShouldSatisfyAllConditions(
                () => contestant.ShouldBe(1337));"
        ).SetName("Assert.Multiple fully qualified");

        yield return new TestCaseData(
            @"var name = ""Joel"";
            var uris = new List<Uri> { new Uri(""https://example.com"") };
            [|Assert.Multiple(() =>
            {
                [|Assert.That(name, Is.Not.Null)|];
                [|Assert.That(uris, Has.Count.EqualTo(1))|];
            })|];",
            @"var name = ""Joel"";
            var uris = new List<Uri> { new Uri(""https://example.com"") };
            this.ShouldSatisfyAllConditions(
                () => { name.ShouldNotBeNull(); },
                () => uris.Count.ShouldBe(1));"
        ).SetName("Assert.Multiple gives a first condition that returns a string a block body");

        yield return new TestCaseData(
            @"var name = ""Joel"";
            [|Assert.Multiple(() => [|Assert.IsNotNull(name)|])|];",
            @"var name = ""Joel"";
            this.ShouldSatisfyAllConditions(
                () => { name.ShouldNotBeNull(); });"
        ).SetName("Assert.Multiple with a single condition that returns a string");

        yield return new TestCaseData(
            @"object name = ""Joel"";
            [|Assert.Multiple(() =>
            {
                [|Assert.That(name, Is.TypeOf(typeof(string)))|];
                [|Assert.IsNotNull(name)|];
            })|];",
            @"object name = ""Joel"";
            this.ShouldSatisfyAllConditions(
                () => { name.ShouldBeOfType<string>(); },
                () => name.ShouldNotBeNull());"
        ).SetName("Assert.Multiple gives a first ShouldBeOfType<string> a block body");
    }

    [Test]
    [TestCaseSource(nameof(AssertMultipleTestCases))]
    public async Task TestAssertMultipleConversion(string before, string after)
    {
        var codeFixTest = new CodeFixTest(WrapInTestMethod(before), WrapInTestMethod(after));
        // Statements left as NUnit asserts are still reported after the fix.
        codeFixTest.FixedState.MarkupHandling = MarkupMode.Allow;
        // ...so fix-all takes a second, no-op pass over them once the Assert.Multiple around them has gone.
        if (after.Contains("[|")) codeFixTest.NumberOfFixAllIterations = 2;

        await codeFixTest.RunAsync(CancellationToken.None);
    }

    // dotnet format runs the fix-all once, so an Assert.Multiple, the asserts inside it and the asserts around it
    // must all convert in a single pass (#32).
    [Test]
    public async Task AssertMultiple_FixAllConvertsInOnePass()
    {
        await new CodeFixTest(
                WrapInTestMethod(@"var contestant = 1337;
            var name = ""Joel"";
            [|Assert.That(contestant, Is.GreaterThan(1000))|];
            [|Assert.Multiple(() =>
            {
                [|Assert.That(contestant, Is.EqualTo(1337))|];
                [|Assert.AreEqual(""Joel"", name)|];
                [|Assert.IsNotNull(name)|];
            })|];
            [|Assert.Multiple(() => [|Assert.AreEqual(1337, contestant)|])|];
            [|Assert.IsTrue(contestant > 1000)|];"),
                WrapInTestMethod(@"var contestant = 1337;
            var name = ""Joel"";
            contestant.ShouldBeGreaterThan(1000);
            this.ShouldSatisfyAllConditions(
                () => contestant.ShouldBe(1337),
                () => name.ShouldBe(""Joel""),
                () => name.ShouldNotBeNull());
            this.ShouldSatisfyAllConditions(
                () => contestant.ShouldBe(1337));
            (contestant > 1000).ShouldBeTrue();"))
            {
                NumberOfFixAllIterations = 1,
                CodeFixTestBehaviors = CodeFixTestBehaviors.SkipFixAllInProjectCheck | CodeFixTestBehaviors.SkipFixAllInSolutionCheck
            }
            .RunAsync(CancellationToken.None);
    }

    // Runner control, not assertions: there is nothing in Shouldly to convert them to (#32).
    [TestCase("Assert.Ignore(\"not yet\");")]
    [TestCase("Assert.Pass();")]
    [TestCase("Assert.Inconclusive(\"no data\");")]
    [TestCase("Assert.Warn(\"slow\");")]
    public async Task RunnerControl_IsNotFlagged(string statement)
    {
        await new CSharpAnalyzerTest<NUnitToShouldlyAnalyzer, NUnitVerifier>
            {
                TestCode = WrapInTestMethod(statement),
                ReferenceAssemblies = CodeFixTest.References
            }
            .RunAsync(CancellationToken.None);
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

        yield return new TestCaseData(
            "public static void TestMethod()",
            @"var contestant = 1337;
            NUnit.Framework.Assert.Multiple(() =>
            {
                [|Assert.That(contestant, Is.EqualTo(1337))|];
            });"
        ).SetName("Fully qualified Assert.Multiple in a static method is not reported");
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

    private static IEnumerable<TestCaseData> NullableReceiverTestCases()
    {
        yield return new TestCaseData(
            @"[|Assert.That(report.Error, Does.Contain(""no route to host""))|];",
            @"report.Error.ShouldNotBeNull().ShouldContain(""no route to host"", Case.Sensitive);"
        ).SetName("Does.Contain on a string? property chains ShouldNotBeNull");

        yield return new TestCaseData(
            @"var error = GetError();
            [|Assert.That(error, Does.StartWith(""no""))|];",
            @"var error = GetError();
            error.ShouldNotBeNull().ShouldStartWith(""no"");"
        ).SetName("Does.StartWith on a string? local chains ShouldNotBeNull");

        yield return new TestCaseData(
            @"[|Assert.That(report.Error, Does.EndWith(""host""))|];",
            @"report.Error.ShouldNotBeNull().ShouldEndWith(""host"");"
        ).SetName("Does.EndWith on a string? chains ShouldNotBeNull");

        yield return new TestCaseData(
            @"[|Assert.That(report.Error, Does.Match(""^no""))|];",
            @"report.Error.ShouldNotBeNull().ShouldMatch(""^no"");"
        ).SetName("Does.Match on a string? chains ShouldNotBeNull");

        yield return new TestCaseData(
            @"[|StringAssert.Contains(""route"", report.Error)|];",
            @"report.Error.ShouldNotBeNull().ShouldContain(""route"", Case.Sensitive);"
        ).SetName("StringAssert.Contains on a string? chains ShouldNotBeNull");

        yield return new TestCaseData(
            @"[|Assert.That(report.Error, Does.Contain(""route""), ""the error names the cause"")|];",
            @"report.Error.ShouldNotBeNull().ShouldContain(""route"", Case.Sensitive, ""the error names the cause"");"
        ).SetName("Does.Contain on a string? with a message chains ShouldNotBeNull");

        yield return new TestCaseData(
            @"[|Assert.That(report.Error, Does.StartWith(""no"").And.Contain(""route""))|];",
            @"report.Error.ShouldNotBeNull().ShouldStartWith(""no"");
            report.Error.ShouldNotBeNull().ShouldContain(""route"", Case.Sensitive);"
        ).SetName("And chain on a string? chains ShouldNotBeNull on every link");

        yield return new TestCaseData(
            @"[|Assert.Multiple(() =>
            {
                [|Assert.That(report.Error, Does.Contain(""route""))|];
                [|Assert.That(report.Error, Is.Not.Null)|];
            })|];",
            @"this.ShouldSatisfyAllConditions(
                () => report.Error.ShouldNotBeNull().ShouldContain(""route"", Case.Sensitive),
                () => report.Error.ShouldNotBeNull());"
        ).SetName("Does.Contain on a string? inside Assert.Multiple chains ShouldNotBeNull");

        yield return new TestCaseData(
            @"[|Assert.That(report.Name, Does.Contain(""dash""))|];",
            @"report.Name.ShouldContain(""dash"", Case.Sensitive);"
        ).SetName("Does.Contain on a non-nullable string is unchanged");

        yield return new TestCaseData(
            @"string? error = ""no route to host"";
            [|Assert.That(error, Does.Contain(""route""))|];",
            @"string? error = ""no route to host"";
            error.ShouldContain(""route"", Case.Sensitive);"
        ).SetName("Does.Contain on a string? known to be non-null is unchanged");

        yield return new TestCaseData(
            @"[|Assert.That(report.Tags, Does.Contain(""dash""))|];",
            @"report.Tags.ShouldContain(""dash"");"
        ).SetName("Does.Contain on a collection is unchanged");
    }

    [Test]
    [TestCaseSource(nameof(NullableReceiverTestCases))]
    public async Task TestNullableReceiverConversion(string before, string after)
    {
        // Warnings are compared too, so a fix that leaves CS8604 behind fails.
        var codeFixTest = new CodeFixTest(WrapInNullableTestMethod(before), WrapInNullableTestMethod(after))
        {
            CompilerDiagnostics = CompilerDiagnostics.Warnings
        };

        await codeFixTest.RunAsync(CancellationToken.None);
    }

    // NUnit's Does.Not.Contain passes on null, so chaining ShouldNotBeNull would make the assert stricter.
    [Test]
    public async Task DoesNotContainOnNullableString_DoesNotChainShouldNotBeNull()
    {
        await new CodeFixTest(
                WrapInNullableTestMethod(@"[|Assert.That(report.Error, Does.Not.Contain(""route""))|];"),
                WrapInNullableTestMethod(@"report.Error.ShouldNotContain(""route"", Case.Sensitive);"))
            .RunAsync(CancellationToken.None);
    }

    private static string WrapInNullableTestMethod(string body) => $@"#nullable enable
#pragma warning disable CS1591
using NUnit.Framework;using System.Collections.Generic;
using Shouldly;
namespace TestNamespace
{{
    public class Report
    {{
        public string? Error {{ get; set; }}
        public string Name {{ get; set; }} = """";
        public List<string> Tags {{ get; }} = new List<string>();
    }}

    public class TestClass
    {{
        [Test]
        public void TestMethod()
        {{
            var report = new Report {{ Error = GetError() }};
            {body}
        }}

        private static string? GetError() => ""no route to host"";
    }}
}}";

    // Forms that were reported but never converted (#34), each from a real suite.
    private static IEnumerable<TestCaseData> PreviouslyUnconvertedFormTestCases()
    {
        yield return new TestCaseData(
            @"[|Assert.That(sample.Ratio, Is.EqualTo(0.8).Within(0.001))|];",
            @"sample.Ratio.ShouldBe(0.8, 0.001);"
        ).SetName("Is.EqualTo.Within on a double");

        yield return new TestCaseData(
            @"[|Assert.That(sample.To, Is.EqualTo(sample.From)
                .Within(TimeSpan.FromSeconds(1)), ""the windows meet"")|];",
            @"sample.To.ShouldBe(sample.From, TimeSpan.FromSeconds(1), ""the windows meet"");"
        ).SetName("Is.EqualTo.Within on a DateTimeOffset with a message");

        yield return new TestCaseData(
            @"[|Assert.That(sample.Median, Is.EqualTo(TimeSpan.FromHours(1)).Within(TimeSpan.FromSeconds(1)))|];",
            @"sample.Median.ShouldNotBeNull().ShouldBe(TimeSpan.FromHours(1), TimeSpan.FromSeconds(1));"
        ).SetName("Is.EqualTo.Within on a nullable actual asserts non-null first");

        yield return new TestCaseData(
            @"double? expected = 0.8;
            [|Assert.That(sample.Ratio, Is.EqualTo(expected).Within(0.001))|];",
            @"double? expected = 0.8;
            sample.Ratio.ShouldBe(expected.Value, 0.001);"
        ).SetName("Is.EqualTo.Within with a nullable expected unwraps it");

        yield return new TestCaseData(
            @"var refused = [|Assert.ThrowsAsync<InvalidOperationException>(() => RefuseAsync())|];",
            @"var refused = Should.Throw<InvalidOperationException>(() => RefuseAsync());"
        ).SetName("Assert.ThrowsAsync");

        yield return new TestCaseData(
            @"[|Assert.DoesNotThrowAsync(() => Task.CompletedTask)|];",
            @"Should.NotThrow(() => Task.CompletedTask);"
        ).SetName("Assert.DoesNotThrowAsync");

        yield return new TestCaseData(
            @"[|Assert.Fail($""no {sample.Ratio} snapshot"")|];",
            @"throw new ShouldAssertException($""no {sample.Ratio} snapshot"");"
        ).SetName("Assert.Fail with a message");

        yield return new TestCaseData(
            @"if (sample.Lines.Count == 0) [|Assert.Fail()|];",
            @"if (sample.Lines.Count == 0) throw new ShouldAssertException(null);"
        ).SetName("Assert.Fail without a message as an embedded statement");

        yield return new TestCaseData(
            @"[|Assert.That(sample.Lines, Has.None.EqualTo(""Price drop""), ""never posted"")|];",
            @"sample.Lines.ShouldNotContain(""Price drop"", ""never posted"");"
        ).SetName("Has.None.EqualTo with a message");

        yield return new TestCaseData(
            @"[|Assert.That(sample.Numbers, Has.None.EqualTo(75).And.None.EqualTo(78))|];",
            @"sample.Numbers.ShouldNotContain(75);
            sample.Numbers.ShouldNotContain(78);"
        ).SetName("Has.None.EqualTo And None.EqualTo");

        yield return new TestCaseData(
            @"[|Assert.That(sample.Lines, Has.Some.EqualTo(""Price drop""))|];",
            @"sample.Lines.ShouldContain(""Price drop"");"
        ).SetName("Has.Some.EqualTo");

        yield return new TestCaseData(
            @"[|Assert.That(sample.Body, Does.Contain(""&#x2B;840 GB"").Or.Contain(""+840 GB""))|];",
            @"new[] { ""&#x2B;840 GB"", ""+840 GB"" }.ShouldContain(item => sample.Body.Contains(item));"
        ).SetName("Does.Contain Or Contain");

        yield return new TestCaseData(
            @"[|Assert.That(sample.Errors, Does.Not.ContainKey(""sonarr""))|];",
            @"sample.Errors.ShouldNotContainKey(""sonarr"");"
        ).SetName("Does.Not.ContainKey");

        yield return new TestCaseData(
            @"[|Assert.That(sample.Errors, Does.ContainKey(""sonarr""))|];",
            @"sample.Errors.ShouldContainKey(""sonarr"");"
        ).SetName("Does.ContainKey");

        yield return new TestCaseData(
            @"[|Assert.That(sample.Body, Does.Not.Match(@""\d,\d+,""), ""a comma decimal would corrupt the list"")|];",
            @"sample.Body.ShouldNotMatch(@""\d,\d+,"", ""a comma decimal would corrupt the list"");"
        ).SetName("Does.Not.Match with a message");

        yield return new TestCaseData(
            @"[|Assert.That(sample.Body, Has.Length.EqualTo(201))|];",
            @"sample.Body.Length.ShouldBe(201);"
        ).SetName("Has.Length.EqualTo");

        yield return new TestCaseData(
            @"[|Assert.That(sample.Lines.Distinct(), Is.SubsetOf(new[] { ""fix"", ""triage"" }))|];",
            @"sample.Lines.Distinct().ShouldBeSubsetOf(new[] { ""fix"", ""triage"" });"
        ).SetName("Is.SubsetOf");

        yield return new TestCaseData(
            @"[|Assert.That(sample.Lines, Is.All.Contain(""Blocked"").And.All.Contain(""/account/unblock""))|];",
            @"sample.Lines.ShouldAllBe(item => item.Contains(""Blocked""));
            sample.Lines.ShouldAllBe(item => item.Contains(""/account/unblock""));"
        ).SetName("Is.All.Contain And All.Contain");

        yield return new TestCaseData(
            @"var uris = new List<string> { ""https://a"" };
            [|Assert.That(uris, Is.All.StartsWith(""https://""))|];",
            @"var uris = new List<string> { ""https://a"" };
            uris.ShouldAllBe(item => item.StartsWith(""https://"", StringComparison.Ordinal));"
        ).SetName("Is.All.StartsWith");

        yield return new TestCaseData(
            @"var files = new[] { ""a.png"" };
            [|Assert.That(files, Has.All.EndWith("".png""))|];",
            @"var files = new[] { ""a.png"" };
            files.ShouldAllBe(item => item.EndsWith("".png"", StringComparison.Ordinal));"
        ).SetName("Has.All.EndWith");

        yield return new TestCaseData(
            @"var files = new[] { ""a.png"" };
            [|Assert.That(files, Is.All.EndsWith("".png""))|];",
            @"var files = new[] { ""a.png"" };
            files.ShouldAllBe(item => item.EndsWith("".png"", StringComparison.Ordinal));"
        ).SetName("Is.All.EndsWith");

        yield return new TestCaseData(
            @"var lines = new[] { ""Blocked"" };
            [|Assert.That(lines, Is.All.Contains(""Block""))|];",
            @"var lines = new[] { ""Blocked"" };
            lines.ShouldAllBe(item => item.Contains(""Block""));"
        ).SetName("Is.All.Contains");

        yield return new TestCaseData(
            @"var uris = new List<string> { ""https://a"" };
            [|Assert.Multiple(() =>
            {
                [|Assert.That(uris, Is.All.StartsWith(""https://""))|];
                [|Assert.That(uris, Has.Count.EqualTo(1))|];
            })|];",
            @"var uris = new List<string> { ""https://a"" };
            this.ShouldSatisfyAllConditions(
                () => uris.ShouldAllBe(item => item.StartsWith(""https://"", StringComparison.Ordinal)),
                () => uris.Count.ShouldBe(1));"
        ).SetName("Is.All.StartsWith inside Assert.Multiple");

        yield return new TestCaseData(
            @"object name = ""Joel"";
            [|Assert.That(name, Is.TypeOf<string>())|];",
            @"object name = ""Joel"";
            name.ShouldBeOfType<string>();"
        ).SetName("Is.TypeOf<T>");

        yield return new TestCaseData(
            @"object name = ""Joel"";
            [|Assert.That(name, Is.TypeOf(typeof(string)))|];",
            @"object name = ""Joel"";
            name.ShouldBeOfType<string>();"
        ).SetName("Is.TypeOf(typeof(T))");

        yield return new TestCaseData(
            @"object name = ""Joel"";
            [|Assert.That(name, Is.Not.TypeOf<int>())|];",
            @"object name = ""Joel"";
            name.ShouldNotBeOfType<int>();"
        ).SetName("Is.Not.TypeOf<T>");

        yield return new TestCaseData(
            @"object name = ""Joel"";
            [|Assert.That(name, Is.Not.TypeOf(typeof(int)))|];",
            @"object name = ""Joel"";
            name.ShouldNotBeOfType<int>();"
        ).SetName("Is.Not.TypeOf(typeof(T))");

        yield return new TestCaseData(
            @"object names = new List<string>();
            [|Assert.That(names, Is.InstanceOf<IEnumerable<string>>())|];",
            @"object names = new List<string>();
            names.ShouldBeAssignableTo<IEnumerable<string>>();"
        ).SetName("Is.InstanceOf<T>");

        yield return new TestCaseData(
            @"object names = new List<string>();
            [|Assert.That(names, Is.InstanceOf(typeof(IEnumerable<string>)))|];",
            @"object names = new List<string>();
            names.ShouldBeAssignableTo<IEnumerable<string>>();"
        ).SetName("Is.InstanceOf(typeof(T))");

        yield return new TestCaseData(
            @"object name = ""Joel"";
            [|Assert.That(name, Is.Not.InstanceOf<IEnumerable<int>>())|];",
            @"object name = ""Joel"";
            name.ShouldNotBeAssignableTo<IEnumerable<int>>();"
        ).SetName("Is.Not.InstanceOf<T>");

        yield return new TestCaseData(
            @"object name = ""Joel"";
            [|Assert.That(name, Is.TypeOf<string>(), ""name should be a string"")|];",
            @"object name = ""Joel"";
            name.ShouldBeOfType<string>(""name should be a string"");"
        ).SetName("Is.TypeOf<T> with a message");

        yield return new TestCaseData(
            @"object name = ""Joel"";
            [|Assert.Multiple(() =>
            {
                [|Assert.That(name, Is.TypeOf<string>())|];
                [|Assert.That(name, Is.Not.InstanceOf<int>())|];
            })|];",
            @"object name = ""Joel"";
            this.ShouldSatisfyAllConditions(
                () => { name.ShouldBeOfType<string>(); },
                () => name.ShouldNotBeAssignableTo<int>());"
        ).SetName("Is.TypeOf<T> inside Assert.Multiple");

        yield return new TestCaseData(
            @"var lines = new[] { ""host: NAS media share 94% full"" };
            [|Assert.That(lines.Select(line => line.Trim()), Has.Some.Contains(""94% full""))|];",
            @"var lines = new[] { ""host: NAS media share 94% full"" };
            lines.Select(line => line.Trim()).ShouldContain(item => item.Contains(""94% full""));"
        ).SetName("Has.Some.Contains");

        yield return new TestCaseData(
            @"var urls = new List<string> { ""https://dash.dicko.dev"" };
            [|Assert.That(urls, Has.None.Contains(""n8n""))|];",
            @"var urls = new List<string> { ""https://dash.dicko.dev"" };
            urls.ShouldNotContain(item => item.Contains(""n8n""));"
        ).SetName("Has.None.Contains");

        yield return new TestCaseData(
            @"var paths = new List<string> { ""/repos/x/pulls?state=open"" };
            [|Assert.That(paths, Has.Some.StartsWith(""/repos/x/pulls?""))|];",
            @"var paths = new List<string> { ""/repos/x/pulls?state=open"" };
            paths.ShouldContain(item => item.StartsWith(""/repos/x/pulls?"", StringComparison.Ordinal));"
        ).SetName("Has.Some.StartsWith");

        yield return new TestCaseData(
            @"var paths = new List<string> { ""/api/v3/series/358"" };
            [|Assert.That(paths, Has.Some.EndsWith(""/series/358""))|];",
            @"var paths = new List<string> { ""/api/v3/series/358"" };
            paths.ShouldContain(item => item.EndsWith(""/series/358"", StringComparison.Ordinal));"
        ).SetName("Has.Some.EndsWith");

        yield return new TestCaseData(
            @"var paths = new List<string> { ""/api/v3/series/358"" };
            [|Assert.That(paths, Has.None.StartWith(""/api/v1/""))|];",
            @"var paths = new List<string> { ""/api/v3/series/358"" };
            paths.ShouldNotContain(item => item.StartsWith(""/api/v1/"", StringComparison.Ordinal));"
        ).SetName("Has.None.StartWith");

        yield return new TestCaseData(
            @"var paths = new List<string> { ""/api/v3/series/358"" };
            [|Assert.That(paths, Has.None.EndsWith("".json""))|];",
            @"var paths = new List<string> { ""/api/v3/series/358"" };
            paths.ShouldNotContain(item => item.EndsWith("".json"", StringComparison.Ordinal));"
        ).SetName("Has.None.EndsWith");

        yield return new TestCaseData(
            @"var urls = new List<string> { ""https://a.dicko.dev:443"" };
            [|Assert.That(urls, Has.All.Contains("".dicko.dev:""))|];",
            @"var urls = new List<string> { ""https://a.dicko.dev:443"" };
            urls.ShouldAllBe(item => item.Contains("".dicko.dev:""));"
        ).SetName("Has.All.Contains");

        yield return new TestCaseData(
            @"var lines = new List<string> { ""GET http://books.test:8080/api/stats"" };
            [|Assert.That(lines, Has.Some.Contains(""/api/stats""), ""the request is logged"")|];",
            @"var lines = new List<string> { ""GET http://books.test:8080/api/stats"" };
            lines.ShouldContain(item => item.Contains(""/api/stats""), ""the request is logged"");"
        ).SetName("Has.Some.Contains with a message");

        yield return new TestCaseData(
            @"var lines = new List<string> { ""GET http://books.test:8080/api/stats"" };
            [|Assert.Multiple(() =>
            {
                [|Assert.That(lines, Has.Some.Contains(""/api/stats""))|];
                [|Assert.That(lines, Has.None.Contains(""key-for-tests""))|];
            })|];",
            @"var lines = new List<string> { ""GET http://books.test:8080/api/stats"" };
            this.ShouldSatisfyAllConditions(
                () => lines.ShouldContain(item => item.Contains(""/api/stats"")),
                () => lines.ShouldNotContain(item => item.Contains(""key-for-tests"")));"
        ).SetName("Has.Some and Has.None Contains inside Assert.Multiple");
    }

    [Test]
    [TestCaseSource(nameof(PreviouslyUnconvertedFormTestCases))]
    public async Task TestPreviouslyUnconvertedFormConversion(string before, string after)
    {
        // Warnings are compared too, so a conversion that leaves e.g. CS8629 or CS0162 behind fails.
        var codeFixTest = new CodeFixTest(WrapInSampleTestMethod(before), WrapInSampleTestMethod(after))
        {
            CompilerDiagnostics = CompilerDiagnostics.Warnings
        };

        await codeFixTest.RunAsync(CancellationToken.None);
    }

    [Test]
    public async Task AssertFail_RemovesTheReturnAfterIt()
    {
        const string signature = "private async Task<IReadOnlyList<string>> SnapshotAsync()";
        await new CodeFixTest(
                WrapInSampleTestMethod(@"for (var tries = 0; tries < 3; tries++)
            {
                await Task.Delay(1);
            }

            [|Assert.Fail(""no snapshot"")|];
            return new List<string>();", signature),
                WrapInSampleTestMethod(@"for (var tries = 0; tries < 3; tries++)
            {
                await Task.Delay(1);
            }

            throw new ShouldAssertException(""no snapshot"");", signature))
            {
                CompilerDiagnostics = CompilerDiagnostics.Warnings
            }
            .RunAsync(CancellationToken.None);
    }

    // Shouldly's ShouldContainKey/ShouldNotContainKey only take an IDictionary (as of 4.3.0), so on an
    // IReadOnlyDictionary the assert stays reported for a hand conversion rather than becoming code that doesn't compile.
    [Test]
    public async Task DoesNotContainKeyOnIReadOnlyDictionary_IsLeftAlone()
    {
        var source = WrapInSampleTestMethod(@"[|Assert.That(sample.Failures, Does.Not.ContainKey(""sonarr""))|];");
        // The fix is still offered (as for every reported assert) but leaves the document unchanged.
        await new CodeFixTest(source, source)
            {
                NumberOfIncrementalIterations = 1,
                NumberOfFixAllIterations = 1
            }
            .RunAsync(CancellationToken.None);
    }

    // Shouldly's type assertions only take a type argument, so a Type held in a variable is left for a hand conversion.
    [Test]
    public async Task IsTypeOfWithATypeVariable_IsLeftAlone()
    {
        var source = WrapInSampleTestMethod(@"object name = ""Joel"";
            var expected = typeof(string);
            [|Assert.That(name, Is.TypeOf(expected))|];");
        await new CodeFixTest(source, source)
            {
                NumberOfIncrementalIterations = 1,
                NumberOfFixAllIterations = 1
            }
            .RunAsync(CancellationToken.None);
    }

    // `item.StartsWith(...)` on a maybe-null element would raise CS8602 (and throw where NUnit just doesn't match),
    // so a sequence of string? is left for a hand conversion.
    [TestCase("Is.All.StartsWith")]
    [TestCase("Has.Some.Contains")]
    [TestCase("Has.None.EndsWith")]
    public async Task StringQuantifierOnNullableStrings_IsLeftAlone(string constraint)
    {
        var source = WrapInSampleTestMethod(@"var uris = new List<string?> { ""https://a"" };
            [|Assert.That(uris, " + constraint + @"(""https://""))|];");
        await new CodeFixTest(source, source)
            {
                NumberOfIncrementalIterations = 1,
                NumberOfFixAllIterations = 1
            }
            .RunAsync(CancellationToken.None);
    }

    [Test]
    public async Task IsAllStartsWithWithoutUsingSystem_QualifiesStringComparison()
    {
        const string source = @"using System.Collections.Generic;
using NUnit.Framework;
using Shouldly;
public class TestClass
{
    [Test]
    public void TestMethod()
    {
        var uris = new List<string> { ""https://a"" };
        [|Assert.That(uris, Is.All.StartsWith(""https://""))|];
    }
}";
        const string fixedSource = @"using System.Collections.Generic;
using NUnit.Framework;
using Shouldly;
public class TestClass
{
    [Test]
    public void TestMethod()
    {
        var uris = new List<string> { ""https://a"" };
        uris.ShouldAllBe(item => item.StartsWith(""https://"", System.StringComparison.Ordinal));
    }
}";
        await new CodeFixTest(source, fixedSource).RunAsync(CancellationToken.None);
    }

    private static IEnumerable<TestCaseData> AsyncAssertMultipleTestCases()
    {
        yield return new TestCaseData(
            @"[|Assert.Multiple(async () =>
            {
                [|Assert.That(sample.Ratio, Is.EqualTo(0.8))|];
                [|Assert.That(sample.Body, Is.EqualTo(""image/png""))|];
                [|Assert.That(await sample.ReadAsByteArrayAsync(), Is.EqualTo(new byte[] { 1 }))|];
            })|];",
            @"var readAsByteArray = await sample.ReadAsByteArrayAsync();
            this.ShouldSatisfyAllConditions(
                () => sample.Ratio.ShouldBe(0.8),
                () => sample.Body.ShouldBe(""image/png""),
                () => readAsByteArray.ShouldBe(new byte[] { 1 }));"
        ).SetName("Assert.Multiple with an async lambda hoists the await into a local");

        yield return new TestCaseData(
            @"var status = 1;
            [|Assert.Multiple(async () =>
            {
                [|Assert.That(await GetStatusAsync(), Is.EqualTo(""Ordered""))|];
                [|Assert.That(await GetStatusAsync().ConfigureAwait(false), Does.StartWith(""Or""))|];
            })|];",
            @"var status = 1;
            var status1 = await GetStatusAsync();
            var status2 = await GetStatusAsync().ConfigureAwait(false);
            this.ShouldSatisfyAllConditions(
                () => status1.ShouldBe(""Ordered""),
                () => status2.ShouldStartWith(""Or""));"
        ).SetName("Assert.Multiple with an async lambda keeps hoisted names unique");

        yield return new TestCaseData(
            @"[|Assert.Multiple(async () => [|Assert.That(sample.Ratio, Is.EqualTo(0.8))|])|];",
            @"this.ShouldSatisfyAllConditions(
                () => sample.Ratio.ShouldBe(0.8));"
        ).SetName("Assert.Multiple with an async lambda and nothing to await");
    }

    [Test]
    [TestCaseSource(nameof(AsyncAssertMultipleTestCases))]
    public async Task TestAsyncAssertMultipleConversion(string before, string after)
    {
        const string signature = "public async Task TestMethod()";
        var codeFixTest = new CodeFixTest(WrapInSampleTestMethod(before, signature), WrapInSampleTestMethod(after, signature));
        await codeFixTest.RunAsync(CancellationToken.None);
    }

    private static IEnumerable<TestCaseData> UnconvertibleAsyncAssertMultipleTestCases()
    {
        yield return new TestCaseData(
            "public void TestMethod()",
            @"Assert.Multiple(async () =>
            {
                [|Assert.That(await GetStatusAsync(), Is.EqualTo(""Ordered""))|];
            });"
        ).SetName("Assert.Multiple with an async lambda in a sync method is not reported");

        yield return new TestCaseData(
            "public async Task TestMethod()",
            @"Assert.Multiple(async () =>
            {
                [|Assert.That(sample.Body ?? await GetStatusAsync(), Is.EqualTo(""Ordered""))|];
            });"
        ).SetName("Assert.Multiple whose await is only evaluated on ?? is not reported");
    }

    [Test]
    [TestCaseSource(nameof(UnconvertibleAsyncAssertMultipleTestCases))]
    public async Task TestUnconvertibleAsyncAssertMultipleIsNotReported(string signature, string body)
    {
        var test = new CSharpAnalyzerTest<NUnitToShouldlyAnalyzer, NUnitVerifier>
        {
            TestCode = WrapInSampleTestMethod(body, signature),
            ReferenceAssemblies = TestReferenceAssemblies
        };

        await test.RunAsync(CancellationToken.None);
    }

    private static string WrapInSampleTestMethod(string body, string signature = "public void TestMethod()") => $@"#nullable enable
#pragma warning disable CS1591
using NUnit.Framework;using System;using System.Collections.Generic;using System.Linq;using System.Threading.Tasks;
using Shouldly;
namespace TestNamespace
{{
    public class Sample
    {{
        public double Ratio {{ get; set; }} = 0.8;
        public TimeSpan? Median {{ get; set; }} = TimeSpan.FromHours(1);
        public DateTimeOffset From {{ get; set; }}
        public DateTimeOffset To {{ get; set; }}
        public string Body {{ get; set; }} = """";
        public List<string> Lines {{ get; }} = new List<string>();
        public List<int> Numbers {{ get; }} = new List<int>();
        public Dictionary<string, string> Errors {{ get; }} = new Dictionary<string, string>();
        public IReadOnlyDictionary<string, string> Failures => Errors;
        public Task<byte[]> ReadAsByteArrayAsync() => Task.FromResult(new byte[] {{ 1 }});
    }}

    public class TestClass
    {{
        private readonly Sample sample = new Sample();

        [Test]
        {signature}
        {{
            {body}
        }}

        private static Task<string> GetStatusAsync() => Task.FromResult(""Ordered"");
        private static Task RefuseAsync() => Task.FromException(new InvalidOperationException());
    }}
}}";

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