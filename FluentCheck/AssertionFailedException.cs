namespace FluentCheck;

public sealed class AssertionFailedException : Exception
{
    public string? Expected { get; init; }
    public string? Actual { get; init; }
    public string? Expression { get; init; }

    public AssertionFailedException(string message) : base(message) { }
}