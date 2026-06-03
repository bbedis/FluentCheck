namespace FluentCheck;

public readonly struct ActionAssertions
{
    private readonly Action _subject;
    private readonly string _expr;
    internal ActionAssertions(Action subject, string expr) { _subject = subject; _expr = expr; }

    public ExceptionAssertions<T> Throw<T>() where T : Exception
    {
        try { _subject(); }
        catch (T ex) { return new ExceptionAssertions<T>(ex, _expr); }
        catch (Exception ex)
        {
            Fail.With($"Expected {_expr} to throw {typeof(T).Name}, but threw {ex.GetType().Name}.");
        }
        Fail.With($"Expected {_expr} to throw {typeof(T).Name}, but no exception was thrown.");
        return new ExceptionAssertions<T>(null!, _expr);
    }

    public ExceptionAssertions<T> ThrowExactly<T>() where T : Exception
    {
        try { _subject(); }
        catch (T ex)
        {
            if (ex.GetType() != typeof(T))
                Fail.With($"Expected {_expr} to throw exactly {typeof(T).Name}, but threw {ex.GetType().Name}.");
            return new ExceptionAssertions<T>(ex, _expr);
        }
        catch (Exception ex)
        {
            Fail.With($"Expected {_expr} to throw exactly {typeof(T).Name}, but threw {ex.GetType().Name}.");
        }
        Fail.With($"Expected {_expr} to throw exactly {typeof(T).Name}, but no exception was thrown.");
        return new ExceptionAssertions<T>(null!, _expr);
    }
}

public readonly struct FuncTaskAssertions
{
    private readonly Func<Task> _subject;
    private readonly string _expr;
    internal FuncTaskAssertions(Func<Task> subject, string expr) { _subject = subject; _expr = expr; }

    public async Task<ExceptionAssertions<T>> ThrowAsync<T>() where T : Exception
    {
        try { await _subject(); }
        catch (T ex) { return new ExceptionAssertions<T>(ex, _expr); }
        catch (Exception ex)
        {
            Fail.With($"Expected {_expr} to throw {typeof(T).Name}, but threw {ex.GetType().Name}.");
        }
        Fail.With($"Expected {_expr} to throw {typeof(T).Name}, but no exception was thrown.");
        return new ExceptionAssertions<T>(null!, _expr);
    }

    public async Task<ExceptionAssertions<T>> ThrowAsyncExactly<T>() where T : Exception
    {
        try { await _subject(); }
        catch (T ex)
        {
            if (ex.GetType() != typeof(T))
                Fail.With($"Expected {_expr} to throw exactly {typeof(T).Name}, but threw {ex.GetType().Name}.");
            return new ExceptionAssertions<T>(ex, _expr);
        }
        catch (Exception ex)
        {
            Fail.With($"Expected {_expr} to throw exactly {typeof(T).Name}, but threw {ex.GetType().Name}.");
        }
        Fail.With($"Expected {_expr} to throw exactly {typeof(T).Name}, but no exception was thrown.");
        return new ExceptionAssertions<T>(null!, _expr);
    }
}

public readonly struct ExceptionAssertions<T> where T : Exception
{
    private readonly T? _subject;
    private readonly string _expr;
    internal ExceptionAssertions(T? subject, string expr) { _subject = subject; _expr = expr; }

    public Assertion<T?> And => new(_subject, _expr);

    public ExceptionAssertions<T> WithMessage(string wildcardPattern)
    {
        if (_subject is null)
            Fail.With($"Expected exception message to match \"{wildcardPattern}\", but exception was null.");
        else
        {
            var regex = "^" + System.Text.RegularExpressions.Regex.Escape(wildcardPattern)
                .Replace("\\*", ".*").Replace("\\?", ".") + "$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(_subject.Message, regex))
                Fail.With($"Expected exception message to match \"{wildcardPattern}\", but found \"{_subject.Message}\".");
        }
        return this;
    }

    public ExceptionAssertions<T> WithInnerException<TInner>() where TInner : Exception
    {
        if (_subject is null)
            Fail.With($"Expected inner exception of type {typeof(TInner).Name}, but exception was null.");
        else if (_subject.InnerException is null)
            Fail.With($"Expected inner exception of type {typeof(TInner).Name}, but no inner exception was present.");
        else if (_subject.InnerException.GetType() != typeof(TInner))
            Fail.With($"Expected inner exception of type {typeof(TInner).Name}, but found {(_subject.InnerException.GetType()).Name}.");
        return this;
    }

    public ExceptionAssertions<T> WithInnerException<TInner>(string expectedMessage) where TInner : Exception
    {
        if (_subject is null)
            Fail.With($"Expected inner exception of type {typeof(TInner).Name} with message \"{expectedMessage}\", but exception was null.");
        else if (_subject.InnerException is null)
            Fail.With($"Expected inner exception of type {typeof(TInner).Name} with message \"{expectedMessage}\", but no inner exception was present.");
        else if (_subject.InnerException.GetType() != typeof(TInner))
            Fail.With($"Expected inner exception of type {typeof(TInner).Name}, but found {(_subject.InnerException.GetType()).Name}.");
        else
        {
            var regex = "^" + System.Text.RegularExpressions.Regex.Escape(expectedMessage)
                .Replace("\\*", ".*").Replace("\\?", ".") + "$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(_subject.InnerException.Message, regex))
                Fail.With($"Expected inner exception message to match \"{expectedMessage}\", but found \"{_subject.InnerException.Message}\".");
        }
        return this;
    }

    public ExceptionAssertions<T> WithParameterName(string expectedName)
    {
        if (_subject is null)
            Fail.With($"Expected parameter name \"{expectedName}\", but exception was null.");
        else if (_subject is ArgumentException arg && arg.ParamName != expectedName)
            Fail.With($"Expected parameter name \"{expectedName}\", but found \"{arg.ParamName}\".");
        else if (_subject is not ArgumentException)
            Fail.With($"Expected parameter name \"{expectedName}\", but exception was not an ArgumentException.");
        return this;
    }

    public ExceptionAssertions<T> Where(Func<T, bool> predicate)
    {
        if (_subject is null)
            Fail.With($"Expected exception to match predicate, but exception was null.");
        else if (!predicate(_subject))
            Fail.With($"Expected exception to match predicate, but it did not.");
        return this;
    }

    public ExceptionAssertions<T> WithMessageContaining(string expectedSubstr)
    {
        if (_subject is null)
            Fail.With($"Expected exception message to contain \"{expectedSubstr}\", but exception was null.");
        else if (!_subject.Message.Contains(expectedSubstr))
            Fail.With($"Expected exception message to contain \"{expectedSubstr}\", but found \"{_subject.Message}\".");
        return this;
    }

    public T? Subject => _subject;
}
