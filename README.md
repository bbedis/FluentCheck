# FluentCheck

A free, permissively licensed assertion library for .NET testing with a fluent `Should()` DSL.

## Quick Start

```csharp
using FluentCheck;

// String assertions
"hello".Should().Be("hello").And.HaveLength(5);
"hello".Should().Contain("ell").And.BeLowerCased();

// Collection assertions
var list = new[] { 1, 2, 3 };
list.Should().Contain(2).And.HaveCount(3);
list.Should().OnlyHaveUniqueItems();

// Object deep equivalency
var obj1 = new { Name = "Alice", Age = 30 };
var obj2 = new { Name = "Alice", Age = 30 };
obj1.Should().BeEquivalentTo(obj2);

// Numeric assertions
42.Should().BeGreaterThan(40).And.BeInRange(30, 50);

// Exception assertions
Action act = () => throw new ArgumentException("bad value", "param");
act.Should().Throw<ArgumentException>()
   .WithParameterName("param")
   .WithMessageContaining("value");
```

## Install

```bash
dotnet add package FluentCheck
```

## Features

- **Fluent DSL** — `Should().Be().And.Contain().And.HaveCount()` chaining
- **Deep equivalency** — recursive property-by-property comparison with reflection-based engine
- **All common types** — string, collection, numeric, DateTime, DateTimeOffset, DateOnly, TimeSpan, bool, enum, nullable, dictionary, Action
- **Zero allocations on pass path** — readonly structs, InterpolatedStringHandler for lazy error messages
- **AssertionScope** — collect multiple failures before throwing
- **AOT compatible** — built with `IsAotCompatible=true`, compiled Expression Trees for hot paths
- **Negation methods** — `NotBe`, `NotContain`, `NotBeNull` etc. on all assertion types

## Performance

- Zero heap allocation for assertion chain (readonly structs throughout)
- `Fail.With` uses `InterpolatedStringHandler` — string built only on failure
- Deep equivalency uses compiled Expression Trees with per-type caching (zero reflection after first call)
- Benchmark against FluentAssertions: [see benchmarks project](FluentCheck.Benchmarks/)

## Comparison to FluentAssertions

FluentCheck is an independent assertion library — not a clone, fork, or replacement. Same testing conventions (method names like `Be()`, `Contain()`, `Throw<T>()` are standard testing patterns) but entirely original internal implementation.

## License

MIT — see [LICENSE](LICENSE)
