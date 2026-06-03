namespace FluentCheck;

public readonly struct CollectionAssertions<T>
{
    private readonly IEnumerable<T>? _subject;
    private readonly string _expr;
    internal CollectionAssertions(IEnumerable<T>? subject, string expr) { _subject = subject; _expr = expr; }

    public Assertion<IEnumerable<T>?> And => new(_subject, _expr);

    public void BeEmpty()
    {
        if (_subject is null || _subject.Any())
            Fail.With($"Expected {_expr} to be empty, but found {_subject?.Count() ?? 0} items.");
    }

    public void Contain(T expected)
    {
        if (_subject is null || !_subject.Contains(expected))
            Fail.With($"Expected {_expr} to contain {expected}, but it did not.");
    }

    public void HaveCount(int expected)
    {
        var count = _subject?.Count() ?? 0;
        if (count != expected)
            Fail.With($"Expected {_expr} to have count {expected}, but found {count}.");
    }
    
    public void OnlyContain(Func<T, bool> predicate)
    {
        if (_subject is null || !_subject.All(predicate))
            Fail.With($"Expected all items in {_expr} to match predicate, but some did not.");
    }

    // --- Negation methods ---

    public void NotContain(T unexpected)
    {
        if (_subject is not null && _subject.Contains(unexpected))
            Fail.With($"Expected {_expr} not to contain {unexpected}, but it did.");
    }

    public void NotHaveCount(int unexpected)
    {
        var count = _subject?.Count() ?? 0;
        if (count == unexpected)
            Fail.With($"Expected {_expr} not to have count {unexpected}, but it did.");
    }

    public void NotOnlyContain(Func<T, bool> predicate)
    {
        if (_subject is null || !_subject.Any(predicate))
            Fail.With($"Expected some item in {_expr} to not match predicate, but none did.");
        else if (_subject.All(predicate))
            Fail.With($"Expected some item in {_expr} to not match predicate, but all matched.");
    }

    // --- New methods ---

    public void ContainInOrder(params T[] expected)
    {
        if (_subject is null)
        {
            Fail.With($"Expected {_expr} to contain {string.Join(", ", expected)} in order, but the subject was null.");
            return;
        }
        var list = _subject.ToList();
        int expectedIdx = 0;
        int subjectIdx = 0;
        while (expectedIdx < expected.Length && subjectIdx < list.Count)
        {
            if (EqualityComparer<T>.Default.Equals(list[subjectIdx], expected[expectedIdx]))
                expectedIdx++;
            subjectIdx++;
        }
        if (expectedIdx != expected.Length)
            Fail.With($"Expected {_expr} to contain {string.Join(", ", expected)} in order, but it did not.");
    }

    public void ContainInConsecutiveOrder(params T[] expected)
    {
        if (_subject is null)
        {
            Fail.With($"Expected {_expr} to contain {string.Join(", ", expected)} in consecutive order, but the subject was null.");
            return;
        }
        var list = _subject.ToList();
        for (int i = 0; i <= list.Count - expected.Length; i++)
        {
            bool match = true;
            for (int j = 0; j < expected.Length; j++)
            {
                if (!EqualityComparer<T>.Default.Equals(list[i + j], expected[j]))
                {
                    match = false;
                    break;
                }
            }
            if (match) return;
        }
        Fail.With($"Expected {_expr} to contain {string.Join(", ", expected)} in consecutive order, but it did not.");
    }

    public void ContainSingle(Func<T, bool>? predicate = null)
    {
        if (_subject is null)
        {
            Fail.With($"Expected {_expr} to contain a single item{(predicate == null ? "" : " matching predicate")}, but the subject was null.");
            return;
        }
        int count = predicate == null ? _subject.Count() : _subject.Count(predicate);
        if (count != 1)
            Fail.With($"Expected {_expr} to contain a single item{(predicate == null ? "" : " matching predicate")}, but found {count}.");
    }

    public void StartWith(T expected)
    {
        if (_subject is null)
        {
            Fail.With($"Expected {_expr} to start with {expected}, but the subject was null.");
            return;
        }
        var first = _subject.FirstOrDefault();
        if (!EqualityComparer<T>.Default.Equals(first, expected))
            Fail.With($"Expected {_expr} to start with {expected}, but found {first}.");
    }

    public void EndWith(T expected)
    {
        if (_subject is null)
        {
            Fail.With($"Expected {_expr} to end with {expected}, but the subject was null.");
            return;
        }
        var list = _subject!.ToList();
        if (!EqualityComparer<T>.Default.Equals(list[^1], expected))
            Fail.With($"Expected {_expr} to end with {expected}, but found {list[^1]}.");
    }

    public void Satisfy(Func<T, bool> predicate)
    {
        if (_subject is null || !_subject.Any(predicate))
            Fail.With($"Expected {_expr} to satisfy predicate, but no item matched.");
    }

    public void NotContainInOrder(params T[] expected)
    {
        if (_subject is null) return;
        var list = _subject.ToList();
        int expectedIdx = 0;
        int subjectIdx = 0;
        while (expectedIdx < expected.Length && subjectIdx < list.Count)
        {
            if (EqualityComparer<T>.Default.Equals(list[subjectIdx], expected[expectedIdx]))
                expectedIdx++;
            subjectIdx++;
        }
        if (expectedIdx == expected.Length)
            Fail.With($"Expected {_expr} not to contain {string.Join(", ", expected)} in order, but it did.");
    }

    public void NotContainInConsecutiveOrder(params T[] expected)
    {
        if (_subject is null) return;
        var list = _subject.ToList();
        for (int i = 0; i <= list.Count - expected.Length; i++)
        {
            bool match = true;
            for (int j = 0; j < expected.Length; j++)
            {
                if (!EqualityComparer<T>.Default.Equals(list[i + j], expected[j]))
                {
                    match = false;
                    break;
                }
            }
            if (match)
            {
                Fail.With($"Expected {_expr} not to contain {string.Join(", ", expected)} in consecutive order, but it did.");
                return;
            }
        }
    }

    public void NotContainSingle(Func<T, bool>? predicate = null)
    {
        if (_subject is null) return;
        int count = predicate == null ? _subject.Count() : _subject.Count(predicate);
        if (count == 1)
        {
            var detail = predicate == null ? "" : " matching predicate";
            Fail.With($"Expected {_expr} not to contain a single item{detail}, but it did.");
        }
    }

    public void NotStartWith(T unexpected)
    {
        if (_subject is null) return;
        var first = _subject.FirstOrDefault();
        if (EqualityComparer<T>.Default.Equals(first, unexpected))
            Fail.With($"Expected {_expr} not to start with {unexpected}, but it did.");
    }

    public void NotEndWith(T unexpected)
    {
        if (_subject is null) return;
        var list = _subject.ToList();
        if (list.Last().Equals(unexpected))
            Fail.With($"Expected {_expr} not to end with {unexpected}, but it did.");
    }

    public void NotSatisfy(Func<T, bool> predicate)
    {
        if (_subject is not null && _subject.Any(predicate))
            Fail.With($"Expected {_expr} not to satisfy predicate, but some item matched.");
    }

    // --- Collection utility methods ---

    public AndConstraint<CollectionAssertions<T>> OnlyHaveUniqueItems()
    {
        if (_subject is null)
        {
            Fail.With($"Expected {_expr} to only have unique items, but the subject was null.");
            return new(this);
        }
        var seen = new HashSet<T>(_subject);
        if (seen.Count != _subject.Count())
            Fail.With($"Expected {_expr} to only have unique items, but there were duplicates.");
        return new(this);
    }

    public AndConstraint<CollectionAssertions<T>> OnlyHaveUniqueItems<TKey>(Func<T, TKey> keySelector)
    {
        if (_subject is null)
        {
            Fail.With($"Expected {_expr} to only have unique items by key, but the subject was null.");
            return new(this);
        }
        var keys = _subject.Select(keySelector);
        var seen = new HashSet<TKey>(keys);
        if (seen.Count != _subject.Count())
            Fail.With($"Expected {_expr} to only have unique items by key, but there were duplicates.");
        return new(this);
    }

    public AndConstraint<CollectionAssertions<T>> BeSubsetOf(IEnumerable<T> expected)
    {
        if (_subject is null)
        {
            Fail.With($"Expected {_expr} to be a subset of {expected}, but the subject was null.");
            return new(this);
        }
        var expectedSet = new HashSet<T>(expected);
        foreach (var item in _subject)
        {
            if (!expectedSet.Contains(item))
                Fail.With($"Expected {_expr} to be a subset of {expected}, but {item} was not in the expected set.");
        }
        return new(this);
    }

    public void HaveElementPreceding(T successor, T predecessor)
    {
        if (_subject is null)
        {
            Fail.With($"Expected {_expr} to have {predecessor} preceding {successor}, but the subject was null.");
            return;
        }
        var list = _subject.ToList();
        for (int i = 0; i < list.Count - 1; i++)
        {
            if (EqualityComparer<T>.Default.Equals(list[i], predecessor) &&
                EqualityComparer<T>.Default.Equals(list[i + 1], successor))
                return;
        }
        Fail.With($"Expected {_expr} to have {predecessor} preceding {successor}, but it did not.");
    }

    public void HaveElementSucceeding(T predecessor, T successor)
    {
        if (_subject is null)
        {
            Fail.With($"Expected {_expr} to have {successor} succeeding {predecessor}, but the subject was null.");
            return;
        }
        var list = _subject.ToList();
        for (int i = 0; i < list.Count - 1; i++)
        {
            if (EqualityComparer<T>.Default.Equals(list[i], predecessor) &&
                EqualityComparer<T>.Default.Equals(list[i + 1], successor))
                return;
        }
        Fail.With($"Expected {_expr} to have {successor} succeeding {predecessor}, but it did not.");
    }

    public void Equal(IEnumerable<T> expected, IEqualityComparer<T>? comparer = null)
    {
        if (_subject is null)
        {
            Fail.With($"Expected {_expr} to be equal to {expected}, but the subject was null.");
            return;
        }
        var expectedList = expected.ToList();
        var subjectList = _subject.ToList();
        if (subjectList.Count != expectedList.Count)
            Fail.With($"Expected {_expr} to have count {expectedList.Count}, but found {subjectList.Count}.");
        var eq = comparer ?? EqualityComparer<T>.Default;
        for (int i = 0; i < subjectList.Count; i++)
        {
            if (!eq.Equals(subjectList[i], expectedList[i]))
                Fail.With($"Expected {_expr}[{i}] to be {expectedList[i]}, but found {subjectList[i]}.");
        }
    }

    public void NotHaveDuplicates()
    {
        if (_subject is null)
        {
            Fail.With($"Expected {_expr} to not have duplicates, but the subject was null.");
            return;
        }
        var seen = new HashSet<T>(_subject);
        if (seen.Count != _subject.Count())
            Fail.With($"Expected {_expr} to not have duplicates, but there were duplicate items.");
    }
}
