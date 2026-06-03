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

## API Comparison with FluentAssertions

FluentCheck provides the same fluent `Should()` DSL as FluentAssertions with matching method names and chaining patterns — but with **zero inherited dependencies** and a fully original implementation. Below are side-by-side comparisons.

### String Assertions

```csharp
var text = "Hello, World!";

// ── Exact match ──────────────────────────────────────────────── FluentCheck / FluentAssertions
text.Should().Be("Hello, World!");       // ✅ same API                   text.Should().Be("Hello, World!");

// ── Containment ────────────────────────────────────────────────
text.Should().Contain("Hello");          // ✅ same API                   text.Should().Contain("Hello");
text.Should().NotContain("goodbye");     // ✅ same API                   text.Should().NotContain("goodbye");
text.Should().StartWith("Hello");        // ✅ same API                   text.Should().StartWith("Hello");
text.Should().EndWith("!");              // ✅ same API                   text.Should().EndWith("!");

// ── Occurrence constraints (FluentCheck unique) ────────────────
text.Should().Contain("l", OccurrenceConstraint.Exactly(3));  // ✅ extra
text.Should().Contain("l", OccurrenceConstraint.AtLeast(2));  // ✅ extra
text.Should().Contain("l", OccurrenceConstraint.Once);        // ✅ extra

// ── Case-insensitive ───────────────────────────────────────────
text.Should().BeEquivalentOf("hello, world!", StringComparison.OrdinalIgnoreCase);
text.Should().ContainEquivalentOf("WORLD", StringComparison.OrdinalIgnoreCase);

// ── Regex ──────────────────────────────────────────────────────
text.Should().MatchRegex(@"Hello.*!");          // ✅ same API                   text.Should().Match(@"Hello.*!");
text.Should().NotMatchRegex(@"goodbye.*");      // ✅ same API                   text.Should().NotMatch(@"goodbye.*");

// ── Case checks ────────────────────────────────────────────────
"abc".Should().BeLowerCased();      // ✅ FluentCheck only            —
"ABC".Should().BeUpperCased();      // ✅ FluentCheck only            —

// ── Length ───────────────────────────────────────────────────────
text.Should().HaveLength(13);           // ✅ same API                   text.Should().HaveLength(13);

// ── OneOf / All / Any ──────────────────────────────────────────
text.Should().BeOneOf("Hello, World!", "Hi");   // ✅ same API          text.Should().BeOneOf("Hello, World!", "Hi");
text.Should().ContainAll("Hello", "World", "!"); // ✅ FluentCheck only —
text.Should().ContainAny("xyz", "Hello", "goodbye"); // ✅ FluentCheck only —
```

### Collection Assertions

```csharp
var numbers = new[] { 1, 2, 3, 4, 5 };

// ── Basic ────────────────────────────────────────────────────── FluentCheck / FluentAssertions
numbers.Should().Contain(3);              // ✅ same API              numbers.Should().Contain(3);
numbers.Should().NotContain(10);          // ✅ same API              numbers.Should().NotContain(10);
numbers.Should().HaveCount(5);            // ✅ same API              numbers.Should().HaveCount(5);
numbers.Should().BeEmpty();               // ✅ same API (negation)   numbers.Should().BeEmpty();

// ── Ordering ───────────────────────────────────────────────────
numbers.Should().ContainInOrder(2, 4);        // ✅ same API          numbers.Should().ContainInOrder(2, 4);
numbers.Should().ContainInConsecutiveOrder(3, 4); // ✅ same API      numbers.Should().ContainInConsecutiveOrder(3, 4);
numbers.Should().StartWith(1);                // ✅ FluentCheck only  —
numbers.Should().EndWith(5);                  // ✅ FluentCheck only  —

// ── Uniqueness ─────────────────────────────────────────────────
numbers.Should().OnlyHaveUniqueItems();   // ✅ same API            numbers.Should().OnlyHaveUniqueItems();
numbers.Should().OnlyHaveUniqueItems(n => n % 2); // ✅ FluentCheck only —
numbers.Should().NotHaveDuplicates();     // ✅ FluentCheck only    —

// ── Subset ─────────────────────────────────────────────────────
new[] { 1, 2 }.Should().BeSubsetOf(numbers);  // ✅ same API       new[] { 1, 2 }.Should().BeSubsetOf(numbers);

// ── Single / Predicate ─────────────────────────────────────────
numbers.Should().ContainSingle(n => n > 4);  // ✅ same API        numbers.Should().ContainSingle(n => n > 4);
numbers.Should().Satisfy(n => n > 10);       // ✅ FluentCheck only —

// ── Equal / Match ──────────────────────────────────────────────
numbers.Should().Equal(1, 2, 3, 4, 5);   // ✅ same API           numbers.Should().Equal(1, 2, 3, 4, 5);
numbers.Should().OnlyContain(n => n > 0); // ✅ same API           numbers.Should().OnlyContain(n => n > 0);
```

