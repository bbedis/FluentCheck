using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace FluentCheck.Equivalency;

/// <summary>
/// Cached metadata for a field, using compiled expression trees for fast access.
/// </summary>
internal sealed class FieldMetadata
{
    public string Name { get; }
    public Type FieldType { get; }

    private readonly Func<object, object?> _getter;
    private readonly Func<object, object, bool>? _directEqualityComparer;

    private FieldMetadata(
        string name,
        Type fieldType,
        Func<object, object?> getter,
        Func<object, object, bool>? directEqualityComparer)
    {
        Name = name;
        FieldType = fieldType;
        _getter = getter;
        _directEqualityComparer = directEqualityComparer;
    }

    public static FieldMetadata Create(FieldInfo field)
    {
        var instanceParam = Expression.Parameter(typeof(object), "instance");
        var typedInstance = Expression.Convert(instanceParam, field.DeclaringType!);
        var fieldAccess = Expression.Field(typedInstance, field);
        var converted = Expression.Convert(fieldAccess, typeof(object));
        var compiledGetter = Expression.Lambda<Func<object, object?>>(converted, instanceParam)
            .Compile(preferInterpretation: false);

        Func<object, object, bool>? directEqualityComparer = null;

        if (field.FieldType.IsValueType || field.FieldType == typeof(string))
        {
            var paramA = Expression.Parameter(typeof(object), "a");
            var paramB = Expression.Parameter(typeof(object), "b");

            var typedA = Expression.Convert(paramA, field.DeclaringType!);
            var typedB = Expression.Convert(paramB, field.DeclaringType!);

            var fieldA = Expression.Field(typedA, field);
            var fieldB = Expression.Field(typedB, field);

            var comparerType = typeof(EqualityComparer<>).MakeGenericType(field.FieldType);
            var defaultProperty = comparerType.GetProperty(nameof(EqualityComparer<object>.Default),
                BindingFlags.Public | BindingFlags.Static)!;
            var equalsMethod = comparerType.GetMethod(nameof(EqualityComparer<object>.Equals),
                [field.FieldType, field.FieldType])!;

            var defaultComparerAccess = Expression.Property(null, defaultProperty);
            var equalsCall = Expression.Call(defaultComparerAccess, equalsMethod, fieldA, fieldB);

            directEqualityComparer = Expression.Lambda<Func<object, object, bool>>(equalsCall, paramA, paramB)
                .Compile(preferInterpretation: false);
        }

        return new FieldMetadata(field.Name, field.FieldType, compiledGetter, directEqualityComparer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public object? GetValueFast(object instance) => _getter(instance);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryCompareUnboxed(object instanceA, object instanceB, EquivalencyContext ctx, out bool? result)
    {
        if (_directEqualityComparer is not null &&
            !(ctx.Options.IgnoreTypeDifference && (FieldType == typeof(double) || FieldType == typeof(float))))
        {
            if (instanceA.GetType() != instanceB.GetType())
            {
                result = null;
                return false;
            }

            // Skip direct comparison for strings when case-insensitive — let TryFastPath handle it
            if (FieldType == typeof(string) && ctx.Options.IgnoringCase)
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
