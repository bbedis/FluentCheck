namespace FluentCheck.Equivalency;

/// <summary>
/// Configuration options for deep equivalency comparisons.
/// </summary>
public sealed class EquivalencyOptions
{
    public static EquivalencyOptions Default { get; } = new();

    public int MaxDepth { get; set; } = 10;
    public bool IgnoreTypeDifference { get; set; } = false;
    public bool IncludeFields { get; set; } = false;
    public bool IgnoreCollectionOrder { get; set; } = true;
    public bool ExcludeNonPublic { get; set; } = true;
    public bool ContinueOnFailure { get; set; } = true;
    public bool IgnoringCase { get; set; } = false;
    public bool StrictTyping { get; set; } = false;

    public HashSet<string>? ExcludedProperties { get; set; }
    public HashSet<string>? ExcludedFields { get; set; }
    public Func<string, Type, bool>? PropertyFilter { get; set; }

    public CustomComparerRegistry? CustomComparers { get; set; }

    public static EquivalencyOptions Configure(Action<EquivalencyOptionsBuilder> configure)
    {
        var builder = new EquivalencyOptionsBuilder();
        configure(builder);
        return builder.Build();
    }
}
