using NUnit.Framework;
using Shouldly.FromAssert.Tests.Infrastructure;

namespace Shouldly.FromAssert.Tests.AssertMultiple;

/// <summary>Assert.Multiple forms that are not reported, because converting them would change behaviour.</summary>
public class UnconvertibleAssertMultipleTests
{
    private static IEnumerable<TestCaseData> Cases()
    {
        yield return new TestCaseData(
            "public async Task TestMethod()",
            @"var contestant = 1337;
            Assert.Multiple(async () =>
            {
                await Task.Yield();
                [|Assert.That(contestant, Is.EqualTo(1337))|];
            });"
        ).SetName("Assert.Multiple with an async lambda is not reported");

        yield return new TestCaseData(
            "public void TestMethod()",
            @"Assert.Multiple(() =>
            {
                var contestant = 1337;
                [|Assert.That(contestant, Is.EqualTo(1337))|];
            });"
        ).SetName("Assert.Multiple declaring a local is not reported");

        yield return new TestCaseData(
            "public static void TestMethod()",
            @"var contestant = 1337;
            Assert.Multiple(() =>
            {
                [|Assert.That(contestant, Is.EqualTo(1337))|];
            });"
        ).SetName("Assert.Multiple in a static method is not reported");

        yield return new TestCaseData(
            "public static void TestMethod()",
            @"var contestant = 1337;
            NUnit.Framework.Assert.Multiple(() =>
            {
                [|Assert.That(contestant, Is.EqualTo(1337))|];
            });"
        ).SetName("Fully qualified Assert.Multiple in a static method is not reported");
    }

    [Test]
    [TestCaseSource(nameof(Cases))]
    public Task IsNotReported(string signature, string body) =>
        new AnalyzerOnlyTest(TestSource.InTestMethod(body, signature)).RunAsync(CancellationToken.None);

    private static IEnumerable<TestCaseData> AsyncCases()
    {
        yield return new TestCaseData(
            "public void TestMethod()",
            @"Assert.Multiple(async () =>
            {
                [|Assert.That(await GetStatusAsync(), Is.EqualTo(""Ordered""))|];
            });"
        ).SetName("Assert.Multiple with an async lambda in a sync method is not reported");

        yield return new TestCaseData(
            "public async Task TestMethod()",
            @"Assert.Multiple(async () =>
            {
                [|Assert.That(sample.Body ?? await GetStatusAsync(), Is.EqualTo(""Ordered""))|];
            });"
        ).SetName("Assert.Multiple whose await is only evaluated on ?? is not reported");
    }

    [Test]
    [TestCaseSource(nameof(AsyncCases))]
    public Task AsyncIsNotReported(string signature, string body) =>
        new AnalyzerOnlyTest(TestSource.InSampleTest(body, signature)).RunAsync(CancellationToken.None);
}