### String Collection Assertions

```csharp
var names = new[] { "alice", "BOB", "charlie" };

// ── Containment ────────────────────────────────────────────────
names.Should().Contain("alice");           // ✅ same API    list.Should().Contain("alice");
names.Should().NotContain("dave");                  // ✅ same API    list.Should().NotContain("dave");
names.Should().ContainAll("alice", "BOB");          // ✅ same       list.Should().Contain("alice").And.Contain("BOB");

// ── Case-sensitive ordering ────────────────────────────────────
names.Should().ContainInOrder("alice", "charlie");     // ✅ unique   —
names.Should().BeInExactOrder("alice", "BOB", "charlie"); // ✅ unique —

// ── Set equivalence ────────────────────────────────────────────
names.Should().BeEquivalentTo(new[] { "BOB", "charlie", "alice" }); // ✅ same pattern

// ── Index access ───────────────────────────────────────────────
names.Should().HaveElement("BOB", 1);              // ✅ FluentCheck only
```

### Object Deep Equivalency

```csharp
var expected = new Person { Name = "Alice", Age = 30, Tags = new[] { "admin" } };
var actual   = new Person { Name = "Alice", Age = 30, Tags = new[] { "admin" } };

// ── Deep structural comparison ─────────────────────────────────
actual.Should().BeEquivalentTo(expected);     // ✅ same API          actual.Should().BeEquivalentTo(expected);

// ── With options (FluentCheck builder pattern) ─────────────────
actual.Should().BeEquivalentTo(expected, opts => opts
    .IgnoringCase()                              // ✅ same        .IgnoringCase()
    .ExcludingProperty(p => p.Age);              // ✅ same          .IgnoringProperty("Age")
    .ExcludingField("internalId");                // ✅ FluentCheck only (fields)

// ── Reference equality ─────────────────────────────────────────
actual.Should().Be(expected);           // ✅ deep by default       — (BeSameReference for ref)
actual.Should().NotBe(expected);        // ✅ deep by default       —
```

### Numeric Assertions

```csharp
int value = 42;

// ── Comparison ─────────────────────────────────────────────────
value.Should().Be(42);                    // ✅ same API            value.Should().Be(42);
value.Should().BeGreaterThan(40);         // ✅ same API            value.Should().BeGreaterThan(40);
value.Should().BeLessThan(50);            // ✅ same API            value.Should().BeLessThan(50);
value.Should().BeAtLeast(42);             // ✅ same API            value.Should().BeAtLeast(42);
value.Should().BeAtMost(42);              // ✅ same API            value.Should().BeAtMost(42);

// ── Range ──────────────────────────────────────────────────────
value.Should().BeInRange(30, 50);       // ✅ same API            value.Should().BeInRange(30, 50);

// ── Sign ───────────────────────────────────────────────────────
42.Should().BePositive();               // ✅ same API            42.Should().BePositive();
(-5).Should().BeNegative();             // ✅ same API            (-5).Should().BeNegative();
0.Should().BeZero();                    // ✅ same API            0.Should().BeZero();

// ── Approximate ────────────────────────────────────────────────
3.14159.Should().BeApproximately(3.14, 0.01);  // ✅ same API     3.14159.Should().BeApproximately(3.14, 0.01);

// ── Negation ───────────────────────────────────────────────────
value.Should().NotBe(99);               // ✅ same API            value.Should().NotBe(99);
value.Should().NotBeGreaterThan(100);   // ✅ FluentCheck only    —
```

### Dictionary Assertions

```csharp
var dict = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };

// ── Key / Value ────────────────────────────────────────────────
dict.Should().ContainKey("a");          // ✅ same API            dict.Should().ContainKey("a");
dict.Should().ContainKeyAndValue("b", 2); // ✅ same API          dict.Should().ContainKeyAndValue("b", 2);
dict.Should().Contain("a", 1);          // ✅ FluentCheck only   —
dict.Should().NotContainKey("z");       // ✅ same API            dict.Should().NotContainKey("z");
dict.Should().NotContainValue(99);      // ✅ same API            dict.Should().NotContainValue(99);

// ── Bulk ───────────────────────────────────────────────────────
dict.Should().ContainKeys("a", "b");    // ✅ FluentCheck only    —
dict.Should().ContainValues(1, 2);      // ✅ FluentCheck only    —

// ── Equality ───────────────────────────────────────────────────
dict.Should().Equal(new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 });  // ✅ same  dict.Should().Equal(new Dictionary<string, int> { ... });
dict.Should().HaveSameCount(new Dictionary<string, int> { ["x"] = 0 });  // ✅ same     dict.Should().HaveSameCount(...);

// ── Empty ──────────────────────────────────────────────────────
dict.Should().NotContain();             // ✅ FluentCheck only    —  dict.Should().NotBeEmpty();
```

