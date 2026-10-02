using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using NUnit.Framework;
using Shouldly.FromAssert.Tests.Infrastructure;

namespace Shouldly.FromAssert.Tests;

public class NUnitToShouldlyConverterTestsAll
{
    private static IEnumerable<TestCaseData> TestCases()
    {
        yield return new TestCaseData(
            "var contestant = 1337;",
            "[|Assert.That(contestant, Is.EqualTo(1337))|];",
            "contestant.ShouldBe(1337);"
        ).SetName("Assert.That with Is.EqualTo");

        yield return new TestCaseData(
            "var contestant = 1337;",
            "[|Assert.AreEqual(1337, contestant)|];",
            "contestant.ShouldBe(1337);"
        ).SetName("Assert.AreEqual");

        yield return new TestCaseData(
            "var contestant = 1337;",
            "[|Assert.AreNotEqual(1336, contestant)|];",
            "contestant.ShouldNotBe(1336);"
        ).SetName("Assert.AreNotEqual");

        yield return new TestCaseData(
            "var contestant = 1337;",
            "[|Assert.IsTrue(contestant > 1000)|];",
            "(contestant > 1000).ShouldBeTrue();"
        ).SetName("Assert.IsTrue");

        yield return new TestCaseData(
            "var contestant = 1337;",
            "[|Assert.IsFalse(contestant < 1000)|];",
            "(contestant < 1000).ShouldBeFalse();"
        ).SetName("Assert.IsFalse");

        yield return new TestCaseData(
            "string contestant = null;",
            "[|Assert.IsNull(contestant)|];",
            "contestant.ShouldBeNull();"
        ).SetName("Assert.IsNull");

        yield return new TestCaseData(
            "var contestant = \"1337\";",
            "[|Assert.IsNotNull(contestant)|];",
            "contestant.ShouldNotBeNull();"
        ).SetName("Assert.IsNotNull");

        yield return new TestCaseData(
            "var expected = new object(); var contestant = expected;",
            "[|Assert.AreSame(expected, contestant)|];",
            "contestant.ShouldBeSameAs(expected);"
        ).SetName("Assert.AreSame");

        yield return new TestCaseData(
            "var expected = new object(); var contestant = new object();",
            "[|Assert.AreNotSame(expected, contestant)|];",
            "contestant.ShouldNotBeSameAs(expected);"
        ).SetName("Assert.AreNotSame");

        yield return new TestCaseData(
            "var contestant = \"1337\";",
            "[|Assert.IsInstanceOf<string>(contestant)|];",
            "contestant.ShouldBeOfType<string>();"
        ).SetName("Assert.IsInstanceOf");

        yield return new TestCaseData(
            "var contestant = \"1337\";",
            "[|Assert.IsNotInstanceOf<int>(contestant)|];",
            "contestant.ShouldNotBeOfType<int>();"
        ).SetName("Assert.IsNotInstanceOf");

        yield return new TestCaseData(
            "var contestants = new List<int> { 1337, 2448, 3559 };",
            "[|Assert.Contains(1337, contestants)|];",
            "contestants.ShouldContain(1337);"
        ).SetName("CollectionAssert.Contains");

        yield return new TestCaseData(
            "var contestants = new List<int> { 1337, 2448, 3559 };",
            "[|CollectionAssert.DoesNotContain(contestants, 1336)|];",
            "contestants.ShouldNotContain(1336);"
        ).SetName("CollectionAssert.DoesNotContain");

        yield return new TestCaseData(
            "var contestants = new List<int>();",
            "[|CollectionAssert.IsEmpty(contestants)|];",
            "contestants.ShouldBeEmpty();"
        ).SetName("CollectionAssert.IsEmpty");

        yield return new TestCaseData(
            "var contestants = new List<int> { 1337 };",
            "[|CollectionAssert.IsNotEmpty(contestants)|];",
            "contestants.ShouldNotBeEmpty();"
        ).SetName("CollectionAssert.IsNotEmpty");

        yield return new TestCaseData(
            "var contestant = 1337;",
            "[|Assert.Greater(contestant, 1000)|];",
            "contestant.ShouldBeGreaterThan(1000);"
        ).SetName("Assert.Greater");

        yield return new TestCaseData(
            "var contestant = 1337;",
            "[|Assert.GreaterOrEqual(contestant, 1337)|];",
            "contestant.ShouldBeGreaterThanOrEqualTo(1337);"
        ).SetName("Assert.GreaterOrEqual");

        yield return new TestCaseData(
            "var contestant = 1337;",
            "[|Assert.Less(contestant, 2000)|];",
            "contestant.ShouldBeLessThan(2000);"
        ).SetName("Assert.Less");

        yield return new TestCaseData(
            "var contestant = 1337;",
            "[|Assert.LessOrEqual(contestant, 1337)|];",
            "contestant.ShouldBeLessThanOrEqualTo(1337);"
        ).SetName("Assert.LessOrEqual");

        yield return new TestCaseData(
            "var contestant = double.NaN;",
            "[|Assert.IsNaN(contestant)|];",
            "double.IsNaN(contestant).ShouldBeTrue();"
        ).SetName("Assert.IsNaN");

        yield return new TestCaseData(
            "var greeting = \"Hello, World!\";",
            "[|StringAssert.StartsWith(\"Hello\", greeting)|];",
            "greeting.ShouldStartWith(\"Hello\");"
        ).SetName("StringAssert.StartsWith");

        yield return new TestCaseData(
            "var greeting = \"Hello, World!\";",
            "[|StringAssert.EndsWith(\"World!\", greeting)|];",
            "greeting.ShouldEndWith(\"World!\");"
        ).SetName("StringAssert.EndsWith");

        yield return new TestCaseData(
            "var greeting = \"Hello, World!\";",
            "[|StringAssert.Contains(\"World\", greeting)|];",
            "greeting.ShouldContain(\"World\", Case.Sensitive);"
        ).SetName("StringAssert.Contains");

        yield return new TestCaseData(
            "var greeting = \"Hello, World!\";",
            "[|StringAssert.DoesNotContain(\"world\", greeting)|];",
            "greeting.ShouldNotContain(\"world\", Case.Sensitive);"
        ).SetName("StringAssert.DoesNotContain");

        yield return new TestCaseData(
            "void ThrowException() { throw new ArgumentException(); }",
            "[|Assert.Throws<ArgumentException>(() => ThrowException())|];",
            "Should.Throw<ArgumentException>(() => ThrowException());"
        ).SetName("Assert.Throws");

        yield return new TestCaseData(
            "void DoNotThrow() { }",
            "[|Assert.DoesNotThrow(() => DoNotThrow())|];",
            "Should.NotThrow(() => DoNotThrow());"
        ).SetName("Assert.DoesNotThrow");

        yield return new TestCaseData(
            "var expected = new List<int> { 1, 2, 3 }; var actual = new List<int> { 1, 2, 3 };",
            "[|CollectionAssert.AreEqual(expected, actual)|];",
            "actual.ShouldBe(expected);"
        ).SetName("CollectionAssert.AreEqual");

        yield return new TestCaseData(
            "var expected = new List<int> { 1, 2, 3 }; var actual = new List<int> { 3, 2, 1 };",
            "[|CollectionAssert.AreEquivalent(expected, actual)|];",
            "actual.ShouldBe(expected, ignoreOrder: true);"
        ).SetName("CollectionAssert.AreEquivalent");

        yield return new TestCaseData(
            "var collection = new List<string> { \"a\", \"b\", \"c\" };",
            "[|CollectionAssert.AllItemsAreInstancesOfType(collection, typeof(string))|];",
            "collection.ShouldAllBe(item => item is string);"
        ).SetName("CollectionAssert.AllItemsAreInstancesOfType");

        yield return new TestCaseData(
            "var collection = new List<string> { \"a\", \"b\", \"c\" };",
            "[|CollectionAssert.AllItemsAreNotNull(collection)|];",
            "collection.ShouldNotContain(item => item == null);"
        ).SetName("CollectionAssert.AllItemsAreNotNull");

        yield return new TestCaseData(
            "var collection = new List<int> { 1, 2, 3 };",
            "[|CollectionAssert.AllItemsAreUnique(collection)|];",
            "collection.ShouldBeUnique();"
        ).SetName("CollectionAssert.AllItemsAreUnique");

        yield return new TestCaseData(
            "var contestant = 1337;",
            "[|Assert.That(contestant, Is.Not.EqualTo(1336))|];",
            "contestant.ShouldNotBe(1336);"
        ).SetName("Assert.That with Is.Not.EqualTo");

        yield return new TestCaseData(
            "var contestants = new List<int> { 1337, 2448, 3559 };",
            "[|Assert.That(contestants, Has.Member(1337))|];",
            "contestants.ShouldContain(1337);"
        ).SetName("Assert.That with Has.Member");

        yield return new TestCaseData(
            "var contestants = new List<int> { 1337, 2448, 3559 };",
            "[|Assert.That(contestants, Has.No.Member(1336))|];",
            "contestants.ShouldNotContain(1336);"
        ).SetName("Assert.That with Has.No.Member");

        yield return new TestCaseData(
            "var contestants = new List<int> { 1337, 2448, 3559 };",
            "[|Assert.That(contestants, Is.Unique)|];",
            "contestants.ShouldBeUnique();"
        ).SetName("Assert.That with Is.Unique");

        yield return new TestCaseData(
            "var greeting = \"Hello, World!\";",
            "[|Assert.That(greeting, Does.Contain(\"World\"))|];",
            "greeting.ShouldContain(\"World\", Case.Sensitive);"
        ).SetName("Assert.That with Does.Contain");

        yield return new TestCaseData(
            "var greetings = new List<string> { \"Hello\", \"World\" };",
            "[|Assert.That(greetings, Does.Contain(\"World\"))|];",
            "greetings.ShouldContain(\"World\");"
        ).SetName("Assert.That with Does.Contain on a collection");

        yield return new TestCaseData(
            "string? greeting = \"Hello, World!\";",
            "[|Assert.That(greeting, Does.Contain(\"World\"))|];",
            "greeting.ShouldContain(\"World\", Case.Sensitive);"
        ).SetName("Assert.That with Does.Contain on a nullable string");

        yield return new TestCaseData(
            "var greeting = \"Hello, World!\";",
            "[|Assert.That(greeting, Does.StartWith(\"Hello\"))|];",
            "greeting.ShouldStartWith(\"Hello\");"
        ).SetName("Assert.That with Does.StartWith");

        yield return new TestCaseData(
            "var greeting = \"Hello, World!\";",
            "[|Assert.That(greeting, Does.EndWith(\"World!\"))|];",
            "greeting.ShouldEndWith(\"World!\");"
        ).SetName("Assert.That with Does.EndWith");

        yield return new TestCaseData(
            "var contestants = new List<int>();",
            "[|Assert.That(contestants, Is.Empty)|];",
            "contestants.ShouldBeEmpty();"
        ).SetName("Assert.That with Is.Empty");

        yield return new TestCaseData(
            "var contestant = 1337;",
            "[|Assert.That(contestant, Is.EqualTo(1337), \"top caps the rows\")|];",
            "contestant.ShouldBe(1337, \"top caps the rows\");"
        ).SetName("Assert.That with a message");

        yield return new TestCaseData(
            "var contestant = 1337;",
            "[|Assert.That(contestant, Is.EqualTo(1337), message: \"top caps the rows\")|];",
            "contestant.ShouldBe(1337, \"top caps the rows\");"
        ).SetName("Assert.That with a named message");

        yield return new TestCaseData(
            "bool? contestant = false;",
            "[|Assert.That(contestant, Is.Not.True, () => \"flag \" + contestant)|];",
            "contestant.ShouldNotBe(true, \"flag \" + contestant);"
        ).SetName("Assert.That with a Func<string> message");

        yield return new TestCaseData(
            "var greeting = \"Hello, World!\";",
            "[|Assert.That(greeting, Does.StartWith(\"Hello\"), \"greets\")|];",
            "greeting.ShouldStartWith(\"Hello\", customMessage: \"greets\");"
        ).SetName("Assert.That with a message where the string overload needs it named");

        yield return new TestCaseData(
            "var contestant = true;",
            "[|Assert.That(contestant)|];",
            "contestant.ShouldBeTrue();"
        ).SetName("Assert.That with a bool");

        yield return new TestCaseData(
            "var contestant = 1337;",
            "[|Assert.That(contestant > 1000, \"too small\")|];",
            "(contestant > 1000).ShouldBeTrue(\"too small\");"
        ).SetName("Assert.That with a bool expression and message");

        yield return new TestCaseData(
            "string contestant = null;",
            "[|Assert.That(contestant, Is.Null)|];",
            "contestant.ShouldBeNull();"
        ).SetName("Assert.That with Is.Null");

        yield return new TestCaseData(
            "var contestant = \"1337\";",
            "[|Assert.That(contestant, Is.Not.Null)|];",
            "contestant.ShouldNotBeNull();"
        ).SetName("Assert.That with Is.Not.Null");

        yield return new TestCaseData(
            "var contestant = true;",
            "[|Assert.That(contestant, Is.True)|];",
            "contestant.ShouldBeTrue();"
        ).SetName("Assert.That with Is.True");

        yield return new TestCaseData(
            "bool? contestant = true;",
            "[|Assert.That(contestant, Is.True)|];",
            "contestant.ShouldBe(true);"
        ).SetName("Assert.That with Is.True on a nullable bool");

        yield return new TestCaseData(
            "var contestant = false;",
            "[|Assert.That(contestant, Is.False)|];",
            "contestant.ShouldBeFalse();"
        ).SetName("Assert.That with Is.False");

        yield return new TestCaseData(
            "var contestant = false;",
            "[|Assert.That(contestant, Is.Not.True)|];",
            "contestant.ShouldNotBe(true);"
        ).SetName("Assert.That with Is.Not.True");

        yield return new TestCaseData(
            "var contestant = 0L;",
            "[|Assert.That(contestant, Is.Zero)|];",
            "contestant.ShouldBe(0);"
        ).SetName("Assert.That with Is.Zero");

        yield return new TestCaseData(
            "var contestants = new List<int> { 1, 3, 3, 7 };",
            "[|Assert.That(contestants, Has.Count.EqualTo(4))|];",
            "contestants.Count.ShouldBe(4);"
        ).SetName("Assert.That with Has.Count.EqualTo");

        yield return new TestCaseData(
            "var contestants = new[] { 1, 3, 3, 7 };",
            "[|Assert.That(contestants, Has.Count.EqualTo(4))|];",
            "contestants.Length.ShouldBe(4);"
        ).SetName("Assert.That with Has.Count.EqualTo on an array");

        yield return new TestCaseData(
            "List<int> contestants = null; var fallback = new List<int> { 1, 3, 3, 7 };",
            "[|Assert.That(contestants ?? fallback, Has.Count.EqualTo(4))|];",
            "(contestants ?? fallback).Count.ShouldBe(4);"
        ).SetName("Assert.That with Has.Count.EqualTo on a coalesce expression");

        yield return new TestCaseData(
            "var item = new Version(1, 0); var contestants = new List<Version> { item };",
            "[|Assert.That(contestants, Has.All.EqualTo(item))|];",
            "contestants.ShouldAllBe(item1 => object.Equals(item1, item));"
        ).SetName("Assert.That with Has.All.EqualTo does not shadow a local named item");

        yield return new TestCaseData(
            "string contestant = null;",
            "[|Assert.That(contestant, (Is.Null))|];",
            "contestant.ShouldBeNull();"
        ).SetName("Assert.That with a parenthesised constraint");

        yield return new TestCaseData(
            "",
            "[|Assert.That(\"1337\", Has.Length.GreaterThan(3))|];",
            "\"1337\".Length.ShouldBeGreaterThan(3);"
        ).SetName("Assert.That with Has.Length.GreaterThan on a literal is not parenthesised");

        yield return new TestCaseData(
            "var greeting = \"Hello, World!\";",
            "[|Assert.That(greeting, Does.Not.Contain(\"world\"))|];",
            "greeting.ShouldNotContain(\"world\", Case.Sensitive);"
        ).SetName("Assert.That with Does.Not.Contain on a string");

        yield return new TestCaseData(
            "var contestants = new List<int> { 1, 3, 3, 7 };",
            "[|Assert.That(contestants, Does.Not.Contain(42))|];",
            "contestants.ShouldNotContain(42);"
        ).SetName("Assert.That with Does.Not.Contain on a collection");

        yield return new TestCaseData(
            "var contestants = new List<int> { 1, 3, 3, 7 };",
            "[|Assert.That(contestants, Is.EquivalentTo(new[] { 7, 3, 3, 1 }))|];",
            "contestants.ShouldBe(new[] { 7, 3, 3, 1 }, ignoreOrder: true);"
        ).SetName("Assert.That with Is.EquivalentTo");

        yield return new TestCaseData(
            "var contestants = new List<int> { 7, 7 };",
            "[|Assert.That(contestants, Is.All.EqualTo(7))|];",
            "contestants.ShouldAllBe(item => item == 7);"
        ).SetName("Assert.That with Is.All.EqualTo");

        yield return new TestCaseData(
            "var version = new Version(1, 0); var contestants = new List<Version> { new Version(1, 0) };",
            "[|Assert.That(contestants, Has.All.EqualTo(version))|];",
            "contestants.ShouldAllBe(item => object.Equals(item, version));"
        ).SetName("Assert.That with Has.All.EqualTo on a reference type");

        yield return new TestCaseData(
            "var contestant = new object(); var other = contestant;",
            "[|Assert.That(contestant, Is.SameAs(other))|];",
            "contestant.ShouldBeSameAs(other);"
        ).SetName("Assert.That with Is.SameAs");

        yield return new TestCaseData(
            "var contestants = new List<string> { null };",
            "[|Assert.That(contestants, Is.All.Null)|];",
            "contestants.ShouldAllBe(item => item == null);"
        ).SetName("Assert.That with Is.All.Null");

        yield return new TestCaseData(
            "var contestants = new List<int> { 1, 3 };",
            "[|Assert.That(contestants, Has.All.Matches<int>(x => x > 0))|];",
            "contestants.ShouldAllBe(x => x > 0);"
        ).SetName("Assert.That with Has.All.Matches");

        yield return new TestCaseData(
            "var contestants = new List<int> { 1, 3 };",
            "[|Assert.That(contestants, Has.None.Matches<int>(x => x < 0))|];",
            "contestants.ShouldNotContain(x => x < 0);"
        ).SetName("Assert.That with Has.None.Matches");

        yield return new TestCaseData(
            "var contestants = new List<int> { 1, 3 }; Predicate<int> positive = x => x > 0;",
            "[|Assert.That(contestants, Has.All.Matches(positive))|];",
            "contestants.ShouldAllBe(item => positive(item));"
        ).SetName("Assert.That with Has.All.Matches on a predicate variable");

        yield return new TestCaseData(
            "var contestant = 1337;",
            "[|Assert.That(contestant, Is.InRange(1000, 2000))|];",
            "contestant.ShouldBeInRange(1000, 2000);"
        ).SetName("Assert.That with Is.InRange");

        yield return new TestCaseData(
            "var contestants = new List<int> { 7, 3, 1 };",
            "[|Assert.That(contestants, Is.Ordered.Descending)|];",
            "contestants.ShouldBeInOrder(SortDirection.Descending);"
        ).SetName("Assert.That with Is.Ordered.Descending");

        yield return new TestCaseData(
            "var contestant = \"1337\";",
            "[|Assert.That(contestant, Does.Match(\"^13\"))|];",
            "contestant.ShouldMatch(\"^13\");"
        ).SetName("Assert.That with Does.Match");

        yield return new TestCaseData(
            "var contestants = new List<bool> { false };",
            "[|Assert.That(contestants, Has.All.False)|];",
            "contestants.ShouldAllBe(item => !item);"
        ).SetName("Assert.That with Has.All.False");

        yield return new TestCaseData(
            "var contestant = new { MediaType = \"image/png\" };",
            "[|Assert.That(contestant?.MediaType, Is.EqualTo(\"image/png\"))|];",
            "(contestant?.MediaType).ShouldBe(\"image/png\");"
        ).SetName("Assert.That with null-conditional receiver");

        yield return new TestCaseData(
            "var contestant = 1337;",
            "[|Assert.AreEqual(true, contestant != 0 && contestant > 1000)|];",
            "(contestant != 0 && contestant > 1000).ShouldBe(true);"
        ).SetName("Assert.AreEqual with binary receiver");

        yield return new TestCaseData(
            "var contestant = 1337;",
            "[|Assert.IsTrue(contestant > 0 ? contestant < 2000 : false)|];",
            "(contestant > 0 ? contestant < 2000 : false).ShouldBeTrue();"
        ).SetName("Assert.IsTrue with conditional receiver");

        yield return new TestCaseData(
            "var contestants = new List<int> { 1 };",
            "[|Assert.That(contestants, Is.Not.Empty)|];",
            "contestants.ShouldNotBeEmpty();"
        ).SetName("Assert.That with Is.Not.Empty");

        yield return new TestCaseData(
            "var contestant = 1337;",
            "[|Assert.That(contestant, Is.GreaterThan(1000))|];",
            "contestant.ShouldBeGreaterThan(1000);"
        ).SetName("Assert.That with Is.GreaterThan");

        yield return new TestCaseData(
            "var contestant = 1337;",
            "[|Assert.That(contestant, Is.GreaterThanOrEqualTo(1337))|];",
            "contestant.ShouldBeGreaterThanOrEqualTo(1337);"
        ).SetName("Assert.That with Is.GreaterThanOrEqualTo");

        yield return new TestCaseData(
            "var contestant = 0.9;",
            "[|Assert.That(contestant, Is.LessThan(0.95))|];",
            "contestant.ShouldBeLessThan(0.95);"
        ).SetName("Assert.That with Is.LessThan");

        yield return new TestCaseData(
            "var contestant = 1337;",
            "[|Assert.That(contestant, Is.LessThanOrEqualTo(1337))|];",
            "contestant.ShouldBeLessThanOrEqualTo(1337);"
        ).SetName("Assert.That with Is.LessThanOrEqualTo");

        yield return new TestCaseData(
            "var contestant = \"EBG\";",
            "[|Assert.That(contestant, Has.Length.LessThanOrEqualTo(3))|];",
            "contestant.Length.ShouldBeLessThanOrEqualTo(3);"
        ).SetName("Assert.That with Has.Length.LessThanOrEqualTo");

        yield return new TestCaseData(
            "var contestant = \"EBG\";",
            "[|Assert.That(contestant.Trim(), Has.Length.GreaterThan(0))|];",
            "contestant.Trim().Length.ShouldBeGreaterThan(0);"
        ).SetName("Assert.That with Has.Length on invocation receiver");

        yield return new TestCaseData(
            "var contestants = new List<int> { 1337 };",
            "[|Assert.That(contestants, Has.Count.GreaterThanOrEqualTo(1))|];",
            "contestants.Count.ShouldBeGreaterThanOrEqualTo(1);"
        ).SetName("Assert.That with Has.Count.GreaterThanOrEqualTo");

        yield return new TestCaseData(
            "var contestant = \"EBG\";",
            "[|Assert.That(contestant ?? \"\", Has.Length.LessThan(4))|];",
            "(contestant ?? \"\").Length.ShouldBeLessThan(4);"
        ).SetName("Assert.That with Has.Length on binary receiver");
    }


