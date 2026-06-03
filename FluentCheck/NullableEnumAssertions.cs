using System.Diagnostics.CodeAnalysis;

namespace FluentCheck;

public readonly struct NullableEnumAssertions<TEnum>
    where TEnum : struct, Enum
{
    private readonly TEnum? _subject;
    private readonly string _expr;

    internal NullableEnumAssertions(TEnum? subject, string expr) { _subject = subject; _expr = expr; }

    public TEnum Which => _subject!.Value;

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

    [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "Underlying type inspection needed.")]
    [RequiresDynamicCode("Uses TypeCode.GetUnderlyingType which may require dynamic code.")]
    public void HaveValue(TEnum expected)
    {
        _subject!.Value.Should().HaveValue(expected);
    }

    public void Be(TEnum expected)
    {
        if (!_subject.HasValue || !_subject.Value.Equals(expected))
            Fail.With($"Expected {_expr} to be {expected}, but found {(_subject.HasValue ? _subject.Value.ToString() : "<null>")}.");
    }

    public void NotBe(TEnum unexpected)
    {
        if (_subject.HasValue && _subject.Value.Equals(unexpected))
            Fail.With($"Expected {_expr} not to be {unexpected}, but it was.");
    }

    [RequiresDynamicCode("Uses Enum.HasFlag which requires runtime reflection for flag checks.")]
    [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "Enum methods require runtime reflection.")]
    [UnconditionalSuppressMessage("AOT", "IL3050:RequiresDynamicCode", Justification = "Enum.HasFlag requires dynamic code.")]
    public void HaveFlag(TEnum flag)
    {
        _subject!.Value.Should().HaveFlag(flag);
    }

    [RequiresDynamicCode("Uses Enum.HasFlag which requires runtime reflection for flag checks.")]
    [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "Enum methods require runtime reflection.")]
    [UnconditionalSuppressMessage("AOT", "IL3050:RequiresDynamicCode", Justification = "Enum.HasFlag requires dynamic code.")]
    public void NotHaveFlag(TEnum flag)
    {
        _subject!.Value.Should().NotHaveFlag(flag);
    }
}
