namespace FluentCheck;

public readonly struct NullableAssertions<T> where T : struct
{
    private readonly T? _subject;
    private readonly string _expr;
    internal NullableAssertions(T? subject, string expr) { _subject = subject; _expr = expr; }

    public T Which => _subject!.Value;

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

public readonly struct GuidAssertions
{
    private readonly Guid _subject;
    private readonly string _expr;
    internal GuidAssertions(Guid subject, string expr) { _subject = subject; _expr = expr; }

    public void Be(Guid expected)
    {
        if (_subject != expected)
            Fail.With($"Expected {_expr} to be {expected}, but found {_subject}.");
    }

    public void BeEmpty()
    {
        if (_subject != Guid.Empty)
            Fail.With($"Expected {_expr} to be empty, but found {_subject}.");
    }

    public void NotBeEmpty()
    {
        if (_subject == Guid.Empty)
            Fail.With($"Expected {_expr} not to be empty.");
    }
}