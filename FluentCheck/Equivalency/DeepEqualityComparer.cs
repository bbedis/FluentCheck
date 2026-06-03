using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace FluentCheck.Equivalency;

/// <summary>
/// Immutable cache key for EquivalencyOptions — zero-alloc readonly struct to avoid GC pressure.
/// </summary>
internal readonly record struct OptionsKey
{
    public readonly int? MaxDepth;
    public readonly bool IgnoreTypeDifference;
    public readonly bool IncludeFields;
    public readonly bool IgnoreCollectionOrder;
    public readonly bool ExcludeNonPublic;
    public readonly bool ContinueOnFailure;
    public readonly bool IgnoringCase;
    public readonly bool StrictTyping;
    public readonly string? ExcludedPropsHash;
    public readonly string? ExcludedFieldsHash;

    public OptionsKey(EquivalencyOptions opts)
    {
        MaxDepth = opts.MaxDepth;
        IgnoreTypeDifference = opts.IgnoreTypeDifference;
        IncludeFields = opts.IncludeFields;
        IgnoreCollectionOrder = opts.IgnoreCollectionOrder;
        ExcludeNonPublic = opts.ExcludeNonPublic;
        ContinueOnFailure = opts.ContinueOnFailure;
        IgnoringCase = opts.IgnoringCase;
        StrictTyping = opts.StrictTyping;
        ExcludedPropsHash = opts.ExcludedProperties is not null ? ToSortedHash(opts.ExcludedProperties) : null;
        ExcludedFieldsHash = opts.ExcludedFields is not null ? ToSortedHash(opts.ExcludedFields) : null;
    }

    private static string? ToSortedHash(IEnumerable<string> items)
    {
        var list = items.ToArray();
        if (list.Length == 0) return "";
        System.Array.Sort(list);
        var sb = new System.Text.StringBuilder(list.Length * 8);
        for (int i = 0; i < list.Length; i++)
        {
            if (i > 0) sb.Append(',');
            sb.Append(list[i]);
        }
        return sb.ToString();
    }
}

/// <summary>
/// Composite key for filtered comparison plans: (expectedType, actualType, optionsKey).
/// </summary>
internal readonly struct FilteredPlanKey : IEquatable<FilteredPlanKey>
{
    public readonly Type ExpectedType;
    public readonly Type ActualType;
    public readonly OptionsKey Options;

    public FilteredPlanKey(Type expected, Type actual, OptionsKey options)
    {
        ExpectedType = expected;
        ActualType = actual;
        Options = options;
    }

    public bool Equals(FilteredPlanKey other)
        => ExpectedType == other.ExpectedType && ActualType == other.ActualType && Options.Equals(other.Options);

    public override bool Equals(object? obj) => obj is FilteredPlanKey other && Equals(other);

    public override int GetHashCode()
    {
        var h = new HashCode();
        h.Add(ExpectedType);
        h.Add(ActualType);
        h.Add(Options);
        return h.ToHashCode();
    }
}

/// <summary>
/// High-performance deep equivalency comparer with caching, expression trees, and path tracking.
/// </summary>
public static class DeepEqualityComparer
{
    private static readonly ConcurrentDictionary<Type, TypeMetadata> _metadataCache = new();
    private static readonly ConcurrentDictionary<(Type, Type), ComparisonPlan> _planCache = new();
    private static readonly ConcurrentDictionary<FilteredPlanKey, ComparisonPlan> _filteredPlanCache = new();
    private static readonly ConcurrentDictionary<Type, Func<object, int>> _countConverters = new();

    public static bool AreEquivalent(object? expected, object? actual,
        [NotNullWhen(false)] out string? failureMessage)
        => AreEquivalent(expected, actual, EquivalencyOptions.Default, out failureMessage);

    public static bool AreEquivalent(object? expected, object? actual, EquivalencyOptions options,
        [NotNullWhen(false)] out string? failureMessage)
    {
        var ctx = new EquivalencyContext(options);
        AreEquivalentInternal(expected, actual, ctx, depth: 0);
        failureMessage = ctx.GetAggregatedFailure();
        return ctx.FailureCount == 0;
    }

