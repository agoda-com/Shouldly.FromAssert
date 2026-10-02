using NUnit.Framework;
using Shouldly.FromAssert.Tests.Infrastructure;

namespace Shouldly.FromAssert.Tests.ClassicAssert;

/// <summary>StringAssert.* methods.</summary>
public class StringAssertTests
{
    private static IEnumerable<TestCaseData> Cases()
    {
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
    }

    [Test]
    [TestCaseSource(nameof(Cases))]
    public Task Converts(string setup, string before, string after) => Verify.Conversion(setup, before, after);
}
