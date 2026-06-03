# CLAUDE.md

This is **FluentCheck** — an independent, free assertion library for .NET testing. It provides a fluent `Should()` DSL for tests: `subject.Should().Be(...).And.Contain(...)`. The API surface matches common assertion library conventions (same method names, same chaining patterns) but the internal implementation is **entirely original** — no copied code, no reverse-engineered internals, no IP from other libraries.

## Positioning

- Free, permissively licensed assertion library — not a clone, fork, or replacement of any commercial product
- Same testing ergonomics (method names like `Be()`, `Contain()`, `Throw<T>()` are standard testing conventions, not proprietary)
- All internal implementation written from scratch based on the public API surface described in testing docs
- Do NOT reference FluentAssertions source code while developing this project
- Do NOT copy error message text, exception formatting, or internal data structures from other libraries

## Project Structure

```
FluentCheck/                     # Library (net10.0, C#14, IsAotCompatible=true)
  ShouldExtensions.cs            # 13+ Should() entry points for all subject types
  ObjectAssertions.cs            # Deep equivalency (reflection), reference equality, type checks
  StringAssertions.cs            # String assertions
  CollectionAssertions.cs        # IEnumerable<T> assertions
  NumericAssertions.cs           # int/long/double/decimal/etc. assertions
  DateOnlyAssertions.cs          # DateOnly assertions
  DictionaryAssertions.cs        # IDictionary assertions
  BooleanAssertions.cs           # bool/bool? assertions
  EnumAssertions.cs              # Enum assertions
  NullableAssertions.cs          # Nullable<T> numeric wrapper
  TimeSpanAssertions.cs          # TimeSpan assertions
  DateTimeOffsetAssertions.cs    # DateTimeOffset assertions
  NullableEnumAssertions.cs      # NullableEnum<T> assertions
  NullableDateTimeAssertions.cs  # DateTime? assertions
  NullableDateOnlyAssertions.cs  # DateOnly? assertions
  NullableGuidAssertions.cs      # Guid? assertions
  StringCollectionAssertions.cs  # IEnumerable<string> assertions
  ActionAssertions.cs            # Action.Throw<T>() / Func<Task>.ThrowAsync<T>()
  AndConstraint.cs               # readonly struct for .And chaining (where TAssertions : struct)
  AndWhichConstraint.cs          # readonly struct for .Which + .And chaining
  Assertion.cs                   # Assertion<T> with And + Which for non-struct types
  Fail.cs                        # InterpolatedStringHandler — lazy failure message, zero alloc if passes
  AssertionScope.cs              # AsyncLocal scope for collecting multiple failures before throwing
  AssertionFailedException.cs    # Exception with Expected/Actual/Expression
  Equivalency/                   # Deep reflection-based equivalency engine
    DeepEqualityComparer.cs      # Core comparison engine: cached expression trees, options-sensitive plan cache
    EquivalencyOptions.cs        # Immutable config (MaxDepth, IgnoringCase, ExcludedProperties/Fields, etc.)
    EquivalencyOptionsBuilder.cs # Fluent builder for EquivalencyOptions
    EquivalencyContext.cs        # Path tracking + failure collection with checkpoint/rollback
    ComparisonPlan.cs            # Pre-built property/field pairs for a type pair
    TypeMetadata.cs              # Reflected PropertyMetadata[] / FieldMetadata[] per type (cached)
    PropertyMetadata.cs          # Compiled getter + direct Equals delegate per property (cached)
    FieldMetadata.cs             # Compiled getter + direct Equals delegate per field (cached)
    CustomComparerRegistry.cs    # Custom comparer registration (Using<T>)

FluentCheck.Tests/               # xUnit test project
  *.Tests.cs                     # One test class per assertion type
  TestDomain.cs                  # Shared test fixtures/subjects
```

# docs
you can use context7 to understand FluentAssertions library

## Commands

```bash
# Build
dotnet build FluentCheck.sln

# Run all tests
dotnet test FluentCheck.sln

# Run single test class
dotnet test FluentCheck.sln --filter "FullyQualifiedName~StringAssertions"

# Run single test method
dotnet test FluentCheck.sln --filter "FullyQualifiedName~BeEmpty"

# Run benchmarks (needs Release for both lib and bench project)
dotnet build FluentCheck.sln -c Release && dotnet run --project FluentCheck.Benchmarks/FluentCheck.Benchmarks.csproj -c Release

# Trim & publish (AOT compatible)
dotnet publish -c Release -r linux-x64 --self-contained
```

## Architecture

### Core Pattern: Should() Extension → Assertion Struct → Methods

Every subject type gets a `Should()` extension (in `ShouldExtensions.cs`) that returns a **readonly struct** assertion type:

```
subject.Should()          → StringAssertions   (readonly struct)
                            .Be("x")            → AndConstraint<StringAssertions> (readonly struct)
                            .And.Contain("y")   → AndConstraint<StringAssertions>
                            .Which              → string (the actual subject value)
```

- **`AndConstraint<T>`** (struct, T:struct) — wraps the assertion instance for `.And` chaining on fluent assertion methods that return no value.
- **`AndWhichConstraint<TSubject, TAssertions>`** (struct) — holds both subject and assertions for `.Which` chaining (e.g., `list.Should().ContainSingle().Which.Should().Be("x")`).
- **`Assertion<T>`** (struct) — generic wrapper with `.And` + `.Which` for non-struct subjects.

### Fail mechanism

`Fail.With` uses `InterpolatedStringHandler` (`AssertionMessageHandler`). String is **only built when the assertion fails** — zero allocation on pass path. If an `AssertionScope` is active, the message is queued via `AddFailure()`; otherwise it throws `AssertionFailedException` immediately.

### AssertionScope

