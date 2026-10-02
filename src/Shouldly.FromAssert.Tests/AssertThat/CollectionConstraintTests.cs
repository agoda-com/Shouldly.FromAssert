using NUnit.Framework;
using Shouldly.FromAssert.Tests.Infrastructure;

namespace Shouldly.FromAssert.Tests.AssertThat;

/// <summary>Assert.That with membership, emptiness, uniqueness, equivalence, order, subset and key constraints.</summary>
public class CollectionConstraintTests
{
    private static IEnumerable<TestCaseData> Cases()
    {
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
            "var greetings = new List<string> { \"Hello\", \"World\" };",
            "[|Assert.That(greetings, Does.Contain(\"World\"))|];",
            "greetings.ShouldContain(\"World\");"
        ).SetName("Assert.That with Does.Contain on a collection");

        yield return new TestCaseData(
            "var contestants = new List<int>();",
            "[|Assert.That(contestants, Is.Empty)|];",
            "contestants.ShouldBeEmpty();"
        ).SetName("Assert.That with Is.Empty");

        yield return new TestCaseData(
            "var contestants = new List<int> { 1 };",
            "[|Assert.That(contestants, Is.Not.Empty)|];",
            "contestants.ShouldNotBeEmpty();"
        ).SetName("Assert.That with Is.Not.Empty");

        // Not.Null.And.Not.Empty must keep both halves (#29): one ShouldNotBeNullOrEmpty on a string (Shouldly has no
        // sequence overload), two asserts otherwise.
        yield return new TestCaseData(
            "var greeting = \"Hello\";",
            "[|Assert.That(greeting, Is.Not.Null.And.Not.Empty)|];",
            "greeting.ShouldNotBeNullOrEmpty();"
        ).SetName("Assert.That with Is.Not.Null.And.Not.Empty on a string");

        yield return new TestCaseData(
            "var contestants = new List<int> { 1337 };",
            "[|Assert.That(contestants, Is.Not.Null.And.Not.Empty)|];",
            "contestants.ShouldNotBeNull();\n            contestants.ShouldNotBeEmpty();"
        ).SetName("Assert.That with Is.Not.Null.And.Not.Empty on a collection");

        yield return new TestCaseData(
            "var greeting = \"Hello\";",
            "[|Assert.That(greeting, Is.Not.Null.And.Not.Empty, \"greets\")|];",
            "greeting.ShouldNotBeNullOrEmpty(\"greets\");"
        ).SetName("Assert.That with Is.Not.Null.And.Not.Empty and a message");

        yield return new TestCaseData(
            "var greeting = \"Hello\";",
            "[|Assert.That(greeting, Is.Not.Empty.And.Not.Null)|];",
            "greeting.ShouldNotBeNullOrEmpty();"
        ).SetName("Assert.That with Is.Not.Empty.And.Not.Null");

        yield return new TestCaseData(
            "var contestants = new List<int> { 1337 };",
            "[|Assert.That(contestants, Is.Not.Null.And.Not.Empty.And.Unique)|];",
            "contestants.ShouldNotBeNull();\n            contestants.ShouldNotBeEmpty();\n            contestants.ShouldBeUnique();"
        ).SetName("Assert.That with Is.Not.Null.And.Not.Empty followed by another link");

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
            "var contestants = new List<int> { 7, 3, 1 };",
            "[|Assert.That(contestants, Is.Ordered.Descending)|];",
            "contestants.ShouldBeInOrder(SortDirection.Descending);"
        ).SetName("Assert.That with Is.Ordered.Descending");
    }

    [Test]
    [TestCaseSource(nameof(Cases))]
    public Task Converts(string setup, string before, string after) => Verify.Conversion(setup, before, after);

    // From #34: forms that were reported but never converted, each from a real suite.
    private static IEnumerable<TestCaseData> SampleCases()
    {
        yield return new TestCaseData(
            @"[|Assert.That(sample.Errors, Does.Not.ContainKey(""sonarr""))|];",
            @"sample.Errors.ShouldNotContainKey(""sonarr"");"
        ).SetName("Does.Not.ContainKey");

        yield return new TestCaseData(
            @"[|Assert.That(sample.Errors, Does.ContainKey(""sonarr""))|];",
            @"sample.Errors.ShouldContainKey(""sonarr"");"
        ).SetName("Does.ContainKey");

        yield return new TestCaseData(
            @"[|Assert.That(sample.Lines.Distinct(), Is.SubsetOf(new[] { ""fix"", ""triage"" }))|];",
            @"sample.Lines.Distinct().ShouldBeSubsetOf(new[] { ""fix"", ""triage"" });"
        ).SetName("Is.SubsetOf");
    }

    [Test]
    [TestCaseSource(nameof(SampleCases))]
    public Task ConvertsInSample(string before, string after) => Verify.SampleConversion(before, after);

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
}
