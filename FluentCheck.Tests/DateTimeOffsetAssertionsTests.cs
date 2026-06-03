namespace FluentCheck.Tests;

public class DateTimeOffsetAssertionsTests
{
    private static DateTimeOffset Dto(int y, int m, int d, int h, int mi, int s, double offsetHours)
        => new DateTimeOffset(y, m, d, h, mi, s, TimeSpan.FromHours(offsetHours));

    [Fact]
    public void Be_WhenEqual_Succeeds()
    {
        Dto(2024, 1, 15, 10, 30, 0, 3).Should().Be(Dto(2024, 1, 15, 10, 30, 0, 3));
    }

    [Fact]
    public void Be_WhenNotEqual_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            Dto(2024, 1, 15, 10, 30, 0, 3).Should().Be(Dto(2024, 1, 15, 11, 30, 0, 3)));
    }

    [Fact]
    public void BeBefore_WhenBefore_Succeeds()
    {
        Dto(2024, 1, 1, 0, 0, 0, 0).Should().BeBefore(Dto(2024, 1, 2, 0, 0, 0, 0));
    }

    [Fact]
    public void BeBefore_WhenEqual_Fails()
    {
        var same = Dto(2024, 1, 1, 0, 0, 0, 0);
        Assert.Throws<AssertionFailedException>(() => same.Should().BeBefore(same));
    }

    [Fact]
    public void BeAfter_WhenAfter_Succeeds()
    {
        Dto(2024, 1, 2, 0, 0, 0, 0).Should().BeAfter(Dto(2024, 1, 1, 0, 0, 0, 0));
    }

    [Fact]
    public void BeAfter_WhenEqual_Fails()
    {
        var same = Dto(2024, 1, 1, 0, 0, 0, 0);
        Assert.Throws<AssertionFailedException>(() => same.Should().BeAfter(same));
    }

    [Fact]
    public void BeOnOrBefore_WhenBefore_Succeeds()
    {
        Dto(2024, 1, 1, 0, 0, 0, 0).Should().BeOnOrBefore(Dto(2024, 1, 2, 0, 0, 0, 0));
    }

    [Fact]
    public void BeOnOrBefore_WhenEqual_Succeeds()
    {
        var same = Dto(2024, 1, 1, 0, 0, 0, 0);
        same.Should().BeOnOrBefore(same);
    }

    [Fact]
    public void BeOnOrBefore_WhenAfter_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            Dto(2024, 1, 2, 0, 0, 0, 0).Should().BeOnOrBefore(Dto(2024, 1, 1, 0, 0, 0, 0)));
    }

    [Fact]
    public void BeOnOrAfter_WhenAfter_Succeeds()
    {
        Dto(2024, 1, 2, 0, 0, 0, 0).Should().BeOnOrAfter(Dto(2024, 1, 1, 0, 0, 0, 0));
    }

    [Fact]
    public void BeOnOrAfter_WhenEqual_Succeeds()
    {
        var same = Dto(2024, 1, 1, 0, 0, 0, 0);
        same.Should().BeOnOrAfter(same);
    }

    [Fact]
    public void BeOnOrAfter_WhenBefore_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            Dto(2024, 1, 1, 0, 0, 0, 0).Should().BeOnOrAfter(Dto(2024, 1, 2, 0, 0, 0, 0)));
    }

    [Fact]
    public void BeInRange_WhenInRange_Succeeds()
    {
        Dto(2024, 1, 10, 0, 0, 0, 0).Should().BeInRange(
            Dto(2024, 1, 1, 0, 0, 0, 0), Dto(2024, 1, 15, 0, 0, 0, 0));
    }

    [Fact]
    public void BeInRange_WhenAtBoundary_Succeeds()
    {
        Dto(2024, 1, 1, 0, 0, 0, 0).Should().BeInRange(
            Dto(2024, 1, 1, 0, 0, 0, 0), Dto(2024, 1, 15, 0, 0, 0, 0));
    }

    [Fact]
    public void BeInRange_WhenBeforeRange_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            Dto(2023, 12, 31, 0, 0, 0, 0).Should().BeInRange(
                Dto(2024, 1, 1, 0, 0, 0, 0), Dto(2024, 1, 15, 0, 0, 0, 0)));
    }

    [Fact]
    public void BeInRange_WhenAfterRange_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            Dto(2024, 1, 16, 0, 0, 0, 0).Should().BeInRange(
                Dto(2024, 1, 1, 0, 0, 0, 0), Dto(2024, 1, 15, 0, 0, 0, 0)));
    }

    [Fact]
    public void BeCloseTo_WhenWithinTolerance_Succeeds()
    {
        var now = DateTimeOffset.UtcNow;
        now.Should().BeCloseTo(now.AddMinutes(2), TimeSpan.FromMinutes(5));
    }

    [Fact]
    public void BeCloseTo_WhenOutsideTolerance_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            DateTimeOffset.UtcNow.Should().BeCloseTo(DateTimeOffset.UtcNow.AddDays(1), TimeSpan.FromHours(1)));
    }

    [Fact]
    public void BeInThePast_WhenInPast_Succeeds()
    {
        Dto(2020, 1, 1, 0, 0, 0, 0).Should().BeInThePast();
    }

    [Fact]
    public void BeInThePast_WhenInFuture_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            Dto(2099, 1, 1, 0, 0, 0, 0).Should().BeInThePast());
    }

    [Fact]
    public void BeInFuture_WhenInFuture_Succeeds()
    {
        Dto(2099, 1, 1, 0, 0, 0, 0).Should().BeInFuture();
    }

    [Fact]
    public void BeInFuture_WhenInPast_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            Dto(2020, 1, 1, 0, 0, 0, 0).Should().BeInFuture());
    }

    [Fact]
    public void BeOneOf_WhenMatchFound_Succeeds()
    {
        var dto = Dto(2024, 6, 15, 12, 0, 0, 0);
        dto.Should().BeOneOf(Dto(2024, 1, 1, 0, 0, 0, 0), Dto(2024, 6, 15, 12, 0, 0, 0), Dto(2024, 12, 25, 0, 0, 0, 0));
    }

    [Fact]
    public void BeOneOf_WhenNoMatch_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            Dto(2024, 6, 15, 12, 0, 0, 0).Should().BeOneOf(Dto(2024, 1, 1, 0, 0, 0, 0), Dto(2024, 12, 25, 0, 0, 0, 0)));
    }

    [Fact]
    public void WithOffset_WhenOffsetMatches_Succeeds()
    {
        Dto(2024, 1, 1, 12, 0, 0, 5).Should().WithOffset(TimeSpan.FromHours(5));
    }

    [Fact]
    public void WithOffset_WhenOffsetMismatch_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            Dto(2024, 1, 1, 12, 0, 0, 5).Should().WithOffset(TimeSpan.FromHours(3)));
    }

    // --- Negation ---

    [Fact]
    public void NotBe_WhenNotEqual_Succeeds()
    {
        Dto(2024, 1, 1, 0, 0, 0, 0).Should().NotBe(Dto(2024, 1, 2, 0, 0, 0, 0));
    }

    [Fact]
    public void NotBe_WhenEqual_Fails()
    {
        var same = Dto(2024, 1, 1, 0, 0, 0, 0);
        Assert.Throws<AssertionFailedException>(() => same.Should().NotBe(same));
    }

    [Fact]
    public void NotBeBefore_WhenNotBefore_Succeeds()
    {
        Dto(2024, 1, 2, 0, 0, 0, 0).Should().NotBeBefore(Dto(2024, 1, 1, 0, 0, 0, 0));
    }

    [Fact]
    public void NotBeBefore_WhenBefore_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            Dto(2024, 1, 1, 0, 0, 0, 0).Should().NotBeBefore(Dto(2024, 1, 2, 0, 0, 0, 0)));
    }

    [Fact]
    public void NotBeAfter_WhenNotAfter_Succeeds()
    {
        Dto(2024, 1, 1, 0, 0, 0, 0).Should().NotBeAfter(Dto(2024, 1, 2, 0, 0, 0, 0));
    }

    [Fact]
    public void NotBeAfter_WhenAfter_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            Dto(2024, 1, 2, 0, 0, 0, 0).Should().NotBeAfter(Dto(2024, 1, 1, 0, 0, 0, 0)));
    }

    [Fact]
    public void NotBeOnOrBefore_WhenAfter_Succeeds()
    {
        Dto(2024, 1, 2, 0, 0, 0, 0).Should().NotBeOnOrBefore(Dto(2024, 1, 1, 0, 0, 0, 0));
    }

    [Fact]
    public void NotBeOnOrBefore_WhenEqual_Fails()
    {
        var same = Dto(2024, 1, 1, 0, 0, 0, 0);
        Assert.Throws<AssertionFailedException>(() => same.Should().NotBeOnOrBefore(same));
    }

    [Fact]
    public void NotBeOnOrAfter_WhenBefore_Succeeds()
    {
        Dto(2024, 1, 1, 0, 0, 0, 0).Should().NotBeOnOrAfter(Dto(2024, 1, 2, 0, 0, 0, 0));
    }

    [Fact]
    public void NotBeOnOrAfter_WhenEqual_Fails()
    {
        var same = Dto(2024, 1, 1, 0, 0, 0, 0);
        Assert.Throws<AssertionFailedException>(() => same.Should().NotBeOnOrAfter(same));
    }

    [Fact]
    public void NotBeInRange_WhenOutsideRange_Succeeds()
    {
        Dto(2023, 12, 31, 0, 0, 0, 0).Should().NotBeInRange(
            Dto(2024, 1, 1, 0, 0, 0, 0), Dto(2024, 1, 15, 0, 0, 0, 0));
    }

    [Fact]
    public void NotBeInRange_WhenInRange_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            Dto(2024, 1, 10, 0, 0, 0, 0).Should().NotBeInRange(
                Dto(2024, 1, 1, 0, 0, 0, 0), Dto(2024, 1, 15, 0, 0, 0, 0)));
    }

    [Fact]
    public void NotBeCloseTo_WhenNotClose_Succeeds()
    {
        DateTimeOffset.UtcNow.AddYears(1).Should().NotBeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromDays(1));
    }

    [Fact]
    public void NotBeCloseTo_WhenClose_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            DateTimeOffset.UtcNow.Should().NotBeCloseTo(DateTimeOffset.UtcNow.AddMinutes(2), TimeSpan.FromHours(1)));
    }

    [Fact]
    public void AssertionScope_CollectsAllFailures()
    {
        var scope = new AssertionScope();
        Dto(2024, 1, 1, 0, 0, 0, 0).Should().Be(Dto(2024, 1, 2, 0, 0, 0, 0));
        Dto(2024, 1, 1, 0, 0, 0, 0).Should().BeAfter(Dto(2024, 1, 2, 0, 0, 0, 0));
        var ex = Assert.Throws<AssertionFailedException>(() => scope.Dispose());
        Assert.Contains("Multiple assertion failures (2)", ex.Message);
    }
}
