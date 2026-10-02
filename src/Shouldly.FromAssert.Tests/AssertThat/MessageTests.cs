using NUnit.Framework;
using Shouldly.FromAssert.Tests.Infrastructure;

namespace Shouldly.FromAssert.Tests.AssertThat;

/// <summary>Assert.That with a message: positional, named, Func<string>, and where Shouldly's string overload needs it named.</summary>
public class MessageTests
{
    private static IEnumerable<TestCaseData> Cases()
    {
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
    }

    [Test]
    [TestCaseSource(nameof(Cases))]
    public Task Converts(string setup, string before, string after) => Verify.Conversion(setup, before, after);
}
