namespace FluentCheck.Tests;

public class DateOnlyAssertionsTests
{
    [Fact] public void Be_equal() =>
        DateOnly.FromDateTime(DateTime.Parse("2024-01-15")).Should().Be(DateOnly.FromDateTime(DateTime.Parse("2024-01-15")));
    [Fact] public void Be_notEqual() => Assert.Throws<AssertionFailedException>(() =>
        DateOnly.FromDateTime(DateTime.Parse("2024-01-15")).Should().Be(DateOnly.FromDateTime(DateTime.Parse("2024-01-16"))));

    [Fact] public void NotBe_equal() => Assert.Throws<AssertionFailedException>(() =>
        DateOnly.FromDateTime(DateTime.Parse("2024-01-15")).Should().NotBe(DateOnly.FromDateTime(DateTime.Parse("2024-01-15"))));
    [Fact] public void NotBe_notEqual() => DateOnly.FromDateTime(DateTime.Parse("2024-01-15")).Should().NotBe(DateOnly.FromDateTime(DateTime.Parse("2024-01-16")));

    [Fact] public void BeBefore() => DateOnly.FromDateTime(DateTime.Parse("2024-01-01")).Should().BeBefore(DateOnly.FromDateTime(DateTime.Parse("2024-01-15")));
    [Fact] public void BeBefore_equal() => Assert.Throws<AssertionFailedException>(() =>
        DateOnly.FromDateTime(DateTime.Parse("2024-01-15")).Should().BeBefore(DateOnly.FromDateTime(DateTime.Parse("2024-01-15"))));
    [Fact] public void NotBeBefore_past() => Assert.Throws<AssertionFailedException>(() =>
        DateOnly.FromDateTime(DateTime.Parse("2024-01-01")).Should().NotBeBefore(DateOnly.FromDateTime(DateTime.Parse("2024-01-15"))));

    [Fact] public void BeOnOrAfter_equal() => DateOnly.FromDateTime(DateTime.Parse("2024-01-15")).Should().BeOnOrAfter(DateOnly.FromDateTime(DateTime.Parse("2024-01-15")));
    [Fact] public void BeOnOrBefore_equal() => DateOnly.FromDateTime(DateTime.Parse("2024-01-15")).Should().BeOnOrBefore(DateOnly.FromDateTime(DateTime.Parse("2024-01-15")));

    [Fact] public void BeOneOf_match() =>
        DateOnly.FromDateTime(DateTime.Parse("2024-01-15")).Should().BeOneOf(
            DateOnly.FromDateTime(DateTime.Parse("2024-01-14")), DateOnly.FromDateTime(DateTime.Parse("2024-01-15")));
    [Fact] public void BeOneOf_noMatch() => Assert.Throws<AssertionFailedException>(() =>
        DateOnly.FromDateTime(DateTime.Parse("2024-01-15")).Should().BeOneOf(
            DateOnly.FromDateTime(DateTime.Parse("2024-01-10")), DateOnly.FromDateTime(DateTime.Parse("2024-01-20"))));

    [Fact] public void HaveDay() => DateOnly.FromDateTime(DateTime.Parse("2024-06-15")).Should().HaveDay(15);
    [Fact] public void HaveDay_wrong() => Assert.Throws<AssertionFailedException>(() =>
        DateOnly.FromDateTime(DateTime.Parse("2024-06-15")).Should().HaveDay(14));
    [Fact] public void HaveMonth() => DateOnly.FromDateTime(DateTime.Parse("2024-06-15")).Should().HaveMonth(6);
    [Fact] public void HaveMonth_wrong() => Assert.Throws<AssertionFailedException>(() =>
        DateOnly.FromDateTime(DateTime.Parse("2024-06-15")).Should().HaveMonth(5));
    [Fact] public void HaveYear() => DateOnly.FromDateTime(DateTime.Parse("2024-06-15")).Should().HaveYear(2024);
    [Fact] public void HaveYear_wrong() => Assert.Throws<AssertionFailedException>(() =>
        DateOnly.FromDateTime(DateTime.Parse("2024-06-15")).Should().HaveYear(2025));
}
