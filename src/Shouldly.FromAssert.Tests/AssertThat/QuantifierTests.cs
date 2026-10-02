using NUnit.Framework;
using Shouldly.FromAssert.Tests.Infrastructure;

namespace Shouldly.FromAssert.Tests.AssertThat;

/// <summary>Assert.That with Is.All, Has.All, Has.Some and Has.None.</summary>
public class QuantifierTests
{
    private static IEnumerable<TestCaseData> Cases()
    {
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
            "var contestants = new List<bool> { false };",
            "[|Assert.That(contestants, Has.All.False)|];",
            "contestants.ShouldAllBe(item => !item);"
        ).SetName("Assert.That with Has.All.False");
    }

    [Test]
    [TestCaseSource(nameof(Cases))]
    public Task Converts(string setup, string before, string after) => Verify.Conversion(setup, before, after);

    // From #34: forms that were reported but never converted, each from a real suite.
    private static IEnumerable<TestCaseData> SampleCases()
    {
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
    }

    [Test]
    [TestCaseSource(nameof(SampleCases))]
    public Task ConvertsInSample(string before, string after) => Verify.SampleConversion(before, after);

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
}
