using NUnit.Framework;
using Shouldly.FromAssert.Tests.Infrastructure;

namespace Shouldly.FromAssert.Tests.AssertMultiple;

/// <summary>Assert.Multiple with an async lambda: awaits are hoisted into locals first.</summary>
public class AsyncAssertMultipleTests
{
    private static IEnumerable<TestCaseData> Cases()
    {
        yield return new TestCaseData(
            @"[|Assert.Multiple(async () =>
            {
                [|Assert.That(sample.Ratio, Is.EqualTo(0.8))|];
                [|Assert.That(sample.Body, Is.EqualTo(""image/png""))|];
                [|Assert.That(await sample.ReadAsByteArrayAsync(), Is.EqualTo(new byte[] { 1 }))|];
            })|];",
            @"var readAsByteArray = await sample.ReadAsByteArrayAsync();
            this.ShouldSatisfyAllConditions(
                () => sample.Ratio.ShouldBe(0.8),
                () => sample.Body.ShouldBe(""image/png""),
                () => readAsByteArray.ShouldBe(new byte[] { 1 }));"
        ).SetName("Assert.Multiple with an async lambda hoists the await into a local");

        yield return new TestCaseData(
            @"var status = 1;
            [|Assert.Multiple(async () =>
            {
                [|Assert.That(await GetStatusAsync(), Is.EqualTo(""Ordered""))|];
                [|Assert.That(await GetStatusAsync().ConfigureAwait(false), Does.StartWith(""Or""))|];
            })|];",
            @"var status = 1;
            var status1 = await GetStatusAsync();
            var status2 = await GetStatusAsync().ConfigureAwait(false);
            this.ShouldSatisfyAllConditions(
                () => status1.ShouldBe(""Ordered""),
                () => status2.ShouldStartWith(""Or""));"
        ).SetName("Assert.Multiple with an async lambda keeps hoisted names unique");

        yield return new TestCaseData(
            @"[|Assert.Multiple(async () => [|Assert.That(sample.Ratio, Is.EqualTo(0.8))|])|];",
            @"this.ShouldSatisfyAllConditions(
                () => sample.Ratio.ShouldBe(0.8));"
        ).SetName("Assert.Multiple with an async lambda and nothing to await");
    }

    [Test]
    [TestCaseSource(nameof(Cases))]
    public async Task Converts(string before, string after)
    {
        const string signature = "public async Task TestMethod()";
        await new CodeFixTest(TestSource.InSampleTest(before, signature), TestSource.InSampleTest(after, signature))
            .RunAsync(CancellationToken.None);
    }
}