    private static bool AreEquivalentInternal(object? expected, object? actual, EquivalencyContext ctx, int depth)
    {
        if (ReferenceEquals(expected, actual)) return true;

        // 🟢 FIX BUG CRITIQUE : Enregistrer l'échec si l'un est null
        if (expected is null || actual is null)
        {
            ctx.AddFailure($"Mismatch: expected {Format(expected)}, but found {Format(actual)}");
            return false;
        }

        if (depth > ctx.Options.MaxDepth) return true;

        if (ctx.Options.CustomComparers?.TryGetComparerNonGeneric(expected.GetType(), out var comparer) == true)
        {
            var contextType = typeof(ComparisonContext<>).MakeGenericType(expected.GetType());
            var context = Activator.CreateInstance(contextType, expected, actual, ctx)!;

            var result = (bool)comparer.DynamicInvoke(context)!;

            if (!result)
                ctx.AddFailure($"Custom comparer for {expected.GetType().Name} returned false");

            return result;
        }

        // 🟢 FIX BUG : Enregistrer l'échec si le chemin rapide retourne false
        if (TryFastPath(expected, actual, ctx, out var fastResult))
        {
            if (fastResult.Value == false)
            {
                ctx.AddFailure($"Expected {Format(expected)}, but found {Format(actual)}");
            }
            return fastResult.Value;
        }

        if (IsDictionaryType(expected, out var eDict) && IsDictionaryType(actual, out var aDict))
            return CompareDictionaries(eDict, aDict, ctx, depth);

        if (expected is IEnumerable eEnum && actual is IEnumerable aEnum)
            return CompareCollections(eEnum, aEnum, ctx, depth);

        var tExpected = expected.GetType();
        var tActual = actual.GetType();

        if (tExpected != tActual && !ctx.Options.IgnoreTypeDifference)
        {
            ctx.AddFailure($"Type mismatch: expected {tExpected.Name}, found {tActual.Name}");
            return false;
        }

        var expectedMeta = _metadataCache.GetOrAdd(tExpected, TypeMetadata.Create);
        var actualMeta = _metadataCache.GetOrAdd(tActual, TypeMetadata.Create);

        // Default options: use simple type-pair cache key (fast path, zero alloc).
        // Options with exclusions: use composite key to avoid cache pollution.
        ComparisonPlan plan;
        if (ctx.Options.ExcludedProperties is null && ctx.Options.ExcludedFields is null)
        {
            plan = _planCache.GetOrAdd((tExpected, tActual), _ =>
                ComparisonPlan.Create(expectedMeta, actualMeta, ctx.Options));
        }
        else
        {
            var optKey = new OptionsKey(ctx.Options);
            plan = _filteredPlanCache.GetOrAdd(new FilteredPlanKey(tExpected, tActual, optKey), _ =>
                ComparisonPlan.Create(expectedMeta, actualMeta, ctx.Options));
        }

        return CompareWithPlan(expected, actual, plan, ctx, depth);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryFastPath(object a, object b, EquivalencyContext ctx, [NotNullWhen(true)] out bool? result)
    {
        // 🟢 FIX BUG : Prise en compte de IgnoringCase pour les chaînes
        if (a is string sa && b is string sb)
        {
            result = string.Equals(sa, sb, ctx.Options.IgnoringCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
            return true;
        }

        var aType = a.GetType();
        var bType = b.GetType();

        // 🟢 FIX BUG : Support de decimal avec IgnoreTypeDifference (decimal.IsPrimitive == false)
        if (ctx.Options.IgnoreTypeDifference && IsNumericType(aType) && IsNumericType(bType) && aType != bType)
        {
            try
            {
                result = Convert.ToDecimal(a) == Convert.ToDecimal(b);
                return true;
            }
            catch { }
        }

        if (aType.IsPrimitive && bType.IsPrimitive)
        {
            result = Equals(a, b);
            return true;
        }

        if (a is DateTime or DateTimeOffset or Guid or decimal ||
            b is DateTime or DateTimeOffset or Guid or decimal)
        {
            result = Equals(a, b);
            return true;
        }

        if (aType.IsEnum && aType == bType)
        {
            result = Equals(a, b);
            return true;
        }

        result = null;
        return false;
    }

    private static bool IsNumericType(Type type) =>
        Type.GetTypeCode(type) switch
        {
            TypeCode.SByte or TypeCode.Byte or TypeCode.Int16 or TypeCode.UInt16 or
            TypeCode.Int32 or TypeCode.UInt32 or TypeCode.Int64 or TypeCode.UInt64 or
            TypeCode.Single or TypeCode.Double or TypeCode.Decimal => true,
            _ => false
        };

    // 🟢 OPTIMISATION : Extraire Count sans forcer LINQ CastIterator
    private static int GetCollectionCount(IEnumerable enumerable)
    {
        if (enumerable is ICollection c) return c.Count;
        if (enumerable is IReadOnlyCollection<object> roc) return roc.Count;

        var type = enumerable.GetType();
        var converter = _countConverters.GetOrAdd(type, t =>
        {
            var prop = t.GetProperty("Count") ?? t.GetProperty("Length");
            if (prop != null && prop.CanRead && prop.PropertyType == typeof(int))
            {
                var method = prop.GetGetMethod()!;
                return (Func<object, int>)Delegate.CreateDelegate(typeof(Func<object, int>), method);
            }
            return _ => enumerable.Cast<object>().Count();
        });

        return converter(enumerable);
    }

    private static bool CompareCollections(IEnumerable expected, IEnumerable actual,
        EquivalencyContext ctx, int depth)
    {
        int eCount = GetCollectionCount(expected);
        int aCount = GetCollectionCount(actual);

        if (eCount != aCount)
        {
            ctx.AddFailure($"Collection count mismatch: expected {eCount}, found {aCount}");
            return false;
        }

        if (eCount == 0) return true;

        if (ctx.Options.IgnoreCollectionOrder)
        {
            return CompareCollectionsUnordered(expected, actual, ctx, depth);
        }

        var eList = expected as IList ?? expected.Cast<object>().ToList();
        var aList = actual as IList ?? actual.Cast<object>().ToList();

        var allPassed = true;
        for (int i = 0; i < eList.Count; i++)
        {
            using (ctx.PushPath($"[{i}]"))
            {
                if (!AreEquivalentInternal(eList[i], aList[i], ctx, depth + 1))
                {
                    allPassed = false;
                    if (!ctx.Options.ContinueOnFailure) return false;
                }
            }
        }
        return allPassed;
    }

    private static bool CompareCollectionsUnordered(IEnumerable expected, IEnumerable actual,
        EquivalencyContext ctx, int depth)
    {
        var actualItems = actual.Cast<object>().ToList();
        var matched = new bool[actualItems.Count];
        var allPassed = true;

        int index = 0;
        foreach (var expectedItem in expected)
        {
            bool found = false;
            for (int i = 0; i < actualItems.Count; i++)
            {
                if (matched[i]) continue;

                using (ctx.PushPath($"[{index}]~[{i}]"))
                {
                    int checkpoint = ctx.FailureCount;

                    if (AreEquivalentInternal(expectedItem, actualItems[i], ctx, depth + 1))
                    {
                        matched[i] = true;
                        found = true;
                        break;
                    }
                    else
                    {
                        ctx.RollbackFailures(checkpoint);
                    }
                }
            }

            if (!found)
            {
                ctx.AddFailure($"Item at index [{index}] not found in actual collection (order ignored)");
                allPassed = false;
                if (!ctx.Options.ContinueOnFailure) return false;
            }
            index++;
        }

        for (int i = 0; i < matched.Length; i++)
        {
            if (!matched[i])
            {
                ctx.AddFailure($"Extra item at index [{i}] in actual collection not found in expected (order ignored)");
                allPassed = false;
                if (!ctx.Options.ContinueOnFailure) return false;
            }
        }

        return allPassed;
    }

    private static bool CompareDictionaries(IDictionary expected, IDictionary actual,
        EquivalencyContext ctx, int depth)
    {
        if (expected.Count != actual.Count)
        {
            ctx.AddFailure($"Dictionary count mismatch: expected {expected.Count}, found {actual.Count}");
            return false;
        }

        var allPassed = true;
        foreach (DictionaryEntry entry in expected)
        {
            using (ctx.PushPath($"[{entry.Key}]"))
            {
                if (!actual.Contains(entry.Key))
                {
                    ctx.AddFailure($"Key \"{entry.Key}\" not found in actual dictionary");
                    allPassed = false;
                    if (!ctx.Options.ContinueOnFailure) return false;
                    continue;
                }

                if (!AreEquivalentInternal(entry.Value, actual[entry.Key], ctx, depth + 1))
                {
                    allPassed = false;
                    if (!ctx.Options.ContinueOnFailure) return false;
                }
            }
        }
        return allPassed;
    }

    private static bool IsDictionaryType(object? obj, [NotNullWhen(true)] out IDictionary? dictionary)
    {
        if (obj is IDictionary dict)
        {
            dictionary = dict;
            return true;
        }

        if (obj is not null && TryAdaptReadOnlyDictionary(obj, out var adapted))
        {
            dictionary = adapted;
            return true;
        }

        dictionary = null;
        return false;
    }

    private static bool TryAdaptReadOnlyDictionary(object obj, [NotNullWhen(true)] out IDictionary? adapted)
    {
        var type = obj.GetType();
        var readOnlyDictInterface = type.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>));

        if (readOnlyDictInterface is null)
        {
            adapted = null;
            return false;
        }

        adapted = new ReadOnlyDictionaryAdapter(obj, readOnlyDictInterface);
        return true;
    }

