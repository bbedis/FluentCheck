namespace FluentCheck.Tests;

public class DateTimeAssertionsTests
{
    [Fact] public void Be_equal() =>
        new DateTime(2024, 1, 15).Should().Be(new DateTime(2024, 1, 15));

    [Fact] public void Be_notEqual() => Assert.Throws<AssertionFailedException>(() =>
        new DateTime(2024, 1, 15).Should().Be(new DateTime(2024, 1, 16)));

    [Fact] public void NotBe_equal() => Assert.Throws<AssertionFailedException>(() =>
        new DateTime(2024, 1, 15).Should().NotBe(new DateTime(2024, 1, 15)));

    [Fact] public void BeBefore() => new DateTime(2024, 1, 1).Should().BeBefore(new DateTime(2024, 1, 15));
    [Fact] public void BeBefore_equal() => Assert.Throws<AssertionFailedException>(() =>
        new DateTime(2024, 1, 15).Should().BeBefore(new DateTime(2024, 1, 15)));

    [Fact] public void NotBeBefore_past() => Assert.Throws<AssertionFailedException>(() =>
        new DateTime(2024, 1, 1).Should().NotBeBefore(new DateTime(2024, 1, 15)));

    [Fact] public void BeAfter() => new DateTime(2024, 1, 15).Should().BeAfter(new DateTime(2024, 1, 1));
    [Fact] public void BeAfter_equal() => Assert.Throws<AssertionFailedException>(() =>
        new DateTime(2024, 1, 15).Should().BeAfter(new DateTime(2024, 1, 15)));

    [Fact] public void NotBeAfter_past() => Assert.Throws<AssertionFailedException>(() =>
        new DateTime(2024, 1, 15).Should().NotBeAfter(new DateTime(2024, 1, 1)));

    [Fact] public void BeOnOrBefore_equal() => new DateTime(2024, 1, 15).Should().BeOnOrBefore(new DateTime(2024, 1, 15));
    [Fact] public void BeOnOrBefore_before() => new DateTime(2024, 1, 14).Should().BeOnOrBefore(new DateTime(2024, 1, 15));
    [Fact] public void BeOnOrBefore_after() => Assert.Throws<AssertionFailedException>(() =>
        new DateTime(2024, 1, 16).Should().BeOnOrBefore(new DateTime(2024, 1, 15)));

    [Fact] public void NotBeOnOrBefore() => Assert.Throws<AssertionFailedException>(() =>
        new DateTime(2024, 1, 16).Should().NotBeOnOrBefore(new DateTime(2024, 1, 15)));

    [Fact] public void BeOnOrAfter_equal() => new DateTime(2024, 1, 15).Should().BeOnOrAfter(new DateTime(2024, 1, 15));
    [Fact] public void BeOnOrAfter_after() => new DateTime(2024, 1, 16).Should().BeOnOrAfter(new DateTime(2024, 1, 15));
    [Fact] public void BeOnOrAfter_before() => Assert.Throws<AssertionFailedException>(() =>
        new DateTime(2024, 1, 14).Should().BeOnOrAfter(new DateTime(2024, 1, 15)));

    [Fact] public void NotBeOnOrAfter_past() => new DateTime(2024, 1, 14).Should().NotBeOnOrAfter(new DateTime(2024, 1, 15));
    [Fact] public void NotBeOnOrAfter_fail() => Assert.Throws<AssertionFailedException>(() =>
        new DateTime(2024, 1, 15).Should().NotBeOnOrAfter(new DateTime(2024, 1, 15)));

    [Fact] public void BeInRange_inside() => new DateTime(2024, 1, 10).Should().BeInRange(
        new DateTime(2024, 1, 1), new DateTime(2024, 1, 31));
    [Fact] public void BeInRange_atBoundaries() => new DateTime(2024, 1, 1).Should().BeInRange(
        new DateTime(2024, 1, 1), new DateTime(2024, 1, 31));
    [Fact] public void BeInRange_outside() => Assert.Throws<AssertionFailedException>(() =>
        new DateTime(2024, 2, 1).Should().BeInRange(
            new DateTime(2024, 1, 1), new DateTime(2024, 1, 31)));

    [Fact] public void BeCloseTo_withinTolerance() =>
        new DateTime(2024, 1, 15, 12, 0, 0).Should().BeCloseTo(
            new DateTime(2024, 1, 15, 12, 0, 30), TimeSpan.FromSeconds(60));

