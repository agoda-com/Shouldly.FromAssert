using NUnit.Framework;
using Shouldly.FromAssert.Tests.Infrastructure;

namespace Shouldly.FromAssert.Tests.AssertThat;

/// <summary>Assert.That with Does.Contain/StartWith/EndWith/Match on a string.</summary>
public class StringConstraintTests
{
    private static IEnumerable<TestCaseData> Cases()
    {
        yield return new TestCaseData(
            "var greeting = \"Hello, World!\";",
            "[|Assert.That(greeting, Does.Contain(\"World\"))|];",
            "greeting.ShouldContain(\"World\", Case.Sensitive);"
        ).SetName("Assert.That with Does.Contain");

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
            "var greeting = \"Hello, World!\";",
            "[|Assert.That(greeting, Does.Not.Contain(\"world\"))|];",
            "greeting.ShouldNotContain(\"world\", Case.Sensitive);"
        ).SetName("Assert.That with Does.Not.Contain on a string");

        yield return new TestCaseData(
            "var contestant = \"1337\";",
            "[|Assert.That(contestant, Does.Match(\"^13\"))|];",
            "contestant.ShouldMatch(\"^13\");"
        ).SetName("Assert.That with Does.Match");
    }

    [Test]
    [TestCaseSource(nameof(Cases))]
    public Task Converts(string setup, string before, string after) => Verify.Conversion(setup, before, after);

    // From #34: forms that were reported but never converted, each from a real suite.
    private static IEnumerable<TestCaseData> SampleCases()
    {
        yield return new TestCaseData(
            @"[|Assert.That(sample.Body, Does.Contain(""&#x2B;840 GB"").Or.Contain(""+840 GB""))|];",
            @"new[] { ""&#x2B;840 GB"", ""+840 GB"" }.ShouldContain(item => sample.Body.Contains(item));"
        ).SetName("Does.Contain Or Contain");

        yield return new TestCaseData(
            @"[|Assert.That(sample.Body, Does.Not.Match(@""\d,\d+,""), ""a comma decimal would corrupt the list"")|];",
            @"sample.Body.ShouldNotMatch(@""\d,\d+,"", ""a comma decimal would corrupt the list"");"
        ).SetName("Does.Not.Match with a message");
    }

    [Test]
    [TestCaseSource(nameof(SampleCases))]
    public Task ConvertsInSample(string before, string after) => Verify.SampleConversion(before, after);
}