    private sealed class ReadOnlyDictionaryAdapter : IDictionary
    {
        private readonly object _source;
        private readonly Type _interfaceType;
        private readonly MethodInfo _getItemMethod;
        private readonly PropertyInfo _keysProperty;
        private Dictionary<object, object?>? _cachedEntries;

        public ReadOnlyDictionaryAdapter(object source, Type interfaceType)
        {
            _source = source;
            _interfaceType = interfaceType;
            _getItemMethod = interfaceType.GetProperty("Item")!.GetGetMethod()!;
            _keysProperty = interfaceType.GetProperty("Keys")!;
        }

        private Dictionary<object, object?> GetEntries()
        {
            if (_cachedEntries is not null) return _cachedEntries;

            var keys = (IEnumerable)_keysProperty.GetValue(_source)!;
            var entries = new Dictionary<object, object?>();

            foreach (var key in keys)
            {
                var value = _getItemMethod.Invoke(_source, [key]);
                entries.Add(key, value);
            }

            _cachedEntries = entries;
            return entries;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEntries().GetEnumerator();
        IDictionaryEnumerator IDictionary.GetEnumerator() => ((IDictionary)GetEntries()).GetEnumerator();

        public void Add(object key, object? value) => throw new NotSupportedException();
        public void Clear() => throw new NotSupportedException();
        public bool Contains(object key) => GetEntries().ContainsKey(key);
        public void Remove(object key) => throw new NotSupportedException();
        public bool IsFixedSize => true;
        public bool IsReadOnly => true;

        public object? this[object key]
        {
            get => GetEntries().TryGetValue(key, out var val) ? val : null;
            set => throw new NotSupportedException();
        }

        public ICollection Keys => GetEntries().Keys;
        public ICollection Values => GetEntries().Values;
        public int Count => GetEntries().Count;
        public bool IsSynchronized => false;
        public object SyncRoot => _source;

        public void CopyTo(Array array, int index) => ((ICollection)GetEntries()).CopyTo(array, index);
    }