    [Test]
    [TestCaseSource(nameof(TestCases))]
    public async Task TestConversion(string setup, string before, string after)
    {
        await new CodeFixTest(
                TestSource.InTestMethod(setup + "\n            " + before),
                TestSource.InTestMethod(setup + "\n            " + after))
            .RunAsync(CancellationToken.None);
    }

    [Test]
    public async Task AssertThatWithAndChain_SplitsIntoOneAssertPerLink()
    {
        await new CodeFixTest(
                TestSource.InTestMethod(@"var path = ""/img/logo.png"";
            // the path is a png under root
            [|Assert.That(path, Does.StartWith(""/"").And.EndWith("".png""))|];"),
                TestSource.InTestMethod(@"var path = ""/img/logo.png"";
            // the path is a png under root
            path.ShouldStartWith(""/"");
            path.ShouldEndWith("".png"");"))
            .RunAsync(CancellationToken.None);
    }

    [Test]
    public async Task AssertThatWithAndChainOfStringContains_KeepsCaseSensitivity()
    {
        await new CodeFixTest(
                TestSource.InTestMethod(@"var path = ""/img/logo.png"";
            [|Assert.That(path, Does.Contain(""img"").And.Contain(""logo""))|];"),
                TestSource.InTestMethod(@"var path = ""/img/logo.png"";
            path.ShouldContain(""img"", Case.Sensitive);
            path.ShouldContain(""logo"", Case.Sensitive);"))
            .RunAsync(CancellationToken.None);
    }

    [Test]
    public async Task AssertThatWithAndChainAndMessage_PassesTheMessageToEveryLink()
    {
        await new CodeFixTest(
                TestSource.InTestMethod(@"var contestant = 1337;
            [|Assert.That(contestant, Is.Not.Zero.And.EqualTo(1337), ""leet"")|];"),
                TestSource.InTestMethod(@"var contestant = 1337;
            contestant.ShouldNotBe(0, ""leet"");
            contestant.ShouldBe(1337, ""leet"");"))
            .RunAsync(CancellationToken.None);
    }

    [Test]
    public async Task AssertThatWithAndChainInEmbeddedStatement_WrapsInBlock()
    {
        await new CodeFixTest(
                TestSource.InTestMethod(@"var contestants = new List<int> { 1, 3, 3, 7 };
            if (contestants.Count > 0)
                [|Assert.That(contestants, Has.Member(1).And.Member(7))|];"),
                TestSource.InTestMethod(@"var contestants = new List<int> { 1, 3, 3, 7 };
            if (contestants.Count > 0)
            {
                contestants.ShouldContain(1);
                contestants.ShouldContain(7);
            }"))
            .RunAsync(CancellationToken.None);
    }

    [Test]
    public async Task AssertThatWithConditionalConstraint_BecomesIfElse()
    {
        await new CodeFixTest(
                TestSource.InTestMethod(@"var expected = true;
            var candidate = new object();
            var picked = candidate;
            [|Assert.That(picked, expected ? Is.SameAs(candidate) : Is.Null)|];"),
                TestSource.InTestMethod(@"var expected = true;
            var candidate = new object();
            var picked = candidate;
            if (expected)
            {
                picked.ShouldBeSameAs(candidate);
            }
            else
            {
                picked.ShouldBeNull();
            }"))
            .RunAsync(CancellationToken.None);
    }

    [Test]
    public async Task AssertThatWithMessageInLambda_ReplacesTheExpression()
    {
        await new CodeFixTest(
                TestSource.InTestMethod(@"string contestant = null;
            Action check = () => [|Assert.That(contestant, Is.Null, ""unset"")|];
            check();"),
                TestSource.InTestMethod(@"string contestant = null;
            Action check = () => contestant.ShouldBeNull(""unset"");
            check();"))
            .RunAsync(CancellationToken.None);
    }

    // A full document: the test class declares its own Assert* helper.
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

    // A full document: it declares its own Assert class and has no NUnit using.
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

    // A full document: it needs a using static.
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
            [|That(1337, Is.EqualTo(1337))|];
        }
    }
}";

        await new AnalyzerOnlyTest(test).RunAsync(CancellationToken.None);
    }

    // A full document: it needs a using static.
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

        await new AnalyzerOnlyTest(test).RunAsync(CancellationToken.None);
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
        var codeFixTest = new CodeFixTest(TestSource.InTestMethod(before), TestSource.InTestMethod(after));
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
                TestSource.InTestMethod(@"var contestant = 1337;
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
                TestSource.InTestMethod(@"var contestant = 1337;
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
        await new AnalyzerOnlyTest(TestSource.InTestMethod(statement)).RunAsync(CancellationToken.None);
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
        await new AnalyzerOnlyTest(TestSource.InTestMethod(body, signature)).RunAsync(CancellationToken.None);
    }

    [Test]
    public async Task AwaitReceiver_IsParenthesised()
    {
        const string signature = "public async Task TestMethod()";
        await new CodeFixTest(
                TestSource.InSampleTest(@"[|Assert.That(await GetStatusAsync(), Is.EqualTo(""Ordered""))|];", signature),
                TestSource.InSampleTest(@"(await GetStatusAsync()).ShouldBe(""Ordered"");", signature))
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
        var codeFixTest = new CodeFixTest(TestSource.InNullableReportTest(before), TestSource.InNullableReportTest(after))
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
                TestSource.InNullableReportTest(@"[|Assert.That(report.Error, Does.Not.Contain(""route""))|];"),
                TestSource.InNullableReportTest(@"report.Error.ShouldNotContain(""route"", Case.Sensitive);"))
            .RunAsync(CancellationToken.None);
    }

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
        var codeFixTest = new CodeFixTest(TestSource.InSampleTest(before), TestSource.InSampleTest(after))
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
                TestSource.InSampleTest(@"for (var tries = 0; tries < 3; tries++)
            {
                await Task.Delay(1);
            }

            [|Assert.Fail(""no snapshot"")|];
            return new List<string>();", signature),
                TestSource.InSampleTest(@"for (var tries = 0; tries < 3; tries++)
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
        var source = TestSource.InSampleTest(@"[|Assert.That(sample.Failures, Does.Not.ContainKey(""sonarr""))|];");
        // The fix is still offered (as for every reported assert) but leaves the document unchanged.
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
        var source = TestSource.InSampleTest(@"var uris = new List<string?> { ""https://a"" };
            [|Assert.That(uris, " + constraint + @"(""https://""))|];");
        await new CodeFixTest(source, source)
            {
                NumberOfIncrementalIterations = 1,
                NumberOfFixAllIterations = 1
            }
            .RunAsync(CancellationToken.None);
    }

    // A full document: it must not have a using System.
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
        var codeFixTest = new CodeFixTest(TestSource.InSampleTest(before, signature), TestSource.InSampleTest(after, signature));
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
        await new AnalyzerOnlyTest(TestSource.InSampleTest(body, signature)).RunAsync(CancellationToken.None);
    }
}
