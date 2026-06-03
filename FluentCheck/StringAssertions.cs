using System.Text.RegularExpressions;

namespace FluentCheck;

public readonly struct StringAssertions
{
    private readonly string? _subject;
    private readonly string _expr;
    internal StringAssertions(string? subject, string expr) { _subject = subject; _expr = expr; }

    public Assertion<string?> And => new(_subject, _expr);

    public void Be(string expected)
    {
        if (!string.Equals(_subject, expected, StringComparison.Ordinal))
            Fail.With($"Expected {_expr} to be \"{expected}\", but found \"{_subject}\".");
    }

    public void Contain(string substring)
    {
        if (_subject is null || !_subject.AsSpan().Contains(substring.AsSpan(), StringComparison.Ordinal))
            Fail.With($"Expected {_expr} to contain \"{substring}\", but found \"{_subject}\".");
    }

    public void StartWith(string prefix)
    {
        if (_subject is null || !_subject.AsSpan().StartsWith(prefix.AsSpan(), StringComparison.Ordinal))
            Fail.With($"Expected {_expr} to start with \"{prefix}\", but found \"{_subject}\".");
    }

    public void MatchRegex(string pattern)
    {
        if (_subject is null || !System.Text.RegularExpressions.Regex.IsMatch(_subject, pattern))
            Fail.With($"Expected {_expr} to match regex /{pattern}/, but found \"{_subject}\".");
    }
    
    public void BeNullOrEmpty()
    {
        if (!string.IsNullOrEmpty(_subject))
            Fail.With($"Expected {_expr} to be null or empty, but found \"{_subject}\".");
    }

    // --- Negation methods ---

    public void NotBe(string unexpected, string because = "", params ReadOnlySpan<object?> becauseArgs)
    {
        if (string.Equals(_subject, unexpected, StringComparison.Ordinal))
            Fail.With($"Expected {_expr} not to be \"{unexpected}\".{FormatBecause(because, becauseArgs)}");
    }

    public void NotBe(string unexpected, StringComparison comparison)
    {
        if (string.Equals(_subject, unexpected, comparison))
            Fail.With($"Expected {_expr} not to be \"{unexpected}\" (comparison: {comparison}).");
    }

    public void NotBeNullOrEmpty()
    {
        if (string.IsNullOrEmpty(_subject))
            Fail.With($"Expected {_expr} not to be null or empty, but found \"{_subject}\".");
    }

    public void NotBeNullOrWhiteSpace()
    {
        if (string.IsNullOrWhiteSpace(_subject))
            Fail.With($"Expected {_expr} not to be null or whitespace, but found \"{_subject}\".");
    }

    public void NotContain(string substring)
    {
        if (_subject is not null && _subject.AsSpan().Contains(substring.AsSpan(), StringComparison.Ordinal))
            Fail.With($"Expected {_expr} not to contain \"{substring}\", but it did.");
    }

    public void NotStartWith(string prefix)
    {
        if (_subject is not null && _subject.AsSpan().StartsWith(prefix.AsSpan(), StringComparison.Ordinal))
            Fail.With($"Expected {_expr} not to start with \"{prefix}\", but it did.");
    }

    public void NotEndWith(string suffix)
    {
        if (_subject is not null && _subject.AsSpan().EndsWith(suffix.AsSpan(), StringComparison.Ordinal))
            Fail.With($"Expected {_expr} not to end with \"{suffix}\", but it did.");
    }

    public void NotMatchRegex(string pattern)
    {
        if (_subject is not null && System.Text.RegularExpressions.Regex.IsMatch(_subject, pattern))
            Fail.With($"Expected {_expr} not to match regex /{pattern}/, but it did.");
    }

    public void BeOneOf(params string[] values)
    {
        if (values == null || values.Length == 0)
            Fail.With($"Expected {_expr} to be one of the provided values, but the values array was empty.");
        bool found = false;
        foreach (var v in values!)
        {
            if (string.Equals(_subject, v, StringComparison.Ordinal))
            {
                found = true;
                break;
            }
        }
        if (!found)
            Fail.With($"Expected {_expr} to be one of {string.Join(", ", values)}, but found \"{_subject}\".");
    }

    public void ContainAll(params string[] substrings)
    {
        if (_subject is null)
        {
            Fail.With($"Expected {_expr} to contain all of {string.Join(", ", substrings)}, but the subject was null.");
            return;
        }
        foreach (var s in substrings)
        {
            if (!_subject.AsSpan().Contains(s.AsSpan(), StringComparison.Ordinal))
                Fail.With($"Expected {_expr} to contain all of {string.Join(", ", substrings)}, but missing \"{s}\".");
        }
    }

    public void ContainAny(params string[] substrings)
    {
        if (_subject is null)
        {
            Fail.With($"Expected {_expr} to contain any of {string.Join(", ", substrings)}, but the subject was null.");
            return;
        }
        bool found = false;
        foreach (var s in substrings)
        {
            if (_subject.AsSpan().Contains(s.AsSpan(), StringComparison.Ordinal))
            {
                found = true;
                break;
            }
        }
        if (!found)
            Fail.With($"Expected {_expr} to contain any of {string.Join(", ", substrings)}, but found none.");
    }

    public void EndWith(string suffix)
    {
        if (_subject is null || !_subject.AsSpan().EndsWith(suffix.AsSpan(), StringComparison.Ordinal))
            Fail.With($"Expected {_expr} to end with \"{suffix}\", but found \"{_subject}\".");
    }

    public void BeLowerCased()
    {
        if (_subject is null)
        {
            Fail.With($"Expected {_expr} to be lower cased, but it was null.");
            return;
        }
        for (int i = 0; i < _subject.Length; i++)
        {
            if (char.IsLetter(_subject[i]) && _subject[i] != char.ToLower(_subject[i]))
            {
                Fail.With($"Expected {_expr} to be lower cased, but found \"{_subject}\".");
                return;
            }
        }
    }

    public void BeUpperCased()
    {
        if (_subject is null)
        {
            Fail.With($"Expected {_expr} to be upper cased, but it was null.");
            return;
        }
        for (int i = 0; i < _subject.Length; i++)
        {
            if (char.IsLetter(_subject[i]) && _subject[i] != char.ToUpper(_subject[i]))
            {
                Fail.With($"Expected {_expr} to be upper cased, but found \"{_subject}\".");
                return;
            }
        }
    }

    public void HaveLength(int expected)
    {
        if (_subject is null || _subject.Length != expected)
            Fail.With($"Expected {_expr} to have length {expected}, but found {(_subject?.Length ?? 0)}.");
    }

    public void Be(string expected, StringComparison comparison)
    {
        if (!string.Equals(_subject, expected, comparison))
            Fail.With($"Expected {_expr} to be \"{expected}\" (comparison: {comparison}), but found \"{_subject}\".");
    }

    // --- Case-insensitive equivalents ---

    public void BeEquivalentOf(string expected, StringComparison comparison = default)
    {
        if (!string.Equals(_subject, expected, comparison))
            Fail.With($"Expected {_expr} to be \"{expected}\" (comparison: {comparison}), but found \"{_subject}\".");
    }

    public void ContainEquivalentOf(string substring, StringComparison comparison = default)
    {
        if (_subject is null || !_subject.AsSpan().Contains(substring.AsSpan(), comparison))
            Fail.With($"Expected {_expr} to contain \"{substring}\" (comparison: {comparison}), but found \"{_subject}\".");
    }

    public void StartWithEquivalentOf(string prefix, StringComparison comparison = default)
    {
        if (_subject is null || !_subject.AsSpan().StartsWith(prefix.AsSpan(), comparison))
            Fail.With($"Expected {_expr} to start with \"{prefix}\" (comparison: {comparison}), but found \"{_subject}\".");
    }

    public void EndWithEquivalentOf(string suffix, StringComparison comparison = default)
    {
        if (_subject is null || !_subject.AsSpan().EndsWith(suffix.AsSpan(), comparison))
            Fail.With($"Expected {_expr} to end with \"{suffix}\" (comparison: {comparison}), but found \"{_subject}\".");
    }

    public void MatchEquivalentOf(string pattern, RegexOptions options = default)
    {
        if (_subject is null || !System.Text.RegularExpressions.Regex.IsMatch(_subject, pattern, options))
            Fail.With($"Expected {_expr} to match regex /{pattern}/ (options: {options}), but found \"{_subject}\".");
    }

    // --- Contain with OccurrenceConstraint ---

    public void Contain(string substring, OccurrenceConstraint occurrence)
    {
        if (_subject is null)
        {
            Fail.With($"Expected {_expr} to contain \"{substring}\" {occurrence.Description}, but the subject was null.");
            return;
        }
        int count = 0;
        int index = 0;
        while ((index = _subject.IndexOf(substring, index, StringComparison.Ordinal)) != -1)
        {
            count++;
            index += substring.Length;
        }
        if (!occurrence.Matches(count))
            Fail.With($"Expected {_expr} to contain \"{substring}\" {occurrence.Description}, but found it {count} time{(count == 1 ? "" : "s")}.");
    }

    public void NotContainEquivalentOf(string substring, StringComparison comparison = default)
    {
        if (_subject is not null && _subject.AsSpan().Contains(substring.AsSpan(), comparison))
            Fail.With($"Expected {_expr} not to contain \"{substring}\" (comparison: {comparison}), but it did.");
    }

    public void NotMatchEquivalentOf(string pattern, RegexOptions options = default)
    {
        if (_subject is not null && System.Text.RegularExpressions.Regex.IsMatch(_subject, pattern, options))
            Fail.With($"Expected {_expr} not to match regex /{pattern}/ (options: {options}), but it did.");
    }

    private static string FormatBecause(string because, ReadOnlySpan<object?> args)
        => string.IsNullOrEmpty(because) ? "" : " Because " + string.Format(because, args.ToArray());
}