    private static bool CompareWithPlan(object expected, object actual,
        ComparisonPlan plan, EquivalencyContext ctx, int depth)
    {
        bool allPassed = true;

        foreach (var pair in plan.Properties)
        {
            allPassed &= CompareMember(expected, actual, pair.Expected, pair.Actual, pair.Expected.Name, ctx, depth);
            if (!allPassed && !ctx.Options.ContinueOnFailure) return false;
        }

        if (ctx.Options.IncludeFields)
        {
            foreach (var pair in plan.Fields)
            {
                allPassed &= CompareField(expected, actual, pair.Expected, pair.Actual, pair.Expected.Name, ctx, depth);
                if (!allPassed && !ctx.Options.ContinueOnFailure) return false;
            }
        }

        return allPassed;
    }

    private static bool CompareMember(object expected, object actual,
        PropertyMetadata prop, PropertyMetadata? actualProp, string name,
        EquivalencyContext ctx, int depth)
    {
        using (ctx.PushPath(name))
        {
            if (prop.TryCompareUnboxed(expected, actual, ctx, out var unboxedResult))
            {
                if (unboxedResult == false)
                {
                    var mE = prop.GetValueFast(expected);
                    var mA = actualProp?.GetValueFast(actual);
                    ctx.AddFailure($"Expected {Format(mE)}, but found {Format(mA)}");
                    return false;
                }
                return true;
            }

            var mE2 = prop.GetValueFast(expected);
            var mA2 = actualProp?.GetValueFast(actual);

            return AreEquivalentInternal(mE2, mA2, ctx, depth + 1);
        }
    }

    private static bool CompareField(object expected, object actual,
        FieldMetadata field, FieldMetadata? actualField, string name,
        EquivalencyContext ctx, int depth)
    {
        using (ctx.PushPath(name))
        {
            if (field.TryCompareUnboxed(expected, actual, ctx, out var unboxedResult))
            {
                if (unboxedResult == false)
                {
                    var fE = field.GetValueFast(expected);
                    var fA = actualField?.GetValueFast(actual);
                    ctx.AddFailure($"Expected {Format(fE)}, but found {Format(fA)}");
                    return false;
                }
                return true;
            }

            var fE2 = field.GetValueFast(expected);
            var fA2 = actualField?.GetValueFast(actual);

            return AreEquivalentInternal(fE2, fA2, ctx, depth + 1);
        }
    }

    private static string Format(object? value) => value switch
    {
        null => "<null>",
        string s => $"\"{s}\"",
        _ => value.ToString() ?? "<null>"
    };
}
