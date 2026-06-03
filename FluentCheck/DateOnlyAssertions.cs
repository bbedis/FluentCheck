namespace FluentCheck;

public readonly struct DateOnlyAssertions
{
    private readonly DateOnly _subject;
    private readonly string _expr;
    internal DateOnlyAssertions(DateOnly subject, string expr) { _subject = subject; _expr = expr; }

    public void Be(DateOnly expected)
    {
        if (_subject != expected)
            Fail.With($"Expected {_expr} to be {expected}, but found {_subject}.");
    }

    public void BeBefore(DateOnly threshold)
    {
        if (_subject >= threshold)
            Fail.With($"Expected {_expr} to be before {threshold}, but found {_subject}.");
    }

    // --- Negation + new methods ---

    public void NotBe(DateOnly unexpected)
    {
        if (_subject == unexpected)
            Fail.With($"Expected {_expr} not to be {unexpected}, but it was.");
    }

    public void BeOnOrAfter(DateOnly threshold)
    {
        if (_subject < threshold)
            Fail.With($"Expected {_expr} to be on or after {threshold}, but found {_subject}.");
    }

    public void BeOnOrBefore(DateOnly threshold)
    {
        if (_subject > threshold)
            Fail.With($"Expected {_expr} to be on or before {threshold}, but found {_subject}.");
    }

    public void BeOneOf(params DateOnly[] values)
    {
        bool found = false;
        foreach (var v in values)
        {
            if (_subject == v) { found = true; break; }
        }
        if (!found)
            Fail.With($"Expected {_expr} to be one of {string.Join(", ", values)}, but found {_subject}.");
    }

    public void HaveDay(int day)
    {
        if (_subject.Day != day)
            Fail.With($"Expected {_expr}.Day to be {day}, but found {_subject.Day}.");
    }

    public void HaveMonth(int month)
    {
        if (_subject.Month != month)
            Fail.With($"Expected {_expr}.Month to be {month}, but found {_subject.Month}.");
    }

    public void HaveYear(int year)
    {
        if (_subject.Year != year)
            Fail.With($"Expected {_expr}.Year to be {year}, but found {_subject.Year}.");
    }

    public void NotBeBefore(DateOnly threshold)
    {
        if (_subject < threshold)
            Fail.With($"Expected {_expr} not to be before {threshold}, but it was.");
    }
}

