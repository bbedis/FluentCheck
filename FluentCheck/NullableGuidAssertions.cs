namespace FluentCheck;

public readonly struct NullableGuidAssertions
{
    private readonly Guid? _subject;
    private readonly string _expr;

    internal NullableGuidAssertions(Guid? subject, string expr) { _subject = subject; _expr = expr; }

    public Guid Which => _subject!.Value;

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
