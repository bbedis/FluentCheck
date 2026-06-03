using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace FluentCheck.Equivalency;

/// <summary>
/// Cached metadata for a property, using compiled expression trees for fast access.
/// Includes a direct comparison delegate for zero-boxing value type comparisons.
/// </summary>
internal sealed class PropertyMetadata
{
    public string Name { get; }
    public Type PropertyType { get; }

    private readonly Func<object, object?> _getter;
    private readonly Func<object, object, bool>? _directEqualityComparer;

    private PropertyMetadata(
        string name,
        Type propertyType,
        Func<object, object?> getter,
        Func<object, object, bool>? directEqualityComparer)
    {
        Name = name;
        PropertyType = propertyType;
        _getter = getter;
        _directEqualityComparer = directEqualityComparer;
    }

    public static PropertyMetadata Create(PropertyInfo prop)
    {
        var instanceParam = Expression.Parameter(typeof(object), "instance");
        var typedInstance = Expression.Convert(instanceParam, prop.DeclaringType!);
        var propertyAccess = Expression.Property(typedInstance, prop);
        var converted = Expression.Convert(propertyAccess, typeof(object));
        var compiledGetter = Expression.Lambda<Func<object, object?>>(converted, instanceParam)
            .Compile(preferInterpretation: false);

        Func<object, object, bool>? directEqualityComparer = null;

        if (prop.PropertyType.IsValueType || prop.PropertyType == typeof(string))
        {
            var paramA = Expression.Parameter(typeof(object), "a");
            var paramB = Expression.Parameter(typeof(object), "b");

            var typedA = Expression.Convert(paramA, prop.DeclaringType!);
            var typedB = Expression.Convert(paramB, prop.DeclaringType!);

            var propA = Expression.Property(typedA, prop);
            var propB = Expression.Property(typedB, prop);

            var comparerType = typeof(EqualityComparer<>).MakeGenericType(prop.PropertyType);
            var defaultProperty = comparerType.GetProperty(nameof(EqualityComparer<object>.Default),
                BindingFlags.Public | BindingFlags.Static)!;
            var equalsMethod = comparerType.GetMethod(nameof(EqualityComparer<object>.Equals),
                [prop.PropertyType, prop.PropertyType])!;

            var defaultComparerAccess = Expression.Property(null, defaultProperty);
            var equalsCall = Expression.Call(defaultComparerAccess, equalsMethod, propA, propB);

            directEqualityComparer = Expression.Lambda<Func<object, object, bool>>(equalsCall, paramA, paramB)
                .Compile(preferInterpretation: false);
        }

        return new PropertyMetadata(prop.Name, prop.PropertyType, compiledGetter, directEqualityComparer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public object? GetValueFast(object instance) => _getter(instance);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryCompareUnboxed(object instanceA, object instanceB, EquivalencyContext ctx, out bool? result)
    {
        if (_directEqualityComparer is not null &&
            !(ctx.Options.IgnoreTypeDifference && (PropertyType == typeof(double) || PropertyType == typeof(float))))
        {
            if (instanceA.GetType() != instanceB.GetType())
            {
                result = null;
                return false;
            }

            // Skip direct comparison for strings when case-insensitive — let TryFastPath handle it
            if (PropertyType == typeof(string) && ctx.Options.IgnoringCase)
            {
                result = null;
                return false;
            }

            result = _directEqualityComparer(instanceA, instanceB);
            return true;
        }

        result = null;
        return false;
    }
}
