namespace FluentCheck;

public sealed class AssertionScope : IDisposable
{
    // AsyncLocal ensures scopes work correctly across async/await state machines
    private static readonly AsyncLocal<AssertionScope?> _current = new();
    private readonly AssertionScope? _parent;
    private readonly List<string> _failures = [];

    public AssertionScope()
    {
        _parent = _current.Value;
        _current.Value = this;
    }

    internal static AssertionScope? Current => _current.Value;
    internal void AddFailure(string message) => _failures.Add(message);

    public void Dispose()
    {
        _current.Value = _parent;
        if (_failures.Count == 0) return;

        var aggregated = string.Join(Environment.NewLine + Environment.NewLine, 
            _failures.Select((f, i) => $"[{i + 1}] {f}"));

        throw new AssertionFailedException(
            $"Multiple assertion failures ({_failures.Count}):{Environment.NewLine}{aggregated}");
    }
}