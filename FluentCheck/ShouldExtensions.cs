using System.Numerics;
using System.Runtime.CompilerServices;

namespace FluentCheck;

public static class ShouldExtensions
{
    // 1. Object
    public static ObjectAssertions Should(this object? subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        => new(subject, expr);

    // 2. String
    public static StringAssertions Should(this string? subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        => new(subject, expr);

    // 3. Boolean
    public static BooleanAssertions Should(this bool subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        => new(subject, expr);

    public static BooleanAssertions Should(this bool? subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        => new(subject ?? false, expr);

    // 4. Numeric (concrete types to avoid generic signature conflicts with Enum)
    public static NumericAssertions<int> Should(this int subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        => new(subject, expr);

    public static NumericAssertions<long> Should(this long subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        => new(subject, expr);

    public static NumericAssertions<float> Should(this float subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        => new(subject, expr);

    public static NumericAssertions<double> Should(this double subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        => new(subject, expr);

    public static NumericAssertions<decimal> Should(this decimal subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        => new(subject, expr);

    public static NumericAssertions<byte> Should(this byte subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        => new(subject, expr);

    public static NumericAssertions<short> Should(this short subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        => new(subject, expr);

    public static NumericAssertions<ushort> Should(this ushort subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        => new(subject, expr);

    public static NumericAssertions<uint> Should(this uint subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        => new(subject, expr);

    public static NumericAssertions<ulong> Should(this ulong subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        => new(subject, expr);

    public static NumericAssertions<sbyte> Should(this sbyte subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        => new(subject, expr);

    public static NumericAssertions<nint> Should(this nint subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        => new(subject, expr);

    public static NumericAssertions<nuint> Should(this nuint subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        => new(subject, expr);

    // Nullable numerics
    public static NullableAssertions<int> Should(this int? subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        => new(subject, expr);

    public static NullableAssertions<long> Should(this long? subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        => new(subject, expr);

    public static NullableAssertions<float> Should(this float? subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        => new(subject, expr);

    public static NullableAssertions<double> Should(this double? subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        => new(subject, expr);

    public static NullableAssertions<decimal> Should(this decimal? subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        => new(subject, expr);

    public static NullableAssertions<byte> Should(this byte? subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        => new(subject, expr);

    public static NullableAssertions<short> Should(this short? subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        => new(subject, expr);

    public static NullableAssertions<ushort> Should(this ushort? subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        => new(subject, expr);

    public static NullableAssertions<uint> Should(this uint? subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        => new(subject, expr);

    public static NullableAssertions<ulong> Should(this ulong? subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        => new(subject, expr);

    public static NullableAssertions<sbyte> Should(this sbyte? subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        => new(subject, expr);

    public static NullableAssertions<nint> Should(this nint? subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        => new(subject, expr);

    public static NullableAssertions<nuint> Should(this nuint? subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        => new(subject, expr);

    // 5. Collections
    public static CollectionAssertions<T> Should<T>(this IEnumerable<T>? subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        => new(subject, expr);

    // 6. Dictionaries — IDictionary preferred: zero alloc, no copy. ImmutableDictionary: cast explicitly.
    public static DictionaryAssertions<TKey, TValue> Should<TKey, TValue>(this IDictionary<TKey, TValue>? subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        where TKey : notnull
        => new(subject, expr);

    // 7. Guid
    public static GuidAssertions Should(this Guid subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        => new(subject, expr);

    // 8. Dates
    public static DateTimeAssertions Should(this DateTime subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        => new(subject, expr);

    public static NullableDateTimeAssertions Should(this DateTime? subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        => new(subject, expr);

    public static DateOnlyAssertions Should(this DateOnly subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        => new(subject, expr);

    public static NullableDateOnlyAssertions Should(this DateOnly? subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        => new(subject, expr);

    // 9. DateTimeOffset + TimeSpan
    public static DateTimeOffsetAssertions Should(this DateTimeOffset subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        => new(subject, expr);

    public static TimeSpanAssertions Should(this TimeSpan subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        => new(subject, expr);

    // 10. NullableGuid
    public static NullableGuidAssertions Should(this Guid? subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        => new(subject, expr);

    // 11. Enums (non-nullable)
    public static EnumAssertions<TEnum> Should<TEnum>(this TEnum subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        where TEnum : struct, Enum
        => new(subject, expr);

    // 11b. Nullable enums
    public static NullableEnumAssertions<TEnum> Should<TEnum>(this TEnum? subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        where TEnum : struct, Enum
        => new(subject, expr);

    // 12. String collection
    public static StringCollectionAssertions Should(this IEnumerable<string> subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        => new(subject, expr);

    // 13. Actions
    public static ActionAssertions Should(this Action subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        => new(subject, expr);

    public static FuncTaskAssertions Should(this Func<Task> subject,
        [CallerArgumentExpression(nameof(subject))] string expr = "")
        => new(subject, expr);
}
