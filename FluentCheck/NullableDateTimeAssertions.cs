namespace FluentCheck;

public readonly struct NullableDateTimeAssertions
{
    private readonly DateTime? _subject;
    private readonly string _expr;

    internal NullableDateTimeAssertions(DateTime? subject, string expr) { _subject = subject; _expr = expr; }

    public DateTime Which => _subject!.Value;

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