    [Fact] public void NotBeCloseTo() => Assert.Throws<AssertionFailedException>(() =>
        new DateTime(2024, 1, 15).Should().NotBeCloseTo(
            new DateTime(2024, 1, 15), TimeSpan.FromSeconds(60)));

    [Fact] public void BeInRelativeRange() => DateTime.UtcNow.Should().BeInRelativeRange(TimeSpan.FromMinutes(1));

    [Fact] public void BeInThePast() => DateTime.UtcNow.AddHours(-1).Should().BeInThePast();
    [Fact] public void BeInThePast_future() => Assert.Throws<AssertionFailedException>(() =>
        DateTime.UtcNow.AddHours(1).Should().BeInThePast());

    [Fact] public void BeInFuture() => DateTime.UtcNow.AddHours(1).Should().BeInFuture();
    [Fact] public void BeInFuture_past() => Assert.Throws<AssertionFailedException>(() =>
        DateTime.UtcNow.AddHours(-1).Should().BeInFuture());

    [Fact] public void BeMidnight_correct() => new DateTime(2024, 1, 1).Should().BeMidnight();
    [Fact] public void BeMidnight_notMidnight() => Assert.Throws<AssertionFailedException>(() =>
        new DateTime(2024, 1, 1, 1, 0, 0).Should().BeMidnight());

    [Fact] public void BeOneOf_match() => new DateTime(2024, 1, 15).Should().BeOneOf(
        new DateTime(2024, 1, 14), new DateTime(2024, 1, 15), new DateTime(2024, 1, 16));
    [Fact] public void BeOneOf_noMatch() => Assert.Throws<AssertionFailedException>(() =>
        new DateTime(2024, 1, 15).Should().BeOneOf(
            new DateTime(2024, 1, 10), new DateTime(2024, 1, 20)));

    [Fact] public void HaveHour() => new DateTime(2024, 1, 1, 14, 30, 0).Should().HaveHour(14);
    [Fact] public void HaveMinute() => new DateTime(2024, 1, 1, 14, 30, 0).Should().HaveMinute(30);
    [Fact] public void HaveSecond() => new DateTime(2024, 1, 1, 14, 30, 45).Should().HaveSecond(45);
    [Fact] public void HaveMillisecond() => new DateTime(2024, 1, 1, 14, 30, 45, 500).Should().HaveMillisecond(500);

    [Fact] public void HaveHour_wrong() => Assert.Throws<AssertionFailedException>(() =>
        new DateTime(2024, 1, 1, 14, 30, 0).Should().HaveHour(10));

    [Fact] public void BeSameDateAs_same() =>
        new DateTime(2024, 1, 15, 10, 0, 0).Should().BeSameDateAs(new DateTime(2024, 1, 15, 23, 59, 59));
    [Fact] public void BeSameDateAs_different() => new DateTime(2024, 1, 15).Should().NotBeSameDateAs(new DateTime(2024, 1, 16));
    [Fact] public void NotBeSameDateAs_fail() => Assert.Throws<AssertionFailedException>(() =>
        new DateTime(2024, 1, 15).Should().NotBeSameDateAs(new DateTime(2024, 1, 15)));

    [Fact] public void BeIn_local() => DateTime.Now.Should().BeIn(DateTimeKind.Local);

    [Fact] public void WithTime_correct() =>
        new DateTime(2024, 1, 1, 14, 30, 0).Should().WithTime(14, 30, 0);
    [Fact] public void WithTime_wrong() => Assert.Throws<AssertionFailedException>(() =>
        new DateTime(2024, 1, 1, 14, 30, 0).Should().WithTime(10, 30, 0));

    [Fact] public void WithYear_correct() => new DateTime(2024, 6, 15).Should().WithYear(2024);
    [Fact] public void WithYear_wrong() => Assert.Throws<AssertionFailedException>(() =>
        new DateTime(2024, 6, 15).Should().WithYear(2025));

    [Fact] public void WithMonth_correct() => new DateTime(2024, 6, 15).Should().WithMonth(6);
    [Fact] public void WithMonth_wrong() => Assert.Throws<AssertionFailedException>(() =>
        new DateTime(2024, 6, 15).Should().WithMonth(5));

    [Fact] public void WithDay_correct() => new DateTime(2024, 6, 15).Should().WithDay(15);
    [Fact] public void WithDay_wrong() => Assert.Throws<AssertionFailedException>(() =>
        new DateTime(2024, 6, 15).Should().WithDay(14));
}
