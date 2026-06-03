using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace FluentCheck.Equivalency;

internal sealed class TypeMetadata
{
    public Type Type { get; }
    public PropertyMetadata[] Properties { get; }
    public FieldMetadata[] Fields { get; }

    private readonly Dictionary<string, PropertyMetadata>? _propMap;
    private readonly Dictionary<string, FieldMetadata>? _fieldMap;

    private TypeMetadata(Type type, PropertyMetadata[] properties, FieldMetadata[] fields)
    {
        Type = type;
        Properties = properties;
        Fields = fields;

        if (properties.Length > 0)
            _propMap = properties.ToDictionary(p => p.Name, StringComparer.Ordinal);
        if (fields.Length > 0)
            _fieldMap = fields.ToDictionary(f => f.Name, StringComparer.Ordinal);
    }

    public static TypeMetadata Create(Type type)
    {
        var bindingFlags = BindingFlags.Public | BindingFlags.Instance;

        var properties = type.GetProperties(bindingFlags)
            .Where(p => p.CanRead && p.GetIndexParameters().Length == 0)
            .Select(PropertyMetadata.Create)
            .ToArray();

        var fields = type.GetFields(bindingFlags)
            .Select(FieldMetadata.Create)
            .ToArray();

        return new TypeMetadata(type, properties, fields);
    }

    public bool TryGetValue(string name, [NotNullWhen(true)] out PropertyMetadata? property)
    {
        property = null;
        return _propMap?.TryGetValue(name, out property) == true;
    }

    public bool TryGetField(string name, [NotNullWhen(true)] out FieldMetadata? field)
    {
        field = null;
        return _fieldMap?.TryGetValue(name, out field) == true;
    }
}
