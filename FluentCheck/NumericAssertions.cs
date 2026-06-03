using System.Numerics;

namespace FluentCheck;

public readonly struct NumericAssertions<T> where T : struct, IComparable<T>, INumber<T>
{
    private readonly T _subject;
    private readonly string _expr;
    internal NumericAssertions(T subject, string expr) { _subject = subject; _expr = expr; }

    public Assertion<T> And => new(_subject, _expr);

    public void Be(T expected)
    {
        if (_subject.CompareTo(expected) != 0)
            Fail.With($"Expected {_expr} to be {expected}, but found {_subject}.");
    }

    public void BeGreaterThan(T threshold)
    {
        if (_subject.CompareTo(threshold) <= 0)
            Fail.With($"Expected {_expr} to be greater than {threshold}, but found {_subject}.");
    }

    public void BePositive()
    {
        if (_subject.CompareTo(T.Zero) <= 0)
            Fail.With($"Expected {_expr} to be positive, but found {_subject}.");
    }

    // --- Negation methods ---

    public void NotBe(T unexpected)
    {
        if (_subject.CompareTo(unexpected) == 0)
            Fail.With($"Expected {_expr} not to be {unexpected}, but it was.");
    }

    public void BeLessThan(T threshold)
    {
        if (_subject.CompareTo(threshold) >= 0)
            Fail.With($"Expected {_expr} to be less than {threshold}, but found {_subject}.");
    }

    public void BeAtMost(T threshold)
    {
        if (_subject.CompareTo(threshold) > 0)
            Fail.With($"Expected {_expr} to be at most {threshold}, but found {_subject}.");
    }

    public void BeAtLeast(T threshold)
    {
        if (_subject.CompareTo(threshold) < 0)
            Fail.With($"Expected {_expr} to be at least {threshold}, but found {_subject}.");
    }

    public void BeInRange(T minimum, T maximum)
    {
        if (_subject.CompareTo(minimum) < 0 || _subject.CompareTo(maximum) > 0)
            Fail.With($"Expected {_expr} to be in range [{minimum}, {maximum}], but found {_subject}.");
    }

    public void BeNegative()
    {
        if (_subject.CompareTo(T.Zero) >= 0)
            Fail.With($"Expected {_expr} to be negative, but found {_subject}.");
    }

    public void BeZero()
    {
        if (_subject.CompareTo(T.Zero) != 0)
            Fail.With($"Expected {_expr} to be zero, but found {_subject}.");
    }

    public void NotBeGreaterThan(T threshold)
    {
        if (_subject.CompareTo(threshold) > 0)
            Fail.With($"Expected {_expr} not to be greater than {threshold}, but it was.");
    }

    public void NotBePositive()
    {
        if (_subject.CompareTo(T.Zero) > 0)
            Fail.With($"Expected {_expr} not to be positive, but it was.");
    }

    public void NotBeNegative()
    {
        if (_subject.CompareTo(T.Zero) < 0)
            Fail.With($"Expected {_expr} not to be negative, but it was.");
    }

    public void NotBeZero()
    {
        if (_subject.CompareTo(T.Zero) == 0)
            Fail.With($"Expected {_expr} not to be zero, but it was.");
    }

    public void BeApproximately(T expected, T tolerance)
    {
        var diff = Math.Abs(Convert.ToDouble(_subject) - Convert.ToDouble(expected));
        if (diff > Convert.ToDouble(tolerance))
            Fail.With($"Expected {_expr} to be approximately {expected} (tolerance: {tolerance}), but difference was {diff}.");
    }
}