namespace FluentCheck.Tests;

public class TimeSpanAssertionsTests
{
    [Fact]
    public void Be_WhenEqual_Succeeds()
    {
        TimeSpan.FromHours(2).Should().Be(TimeSpan.FromHours(2));
    }

    [Fact]
    public void Be_WhenNotEqual_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            TimeSpan.FromHours(2).Should().Be(TimeSpan.FromHours(3)));
    }

    [Fact]
    public void BeGreaterThan_WhenGreater_Succeeds()
    {
        TimeSpan.FromHours(3).Should().BeGreaterThan(TimeSpan.FromHours(2));
    }

    [Fact]
    public void BeGreaterThan_WhenEqual_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            TimeSpan.FromHours(2).Should().BeGreaterThan(TimeSpan.FromHours(2)));
    }

    [Fact]
    public void BeLessThan_WhenLess_Succeeds()
    {
        TimeSpan.FromHours(1).Should().BeLessThan(TimeSpan.FromHours(2));
    }

    [Fact]
    public void BeLessThan_WhenEqual_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            TimeSpan.FromHours(2).Should().BeLessThan(TimeSpan.FromHours(2)));
    }

    [Fact]
    public void BeApproximately_WhenWithinTolerance_Succeeds()
    {
        TimeSpan.FromHours(2).Should().BeApproximately(
            TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(1)), TimeSpan.FromMinutes(5));
    }

    [Fact]
    public void BeApproximately_WhenOutsideTolerance_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            TimeSpan.FromHours(2).Should().BeApproximately(TimeSpan.FromHours(3), TimeSpan.FromMinutes(5)));
    }

    [Fact]
    public void BeZero_WhenZero_Succeeds()
    {
        TimeSpan.Zero.Should().BeZero();
    }

    [Fact]
    public void BeZero_WhenNotZero_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            TimeSpan.FromMinutes(1).Should().BeZero());
    }

    [Fact]
    public void BePositive_WhenPositive_Succeeds()
    {
        TimeSpan.FromMinutes(1).Should().BePositive();
    }

    [Fact]
    public void BePositive_WhenZero_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            TimeSpan.Zero.Should().BePositive());
    }

    [Fact]
    public void BeNegative_WhenNegative_Succeeds()
    {
        TimeSpan.FromHours(-1).Should().BeNegative();
    }

    [Fact]
    public void BeNegative_WhenZero_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            TimeSpan.Zero.Should().BeNegative());
    }

    [Fact]
    public void BeOneOf_WhenMatchFound_Succeeds()
    {
        TimeSpan.FromHours(2).Should().BeOneOf(
            TimeSpan.FromHours(1), TimeSpan.FromHours(2), TimeSpan.FromHours(3));
    }

    [Fact]
    public void BeOneOf_WhenNoMatch_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            TimeSpan.FromHours(2).Should().BeOneOf(TimeSpan.FromHours(1), TimeSpan.FromHours(3)));
    }

    // --- Negation ---

    [Fact]
    public void NotBe_WhenNotEqual_Succeeds()
    {
        TimeSpan.FromHours(2).Should().NotBe(TimeSpan.FromHours(3));
    }

    [Fact]
    public void NotBe_WhenEqual_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            TimeSpan.FromHours(2).Should().NotBe(TimeSpan.FromHours(2)));
    }

    [Fact]
    public void NotBeGreaterThan_WhenNotGreater_Succeeds()
    {
        TimeSpan.FromHours(1).Should().NotBeGreaterThan(TimeSpan.FromHours(2));
    }

    [Fact]
    public void NotBeGreaterThan_WhenGreater_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            TimeSpan.FromHours(3).Should().NotBeGreaterThan(TimeSpan.FromHours(2)));
    }

    [Fact]
    public void NotBeLessThan_WhenNotLess_Succeeds()
    {
        TimeSpan.FromHours(2).Should().NotBeLessThan(TimeSpan.FromHours(1));
    }

    [Fact]
    public void NotBeLessThan_WhenLess_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            TimeSpan.FromMinutes(30).Should().NotBeLessThan(TimeSpan.FromHours(1)));
    }

    [Fact]
    public void NotBeZero_WhenNotZero_Succeeds()
    {
        TimeSpan.FromMinutes(1).Should().NotBeZero();
    }

    [Fact]
    public void NotBeZero_WhenZero_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            TimeSpan.Zero.Should().NotBeZero());
    }

    [Fact]
    public void NotBePositive_WhenNotPositive_Succeeds()
    {
        TimeSpan.Zero.Should().NotBePositive();
    }

    [Fact]
    public void NotBePositive_WhenPositive_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            TimeSpan.FromMinutes(1).Should().NotBePositive());
    }

    [Fact]
    public void NotBeNegative_WhenNotNegative_Succeeds()
    {
        TimeSpan.Zero.Should().NotBeNegative();
    }

    [Fact]
    public void NotBeNegative_WhenNegative_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            TimeSpan.FromHours(-1).Should().NotBeNegative());
    }

    [Fact]
    public void AssertionScope_CollectsAllFailures()
    {
        var scope = new AssertionScope();
        TimeSpan.FromHours(1).Should().Be(TimeSpan.FromHours(2));
        TimeSpan.FromHours(1).Should().BeGreaterThan(TimeSpan.FromHours(3));
        var ex = Assert.Throws<AssertionFailedException>(() => scope.Dispose());
        Assert.Contains("Multiple assertion failures (2)", ex.Message);
    }
}
