namespace FluentCheck.Tests;

public class NumericAssertionsTests
{
    [Fact] public void Be_equal() => 5.Should().Be(5);
    [Fact] public void Be_notEqual() => Assert.Throws<AssertionFailedException>(() => 5.Should().Be(6));

    [Fact] public void NotBe_notEqual() => 5.Should().NotBe(6);
    [Fact] public void NotBe_equal() => Assert.Throws<AssertionFailedException>(() => 5.Should().NotBe(5));

    [Fact] public void BeGreaterThan() => 5.Should().BeGreaterThan(3);
    [Fact] public void BeGreaterThan_equal() => Assert.Throws<AssertionFailedException>(() => 5.Should().BeGreaterThan(5));

    [Fact] public void BeLessThan() => 3.Should().BeLessThan(5);
    [Fact] public void BeLessThan_equal() => Assert.Throws<AssertionFailedException>(() => 5.Should().BeLessThan(5));

    [Fact] public void BeAtMost_equal() => 5.Should().BeAtMost(5);
    [Fact] public void BeAtMost_less() => 4.Should().BeAtMost(5);
    [Fact] public void BeAtMost_greater() => Assert.Throws<AssertionFailedException>(() => 6.Should().BeAtMost(5));

    [Fact] public void BeAtLeast_equal() => 5.Should().BeAtLeast(5);
    [Fact] public void BeAtLeast_greater() => 6.Should().BeAtLeast(5);
    [Fact] public void BeAtLeast_less() => Assert.Throws<AssertionFailedException>(() => 4.Should().BeAtLeast(5));

    [Fact] public void BeInRange_inside() => 5.Should().BeInRange(1, 10);
    [Fact] public void BeInRange_atMin() => 1.Should().BeInRange(1, 10);
    [Fact] public void BeInRange_atMax() => 10.Should().BeInRange(1, 10);
    [Fact] public void BeInRange_below() => Assert.Throws<AssertionFailedException>(() => 0.Should().BeInRange(1, 10));
    [Fact] public void BeInRange_above() => Assert.Throws<AssertionFailedException>(() => 11.Should().BeInRange(1, 10));

    [Fact] public void BePositive() => 1.Should().BePositive();
    [Fact] public void BePositive_zero() => Assert.Throws<AssertionFailedException>(() => 0.Should().BePositive());

    [Fact] public void BeNegative() => ((int)-1).Should().BeNegative();
    [Fact] public void BeNegative_zero() => Assert.Throws<AssertionFailedException>(() => 0.Should().BeNegative());

    [Fact] public void BeZero() => 0.Should().BeZero();
    [Fact] public void BeZero_long() => 0L.Should().BeZero();
    [Fact] public void NotBeZero() => 1.Should().NotBeZero();
    [Fact] public void NotBeZero_zero() => Assert.Throws<AssertionFailedException>(() => 0.Should().NotBeZero());

    [Fact] public void NotBeGreaterThan() => 3.Should().NotBeGreaterThan(5);
    [Fact] public void NotBeGreaterThan_equal() => 5.Should().NotBeGreaterThan(5);
    [Fact] public void NotBeGreaterThan_greater() => Assert.Throws<AssertionFailedException>(() => 6.Should().NotBeGreaterThan(5));

    [Fact] public void NotBePositive_negative() => ((int)-1).Should().NotBePositive();
    [Fact] public void NotBePositive_zero() => 0.Should().NotBePositive();
    [Fact] public void NotBePositive_positive() => Assert.Throws<AssertionFailedException>(() => 1.Should().NotBePositive());

    [Fact] public void NotBeNegative_positive() => 1.Should().NotBeNegative();
    [Fact] public void NotBeNegative_zero() => 0.Should().NotBeNegative();
    [Fact] public void NotBeNegative_negative() => Assert.Throws<AssertionFailedException>(() => ((int)-1).Should().NotBeNegative());

    [Fact] public void BeApproximately_close() => 5.0.Should().BeApproximately(4.9, 0.1);
    [Fact] public void BeApproximately_double() => 5.0d.Should().BeApproximately(4.9d, 0.1d);
    [Fact] public void BeApproximately_fails()
    {
        Assert.Throws<AssertionFailedException>(() => 5.0.Should().BeApproximately(4.0, 0.1));
    }

    [Fact] public void Be_int() => 42.Should().Be(42);
    [Fact] public void Be_decimal() => 3.14m.Should().Be(3.14m);
}
