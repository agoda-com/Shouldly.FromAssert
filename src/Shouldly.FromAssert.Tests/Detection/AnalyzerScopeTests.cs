using NUnit.Framework;
using Shouldly.FromAssert.Tests.Infrastructure;

namespace Shouldly.FromAssert.Tests.Detection;

/// <summary>What is and isn't reported, whatever the assertion.</summary>
public class AnalyzerScopeTests
{
    // A full document: the test class declares its own Assert* helper.
    [Test]
    public async Task UnqualifiedMethodStartingWithAssert_IsNotFlagged()
    {
        var test = @"
using NUnit.Framework;
namespace TestNamespace
{
    public class TestClass
    {
        [Test]
        public void TestMethod()
        {
            AssertNoUnionTypedEnum(1337, ""auction"");
        }

        private static void AssertNoUnionTypedEnum(int node, string path) { }
    }
}";

        await new CodeFixTest(test, test).RunAsync(CancellationToken.None);
    }

    // A full document: it declares its own Assert class and has no NUnit using.
    [Test]
    public async Task UserDefinedAssertClass_IsNotFlagged()
    {
        var test = @"
namespace TestNamespace
{
    public static class Assert
    {
        public static void AreEqual(int expected, int actual) { }
    }

    public class TestClass
    {
        public void TestMethod()
        {
            Assert.AreEqual(1337, 1337);
        }
    }
}";

        await new CodeFixTest(test, test).RunAsync(CancellationToken.None);
    }

    // A full document: it needs a using static.
    [Test]
    public async Task UsingStaticNUnitAssert_IsFlagged()
    {
        var test = @"
using NUnit.Framework;
using static NUnit.Framework.Assert;
namespace TestNamespace
{
    public class TestClass
    {
        [Test]
        public void TestMethod()
        {
            [|That(1337, Is.EqualTo(1337))|];
        }
    }
}";

        await new AnalyzerOnlyTest(test).RunAsync(CancellationToken.None);
    }

    // A full document: it needs a using static.
    [Test]
    public async Task UsingStaticMultipleInStaticMethod_IsNotFlagged()
    {
        var test = @"
using NUnit.Framework;
using static NUnit.Framework.Assert;
namespace TestNamespace
{
    public class TestClass
    {
        [Test]
        public static void TestMethod()
        {
            Multiple(() =>
            {
                [|Assert.That(1337, Is.EqualTo(1337))|];
            });
        }
    }
}";

        await new AnalyzerOnlyTest(test).RunAsync(CancellationToken.None);
    }

    // Runner control, not assertions: there is nothing in Shouldly to convert them to (#32).
    [TestCase("Assert.Ignore(\"not yet\");")]
    [TestCase("Assert.Pass();")]
    [TestCase("Assert.Inconclusive(\"no data\");")]
    [TestCase("Assert.Warn(\"slow\");")]
    public async Task RunnerControl_IsNotFlagged(string statement)
    {
        await new AnalyzerOnlyTest(TestSource.InTestMethod(statement)).RunAsync(CancellationToken.None);
    }
}