### Boolean Assertions

```csharp
bool flag = true;

// ── Truthiness ─────────────────────────────────────────────────
flag.Should().BeTrue();               // ✅ same API              flag.Should().BeTrue();
flag.Should().BeFalse();              // ❌ fails (subject is true)  flag.Should().BeFalse();  // ❌ fails
true.Should().Be(true);               // ✅ FluentCheck only     —
flag.Should().NotBe(false);           // ✅ FluentCheck only     —
```

### Enum Assertions

```csharp
Status status = Status.Active;

// ── Value ──────────────────────────────────────────────────────
status.Should().Be(Status.Active);        // ✅ same API           status.Should().Be(Status.Active);
status.Should().NotBe(Status.Inactive);   // ✅ same API           status.Should().NotBe(Status.Inactive);

// ── OneOf ──────────────────────────────────────────────────────
status.Should().BeOneOf(Status.Active, Status.Pending);  // ✅ same  status.Should().BeOneOf(Status.Active, Status.Pending);

// ── Flags ──────────────────────────────────────────────────────
Permissions p = Permissions.Read | Permissions.Write;
p.Should().HaveFlag(Permissions.Read);      // ✅ same API          p.Should().HaveFlag(Permissions.Read);
p.Should().NotHaveFlag(Permissions.Execute); // ✅ same API         p.Should().NotHaveFlag(Permissions.Execute);
p.Should().HaveValue((int)(Permissions.Read | Permissions.Write)); // ✅ FluentCheck only —
```

### DateTime / DateTimeOffset / DateOnly / TimeSpan

```csharp
var dt    = new DateTime(2026, 6, 3, 14, 30, 0);
var dto   = new DateTimeOffset(2026, 6, 3, 14, 30, 0, TimeSpan.FromHours(2));
var date  = new DateOnly(2026, 6, 3);
var ts    = TimeSpan.FromHours(2);

// ── DateTime / DateTimeOffset ──────────────────────────────────
dt.Should().Be(expectedDt);               // ✅ same API            dt.Should().Be(expectedDt);
dto.Should().BeBefore(threshold);         // ✅ same API            dto.Should().BeBefore(threshold);
dto.Should().BeAfter(threshold);          // ✅ same API            dto.Should().BeAfter(threshold);
dto.Should().BeOnOrBefore(threshold);     // ✅ same API            dto.Should().BeOnOrBefore(threshold);
dto.Should().BeOnOrAfter(threshold);      // ✅ same API            dto.Should().BeOnOrAfter(threshold);
dto.Should().BeInRange(min, max);         // ✅ same API            dto.Should().BeInRange(min, max);
dto.Should().BeCloseTo(expected, tolerance);  // ✅ same API        dto.Should().BeCloseTo(expected, tolerance);
dto.Should().BeInRelativeRange(TimeSpan.FromMinutes(5)); // ✅ same  dto.Should().BeInRelativeRange(TimeSpan.FromMinutes(5));
dto.Should().BeInThePast();               // ✅ FluentCheck only    —
dto.Should().BeInFuture();                // ✅ FluentCheck only    —
dto.Should().BeOneOf(d1, d2, d3);         // ✅ FluentCheck only   —
dto.Should().WithOffset(TimeSpan.FromHours(2)); // ✅ FluentCheck only —

// ── Date accessors ─────────────────────────────────────────────
dto.Should().HaveHour(14);                // ✅ same API            dto.Should().HaveHour(14);
dto.Should().HaveMinute(30);              // ✅ same API            dto.Should().HaveMinute(30);
dto.Should().HaveSecond(0);               // ✅ same API            dto.Should().HaveSecond(0);
dto.Should().HaveMillisecond(0);          // ✅ FluentCheck only    —
dto.Should().HaveOffset(TimeSpan.FromHours(2)); // ✅ FluentCheck only —
dto.Should().BeSameDateAs(otherDto);      // ✅ FluentCheck only    —

// ── Fluent date builders ───────────────────────────────────────
dto.Should().WithYear(2026).WithMonth(6).WithDay(3);  // ✅ FluentCheck only
// same as: .HaveYear(2026).And.HaveMonth(6).And.HaveDay(3)

// ── DateOnly ───────────────────────────────────────────────────
date.Should().Be(new DateOnly(2026, 6, 3));  // ✅ same API         date.Should().Be(new DateOnly(2026, 6, 3));

// ── TimeSpan ───────────────────────────────────────────────────
ts.Should().Be(TimeSpan.FromHours(2));        // ✅ same API         ts.Should().Be(TimeSpan.FromHours(2));
ts.Should().BeGreaterThan(TimeSpan.FromMinutes(60)); // ✅ same API  ts.Should().BeGreaterThan(TimeSpan.FromMinutes(60));
ts.Should().BeApproximately(TimeSpan.FromHours(2), TimeSpan.FromSeconds(1)); // ✅ same
ts.Should().BeWithin(TimeSpan.FromMinutes(1));  // ✅ FluentCheck only —
ts.Should().BeOneOf(TimeSpan.Zero, ts);         // ✅ FluentCheck only —
```

