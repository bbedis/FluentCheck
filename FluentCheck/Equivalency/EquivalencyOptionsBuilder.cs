using System;
using System.Collections.Generic;

namespace FluentCheck.Equivalency;

public sealed class EquivalencyOptionsBuilder
{
    private EquivalencyOptions _options = EquivalencyOptions.Default;
    private CustomComparerRegistry _registry = new();

    private EquivalencyOptions Copy() => new()
    {
        MaxDepth = _options.MaxDepth,
        IgnoreTypeDifference = _options.IgnoreTypeDifference,
        IncludeFields = _options.IncludeFields,
        IgnoreCollectionOrder = _options.IgnoreCollectionOrder,
        ExcludeNonPublic = _options.ExcludeNonPublic,
        ContinueOnFailure = _options.ContinueOnFailure,
        IgnoringCase = _options.IgnoringCase,
        StrictTyping = _options.StrictTyping,
        ExcludedProperties = _options.ExcludedProperties?.ToHashSet(),
        ExcludedFields = _options.ExcludedFields?.ToHashSet(),
        PropertyFilter = _options.PropertyFilter,
        CustomComparers = _registry, // Use fresh registry, not leaked reference
    };

    public EquivalencyOptionsBuilder Using<T>(Func<ComparisonContext<T>, bool> comparer)
    {
        _registry.Add(comparer);
        return this;
    }

    public EquivalencyOptionsBuilder WithIgnoredCollectionOrder()
    {
        var copy = Copy();
        copy.IgnoreCollectionOrder = true;
        _options = copy;
        return this;
    }

    public EquivalencyOptionsBuilder WithIgnoredTypeDifference()
    {
        var copy = Copy();
        copy.IgnoreTypeDifference = true;
        _options = copy;
        return this;
    }

    public EquivalencyOptionsBuilder WithMaxDepth(int depth)
    {
        var copy = Copy();
        copy.MaxDepth = depth;
        _options = copy;
        return this;
    }

    public EquivalencyOptionsBuilder ExcludingProperty(string propertyName)
    {
        var copy = Copy();
        var props = (copy.ExcludedProperties ?? new HashSet<string>()).ToList();
        props.Add(propertyName);
        copy.ExcludedProperties = props.ToHashSet();
        _options = copy;
        return this;
    }

    public EquivalencyOptionsBuilder IncludingFields()
    {
        var copy = Copy();
        copy.IncludeFields = true;
        _options = copy;
        return this;
    }

    public EquivalencyOptionsBuilder ContinueOnFailure(bool value = true)
    {
        var copy = Copy();
        copy.ContinueOnFailure = value;
        _options = copy;
        return this;
    }

    public EquivalencyOptionsBuilder IgnoringCase()
    {
        var copy = Copy();
        copy.IgnoringCase = true;
        _options = copy;
        return this;
    }

    public EquivalencyOptionsBuilder WithStrictTyping()
    {
        var copy = Copy();
        copy.StrictTyping = true;
        _options = copy;
        return this;
    }

    public EquivalencyOptions Build()
    {
        return new EquivalencyOptions
        {
            MaxDepth = _options.MaxDepth,
            IgnoreTypeDifference = _options.IgnoreTypeDifference,
            IncludeFields = _options.IncludeFields,
            IgnoreCollectionOrder = _options.IgnoreCollectionOrder,
            ExcludeNonPublic = _options.ExcludeNonPublic,
            ContinueOnFailure = _options.ContinueOnFailure,
            IgnoringCase = _options.IgnoringCase,
            StrictTyping = _options.StrictTyping,
            ExcludedProperties = _options.ExcludedProperties,
            ExcludedFields = _options.ExcludedFields,
            PropertyFilter = _options.PropertyFilter,
            CustomComparers = _registry,
        };
    }
}
