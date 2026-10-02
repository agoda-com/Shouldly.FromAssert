using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;
using Shouldly.FromAssert.Tests.Infrastructure;

namespace Shouldly.FromAssert.Tests.AssertMultiple;

/// <summary>Assert.Multiple with a sync lambda becomes ShouldSatisfyAllConditions.</summary>
public class AssertMultipleTests
{
    private static IEnumerable<TestCaseData> MultipleCases()
    {
        yield return new TestCaseData(
            @"var contestant = 1337;
            var name = ""Joel"";
            [|Assert.Multiple(() =>
            {
                [|Assert.That(contestant, Is.EqualTo(1337))|];
                [|Assert.IsNotNull(name)|];
            })|];",
            @"var contestant = 1337;
            var name = ""Joel"";
            this.ShouldSatisfyAllConditions(
                () => contestant.ShouldBe(1337),
                () => name.ShouldNotBeNull());"
        ).SetName("Assert.Multiple converts each inner assertion into a condition");

        yield return new TestCaseData(
            @"var contestants = new List<int> { 1 };
            [|Assert.Multiple(() =>
            {
                [|Assert.That(contestants, Has.Exactly(1).Items)|];
                contestants.ShouldNotBeEmpty();
            })|];",
            @"var contestants = new List<int> { 1 };
            this.ShouldSatisfyAllConditions(
                () => [|Assert.That(contestants, Has.Exactly(1).Items)|],
                () => contestants.ShouldNotBeEmpty());"
        ).SetName("Assert.Multiple keeps statements it cannot convert");

        yield return new TestCaseData(
            @"var contestant = 1337;
            [|Assert.Multiple(() => [|Assert.AreEqual(1337, contestant)|])|];",
            @"var contestant = 1337;
            this.ShouldSatisfyAllConditions(
                () => contestant.ShouldBe(1337));"
        ).SetName("Assert.Multiple with an expression-bodied lambda");

        yield return new TestCaseData(
            @"var contestant = 1337;
            [|Assert.Multiple(() =>
            {
                [|Assert.That(contestant, Is.EqualTo(1337), ""leet"")|];
                [|Assert.That(contestant > 1000)|];
                [|Assert.That(contestant, Is.GreaterThan(1000).And.LessThan(2000))|];
            })|];",
            @"var contestant = 1337;
            this.ShouldSatisfyAllConditions(
                () => contestant.ShouldBe(1337, ""leet""),
                () => (contestant > 1000).ShouldBeTrue(),
                () => contestant.ShouldBeGreaterThan(1000),
                () => contestant.ShouldBeLessThan(2000));"
        ).SetName("Assert.Multiple converts inner asserts with a message, a bare bool and an And chain");

        yield return new TestCaseData(
            @"var contestant = 1337;
            [|NUnit.Framework.Assert.Multiple(() =>
            {
                [|Assert.That(contestant, Is.EqualTo(1337))|];
            })|];",
            @"var contestant = 1337;
            this.ShouldSatisfyAllConditions(
                () => contestant.ShouldBe(1337));"
        ).SetName("Assert.Multiple fully qualified");

        yield return new TestCaseData(
            @"var name = ""Joel"";
            var uris = new List<Uri> { new Uri(""https://example.com"") };
            [|Assert.Multiple(() =>
            {
                [|Assert.That(name, Is.Not.Null)|];
                [|Assert.That(uris, Has.Count.EqualTo(1))|];
            })|];",
            @"var name = ""Joel"";
            var uris = new List<Uri> { new Uri(""https://example.com"") };
            this.ShouldSatisfyAllConditions(
                () => { name.ShouldNotBeNull(); },
                () => uris.Count.ShouldBe(1));"
        ).SetName("Assert.Multiple gives a first condition that returns a string a block body");

        yield return new TestCaseData(
            @"var name = ""Joel"";
            [|Assert.Multiple(() => [|Assert.IsNotNull(name)|])|];",
            @"var name = ""Joel"";
            this.ShouldSatisfyAllConditions(
                () => { name.ShouldNotBeNull(); });"
        ).SetName("Assert.Multiple with a single condition that returns a string");

        yield return new TestCaseData(
            @"object name = ""Joel"";
            [|Assert.Multiple(() =>
            {
                [|Assert.That(name, Is.TypeOf(typeof(string)))|];
                [|Assert.IsNotNull(name)|];
            })|];",
            @"object name = ""Joel"";
            this.ShouldSatisfyAllConditions(
                () => { name.ShouldBeOfType<string>(); },
                () => name.ShouldNotBeNull());"
        ).SetName("Assert.Multiple gives a first ShouldBeOfType<string> a block body");

        yield return new TestCaseData(
            @"var name = ""Joel"";
            var contestants = new List<int> { 1, 3, 3, 7 };
            [|Assert.Multiple(() =>
            {
                [|Assert.That(contestants, Is.EqualTo(new[] { 1, 3, 3, 7 }), ""in order"")|];
                [|Assert.That(name, Is.Not.Null.And.Not.Empty)|];
            })|];",
            @"var name = ""Joel"";
            var contestants = new List<int> { 1, 3, 3, 7 };
            this.ShouldSatisfyAllConditions(
                () => contestants.ShouldBe(new[] { 1, 3, 3, 7 }, ignoreOrder: false, customMessage: ""in order""),
                () => name.ShouldNotBeNullOrEmpty());"
        ).SetName("Assert.Multiple with a sequence message and Is.Not.Null.And.Not.Empty");
    }

    [Test]
    [TestCaseSource(nameof(MultipleCases))]
    public async Task Converts(string before, string after)
    {
        var codeFixTest = new CodeFixTest(TestSource.InTestMethod(before), TestSource.InTestMethod(after));
        // Statements left as NUnit asserts are still reported after the fix.
        codeFixTest.FixedState.MarkupHandling = MarkupMode.Allow;
        // ...so fix-all takes a second, no-op pass over them once the Assert.Multiple around them has gone.
        if (after.Contains("[|")) codeFixTest.NumberOfFixAllIterations = 2;

        await codeFixTest.RunAsync(CancellationToken.None);
    }

    // From #34 (forms that were reported but never converted, each from a real suite) and #42.
    private static IEnumerable<TestCaseData> SampleCases()
    {
        yield return new TestCaseData(
            @"var uris = new List<string> { ""https://a"" };
            [|Assert.Multiple(() =>
            {
                [|Assert.That(uris, Is.All.StartsWith(""https://""))|];
                [|Assert.That(uris, Has.Count.EqualTo(1))|];
            })|];",
            @"var uris = new List<string> { ""https://a"" };
            this.ShouldSatisfyAllConditions(
                () => uris.ShouldAllBe(item => item.StartsWith(""https://"", StringComparison.Ordinal)),
                () => uris.Count.ShouldBe(1));"
        ).SetName("Is.All.StartsWith inside Assert.Multiple");

        yield return new TestCaseData(
            @"var lines = new List<string> { ""GET http://books.test:8080/api/stats"" };
            [|Assert.Multiple(() =>
            {
                [|Assert.That(lines, Has.Some.Contains(""/api/stats""))|];
                [|Assert.That(lines, Has.None.Contains(""key-for-tests""))|];
            })|];",
            @"var lines = new List<string> { ""GET http://books.test:8080/api/stats"" };
            this.ShouldSatisfyAllConditions(
                () => lines.ShouldContain(item => item.Contains(""/api/stats"")),
                () => lines.ShouldNotContain(item => item.Contains(""key-for-tests"")));"
        ).SetName("Has.Some and Has.None Contains inside Assert.Multiple");

        yield return new TestCaseData(
            @"object name = ""Joel"";
            [|Assert.Multiple(() =>
            {
                [|Assert.That(name, Is.TypeOf<string>())|];
                [|Assert.That(name, Is.Not.InstanceOf<int>())|];
            })|];",
            @"object name = ""Joel"";
            this.ShouldSatisfyAllConditions(
                () => { name.ShouldBeOfType<string>(); },
                () => name.ShouldNotBeAssignableTo<int>());"
        ).SetName("Is.TypeOf<T> inside Assert.Multiple");
    }

    [Test]
    [TestCaseSource(nameof(SampleCases))]
    public Task ConvertsInSample(string before, string after) => Verify.SampleConversion(before, after);

    // Nullable receivers (#35): a maybe-null string gets ShouldNotBeNull() chained so the fix leaves no CS8604 behind.
    private static IEnumerable<TestCaseData> ReportCases()
    {
        yield return new TestCaseData(
            @"[|Assert.Multiple(() =>
            {
                [|Assert.That(report.Error, Does.Contain(""route""))|];
                [|Assert.That(report.Error, Is.Not.Null)|];
            })|];",
            @"this.ShouldSatisfyAllConditions(
                () => report.Error.ShouldNotBeNull().ShouldContain(""route"", Case.Sensitive),
                () => report.Error.ShouldNotBeNull());"
        ).SetName("Does.Contain on a string? inside Assert.Multiple chains ShouldNotBeNull");
    }

    [Test]
    [TestCaseSource(nameof(ReportCases))]
    public Task ConvertsOnReport(string before, string after) => Verify.ReportConversion(before, after);

    // dotnet format runs the fix-all once, so an Assert.Multiple, the asserts inside it and the asserts around it
    // must all convert in a single pass (#32).
    [Test]
    public async Task AssertMultiple_FixAllConvertsInOnePass()
    {
        await new CodeFixTest(
                TestSource.InTestMethod(@"var contestant = 1337;
            var name = ""Joel"";
            [|Assert.That(contestant, Is.GreaterThan(1000))|];
            [|Assert.Multiple(() =>
            {
                [|Assert.That(contestant, Is.EqualTo(1337))|];
                [|Assert.AreEqual(""Joel"", name)|];
                [|Assert.IsNotNull(name)|];
            })|];
            [|Assert.Multiple(() => [|Assert.AreEqual(1337, contestant)|])|];
            [|Assert.IsTrue(contestant > 1000)|];"),
                TestSource.InTestMethod(@"var contestant = 1337;
            var name = ""Joel"";
            contestant.ShouldBeGreaterThan(1000);
            this.ShouldSatisfyAllConditions(
                () => contestant.ShouldBe(1337),
                () => name.ShouldBe(""Joel""),
                () => name.ShouldNotBeNull());
            this.ShouldSatisfyAllConditions(
                () => contestant.ShouldBe(1337));
            (contestant > 1000).ShouldBeTrue();"))
            {
                NumberOfFixAllIterations = 1,
                CodeFixTestBehaviors = CodeFixTestBehaviors.SkipFixAllInProjectCheck | CodeFixTestBehaviors.SkipFixAllInSolutionCheck
            }
            .RunAsync(CancellationToken.None);
    }
}
