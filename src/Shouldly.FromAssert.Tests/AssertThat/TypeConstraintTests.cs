using NUnit.Framework;
using Shouldly.FromAssert.Tests.Infrastructure;

namespace Shouldly.FromAssert.Tests.AssertThat;

/// <summary>Assert.That with Is.TypeOf and Is.InstanceOf constraints, generic or with a typeof argument.</summary>
public class TypeConstraintTests
{
    private static IEnumerable<TestCaseData> SampleCases()
    {
        yield return new TestCaseData(
            @"object name = ""Joel"";
            [|Assert.That(name, Is.TypeOf<string>())|];",
            @"object name = ""Joel"";
            name.ShouldBeOfType<string>();"
        ).SetName("Is.TypeOf<T>");

        yield return new TestCaseData(
            @"object name = ""Joel"";
            [|Assert.That(name, Is.TypeOf(typeof(string)))|];",
            @"object name = ""Joel"";
            name.ShouldBeOfType<string>();"
        ).SetName("Is.TypeOf(typeof(T))");

        yield return new TestCaseData(
            @"object name = ""Joel"";
            [|Assert.That(name, Is.Not.TypeOf<int>())|];",
            @"object name = ""Joel"";
            name.ShouldNotBeOfType<int>();"
        ).SetName("Is.Not.TypeOf<T>");

        yield return new TestCaseData(
            @"object name = ""Joel"";
            [|Assert.That(name, Is.Not.TypeOf(typeof(int)))|];",
            @"object name = ""Joel"";
            name.ShouldNotBeOfType<int>();"
        ).SetName("Is.Not.TypeOf(typeof(T))");

        yield return new TestCaseData(
            @"object names = new List<string>();
            [|Assert.That(names, Is.InstanceOf<IEnumerable<string>>())|];",
            @"object names = new List<string>();
            names.ShouldBeAssignableTo<IEnumerable<string>>();"
        ).SetName("Is.InstanceOf<T>");

        yield return new TestCaseData(
            @"object names = new List<string>();
            [|Assert.That(names, Is.InstanceOf(typeof(IEnumerable<string>)))|];",
            @"object names = new List<string>();
            names.ShouldBeAssignableTo<IEnumerable<string>>();"
        ).SetName("Is.InstanceOf(typeof(T))");

        yield return new TestCaseData(
            @"object name = ""Joel"";
            [|Assert.That(name, Is.Not.InstanceOf<IEnumerable<int>>())|];",
            @"object name = ""Joel"";
            name.ShouldNotBeAssignableTo<IEnumerable<int>>();"
        ).SetName("Is.Not.InstanceOf<T>");

        yield return new TestCaseData(
            @"object name = ""Joel"";
            [|Assert.That(name, Is.TypeOf<string>(), ""name should be a string"")|];",
            @"object name = ""Joel"";
            name.ShouldBeOfType<string>(""name should be a string"");"
        ).SetName("Is.TypeOf<T> with a message");
    }

    [Test]
    [TestCaseSource(nameof(SampleCases))]
    public Task ConvertsInSample(string before, string after) => Verify.SampleConversion(before, after);

    // Shouldly's type assertions only take a type argument, so a Type held in a variable is left for a hand conversion.
    [Test]
    public async Task IsTypeOfWithATypeVariable_IsLeftAlone()
    {
        var source = TestSource.InSampleTest(@"object name = ""Joel"";
            var expected = typeof(string);
            [|Assert.That(name, Is.TypeOf(expected))|];");
        await new CodeFixTest(source, source)
            {
                NumberOfIncrementalIterations = 1,
                NumberOfFixAllIterations = 1
            }
            .RunAsync(CancellationToken.None);
    }
}
