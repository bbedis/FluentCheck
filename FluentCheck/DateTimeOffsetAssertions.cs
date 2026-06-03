namespace FluentCheck;

public readonly struct DateTimeOffsetAssertions
{
    private readonly DateTimeOffset _subject;
    private readonly string _expr;

    internal DateTimeOffsetAssertions(DateTimeOffset subject, string expr) { _subject = subject; _expr = expr; }

    public void Be(DateTimeOffset expected)
    {
        if (_subject != expected)
            Fail.With($"Expected {_expr} to be {expected:o}, but found {_subject:o}.");
    }

    public void BeBefore(DateTimeOffset threshold)
    {
        if (_subject >= threshold)
            Fail.With($"Expected {_expr} to be before {threshold:o}, but found {_subject:o}.");
    }

    public void BeAfter(DateTimeOffset threshold)
    {
        if (_subject <= threshold)
            Fail.With($"Expected {_expr} to be after {threshold:o}, but found {_subject:o}.");
    }

    public void BeOnOrBefore(DateTimeOffset threshold)
    {
        if (_subject > threshold)
            Fail.With($"Expected {_expr} to be on or before {threshold:o}, but found {_subject:o}.");
    }

    public void BeOnOrAfter(DateTimeOffset threshold)
    {
        if (_subject < threshold)
            Fail.With($"Expected {_expr} to be on or after {threshold:o}, but found {_subject:o}.");
    }

    public void BeInRange(DateTimeOffset minimum, DateTimeOffset maximum)
    {
        if (_subject < minimum || _subject > maximum)
            Fail.With($"Expected {_expr} to be in range [{minimum:o}, {maximum:o}], but found {_subject:o}.");
    }

    public void BeCloseTo(DateTimeOffset expected, TimeSpan tolerance)
    {
        var diff = (_subject - expected).Duration();
        if (diff > tolerance)
            Fail.With($"Expected {_expr} to be within {tolerance} of {expected:o}, but difference was {diff}.");
    }

    public void BeInRelativeRange(TimeSpan tolerance)
    {
        var now = DateTimeOffset.Now;
        var diff = (_subject - now).Duration();
        if (diff > tolerance)
            Fail.With($"Expected {_expr} to be within {tolerance} of now, but difference was {diff}.");
    }

    public void BeInThePast()
    {
        if (_subject >= DateTimeOffset.Now)
            Fail.With($"Expected {_expr} to be in the past, but found {_subject:o}.");
    }

    public void BeInFuture()
    {
        if (_subject <= DateTimeOffset.Now)
            Fail.With($"Expected {_expr} to be in the future, but found {_subject:o}.");
    }

    public void BeOneOf(params DateTimeOffset[] values)
    {
        bool found = false;
        foreach (var v in values)
        {
            if (_subject == v) { found = true; break; }
        }
        if (!found)
            Fail.With($"Expected {_expr} to be one of {string.Join(", ", values.Select(v => v.ToString("o")))}, but found {_subject:o}.");
    }

    public void WithOffset(TimeSpan offset)
    {
        if (_subject.Offset != offset)
            Fail.With($"Expected {_expr}.Offset to be {offset}, but found {_subject.Offset}.");
    }

    // --- Negation ---

    public void NotBe(DateTimeOffset unexpected)
    {
        if (_subject == unexpected)
            Fail.With($"Expected {_expr} not to be {unexpected:o}, but it was.");
    }

    public void NotBeBefore(DateTimeOffset threshold)
    {
        if (_subject < threshold)
            Fail.With($"Expected {_expr} not to be before {threshold:o}, but it was.");
    }

    public void NotBeAfter(DateTimeOffset threshold)
    {
        if (_subject > threshold)
            Fail.With($"Expected {_expr} not to be after {threshold:o}, but it was.");
    }

    public void NotBeOnOrBefore(DateTimeOffset threshold)
    {
        if (_subject <= threshold)
            Fail.With($"Expected {_expr} not to be on or before {threshold:o}, but it was.");
    }

    public void NotBeOnOrAfter(DateTimeOffset threshold)
    {
        if (_subject >= threshold)
            Fail.With($"Expected {_expr} not to be on or after {threshold:o}, but it was.");
    }

    public void NotBeInRange(DateTimeOffset minimum, DateTimeOffset maximum)
    {
        if (_subject >= minimum && _subject <= maximum)
            Fail.With($"Expected {_expr} not to be in range [{minimum:o}, {maximum:o}], but found {_subject:o}.");
    }

    public void NotBeCloseTo(DateTimeOffset unexpected, TimeSpan tolerance)
    {
        var diff = (_subject - unexpected).Duration();
        if (diff <= tolerance)
            Fail.With($"Expected {_expr} not to be within {tolerance} of {unexpected:o}, but it was.");
    }

    // --- Accessor methods ---

    public void HaveHour(int hour)
    {
        if (_subject.Hour != hour)
            Fail.With($"Expected {_expr}.Hour to be {hour}, but found {_subject.Hour}.");
    }

    public void HaveMinute(int minute)
    {
        if (_subject.Minute != minute)
            Fail.With($"Expected {_expr}.Minute to be {minute}, but found {_subject.Minute}.");
    }

    public void HaveSecond(int second)
    {
        if (_subject.Second != second)
            Fail.With($"Expected {_expr}.Second to be {second}, but found {_subject.Second}.");
    }

    public void HaveMillisecond(int millisecond)
    {
        if (_subject.Millisecond != millisecond)
            Fail.With($"Expected {_expr}.Millisecond to be {millisecond}, but found {_subject.Millisecond}.");
    }

    public void HaveOffset(TimeSpan offset)
    {
        if (_subject.Offset != offset)
            Fail.With($"Expected {_expr}.Offset to be {offset}, but found {_subject.Offset}.");
    }

    public void BeSameDateAs(DateTimeOffset other)
    {
        if (_subject.Date != other.Date)
            Fail.With($"Expected {_expr} to be on the same date as {other:d}, but found {_subject.Date}.");
    }

    public void NotBeSameDateAs(DateTimeOffset other)
    {
        if (_subject.Date == other.Date)
            Fail.With($"Expected {_expr} not to be on the same date as {other:d}, but it was.");
    }

    public AndConstraint<DateTimeOffsetAssertions> WithYear(int year)
    {
        if (_subject.Year != year)
            Fail.With($"Expected {_expr}.Year to be {year}, but found {_subject.Year}.");
        return new(this);
    }

    public AndConstraint<DateTimeOffsetAssertions> WithMonth(int month)
    {
        if (_subject.Month != month)
            Fail.With($"Expected {_expr}.Month to be {month}, but found {_subject.Month}.");
        return new(this);
    }

    public AndConstraint<DateTimeOffsetAssertions> WithDay(int day)
    {
        if (_subject.Day != day)
            Fail.With($"Expected {_expr}.Day to be {day}, but found {_subject.Day}.");
        return new(this);
    }
}
