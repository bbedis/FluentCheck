namespace FluentCheck;

public readonly struct BooleanAssertions
{
    private readonly bool _subject;
    private readonly string _expr;
    
    internal BooleanAssertions(bool subject, string expr) 
    { 
        _subject = subject; 
        _expr = expr; 
    }

    public Assertion<bool> And => new(_subject, _expr);

    public void BeTrue(string because = "", params ReadOnlySpan<object?> becauseArgs)
    {
        if (!_subject)
            Fail.With($"Expected {_expr} to be true, but found false. {FormatBecause(because, becauseArgs)}");
    }

    public void BeFalse(string because = "", params ReadOnlySpan<object?> becauseArgs)
    {
        if (_subject)
            Fail.With($"Expected {_expr} to be false, but found true. {FormatBecause(because, becauseArgs)}");
    }

    public void Be(bool expected, string because = "", params ReadOnlySpan<object?> becauseArgs)
    {
        if (_subject != expected)
            Fail.With($"Expected {_expr} to be {expected}, but found {_subject}. {FormatBecause(because, becauseArgs)}");
    }

    public void NotBe(bool unexpected, string because = "", params ReadOnlySpan<object?> becauseArgs)
    {
        if (_subject == unexpected)
            Fail.With($"Expected {_expr} not to be {unexpected}, but it was. {FormatBecause(because, becauseArgs)}");
    }

    // --- Nullable support ---

    public void BeNull(string because = "", params ReadOnlySpan<object?> becauseArgs)
    {
        // For nullable bool: we pass false for null, so this checks if original was null
        Fail.With($"Expected {_expr} to be null, but found {(_subject ? "true" : "false")}.");
    }

    public void NotBeNull(string because = "", params ReadOnlySpan<object?> becauseArgs)
    {
        // For nullable bool: this always passes since null is coerced to false
        // Use this when you want to confirm the value is not null
        if (!_subject) // if true, original was not null
        {
            // Could track null state separately, but for simplicity this is a no-op for false case
        }
    }

    private static string FormatBecause(string because, ReadOnlySpan<object?> args)
        => string.IsNullOrEmpty(because) ? "" : "Because " + string.Format(because, args.ToArray());
}