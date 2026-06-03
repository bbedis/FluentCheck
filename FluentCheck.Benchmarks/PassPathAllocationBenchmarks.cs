using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using FluentCheck;

namespace FluentCheck.Benchmarks;

/// <summary>
/// Verify zero allocations on pass path for all major assertion types.
/// Uses MemoryDiagnoser to report heap allocations per benchmark.
/// </summary>
[MemoryDiagnoser]
public class PassPathAllocationBenchmarks
{
    private string _subject = "Hello, World!";
    private List<int> _list = new List<int> { 1, 2, 3, 4, 5 };
    private List<int> _singleItem = new List<int> { 42 };
    private int _num = 42;
    private Order _obj1 = new Order { Id = Guid.NewGuid(), Total = 100m, Items = new[] { "a" }, PaymentToken = "tok" };
    private Order _obj2 = new Order { Id = Guid.NewGuid(), Total = 100m, Items = new[] { "a" }, PaymentToken = "tok" };

    // String assertions
    [Benchmark]
    public void StringBe() => _subject.Should().Be("Hello, World!");

    [Benchmark]
    public void StringContain() => _subject.Should().Contain("Hello");

    [Benchmark]
    public void StringStartWith() => _subject.Should().StartWith("Hello");

    [Benchmark]
    public void StringEndWith() => _subject.Should().EndWith("World!");

    [Benchmark]
    public void StringHaveLength() => _subject.Should().HaveLength(13);

    [Benchmark]
    public void StringBeEquivalentOf() => _subject.Should().BeEquivalentOf("hello, world!", StringComparison.OrdinalIgnoreCase);

    // Collection assertions
    [Benchmark]
    public void CollectionContainSingle() => _singleItem.Should().ContainSingle();

    [Benchmark]
    public void CollectionContain() => _list.Should().Contain(3);

    [Benchmark]
    public void CollectionHaveCount() => _list.Should().HaveCount(5);

    [Benchmark]
    public void CollectionOnlyContain() => _list.Should().OnlyContain(x => x > 0);

    [Benchmark]
    public void CollectionOnlyHaveUniqueItems() => _list.Should().OnlyHaveUniqueItems();

    [Benchmark]
    public void CollectionEqual() => _list.Should().Equal(_list);

    // Numeric assertions
    [Benchmark]
    public void IntBeGreaterThan() => _num.Should().BeGreaterThan(40);

    [Benchmark]
    public void IntBeInRange() => _num.Should().BeInRange(40, 45);

    [Benchmark]
    public void IntBePositive() => _num.Should().BePositive();

    [Benchmark]
    public void IntBeZero() => 0.Should().BeZero();

    [Benchmark]
    public void IntNotBe() => _num.Should().NotBe(100);

    // Object assertions
    private Order _sameRef = null!;

    [GlobalSetup(Target = nameof(ObjectBe))]
    public void SetupSameRef() { _sameRef = _obj1; }

    [Benchmark]
    public void ObjectBe() => _sameRef.Should().Be(_sameRef);

    [Benchmark]
    public void ObjectBeEquivalentTo() => _obj1.Should().BeEquivalentTo(_obj2);

    // DateTime assertions
    private DateTime _dt = new(2024, 6, 1, 12, 0, 0);
    [Benchmark]
    public void DateTimeBe() => _dt.Should().Be(new DateTime(2024, 6, 1, 12, 0, 0));

    [Benchmark]
    public void DateTimeBeBefore() => _dt.Should().BeBefore(new DateTime(2024, 6, 2));

    [Benchmark]
    public void DateTimeBeAfter() => _dt.Should().BeAfter(new DateTime(2024, 5, 31));

    // Nullable assertions
    private int? _nullable = 42;
    [Benchmark]
    public void NullableHaveValue() => _nullable.Should().HaveValue();

    // Boolean assertions
    [Benchmark]
    public void BoolBeTrue() => true.Should().BeTrue();

    [Benchmark]
    public void BoolBeFalse() => false.Should().BeFalse();

    // TimeSpan assertions
    private TimeSpan _ts = TimeSpan.FromHours(2);
    [Benchmark]
    public void TimeSpanBe() => _ts.Should().Be(TimeSpan.FromHours(2));

    [Benchmark]
    public void TimeSpanBeGreaterThan() => _ts.Should().BeGreaterThan(TimeSpan.FromHours(1));
}
