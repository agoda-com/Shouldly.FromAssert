using NUnit.Framework;
using Shouldly.FromAssert.Tests.Infrastructure;

namespace Shouldly.FromAssert.Tests.AssertThat;

/// <summary>Constraints built from others: And chains and conditional constraints.</summary>
public class CompositionTests
{
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
}
