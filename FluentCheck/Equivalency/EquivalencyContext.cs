using System;
using System.Collections.Generic;

namespace FluentCheck.Equivalency;

/// <summary>
/// Tracks the current comparison path and collects failures during deep equivalency checks.
/// Supports checkpoint/rollback for speculative comparisons (e.g., unordered collections).
/// </summary>
public sealed class EquivalencyContext
{
    private readonly List<string> _path = [];
    private readonly List<string> _failures = [];

    public EquivalencyOptions Options { get; }

    public EquivalencyContext(EquivalencyOptions options) => Options = options;

    public PathPopper PushPath(string segment)
    {
        _path.Add(segment);
        return new PathPopper(this);
    }

    public void AddFailure(string message)
    {
        var fullPath = string.Join(".", _path);
        _failures.Add(string.IsNullOrEmpty(fullPath)
            ? message
            : $"{fullPath}: {message}");
    }

    public string? GetAggregatedFailure()
        => _failures.Count == 0 ? null : string.Join(Environment.NewLine, _failures);

    public int FailureCount => _failures.Count;

    public void RollbackFailures(int checkpoint)
    {
        if (_failures.Count > checkpoint)
        {
            _failures.RemoveRange(checkpoint, _failures.Count - checkpoint);
        }
    }

    public readonly ref struct PathPopper
    {
        private readonly EquivalencyContext _ctx;
        public PathPopper(EquivalencyContext ctx) => _ctx = ctx;
        public void Dispose() => _ctx._path.RemoveAt(_ctx._path.Count - 1);
    }
}
