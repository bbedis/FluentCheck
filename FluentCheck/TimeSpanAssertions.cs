namespace FluentCheck;

public readonly struct TimeSpanAssertions
{
    private readonly TimeSpan _subject;
    private readonly string _expr;

    internal TimeSpanAssertions(TimeSpan subject, string expr) { _subject = subject; _expr = expr; }

    public void Be(TimeSpan expected, string because = "", params ReadOnlySpan<object?> becauseArgs)
    {
        if (_subject != expected)
            Fail.With($"Expected {_expr} to be {expected}, but found {_subject}. {FormatBecause(because, becauseArgs)}");
    }

    public void BeGreaterThan(TimeSpan expected)
    {
        if (_subject <= expected)
            Fail.With($"Expected {_expr} to be greater than {expected}, but found {_subject}.");
    }

    public void BeLessThan(TimeSpan expected)
    {
        if (_subject >= expected)
            Fail.With($"Expected {_expr} to be less than {expected}, but found {_subject}.");
    }

    public void BeApproximately(TimeSpan expected, TimeSpan tolerance)
    {
        var diff = (_subject - expected).Duration();
        if (diff > tolerance)
            Fail.With($"Expected {_expr} to be within {tolerance} of {expected}, but difference was {diff}.");
    }

    public void BeWithin(TimeSpan range)
    {
        var now = DateTime.Now;
        var subjectAsDateTime = new DateTime(_subject.Ticks);
        var diff = (subjectAsDateTime - now).Duration();
        if (diff > range)
            Fail.With($"Expected {_expr} to be within {range} of now, but difference was {diff}.");
    }

    public void BeZero()
    {
        if (_subject != TimeSpan.Zero)
            Fail.With($"Expected {_expr} to be zero, but found {_subject}.");
    }

    public void BePositive()
    {
        if (_subject <= TimeSpan.Zero)
            Fail.With($"Expected {_expr} to be positive, but found {_subject}.");
    }

    public void BeNegative()
    {
        if (_subject >= TimeSpan.Zero)
            Fail.With($"Expected {_expr} to be negative, but found {_subject}.");
    }

    public void BeOneOf(params TimeSpan[] values)
    {
        bool found = false;
        foreach (var v in values)
        {
            if (_subject == v) { found = true; break; }
        }
        if (!found)
            Fail.With($"Expected {_expr} to be one of {string.Join(", ", values)}, but found {_subject}.");
    }

    // --- Negation ---

    public void NotBe(TimeSpan unexpected)
    {
        if (_subject == unexpected)
            Fail.With($"Expected {_expr} not to be {unexpected}, but it was.");
    }

    public void NotBeGreaterThan(TimeSpan unexpected)
    {
        if (_subject > unexpected)
            Fail.With($"Expected {_expr} not to be greater than {unexpected}, but it was.");
    }

    public void NotBeLessThan(TimeSpan unexpected)
    {
        if (_subject < unexpected)
            Fail.With($"Expected {_expr} not to be less than {unexpected}, but it was.");
    }

    public void NotBeZero()
    {
        if (_subject == TimeSpan.Zero)
            Fail.With($"Expected {_expr} not to be zero, but it was.");
    }

    public void NotBePositive()
    {
        if (_subject > TimeSpan.Zero)
            Fail.With($"Expected {_expr} not to be positive, but it was.");
    }

    public void NotBeNegative()
    {
        if (_subject < TimeSpan.Zero)
            Fail.With($"Expected {_expr} not to be negative, but it was.");
    }

    private static string FormatBecause(string because, ReadOnlySpan<object?> args)
        => string.IsNullOrEmpty(because) ? "" : " Because " + string.Format(because, args.ToArray());
}
