namespace FluentCheck;

public readonly struct DictionaryAssertions<TKey, TValue> where TKey : notnull
{
    private readonly IDictionary<TKey, TValue>? _subject;
    private readonly string _expr;
    private readonly bool _hasSubject;

    internal DictionaryAssertions(IDictionary<TKey, TValue>? subject, string expr)
    {
        _expr = expr;
        _subject = subject;
        _hasSubject = subject is not null;
    }

    internal DictionaryAssertions(IReadOnlyDictionary<TKey, TValue>? subject, string expr)
    {
        _expr = expr;
        _subject = subject == null ? null : new Dictionary<TKey, TValue>(subject);
        _hasSubject = _subject != null;
    }

    public Assertion<IDictionary<TKey, TValue>?> And => new(_subject, _expr);

    public void ContainKey(TKey key)
    {
        if (!_hasSubject || !_subject!.ContainsKey(key))
            Fail.With($"Expected {_expr} to contain key {key}, but it did not.");
    }

    public void ContainKeyAndValue(TKey key, TValue value)
    {
        if (!_hasSubject)
        {
            Fail.With($"Expected {_expr} to contain key {key} with value {value}, but the dictionary was <null>.");
            return;
        }

        if (!_subject!.TryGetValue(key, out var val))
        {
            Fail.With($"Expected {_expr} to contain key {key} with value {value}, but the key was not found.");
            return;
        }

        if (!EqualityComparer<TValue>.Default.Equals(val, value))
        {
            Fail.With($"Expected {_expr} to contain key {key} with value {value}, but found {val?.ToString() ?? "<null>"}.");
        }
    }

    // --- Negation methods ---

    public void NotContainKey(TKey key)
    {
        if (_hasSubject && _subject!.ContainsKey(key))
            Fail.With($"Expected {_expr} not to contain key {key}, but it did.");
    }

    public void NotContainValue(TValue value)
    {
        if (!_hasSubject) return;
        foreach (var val in _subject!.Values)
        {
            if (EqualityComparer<TValue>.Default.Equals(val, value))
            {
                Fail.With($"Expected {_expr} not to contain value {value}, but it did.");
                return;
            }
        }
    }

    public void NotContainKeyAndValue(TKey key, TValue value)
    {
        if (!_hasSubject) return;
        if (_subject!.TryGetValue(key, out var val) && EqualityComparer<TValue>.Default.Equals(val, value))
        {
            Fail.With($"Expected {_expr} not to contain key {key} with value {value}, but it did.");
        }
    }

    // --- New methods ---

    public void ContainKeys(params TKey[] keys)
    {
        if (!_hasSubject)
        {
            Fail.With($"Expected {_expr} to contain keys {string.Join(", ", keys)}, but the dictionary was null.");
            return;
        }
        foreach (var k in keys)
        {
            if (!_subject!.ContainsKey(k))
                Fail.With($"Expected {_expr} to contain key {k}, but it did not.");
        }
    }

    public void ContainValues(params TValue[] values)
    {
        if (!_hasSubject)
        {
            Fail.With($"Expected {_expr} to contain values {string.Join(", ", values)}, but the dictionary was null.");
            return;
        }
        foreach (var v in values)
        {
            bool found = false;
            foreach (var val in _subject!.Values)
            {
                if (EqualityComparer<TValue>.Default.Equals(val, v))
                {
                    found = true;
                    break;
                }
            }
            if (!found)
                Fail.With($"Expected {_expr} to contain value {v}, but it did not.");
        }
    }

    public void Contain(TKey key, TValue value) => ContainKeyAndValue(key, value);

    public void Equal(IDictionary<TKey, TValue> expected)
    {
        if (!_hasSubject)
        {
            Fail.With($"Expected {_expr} to be equivalent to {expected}, but the subject was null.");
            return;
        }
        if (_subject!.Count != expected.Count)
            Fail.With($"Expected {_expr} to have count {expected.Count}, but found {_subject.Count}.");
        foreach (var kvp in expected)
        {
            if (!_subject!.TryGetValue(kvp.Key, out var val) || !EqualityComparer<TValue>.Default.Equals(val, kvp.Value))
                Fail.With($"Expected {_expr} to contain key {kvp.Key} with value {kvp.Value}, but found {val?.ToString() ?? "<null>"}.");
        }
    }

    public void HaveSameCount(IDictionary<TKey, TValue> expected)
    {
        if (!_hasSubject)
        {
            Fail.With($"Expected {_expr} to have same count as {expected}, but the subject was null.");
            return;
        }
        if (_subject!.Count != expected.Count)
            Fail.With($"Expected {_expr} to have count {expected.Count}, but found {_subject.Count}.");
    }

    public void NotContain()
    {
        if (_hasSubject && _subject!.Count > 0)
            Fail.With($"Expected {_expr} to be empty, but found {_subject.Count} items.");
    }
}
