using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using FluentCheck.Equivalency;

namespace FluentCheck;

public readonly struct ObjectAssertions
{
    private readonly object? _subject;
    private readonly string _expr;

    internal ObjectAssertions(object? subject, string expr)
    {
        _subject = subject;
        _expr = expr;
    }

    public Assertion<object?> And => new(_subject, _expr);

    public void Be(object? expected, string because = "", params ReadOnlySpan<object?> becauseArgs)
    {
        if (_subject is null && expected is null) return;
        if (_subject is null || expected is null)
        {
            Fail.With($"Expected {_expr} to be {Format(expected)}, but found {Format(_subject)}. {FormatBecause(because, becauseArgs)}");
            return;
        }
        bool equal = DeepEqualityComparer.AreEquivalent(_subject, expected, out _);
        if (!equal)
            Fail.With($"Expected {_expr} to be {Format(expected)}, but found {Format(_subject)}. {FormatBecause(because, becauseArgs)}");
    }

    public void NotBe(object? unexpected, string because = "", params ReadOnlySpan<object?> becauseArgs)
    {
        if (object.Equals(_subject, unexpected))
            Fail.With($"Expected {_expr} not to be {Format(unexpected)}. {FormatBecause(because, becauseArgs)}");
    }

    public void BeNull(string because = "", params ReadOnlySpan<object?> becauseArgs)
    {
        if (_subject is not null)
            Fail.With($"Expected {_expr} to be <null>, but found {Format(_subject)}. {FormatBecause(because, becauseArgs)}");
    }

    public void NotBeNull(string because = "", params ReadOnlySpan<object?> becauseArgs)
    {
        if (_subject is null)
            Fail.With($"Expected {_expr} not to be <null>. {FormatBecause(because, becauseArgs)}");
    }

    public void BeOfType<TExpected>(string because = "", params ReadOnlySpan<object?> becauseArgs)
    {
        if (_subject?.GetType() != typeof(TExpected))
            Fail.With($"Expected {_expr} to be of type {typeof(TExpected).Name}, but found {_subject?.GetType().Name ?? "<null>"}. {FormatBecause(because, becauseArgs)}");
    }

    // --- Deep Equivalence (Reflection-based) ---

    public void BeEquivalentTo(object? expected, Func<EquivalencyOptionsBuilder, EquivalencyOptionsBuilder>? configure = null, string because = "", params ReadOnlySpan<object?> becauseArgs)
    {
        var options = EquivalencyOptions.Configure(builder =>
        {
            configure?.Invoke(builder);
        });

        bool result;
        string? failureMessage;
        try
        {
            result = DeepEqualityComparer.AreEquivalent(_subject, expected, options, out failureMessage);
        }
        catch (Exception ex)
        {
            Fail.With($"Expected {_expr} to be structurally equivalent to {Format(expected)}, but comparison threw: {ex.GetType().Name}: {ex.Message} {FormatBecause(because, becauseArgs)}");
            return;
        }

        if (!result)
        {
            Fail.With($"Expected {_expr} to be structurally equivalent to {Format(expected)}, but they differ.{(string.IsNullOrEmpty(failureMessage) ? "" : "\n" + failureMessage)} {FormatBecause(because, becauseArgs)}");
        }
    }

    public void NotBeEquivalentTo(object? unexpected, Func<EquivalencyOptionsBuilder, EquivalencyOptionsBuilder>? configure = null, string because = "", params ReadOnlySpan<object?> becauseArgs)
    {
        var options = EquivalencyOptions.Configure(builder =>
        {
            configure?.Invoke(builder);
        });

        bool result;
        try
        {
            result = DeepEqualityComparer.AreEquivalent(_subject, unexpected, options, out _);
        }
        catch
        {
            result = false;
        }

        if (result)
            Fail.With($"Expected {_expr} not to be structurally equivalent to {Format(unexpected)}, but they were.{FormatBecause(because, becauseArgs)}");
    }

    private static string Format(object? value) => value switch
    {
        null => "<null>",
        string s => $"\"{s}\"",
        _ => value.ToString() ?? "<null>"
    };

    private static string FormatBecause(string because, ReadOnlySpan<object?> args)
        => string.IsNullOrEmpty(because) ? "" : " Because " + string.Format(because, args.ToArray());
}
