using NUnit.Framework;
using Shouldly.FromAssert.Tests.Infrastructure;

namespace Shouldly.FromAssert.Tests.Receivers;

/// <summary>The shape of the actual value: when it needs parentheses, lambda bodies, and not shadowing a local.</summary>
public class ReceiverShapeTests
{
    private static IEnumerable<TestCaseData> Cases()
    {
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
            "List<int> contestants = null; var fallback = new List<int> { 1, 3, 3, 7 };",
            "[|Assert.That(contestants ?? fallback, Has.Count.EqualTo(4))|];",
            "(contestants ?? fallback).Count.ShouldBe(4);"
        ).SetName("Assert.That with Has.Count.EqualTo on a coalesce expression");

        yield return new TestCaseData(
            "",
            "[|Assert.That(\"1337\", Has.Length.GreaterThan(3))|];",
            "\"1337\".Length.ShouldBeGreaterThan(3);"
        ).SetName("Assert.That with Has.Length.GreaterThan on a literal is not parenthesised");

        yield return new TestCaseData(
            "var contestant = \"EBG\";",
            "[|Assert.That(contestant.Trim(), Has.Length.GreaterThan(0))|];",
            "contestant.Trim().Length.ShouldBeGreaterThan(0);"
        ).SetName("Assert.That with Has.Length on invocation receiver");

        yield return new TestCaseData(
            "var contestant = \"EBG\";",
            "[|Assert.That(contestant ?? \"\", Has.Length.LessThan(4))|];",
            "(contestant ?? \"\").Length.ShouldBeLessThan(4);"
        ).SetName("Assert.That with Has.Length on binary receiver");

        yield return new TestCaseData(
            "var item = new Version(1, 0); var contestants = new List<Version> { item };",
            "[|Assert.That(contestants, Has.All.EqualTo(item))|];",
            "contestants.ShouldAllBe(item1 => object.Equals(item1, item));"
        ).SetName("Assert.That with Has.All.EqualTo does not shadow a local named item");
    }

    [Test]
    [TestCaseSource(nameof(Cases))]
    public Task Converts(string setup, string before, string after) => Verify.Conversion(setup, before, after);

    [Test]
    public async Task AwaitReceiver_IsParenthesised()
    {
        const string signature = "public async Task TestMethod()";
        await new CodeFixTest(
                TestSource.InSampleTest(@"[|Assert.That(await GetStatusAsync(), Is.EqualTo(""Ordered""))|];", signature),
                TestSource.InSampleTest(@"(await GetStatusAsync()).ShouldBe(""Ordered"");", signature))
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
}
