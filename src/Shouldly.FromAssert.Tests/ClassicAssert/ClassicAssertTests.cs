using NUnit.Framework;
using Shouldly.FromAssert.Tests.Infrastructure;

namespace Shouldly.FromAssert.Tests.ClassicAssert;

/// <summary>Classic Assert.* methods.</summary>
public class ClassicAssertTests
{
    private static IEnumerable<TestCaseData> Cases()
    {
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
    }

    [Test]
    [TestCaseSource(nameof(Cases))]
    public Task Converts(string setup, string before, string after) => Verify.Conversion(setup, before, after);
}
