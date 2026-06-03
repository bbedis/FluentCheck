namespace FluentCheck;

public readonly struct StringCollectionAssertions
{
    private readonly string[]? _subject;
    private readonly string _expr;

    internal StringCollectionAssertions(IEnumerable<string>? subject, string expr)
    {
        _expr = expr;
        _subject = subject?.ToArray();
    }

    // --- Base collection methods ---

    public string[] And => _subject ?? Array.Empty<string>();

    public void Contain(string value)
    {
        if (_subject == null || !_subject.Contains(value))
            Fail.With($"Expected {_expr} to contain \"{value}\", but it does not.");
    }

    public void NotContain(string value)
    {
        if (_subject != null && _subject.Contains(value))
            Fail.With($"Expected {_expr} not to contain \"{value}\", but it does.");
    }

    public void HaveCount(int expected)
    {
        if ((_subject?.Length) != expected)
            Fail.With($"Expected {_expr} to have {expected} element(s), but found {( _subject?.Length ?? 0)}.");
    }

    public void NotHaveCount(int unexpected)
    {
        if ((_subject?.Length) == unexpected)
            Fail.With($"Expected {_expr} not to have {unexpected} element(s), but it did.");
    }

    public void ContainSingle()
    {
        if ((_subject?.Length) != 1)
            Fail.With($"Expected {_expr} to contain a single element, but found {( _subject?.Length ?? 0)}.");
    }

    public void ContainSingle(string value)
    {
        var count = _subject?.Count(x => x == value) ?? 0;
        if (count != 1)
            Fail.With($"Expected {_expr} to contain exactly one \"{value}\", but found {count}.");
    }

    public void NotContain(IEnumerable<string> values)
    {
        var valuesArray = values as string[] ?? values.ToArray();
        if (valuesArray.Length == 0) return;
        foreach (var v in valuesArray)
        {
            if (_subject != null && _subject.Contains(v))
                Fail.With($"Expected {_expr} not to contain \"{v}\", but it does.");
        }
    }

    public void StartWith(string prefix)
    {
        if (_subject == null || _subject.Length == 0 || !_subject[0].StartsWith(prefix))
            Fail.With($"Expected {_expr} to start with \"{prefix}\", but found {( _subject == null || _subject.Length == 0 ? "<empty>" : $"\"{_subject[0]}\"" )}.");
    }

    public void EndWith(string suffix)
    {
        if (_subject == null || _subject.Length == 0 || !_subject[_subject.Length - 1].EndsWith(suffix))
            Fail.With($"Expected {_expr} to end with \"{suffix}\", but found {( _subject == null || _subject.Length == 0 ? "<empty>" : $"\"{_subject[_subject.Length - 1]}\"" )}.");
    }

    public void NotStartWith(string unexpectedPrefix)
    {
        if (_subject != null && _subject.Length > 0 && _subject[0].StartsWith(unexpectedPrefix))
            Fail.With($"Expected {_expr} not to start with \"{unexpectedPrefix}\", but it does.");
    }

    public void NotEndWith(string unexpectedSuffix)
    {
        if (_subject != null && _subject.Length > 0 && _subject[_subject.Length - 1].EndsWith(unexpectedSuffix))
            Fail.With($"Expected {_expr} not to end with \"{unexpectedSuffix}\", but it does.");
    }

    public void HaveElement(string expected, int index)
    {
        if (_subject == null || index < 0 || index >= _subject.Length)
            Fail.With($"Expected {_expr}[{index}] to be \"{expected}\", but collection has {( _subject?.Length ?? 0)} elements.");
        if (_subject![index] != expected)
            Fail.With($"Expected {_expr}[{index}] to be \"{expected}\", but found \"{_subject[index]}\".");
    }

    public void HaveElement(Func<string, bool> predicate)
    {
        if (_subject == null || !_subject.Any(predicate))
            Fail.With($"Expected {_expr} to contain an element matching the predicate, but no match was found.");
    }

    public void Satisfy(Func<string, bool> predicate)
    {
        HaveElement(predicate);
    }

    public void HaveElements(params string[] expected)
    {
        if (_subject?.Length != expected.Length)
            Fail.With($"Expected {_expr} to have elements [{string.Join(", ", expected)}], but found [{string.Join(", ", _subject ?? Array.Empty<string>())}].");
        foreach (var e in expected)
        {
            if (!_subject.Contains(e))
                Fail.With($"Expected {_expr} to contain \"{e}\", but it does not.");
        }
    }

    public void HaveElements(IEnumerable<string> expected) => HaveElements(expected.ToArray());

    public void HaveSameCount<T>(IEnumerable<T> expected)
    {
        var expectedCount = expected as ICollection<T> ?? expected.ToArray();
        if ((_subject?.Length) != expectedCount.Count)
            Fail.With($"Expected {_expr} to have the same count as {expectedCount.Count}, but found {( _subject?.Length ?? 0)}.");
    }

    // --- String-specific ordering methods ---

    public void ContainInOrder(params string[] values)
    {
        if (_subject == null || values.Length == 0) return;
        int lastIdx = -1;
        for (int v = 0; v < values.Length; v++)
        {
            int found = -1;
            for (int i = lastIdx + 1; i < _subject.Length; i++)
            {
                if (_subject[i] == values[v]) { found = i; break; }
            }
            if (found == -1)
                Fail.With($"Expected {_expr} to contain \"{values[v]}\" in order, but it was not found.");
            lastIdx = found;
        }
    }

    public void ContainInOrder(IEnumerable<string> values) => ContainInOrder(values.ToArray());

    public void ContainInConsecutiveOrder(params string[] values)
    {
        if (_subject == null || values.Length == 0 || values.Length > _subject.Length) return;
        for (int i = 0; i <= _subject.Length - values.Length; i++)
        {
            bool match = true;
            for (int j = 0; j < values.Length; j++)
            {
                if (_subject[i + j] != values[j]) { match = false; break; }
            }
            if (match) return;
        }
        Fail.With($"Expected {_expr} to contain consecutive sequence [{string.Join(", ", values)}], but it was not found.");
    }

    public void ContainInConsecutiveOrder(IEnumerable<string> values) => ContainInConsecutiveOrder(values.ToArray());

    public void BeInOrder(Comparison<string>? comparison = null)
    {
        if (_subject == null) return;
        var cmp = comparison ?? string.Compare;
        for (int i = 1; i < _subject.Length; i++)
        {
            if (cmp(_subject[i - 1], _subject[i]) > 0)
                Fail.With($"Expected {_expr} to be in ascending order, but element at index {i} is greater than element at index {i - 1}.");
        }
    }

    public void BeInExactOrder(params string[] expected)
    {
        if (_subject == null || _subject.Length != expected.Length)
            Fail.With($"Expected {_expr} to have {expected.Length} elements in exact order, but found {(_subject?.Length ?? 0)}.");
        for (int i = 0; i < _subject!.Length; i++)
        {
            if (_subject[i] != expected[i])
                Fail.With($"Expected {_expr}[{i}] to be \"{expected[i]}\", but found \"{_subject[i]}\".");
        }
    }

    public void BeInExactOrder(IEnumerable<string> expected) => BeInExactOrder(expected.ToArray());

    public void ContainAll(params string[] values)
    {
        if (_subject == null)
            Fail.With($"Expected {_expr} to contain all of [{string.Join(", ", values)}], but collection is null.");
        foreach (var v in values)
        {
            if (!_subject.Contains(v))
                Fail.With($"Expected {_expr} to contain \"{v}\", but it does not.");
        }
    }

    public void ContainAny(params string[] values)
    {
        if (_subject == null || _subject.Length == 0)
            Fail.With($"Expected {_expr} to contain any of [{string.Join(", ", values)}], but collection is empty.");
        foreach (var v in values)
        {
            if (_subject.Contains(v)) return;
        }
        Fail.With($"Expected {_expr} to contain any of [{string.Join(", ", values)}], but none were found.");
    }

    public void BeEmpty()
    {
        if (_subject == null || _subject.Length != 0)
            Fail.With($"Expected {_expr} to be empty, but found {(_subject?.Length ?? -1)} element(s).");
    }

    public void NotBeEmpty()
    {
        if (_subject == null || _subject.Length == 0)
            Fail.With($"Expected {_expr} not to be empty, but it was.");
    }

    // --- Negation variants ---

    public void NotContainInOrder(params string[] values)
    {
        if (_subject == null || values.Length == 0) return;
        int lastIdx = -1;
        foreach (var v in values)
        {
            int found = -1;
            for (int i = lastIdx + 1; i < _subject.Length; i++)
            {
                if (_subject[i] == v) { found = i; break; }
            }
            if (found == -1) return;
            lastIdx = found;
        }
        Fail.With($"Expected {_expr} not to contain \"{string.Join(", ", values)}\" in order, but it does.");
    }

    public void NotContainInConsecutiveOrder(params string[] values)
    {
        if (_subject == null || values.Length == 0 || values.Length > _subject.Length) return;
        for (int i = 0; i <= _subject.Length - values.Length; i++)
        {
            bool match = true;
            for (int j = 0; j < values.Length; j++)
            {
                if (_subject[i + j] != values[j]) { match = false; break; }
            }
            if (match)
            {
                Fail.With($"Expected {_expr} not to contain consecutive sequence [{string.Join(", ", values)}], but it does.");
                return;
            }
        }
    }

    public void NotContainAll(params string[] values)
    {
        if (_subject == null) return;
        foreach (var v in values)
        {
            if (!_subject.Contains(v)) return;
        }
        Fail.With($"Expected {_expr} not to contain all of [{string.Join(", ", values)}], but it does.");
    }

    public void NotContainAny(params string[] values)
    {
        if (_subject == null || _subject.Length == 0) return;
        foreach (var v in values)
        {
            if (_subject.Contains(v))
                Fail.With($"Expected {_expr} not to contain any of [{string.Join(", ", values)}], but found \"{v}\".");
        }
    }

    public void BeEquivalentTo(IEnumerable<string> expected)
    {
        var expectedArray = expected as string[] ?? expected.ToArray();
        if ((_subject?.Length) != expectedArray.Length)
            Fail.With($"Expected {_expr} to be equivalent to [{string.Join(", ", expectedArray)}], but found [{string.Join(", ", _subject ?? Array.Empty<string>())}].");
        var subjectSet = new HashSet<string>(_subject ?? Array.Empty<string>());
        foreach (var e in expectedArray)
        {
            if (!subjectSet.Remove(e))
                Fail.With($"Expected {_expr} to contain \"{e}\", but it does not.");
        }
    }
}