`AsyncLocal<AssertionScope>` enables nested scope stacking across async boundaries. Collects failures, aggregates on dispose.

### Deep Equivalency Engine (Equivalency/)

**Core flow**: `DeepEqualityComparer.AreEquivalent()` handles deep comparison:
- `TryFastPath()` — handles primitives, DateTime, Guid, decimal, enums, strings (zero reflection)
- `TypeMetadata.Create()` → cached in `ConcurrentDictionary<Type, TypeMetadata>` — reflects properties/fields once
- `ComparisonPlan.Create()` → cached per `(Type, Type)` for default options, or `(Type, Type, OptionsKey)` when exclusions present
- `PropertyMetadata` / `FieldMetadata` — compiled expression-tree getters + `EqualityComparer<T>.Equals` delegates (per property/field, cached)
- `EquivalencyContext` — tracks comparison path, collects failures, supports checkpoint/rollback (unordered collections)
- `OptionsKey` — immutable struct for options-sensitive cache keys (zero alloc)
- Cyclic reference detection via `ReferenceEquals` short-circuit
- Enum comparison by name or value (configurable)
- String comparison case-sensitive or insensitive
- Collection comparison (ordered vs unordered via `SetEqual`)
- Custom member mapping, exclusion, inclusion
- Configured via `EquivalencyOptions` (immutable) with `EquivalencyOptionsBuilder` fluent API (e.g., `.ExcludingProperty("Id").IgnoringCase()`)

**Cache layer**: two dictionaries — `_planCache<(Type,Type)>` for default options (fast path), `_filteredPlanCache<(Type,Type,OptionsKey)>` for filtered comparisons. `PropertyMetadata[]` cached per type. Direct member-by-member comparison — no JSON serialization.

### Negation Pattern

All assertion types include inline negation methods (e.g., `NotBe`, `NotContain`, `NotBeNull`). Pattern: check the negated condition, `Fail.With` on failure. No extra allocations.

### New Assertion Types (Phase 5)

New types follow the same readonly struct pattern:
```csharp
public readonly struct MyTypeAssertions {
    private readonly MyType _subject;
    private readonly string _expr;
    internal MyTypeAssertions(MyType subject, string expr) { _subject = subject; _expr = expr; }
    public AndConstraint<MyTypeAssertions> Be(MyType expected, ...) { ... }
}
```

Plus a `Should()` extension in `ShouldExtensions.cs`.

### Key Decisions

- **Deep Equivalency approach**: Reflection → compiled Expression Tree is the best performance/flexibility trade-off. JSON serialization is rejected — adds full graph traversal, string allocations, format/ordering issues, 3-10x slower on deep graphs.
- Reflection → compiled Expression Tree: hot paths (member access, getters/setters) use `Expression<TDelegate>.Compile()` — first call pays compile cost, subsequent calls hit JIT-compiled delegate, ~10-100x faster than `MethodInfo.Invoke`/`FieldInfo.GetValue`.
- **Architecture**: Core API → Type Metadata Cache (`ConcurrentDictionary<Type, TypeMetadata>`) → Property/Field Metadata Cache (per-member expression-tree delegates) → ComparisonPlan cache (per type-pair, options-sensitive) — zero reflection after first-call compilation.
- `AndConstraint<T>` is `struct where TAssertions : struct` — zero alloc
- `IsAotCompatible=true` in csproj — no dynamic reflection in hot paths, use `[DynamicDependency]` on reflection-based comparers
- No nullable reference types in assertions (internal fields use `!` for null suppression)
- All assertion types are readonly structs — no heap allocation for assertions

### Test Patterns

- xUnit with `[Fact]` for positive tests, `[Theory]` with `[InlineData]` for parameterized
- Negation tests verify `AssertionFailedException` is thrown
- Subject creation inline in each test (no shared mutable state)
- `TestDomain.cs` holds reusable POCOs for equivalency testing

### Writing New Methods

1. Add method to appropriate `*Assertions.cs` struct
2. Use `Fail.With($"Expected {_expr} to...")` for failure
3. Return `AndConstraint<Self>` for fluent chaining, `Assertion<T>` for subject extraction
4. If negation, prefix with `Not` and invert the condition
5. Add corresponding test in `FluentCheck.Tests/`

### Performance Goals

- Benchmark against FluentAssertions (same assertions, same input sizes)
- Target: match or exceed FluentAssertions performance — zero alloc on pass path, avoid boxing, minimize reflection
- `Fail.With` uses `InterpolatedStringHandler` — string built only on failure (already implemented)
- Assertion types are readonly structs — zero heap allocation for the assertion chain itself
- `AndConstraint<T>` / `AndWhichConstraint<TSubject, TAssertions>` — zero alloc return types
- Reflection in deep equivalency: profile before optimizing; use `[DynamicDependency]` for AOT
- **Compiled Expression Trees**: for hot reflection paths (property access, field read/write in Equivalency/), build `Expression<TDelegate>` once per member type, cache compiled delegate per property/field. Zero reflection after first call. Avoids `MethodInfo.Invoke` boxing and virtual dispatch overhead per call.
- Add a benchmark project (BenchmarkDotNet) after all methods are implemented — compare `Be()`, `Contain()`, deep equivalency, etc.
- Never sacrifice readability for premature micro-optimization. Profile first, optimize what matters.

### Feature Goals

Methods to implement (based on public testing API conventions): case-insensitive string comparison, substring containment with occurrence counts, more `DateTime` accessors (`HaveMillisecond`, `HaveOffset`), collection uniqueness checks, `Throw<T>()` with inner exception and parameter name inspection, deep equivalency options (exclude/include/mapping/strict typing). All implemented from scratch — reference only public documentation of what these assertions should do, not how another library does it.