### Nullable Numeric Assertions

```csharp
int? maybe = 42;

maybe.Should().HaveValue(42);           // ✅ FluentCheck only      maybe.Should().HaveValue(42);
maybe.Should().BeInRange(30, 50);      // ✅ FluentCheck only      —
maybe.Should().BePositive();           // ✅ FluentCheck only      —
maybe.Should().NotHaveValue();         // ❌ fails (it has value)  maybe.Should().NotHaveValue();  // ❌ fails
```

### Exception Assertions

```csharp
Action act = () => throw new ArgumentException("bad value", "param");

// ── Throw type ─────────────────────────────────────────────────
act.Should().Throw<ArgumentException>();    // ✅ same API          act.Should().Throw<ArgumentException>();
act.Should().ThrowExactly<ArgumentException>(); // ✅ FluentCheck only —

// ── Chained assertions ─────────────────────────────────────────
act.Should().Throw<ArgumentException>()
   .WithMessageContaining("value")       // ✅ same API            .WithMessage("*value*")
   .WithParameterName("param")           // ✅ same API            .WithParameterName("param")
   .WithInnerException<InvalidOperationException>();  // ✅ same   .WithInnerException<InvalidOperationException>()
   .WithMessage("*inner*");

// ── Predicate filter ───────────────────────────────────────────
act.Should().Throw<ArgumentException>()
   .Where(ex => ex.Message.Contains("value"));  // ✅ FluentCheck only —

// ── Async ──────────────────────────────────────────────────────
async Task AsyncAct() => throw new TimeoutException();
await AsyncAct.Should().ThrowAsync<TimeoutException>();  // ✅ same  await AsyncAct.Should().ThrowAsync<TimeoutException>();
await AsyncAct.Should().ThrowAsyncExactly<TimeoutException>();  // ✅ FluentCheck only —

// ── Extract subject ────────────────────────────────────────────
var ex = act.Should().Throw<ArgumentException>().And;  // ✅ same   var ex = act.Should().Throw<ArgumentException>().And;
ex.Message.Should().Contain("value");

// ── Access raw exception ───────────────────────────────────────
act.Should().Throw<ArgumentException>().Subject.Message;  // ✅ FluentCheck only —
```

### AssertionScope (Batch Failures)

```csharp
// FluentCheck
AssertionScope.Begin();
"name".Should().HaveLength(5);
"bob".Should().HaveLength(5);
"carl".Should().HaveLength(5);
AssertionScope.Throw(); // throws once with all 3 failures

// FluentAssertions (equivalent)
using var scope = new AssertionScope();
"name".Should().HaveLength(5);
"bob".Should().HaveLength(5);
"carl".Should().HaveLength(5);
scope.Dispose(); // throws once with all 3 failures
```

### `.Which` Chaining (Subject Extraction)

```csharp
// FluentCheck — .Which on non-struct assertions
var name = list.Should().ContainSingle().Which;  // returns the matched item
name.Should().StartWith("A");

// FluentAssertions (same)
var name = list.Should().ContainSingle().Which;
name.Should().StartWith("A");
```

### Negation Methods

Every FluentCheck assertion type includes negated variants:

| Method | Negation |
|--------|----------|
| `Be(x)` | `NotBe(x)` |
| `Contain(x)` | `NotContain(x)` |
| `StartWith(x)` | `NotStartWith(x)` |
| `EndWith(x)` | `NotEndWith(x)` |
| `MatchRegex(x)` | `NotMatchRegex(x)` |
| `HaveCount(n)` | `NotHaveCount(n)` |
| `BeGreaterThan(x)` | `NotBeGreaterThan(x)` |
| `BePositive()` | `NotBePositive()` |
| `BeNegative()` | `NotBeNegative()` |
| `BeZero()` | `NotBeZero()` |
| `OnlyContain(p)` | `NotOnlyContain(p)` |
| `Satisfy(p)` | `NotSatisfy(p)` |
| `BeEquivalentTo(x)` | `NotBeEquivalentTo(x)` |
| `BeBefore(t)` | `NotBeBefore(t)` |
| `BeAfter(t)` | `NotBeAfter(t)` |
| `BeCloseTo(t, tol)` | `NotBeCloseTo(t, tol)` |

## License

MIT — see [LICENSE](LICENSE)
