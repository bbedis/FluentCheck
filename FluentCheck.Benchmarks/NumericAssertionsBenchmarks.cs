using BenchmarkDotNet.Attributes;
using FluentCheck;

namespace FluentCheck.Benchmarks;

public class NumericAssertionsBenchmarks
{
    private int _intSubject = 42;
    private double _doubleSubject = 3.14159;
    private decimal _decimalSubject = 99.99m;
    private long _longSubject = 123456789L;

    [Benchmark(Baseline = true)]
    public void IntBeGreaterThanPass() => _intSubject.Should().BeGreaterThan(40);

    [Benchmark]
    public void IntBeGreaterThanFail() => _intSubject.Should().BeGreaterThan(50);

    [Benchmark]
    public void IntBeLessThanPass() => _intSubject.Should().BeLessThan(50);

    [Benchmark]
    public void IntBeLessThanFail() => _intSubject.Should().BeLessThan(30);

    [Benchmark]
    public void IntBeAtMostPass() => _intSubject.Should().BeAtMost(42);

    [Benchmark]
    public void IntBeAtMostFail() => _intSubject.Should().BeAtMost(41);

    [Benchmark]
    public void IntBeAtLeastPass() => _intSubject.Should().BeAtLeast(42);

    [Benchmark]
    public void IntBeAtLeastFail() => _intSubject.Should().BeAtLeast(43);

    [Benchmark]
    public void IntBeInRangePass() => _intSubject.Should().BeInRange(40, 45);

    [Benchmark]
    public void IntBeInRangeFail() => _intSubject.Should().BeInRange(50, 60);

    [Benchmark]
    public void IntBeZeroFail() => _intSubject.Should().BeZero();

    [Benchmark]
    public void IntBePositivePass() => _intSubject.Should().BePositive();

    [Benchmark]
    public void IntBePositiveFail() => (-42).Should().BePositive();

    [Benchmark]
    public void IntBeNegativeFail() => _intSubject.Should().BeNegative();

    [Benchmark]
    public void IntNotBePass() => _intSubject.Should().NotBe(100);

    [Benchmark]
    public void IntNotBeGreaterThanPass() => _intSubject.Should().NotBeGreaterThan(50);

    [Benchmark]
    public void IntNotBeGreaterThanFail() => _intSubject.Should().NotBeGreaterThan(41);

    [Benchmark]
    public void DoubleBeApproximately() => _doubleSubject.Should().BeApproximately(3.14, 0.01);

    [Benchmark]
    public void DoubleBeApproximatelyFail() => _doubleSubject.Should().BeApproximately(2.72, 0.01);

    [Benchmark]
    public void DecimalBeGreaterThan() => _decimalSubject.Should().BeGreaterThan(99m);

    [Benchmark]
    public void LongBeGreaterThan() => _longSubject.Should().BeGreaterThan(100000000L);

    [Benchmark]
    public void DoubleBeZero() => 0.0.Should().BeZero();

    [Benchmark]
    public void DoubleBePositive() => 1.0.Should().BePositive();

    [Benchmark]
    public void DoubleBeNegative() => (-1.0).Should().BeNegative();
}
