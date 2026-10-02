using NUnit.Framework;
using Shouldly.FromAssert.Tests.Infrastructure;

namespace Shouldly.FromAssert.Tests.AssertThat;

/// <summary>Assert.That with equality, sameness, null, bool and zero constraints, and a bare bool.</summary>
public class EqualityAndNullTests
{
    private static IEnumerable<TestCaseData> Cases()
    {
        yield return new TestCaseData(
            "var contestant = 1337;",
            "[|Assert.That(contestant, Is.EqualTo(1337))|];",
            "contestant.ShouldBe(1337);"
        ).SetName("Assert.That with Is.EqualTo");

        yield return new TestCaseData(
            "var contestant = 1337;",
            "[|Assert.That(contestant, Is.Not.EqualTo(1336))|];",
            "contestant.ShouldNotBe(1336);"
        ).SetName("Assert.That with Is.Not.EqualTo");

        yield return new TestCaseData(
            "var contestant = new object(); var other = contestant;",
            "[|Assert.That(contestant, Is.SameAs(other))|];",
            "contestant.ShouldBeSameAs(other);"
        ).SetName("Assert.That with Is.SameAs");

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
            "string contestant = null;",
            "[|Assert.That(contestant, (Is.Null))|];",
            "contestant.ShouldBeNull();"
        ).SetName("Assert.That with a parenthesised constraint");

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
    }

    [Test]
    [TestCaseSource(nameof(Cases))]
    public Task Converts(string setup, string before, string after) => Verify.Conversion(setup, before, after);

    // From #34: forms that were reported but never converted, each from a real suite.
    private static IEnumerable<TestCaseData> SampleCases()
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
    }

    [Test]
    [TestCaseSource(nameof(SampleCases))]
    public Task ConvertsInSample(string before, string after) => Verify.SampleConversion(before, after);
}
