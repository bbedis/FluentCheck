using BenchmarkDotNet.Attributes;
using FluentCheck;

namespace FluentCheck.Benchmarks;

public class CollectionAssertionsBenchmarks
{
    private List<int> _smallList = null!, _largeList = null!, _largeListModified = null!;
    private List<Order> _orderList1 = null!, _orderList2 = null!;

    [GlobalSetup]
    public void Setup()
    {
        _smallList = new List<int> { 1, 2, 3, 4, 5 };
        _largeList = Enumerable.Range(1, 1000).ToList();
        _largeListModified = new List<int>(_largeList);
        _largeListModified[^1] = 0;

        _orderList1 = Enumerable.Range(0, 100).Select(i => new Order { Id = Guid.NewGuid(), Total = i * 10m, Items = new[] { $"item{i}" }, PaymentToken = $"tok_{i}" }).ToList();
        _orderList2 = _orderList1.Select(o => new Order { Id = o.Id, Total = o.Total, Items = (string[])o.Items.Clone(), PaymentToken = o.PaymentToken }).ToList();
    }

    [Benchmark(Baseline = true)]
    public void NotBeEmpty() => _smallList.Should().ContainSingle();

    [Benchmark]
    public void ContainPass() => _largeList.Should().Contain(42);

    [Benchmark]
    public void ContainFail() => _largeList.Should().Contain(99999);

    [Benchmark]
    public void HaveCountPass() => _largeList.Should().HaveCount(1000);

    [Benchmark]
    public void HaveCountFail() => _largeList.Should().HaveCount(999);

    [Benchmark]
    public void OnlyContain() => _smallList.Should().OnlyContain(x => x > 0);

    [Benchmark]
    public void OnlyContainFail() => _smallList.Should().OnlyContain(x => x > 10);

    [Benchmark]
    public void ContainInOrder() => _largeList.Should().ContainInOrder(new[] { 10, 50, 100, 500, 999 });

    [Benchmark]
    public void ContainInConsecutiveOrder() => _largeList.Should().ContainInConsecutiveOrder(new[] { 100, 101, 102, 103 });

    [Benchmark]
    public void OnlyHaveUniqueItems() => _largeList.Should().OnlyHaveUniqueItems();

    [Benchmark]
    public void OnlyHaveUniqueItemsKey() => _orderList1.Should().OnlyHaveUniqueItems(o => o.Id);

    [Benchmark]
    public void BeSubsetOf() => _smallList.Should().BeSubsetOf(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });

    [Benchmark]
    public void HaveElementPreceding() => _largeList.Should().HaveElementPreceding(100, 99);

    [Benchmark]
    public void HaveElementSucceeding() => _largeList.Should().HaveElementSucceeding(99, 100);

    [Benchmark]
    public void EqualDefault() => _largeList.Should().Equal(_largeList);

    [Benchmark]
    public void ContainSinglePass() => _smallList.Should().ContainSingle(x => x == 3);

    [Benchmark]
    public void ContainSingleFail() => _largeList.Should().ContainSingle(x => x == 99999);

    [Benchmark]
    public void StartWith() => _largeList.Should().StartWith(1);

    [Benchmark]
    public void EndWith() => _largeList.Should().EndWith(1000);

    [Benchmark]
    public void NotHaveDuplicates() => _largeList.Should().NotHaveDuplicates();

    [Benchmark]
    public void Satisfy() => _largeList.Should().Satisfy(x => x > 500);

    [Benchmark]
    public void NotHaveCount() => _largeList.Should().NotHaveCount(999);

    [Benchmark]
    public void NotContain() => _largeList.Should().NotContain(99999);

    [Benchmark]
    public void NotSatisfy() => _largeList.Should().NotSatisfy(x => x == 99999);

    [Benchmark]
    public void NotOnlyContain() => _smallList.Should().NotOnlyContain(x => x > 10);
}
