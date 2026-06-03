using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FluentCheck.Equivalency;

public sealed class CustomComparerRegistry
{
    private readonly Dictionary<Type, Delegate> _comparers = new();

    public void Add<T>(Func<ComparisonContext<T>, bool> comparer)
    {
        _comparers[typeof(T)] = comparer;
    }

    public bool TryGetComparer<T>([NotNullWhen(true)] out Func<ComparisonContext<T>, bool>? comparer)
    {
        if (_comparers.TryGetValue(typeof(T), out var del) && del is Func<ComparisonContext<T>, bool> typed)
        {
            comparer = typed;
            return true;
        }
        comparer = null;
        return false;
    }

    public bool TryGetComparerNonGeneric(Type type, [NotNullWhen(true)] out Delegate? comparer)
        => _comparers.TryGetValue(type, out comparer);

    public bool HasComparerFor(Type type) => _comparers.ContainsKey(type);
}

/// <summary>
/// Context passed to custom comparers, providing access to subject, expectation, and nested equivalency.
/// </summary>
public readonly struct ComparisonContext<T>
{
    public T Subject { get; }
    public T Expectation { get; }
    public EquivalencyContext EquivalencyContext { get; }

    internal ComparisonContext(T subject, T expectation, EquivalencyContext ctx)
    {
        Subject = subject;
        Expectation = expectation;
        EquivalencyContext = ctx;
    }

    public bool AreEquivalent(object? a, object? b)
        => DeepEqualityComparer.AreEquivalent(a, b, EquivalencyContext.Options, out _);
}