/// <summary>
/// Constraint for how many times a substring should appear in a string.
/// </summary>
public readonly struct OccurrenceConstraint
{
    private readonly string _description;
    private readonly Func<int, bool> _matches;

    private OccurrenceConstraint(string description, Func<int, bool> matches)
    {
        _description = description;
        _matches = matches;
    }

    /// <summary>
    /// Expect exactly N occurrences.
    /// </summary>
    public static OccurrenceConstraint Exactly(int count)
        => new($"exactly {count} time{(count == 1 ? "" : "s")}", c => c == count);

    /// <summary>
    /// Expect at least N occurrences.
    /// </summary>
    public static OccurrenceConstraint AtLeast(int count)
        => new($"at least {count} time{(count == 1 ? "" : "s")}", c => c >= count);

    /// <summary>
    /// Expect at most N occurrences.
    /// </summary>
    public static OccurrenceConstraint AtMost(int count)
        => new($"at most {count} time{(count == 1 ? "" : "s")}", c => c <= count);

    /// <summary>
    /// Expect exactly once.
    /// </summary>
    public static OccurrenceConstraint Once => Exactly(1);

    /// <summary>
    /// Expect exactly twice.
    /// </summary>
    public static OccurrenceConstraint Twice => Exactly(2);

    /// <summary>
    /// Expect exactly three times.
    /// </summary>
    public static OccurrenceConstraint Thrice => Exactly(3);

    internal string Description => _description;
    internal bool Matches(int actual) => _matches(actual);
}