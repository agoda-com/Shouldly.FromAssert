using Microsoft.CodeAnalysis.Testing;

namespace Shouldly.FromAssert.Tests.Infrastructure;

/// <summary>Runs one before/after conversion case in the matching <see cref="TestSource"/> wrapper.</summary>
internal static class Verify
{
    /// <summary>The setup, then one assertion, in <see cref="TestSource.InTestMethod"/>.</summary>
    public static Task Conversion(string setup, string before, string after) =>
        new CodeFixTest(
                TestSource.InTestMethod(setup + "\n            " + before),
                TestSource.InTestMethod(setup + "\n            " + after))
            .RunAsync(CancellationToken.None);

    /// <summary>In <see cref="TestSource.InSampleTest"/>. Warnings are compared too, so a conversion that leaves e.g. CS8629 or CS0162 behind fails.</summary>
    public static Task SampleConversion(string before, string after) =>
        new CodeFixTest(TestSource.InSampleTest(before), TestSource.InSampleTest(after))
            {
                CompilerDiagnostics = CompilerDiagnostics.Warnings
            }
            .RunAsync(CancellationToken.None);

    /// <summary>In <see cref="TestSource.InNullableReportTest"/>. Warnings are compared too, so a fix that leaves CS8604 behind fails.</summary>
    public static Task ReportConversion(string before, string after) =>
        new CodeFixTest(TestSource.InNullableReportTest(before), TestSource.InNullableReportTest(after))
            {
                CompilerDiagnostics = CompilerDiagnostics.Warnings
            }
            .RunAsync(CancellationToken.None);
}
