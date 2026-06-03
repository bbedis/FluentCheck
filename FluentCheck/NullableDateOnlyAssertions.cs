namespace FluentCheck;

public readonly struct NullableDateOnlyAssertions
{
    private readonly DateOnly? _subject;
    private readonly string _expr;

    internal NullableDateOnlyAssertions(DateOnly? subject, string expr) { _subject = subject; _expr = expr; }

    public DateOnly Which => _subject!.Value;

    public void HaveValue()
    {
        if (!_subject.HasValue)
            Fail.With($"Expected {_expr} to have a value, but it was null.");
    }

    public void NotHaveValue()
    {
        if (_subject.HasValue)
            Fail.With($"Expected {_expr} to be null, but found {_subject.Value}.");
    }
}
