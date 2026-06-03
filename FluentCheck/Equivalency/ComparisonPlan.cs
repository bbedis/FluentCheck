using System;
using System.Linq;

namespace FluentCheck.Equivalency;

internal sealed class ComparisonPlan
{
    public record PropertyPair(PropertyMetadata Expected, PropertyMetadata? Actual);
    public record FieldPair(FieldMetadata Expected, FieldMetadata? Actual);

    public PropertyPair[] Properties { get; init; } = [];
    public FieldPair[] Fields { get; init; } = [];

    public static ComparisonPlan Create(TypeMetadata expected, TypeMetadata actual, EquivalencyOptions options)
    {
        var propPairs = expected.Properties
            .Where(p => options.ExcludedProperties?.Contains(p.Name) != true)
            .Select(p => new PropertyPair(p, actual.TryGetValue(p.Name, out var actualProp) ? actualProp : null))
            .ToArray();

        var fieldPairs = options.IncludeFields
            ? expected.Fields
                .Where(f => options.ExcludedFields?.Contains(f.Name) != true)
                .Select(f => new FieldPair(f, actual.TryGetField(f.Name, out var actualField) ? actualField : null))
                .ToArray()
            : [];

        return new ComparisonPlan { Properties = propPairs, Fields = fieldPairs };
    }
}
