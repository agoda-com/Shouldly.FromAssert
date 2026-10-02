using NUnit.Framework;
using Shouldly.FromAssert.Tests.Infrastructure;

namespace Shouldly.FromAssert.Tests.AssertThat;

/// <summary>Assert.That with ordering and range constraints, and Has.Count/Has.Length.</summary>
public class ComparisonTests
{
    private static IEnumerable<TestCaseData> Cases()
    {
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
            "var contestant = 1337;",
            "[|Assert.That(contestant, Is.InRange(1000, 2000))|];",
            "contestant.ShouldBeInRange(1000, 2000);"
        ).SetName("Assert.That with Is.InRange");

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
            "var contestants = new List<int> { 1337 };",
            "[|Assert.That(contestants, Has.Count.GreaterThanOrEqualTo(1))|];",
            "contestants.Count.ShouldBeGreaterThanOrEqualTo(1);"
        ).SetName("Assert.That with Has.Count.GreaterThanOrEqualTo");

        yield return new TestCaseData(
            "var contestant = \"EBG\";",
            "[|Assert.That(contestant, Has.Length.LessThanOrEqualTo(3))|];",
            "contestant.Length.ShouldBeLessThanOrEqualTo(3);"
        ).SetName("Assert.That with Has.Length.LessThanOrEqualTo");
    }

    [Test]
    [TestCaseSource(nameof(Cases))]
    public Task Converts(string setup, string before, string after) => Verify.Conversion(setup, before, after);

    // From #34: forms that were reported but never converted, each from a real suite.
    private static IEnumerable<TestCaseData> SampleCases()
    {
        yield return new TestCaseData(
            @"[|Assert.That(sample.Body, Has.Length.EqualTo(201))|];",
            @"sample.Body.Length.ShouldBe(201);"
        ).SetName("Has.Length.EqualTo");
    }

    [Test]
    [TestCaseSource(nameof(SampleCases))]
    public Task ConvertsInSample(string before, string after) => Verify.SampleConversion(before, after);
}
