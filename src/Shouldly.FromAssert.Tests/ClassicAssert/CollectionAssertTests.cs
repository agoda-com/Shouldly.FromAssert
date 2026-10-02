using NUnit.Framework;
using Shouldly.FromAssert.Tests.Infrastructure;

namespace Shouldly.FromAssert.Tests.ClassicAssert;

/// <summary>CollectionAssert.* methods (and Assert.Contains).</summary>
public class CollectionAssertTests
{
    private static IEnumerable<TestCaseData> Cases()
    {
        yield return new TestCaseData(
            "var contestants = new List<int> { 1337, 2448, 3559 };",
            "[|Assert.Contains(1337, contestants)|];",
            "contestants.ShouldContain(1337);"
        ).SetName("CollectionAssert.Contains");

        yield return new TestCaseData(
            "var contestants = new List<int> { 1337, 2448, 3559 };",
            "[|CollectionAssert.DoesNotContain(contestants, 1336)|];",
            "contestants.ShouldNotContain(1336);"
        ).SetName("CollectionAssert.DoesNotContain");

        yield return new TestCaseData(
            "var contestants = new List<int>();",
            "[|CollectionAssert.IsEmpty(contestants)|];",
            "contestants.ShouldBeEmpty();"
        ).SetName("CollectionAssert.IsEmpty");

        yield return new TestCaseData(
            "var contestants = new List<int> { 1337 };",
            "[|CollectionAssert.IsNotEmpty(contestants)|];",
            "contestants.ShouldNotBeEmpty();"
        ).SetName("CollectionAssert.IsNotEmpty");

        yield return new TestCaseData(
            "var expected = new List<int> { 1, 2, 3 }; var actual = new List<int> { 1, 2, 3 };",
            "[|CollectionAssert.AreEqual(expected, actual)|];",
            "actual.ShouldBe(expected);"
        ).SetName("CollectionAssert.AreEqual");

        yield return new TestCaseData(
            "var expected = new List<int> { 1, 2, 3 }; var actual = new List<int> { 3, 2, 1 };",
            "[|CollectionAssert.AreEquivalent(expected, actual)|];",
            "actual.ShouldBe(expected, ignoreOrder: true);"
        ).SetName("CollectionAssert.AreEquivalent");

        yield return new TestCaseData(
            "var collection = new List<string> { \"a\", \"b\", \"c\" };",
            "[|CollectionAssert.AllItemsAreInstancesOfType(collection, typeof(string))|];",
            "collection.ShouldAllBe(item => item is string);"
        ).SetName("CollectionAssert.AllItemsAreInstancesOfType");

        yield return new TestCaseData(
            "var collection = new List<string> { \"a\", \"b\", \"c\" };",
            "[|CollectionAssert.AllItemsAreNotNull(collection)|];",
            "collection.ShouldNotContain(item => item == null);"
        ).SetName("CollectionAssert.AllItemsAreNotNull");

        yield return new TestCaseData(
            "var collection = new List<int> { 1, 2, 3 };",
            "[|CollectionAssert.AllItemsAreUnique(collection)|];",
            "collection.ShouldBeUnique();"
        ).SetName("CollectionAssert.AllItemsAreUnique");
    }

    [Test]
    [TestCaseSource(nameof(Cases))]
    public Task Converts(string setup, string before, string after) => Verify.Conversion(setup, before, after);
}
