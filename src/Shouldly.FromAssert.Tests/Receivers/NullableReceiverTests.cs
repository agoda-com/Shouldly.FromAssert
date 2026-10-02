using NUnit.Framework;
using Shouldly.FromAssert.Tests.Infrastructure;

namespace Shouldly.FromAssert.Tests.Receivers;

/// <summary>A maybe-null string receiver, and when ShouldNotBeNull() must not be chained.</summary>
public class NullableReceiverTests
{
    // Nullable receivers (#35): a maybe-null string gets ShouldNotBeNull() chained so the fix leaves no CS8604 behind.
    private static IEnumerable<TestCaseData> ReportCases()
    {
        yield return new TestCaseData(
            @"[|Assert.That(report.Error, Does.Contain(""no route to host""))|];",
            @"report.Error.ShouldNotBeNull().ShouldContain(""no route to host"", Case.Sensitive);"
        ).SetName("Does.Contain on a string? property chains ShouldNotBeNull");

        yield return new TestCaseData(
            @"var error = GetError();
            [|Assert.That(error, Does.StartWith(""no""))|];",
            @"var error = GetError();
            error.ShouldNotBeNull().ShouldStartWith(""no"");"
        ).SetName("Does.StartWith on a string? local chains ShouldNotBeNull");

        yield return new TestCaseData(
            @"[|Assert.That(report.Error, Does.EndWith(""host""))|];",
            @"report.Error.ShouldNotBeNull().ShouldEndWith(""host"");"
        ).SetName("Does.EndWith on a string? chains ShouldNotBeNull");

        yield return new TestCaseData(
            @"[|Assert.That(report.Error, Does.Match(""^no""))|];",
            @"report.Error.ShouldNotBeNull().ShouldMatch(""^no"");"
        ).SetName("Does.Match on a string? chains ShouldNotBeNull");

        yield return new TestCaseData(
            @"[|StringAssert.Contains(""route"", report.Error)|];",
            @"report.Error.ShouldNotBeNull().ShouldContain(""route"", Case.Sensitive);"
        ).SetName("StringAssert.Contains on a string? chains ShouldNotBeNull");

        yield return new TestCaseData(
            @"[|Assert.That(report.Error, Does.Contain(""route""), ""the error names the cause"")|];",
            @"report.Error.ShouldNotBeNull().ShouldContain(""route"", Case.Sensitive, ""the error names the cause"");"
        ).SetName("Does.Contain on a string? with a message chains ShouldNotBeNull");

        yield return new TestCaseData(
            @"[|Assert.That(report.Error, Does.StartWith(""no"").And.Contain(""route""))|];",
            @"report.Error.ShouldNotBeNull().ShouldStartWith(""no"");
            report.Error.ShouldNotBeNull().ShouldContain(""route"", Case.Sensitive);"
        ).SetName("And chain on a string? chains ShouldNotBeNull on every link");

        yield return new TestCaseData(
            @"[|Assert.That(report.Name, Does.Contain(""dash""))|];",
            @"report.Name.ShouldContain(""dash"", Case.Sensitive);"
        ).SetName("Does.Contain on a non-nullable string is unchanged");

        yield return new TestCaseData(
            @"string? error = ""no route to host"";
            [|Assert.That(error, Does.Contain(""route""))|];",
            @"string? error = ""no route to host"";
            error.ShouldContain(""route"", Case.Sensitive);"
        ).SetName("Does.Contain on a string? known to be non-null is unchanged");

        yield return new TestCaseData(
            @"[|Assert.That(report.Tags, Does.Contain(""dash""))|];",
            @"report.Tags.ShouldContain(""dash"");"
        ).SetName("Does.Contain on a collection is unchanged");
    }

    [Test]
    [TestCaseSource(nameof(ReportCases))]
    public Task ConvertsOnReport(string before, string after) => Verify.ReportConversion(before, after);

    // NUnit's Does.Not.Contain passes on null, so chaining ShouldNotBeNull would make the assert stricter.
    [Test]
    public async Task DoesNotContainOnNullableString_DoesNotChainShouldNotBeNull()
    {
        await new CodeFixTest(
                TestSource.InNullableReportTest(@"[|Assert.That(report.Error, Does.Not.Contain(""route""))|];"),
                TestSource.InNullableReportTest(@"report.Error.ShouldNotContain(""route"", Case.Sensitive);"))
            .RunAsync(CancellationToken.None);
    }
}