public readonly struct DateTimeAssertions
{
    private readonly DateTime _subject;
    private readonly string _expr;
    internal DateTimeAssertions(DateTime subject, string expr) { _subject = subject; _expr = expr; }

    public void Be(DateTime expected)
    {
        if (_subject != expected)
            Fail.With($"Expected {_expr} to be {expected}, but found {_subject}.");
    }

    public void BeCloseTo(DateTime expected, TimeSpan tolerance)
    {
        var diff = (_subject - expected).Duration();
        if (diff > tolerance)
            Fail.With($"Expected {_expr} to be within {tolerance} of {expected}, but difference was {diff}.");
    }

    // --- Negation methods ---

    public void NotBeCloseTo(DateTime unexpected, TimeSpan tolerance)
    {
        var diff = (_subject - unexpected).Duration();
        if (diff <= tolerance)
            Fail.With($"Expected {_expr} not to be within {tolerance} of {unexpected}, but it was.");
    }

    public void BeBefore(DateTime threshold)
    {
        if (_subject >= threshold)
            Fail.With($"Expected {_expr} to be before {threshold}, but found {_subject}.");
    }

    public void BeAfter(DateTime threshold)
    {
        if (_subject <= threshold)
            Fail.With($"Expected {_expr} to be after {threshold}, but found {_subject}.");
    }

    public void BeOnOrBefore(DateTime threshold)
    {
        if (_subject > threshold)
            Fail.With($"Expected {_expr} to be on or before {threshold}, but found {_subject}.");
    }

    public void BeOnOrAfter(DateTime threshold)
    {
        if (_subject < threshold)
            Fail.With($"Expected {_expr} to be on or after {threshold}, but found {_subject}.");
    }

    public void BeInRange(DateTime minimum, DateTime maximum)
    {
        if (_subject < minimum || _subject > maximum)
            Fail.With($"Expected {_expr} to be in range [{minimum}, {maximum}], but found {_subject}.");
    }

    public void BeInRelativeRange(TimeSpan tolerance)
    {
        var now = DateTime.UtcNow;
        var diff = (_subject - now).Duration();
        if (diff > tolerance)
            Fail.With($"Expected {_expr} to be within {tolerance} of now, but difference was {diff}.");
    }

    public void BeInThePast()
    {
        if (_subject >= DateTime.UtcNow)
            Fail.With($"Expected {_expr} to be in the past, but found {_subject}.");
    }

    public void BeInFuture()
    {
        if (_subject <= DateTime.UtcNow)
            Fail.With($"Expected {_expr} to be in the future, but found {_subject}.");
    }

    public void BeMidnight()
    {
        if (_subject.TimeOfDay != TimeSpan.Zero)
            Fail.With($"Expected {_expr} to be at midnight, but found {_subject.TimeOfDay}.");
    }

    public void BeOneOf(params DateTime[] values)
    {
        bool found = false;
        foreach (var v in values)
        {
            if (_subject == v) { found = true; break; }
        }
        if (!found)
            Fail.With($"Expected {_expr} to be one of {string.Join(", ", values.Select(v => v.ToString("o")))}, but found {_subject}.");
    }

    public void NotBeBefore(DateTime threshold)
    {
        if (_subject < threshold)
            Fail.With($"Expected {_expr} not to be before {threshold}, but it was.");
    }

    public void NotBeAfter(DateTime threshold)
    {
        if (_subject > threshold)
            Fail.With($"Expected {_expr} not to be after {threshold}, but it was.");
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

    public void BeSameDateAs(DateTime other)
    {
        if (_subject.Date != other.Date)
            Fail.With($"Expected {_expr} to be on the same date as {other:d}, but found {_subject.Date}.");
    }

    public void NotBeSameDateAs(DateTime other)
    {
        if (_subject.Date == other.Date)
            Fail.With($"Expected {_expr} not to be on the same date as {other:d}, but it was.");
    }

    public void BeExactly(TimeSpan tolerance)
    {
        var now = DateTime.UtcNow;
        var diff = (_subject - now).Duration();
        if (diff > tolerance)
            Fail.With($"Expected {_expr} to be within {tolerance} of now, but difference was {diff}.");
    }

    public void BeAtLeast(TimeSpan tolerance) => BeExactly(tolerance);

    public void BeMoreThan(TimeSpan tolerance) => BeExactly(tolerance);

    public void BeWithin(TimeSpan range)
    {
        var now = DateTime.Now;
        var diff = (_subject - now).Duration();
        if (diff > range)
            Fail.With($"Expected {_expr} to be within {range} of now, but difference was {diff}.");
    }

    public void NotBe(DateTime unexpected)
    {
        if (_subject == unexpected)
            Fail.With($"Expected {_expr} not to be {unexpected}, but it was.");
    }

    public void BeIn(DateTimeKind kind)
    {
        if (_subject.Kind != kind && _subject.Kind != DateTimeKind.Unspecified)
            Fail.With($"Expected {_expr}.Kind to be {kind}, but found {_subject.Kind}.");
    }

    public void NotBeIn(DateTimeKind kind)
    {
        if (_subject.Kind == kind || _subject.Kind == DateTimeKind.Unspecified)
            Fail.With($"Expected {_expr}.Kind not to be {kind}, but found {_subject.Kind}.");
    }

    public AndConstraint<DateTimeAssertions> WithTime(int hour, int minute, int second = 0, int millisecond = 0)
    {
        var expected = new DateTime(_subject.Year, _subject.Month, _subject.Day, hour, minute, second, millisecond, _subject.Kind);
        if (_subject != expected)
            Fail.With($"Expected {_expr} to have time {expected:T}, but found {_subject.TimeOfDay}.");
        return new(this);
    }

    public AndConstraint<DateTimeAssertions> WithYear(int year)
    {
        if (_subject.Year != year)
            Fail.With($"Expected {_expr}.Year to be {year}, but found {_subject.Year}.");
        return new(this);
    }

    public AndConstraint<DateTimeAssertions> WithMonth(int month)
    {
        if (_subject.Month != month)
            Fail.With($"Expected {_expr}.Month to be {month}, but found {_subject.Month}.");
        return new(this);
    }

    public AndConstraint<DateTimeAssertions> WithDay(int day)
    {
        if (_subject.Day != day)
            Fail.With($"Expected {_expr}.Day to be {day}, but found {_subject.Day}.");
        return new(this);
    }

    public void NotBeOnOrBefore(DateTime threshold)
    {
        if (_subject > threshold)
            Fail.With($"Expected {_expr} not to be on or before {threshold}, but it was.");
    }

    public void NotBeOnOrAfter(DateTime threshold)
    {
        if (_subject >= threshold)
            Fail.With($"Expected {_expr} not to be on or after {threshold}, but it was.");
    }
}