using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;
using Shouldly.FromAssert.Tests.Infrastructure;

namespace Shouldly.FromAssert.Tests.ClassicAssert;

/// <summary>Assert.Throws/DoesNotThrow and their async forms, and Assert.Fail.</summary>
public class ExceptionAndFailTests
{
    private static IEnumerable<TestCaseData> Cases()
    {
        yield return new TestCaseData(
            "void ThrowException() { throw new ArgumentException(); }",
            "[|Assert.Throws<ArgumentException>(() => ThrowException())|];",
            "Should.Throw<ArgumentException>(() => ThrowException());"
        ).SetName("Assert.Throws");

        yield return new TestCaseData(
            "void DoNotThrow() { }",
            "[|Assert.DoesNotThrow(() => DoNotThrow())|];",
            "Should.NotThrow(() => DoNotThrow());"
        ).SetName("Assert.DoesNotThrow");
    }

    [Test]
    [TestCaseSource(nameof(Cases))]
    public Task Converts(string setup, string before, string after) => Verify.Conversion(setup, before, after);

    // From #34: forms that were reported but never converted, each from a real suite.
    private static IEnumerable<TestCaseData> SampleCases()
    {
        yield return new TestCaseData(
            @"var refused = [|Assert.ThrowsAsync<InvalidOperationException>(() => RefuseAsync())|];",
            @"var refused = Should.Throw<InvalidOperationException>(() => RefuseAsync());"
        ).SetName("Assert.ThrowsAsync");

        yield return new TestCaseData(
            @"[|Assert.DoesNotThrowAsync(() => Task.CompletedTask)|];",
            @"Should.NotThrow(() => Task.CompletedTask);"
        ).SetName("Assert.DoesNotThrowAsync");

        yield return new TestCaseData(
            @"[|Assert.Fail($""no {sample.Ratio} snapshot"")|];",
            @"throw new ShouldAssertException($""no {sample.Ratio} snapshot"");"
        ).SetName("Assert.Fail with a message");

        yield return new TestCaseData(
            @"if (sample.Lines.Count == 0) [|Assert.Fail()|];",
            @"if (sample.Lines.Count == 0) throw new ShouldAssertException(null);"
        ).SetName("Assert.Fail without a message as an embedded statement");
    }

    [Test]
    [TestCaseSource(nameof(SampleCases))]
    public Task ConvertsInSample(string before, string after) => Verify.SampleConversion(before, after);

    [Test]
    public async Task AssertFail_RemovesTheReturnAfterIt()
    {
        const string signature = "private async Task<IReadOnlyList<string>> SnapshotAsync()";
        await new CodeFixTest(
                TestSource.InSampleTest(@"for (var tries = 0; tries < 3; tries++)
            {
                await Task.Delay(1);
            }

            [|Assert.Fail(""no snapshot"")|];
            return new List<string>();", signature),
                TestSource.InSampleTest(@"for (var tries = 0; tries < 3; tries++)
            {
                await Task.Delay(1);
            }

            throw new ShouldAssertException(""no snapshot"");", signature))
            {
                CompilerDiagnostics = CompilerDiagnostics.Warnings
            }
            .RunAsync(CancellationToken.None);
    }
}
