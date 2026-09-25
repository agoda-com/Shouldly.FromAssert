# Shouldly.FromAssert

[![NuGet](https://img.shields.io/nuget/v/Shouldly.FromAssert.svg)](https://www.nuget.org/packages/Shouldly.FromAssert)
[![License](https://img.shields.io/github/license/agoda-com/Shouldly.FromAssert)](https://github.com/agoda-com/Shouldly.FromAssert/blob/main/LICENSE)

Because life's too short for hard-to-read assertions! 🔍

## What is this?

Shouldly.FromAssert is a Roslyn analyzer that helps you migrate from traditional NUnit assertions to the more human-readable [Shouldly](https://github.com/shouldly/shouldly) library. It's like having a friendly code review buddy who's really into making tests more readable (but doesn't drink your coffee).

## Features

- Automatically detects NUnit assertions and suggests Shouldly alternatives
- Provides code fixes to transform assertions with a single click
- Supports a wide range of assertion types:
  - Basic assertions (`Assert.AreEqual` → `ShouldBe`)
  - String assertions (`StringAssert.Contains` → `ShouldContain`)
  - Collection assertions (`CollectionAssert.Contains` → `ShouldContain`)
  - Type assertions (`Assert.IsInstanceOf` → `ShouldBeOfType`)
  - And many more!

## Installation

```shell
dotnet add package Shouldly.FromAssert
```

## Usage

1. Install the package
2. Write your tests as usual with NUnit
3. Look for the analyzer suggestions (they'll appear as warnings)
4. Click the lightbulb 💡 or press (Alt+Enter or Ctrl+. depending on the IDE religeon you practice)
5. Select "Convert to Shouldly"
6. Watch your assertions transform into beautiful, readable Shouldly statements

### Before and After

```csharp
// Before: 😕
Assert.That(contestant, Is.EqualTo(1337));
CollectionAssert.Contains(contestants, winner);
StringAssert.StartsWith("Hello", greeting);

// After: 😊
contestant.ShouldBe(1337);
contestants.ShouldContain(winner);
greeting.ShouldStartWith("Hello");
```

## Supported Conversions

Here are some examples of the transformations this analyzer can perform:

| NUnit                                              | Shouldly                                    |
|----------------------------------------------------|--------------------------------------------|
| `Assert.That(x, Is.EqualTo(y))`                    | `x.ShouldBe(y)`                           |
| `Assert.IsTrue(x > 10)`                            | `(x > 10).ShouldBeTrue()`                 |
| `Assert.IsNull(x)`                                 | `x.ShouldBeNull()`                        |
| `CollectionAssert.Contains(list, item)`            | `list.ShouldContain(item)`                |
| `StringAssert.StartsWith("Hi", str)`               | `str.ShouldStartWith("Hi")`               |
| `Assert.Throws<ArgumentException>(() => method())`  | `Should.Throw<ArgumentException>(() => method())` |
| `Assert.Multiple(() => { Assert.That(a, Is.EqualTo(1)); Assert.IsNotNull(b); })` | `this.ShouldSatisfyAllConditions(() => a.ShouldBe(1), () => b.ShouldNotBeNull())` |
| `Assert.That(x, Is.EqualTo(y), "why")`             | `x.ShouldBe(y, "why")`                     |
| `Assert.That(x, Is.Null)`                          | `x.ShouldBeNull()`                        |
| `Assert.That(list, Has.Count.EqualTo(3))`          | `list.Count.ShouldBe(3)`                  |
| `Assert.That(list, Is.EquivalentTo(other))`        | `list.ShouldBe(other, ignoreOrder: true)` |
| `Assert.That(s, Does.StartWith("/").And.EndWith(".png"))` | `s.ShouldStartWith("/");` `s.ShouldEndWith(".png");` |

And many more! Check out the tests for a complete list of supported conversions.

`Assert.Multiple` is only converted (and only reported) when its lambda is synchronous, contains nothing but expression statements, and sits in an instance member, since `ShouldSatisfyAllConditions` needs `this` and takes one `Action` per condition. Hoist locals and awaited values out of the block first. With nullable enabled, a `x!.` on the first statement no longer carries into the later lambdas; add `x.ShouldNotBeNull();` above the block instead.

## Contributing

Found a bug? Have a suggestion? Want to add support for more assertions? We'd love your help! Please feel free to:

1. Open an issue
2. Submit a pull request
3. Start a discussion
4. Share your success stories

## License

This project is licensed under the Apache License 2.0 - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- The awesome [Shouldly](https://github.com/shouldly/shouldly) team for making assertions more human
- The [Roslyn](https://github.com/dotnet/roslyn) team for making this possible
- Coffee ☕, for making developers possible

---

Made with 💚 by [Agoda](https://github.com/agoda-com)
