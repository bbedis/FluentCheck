using System.Diagnostics.CodeAnalysis;

namespace FluentCheck;

public readonly struct EnumAssertions<TEnum>
    where TEnum : struct, Enum
{
    private readonly TEnum _subject;
    private readonly string _expr;

    internal EnumAssertions(TEnum subject, string expr) { _subject = subject; _expr = expr; }

    public void Be(TEnum expected)
    {
        if (!_subject.Equals(expected))
            Fail.With($"Expected {_expr} to be {expected}, but found {_subject}.");
    }

    public void NotBe(TEnum unexpected)
    {
        if (_subject.Equals(unexpected))
            Fail.With($"Expected {_expr} not to be {unexpected}, but it was.");
    }

    public void BeOneOf(params TEnum[] values)
    {
        foreach (var v in values)
        {
            if (_subject.Equals(v)) return;
        }
        Fail.With($"Expected {_expr} to be one of {string.Join(", ", values)}, but found {_subject}.");
    }

    [RequiresDynamicCode("Uses Enum.HasFlag which requires runtime reflection for flag checks.")]
    [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "Enum methods require runtime reflection.")]
    [UnconditionalSuppressMessage("AOT", "IL3050:RequiresDynamicCode", Justification = "Enum.HasFlag requires dynamic code.")]
    public void HaveFlag(TEnum flag)
    {
        if (!((Enum)_subject).HasFlag(flag))
            Fail.With($"Expected {_expr} to have flag {flag}, but found {_subject}.");
    }

    [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "Underlying type inspection needed.")]
    [RequiresDynamicCode("Uses TypeCode.GetUnderlyingType which may require dynamic code.")]
    public void HaveValue(TEnum expected)
    {
        // For bitwise-combined flags, compare underlying numeric values
        var subjectUnderlying = ConvertToUnderlying(_subject);
        var expectedUnderlying = ConvertToUnderlying(expected);
        if (!Equals(subjectUnderlying, expectedUnderlying))
            Fail.With($"Expected {_expr} to have value {expectedUnderlying}, but found {subjectUnderlying}.");
    }

    [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "Underlying type inspection needed.")]
    [RequiresDynamicCode("Uses TypeCode.GetUnderlyingType which may require dynamic code.")]
    public void NotHaveFlag(TEnum flag)
    {
        if (((Enum)_subject).HasFlag(flag))
            Fail.With($"Expected {_expr} not to have flag {flag}, but it did.");
    }

    private static object ConvertToUnderlying(TEnum value)
    {
        return Convert.ChangeType(value, Enum.GetUnderlyingType(typeof(TEnum)));
    }
}
