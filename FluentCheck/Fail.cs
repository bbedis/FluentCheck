using System.Runtime.CompilerServices;

namespace FluentCheck;

internal static class Fail
{
    // The magic: The compiler will only build the string if the assertion fails.
    public static void With([InterpolatedStringHandlerArgument] ref AssertionMessageHandler message)
    {
        var scope = AssertionScope.Current;
        if (scope is not null)
        {
            scope.AddFailure(message.ToStringAndClear());
            return;
        }
        throw new AssertionFailedException(message.ToStringAndClear());
    }
}

[InterpolatedStringHandler]
public ref struct AssertionMessageHandler
{
    private DefaultInterpolatedStringHandler _handler;

    public AssertionMessageHandler(int literalLength, int formattedCount)
    {
        _handler = new DefaultInterpolatedStringHandler(literalLength, formattedCount);
    }

    public void AppendLiteral(string s) => _handler.AppendLiteral(s);
    public void AppendFormatted<T>(T t) => _handler.AppendFormatted(t);
    public void AppendFormatted<T>(T t, string? format) => _handler.AppendFormatted(t, format: format);
    
    public string ToStringAndClear() => _handler.ToStringAndClear();
}