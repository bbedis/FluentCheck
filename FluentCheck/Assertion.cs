using System.Runtime.CompilerServices;

namespace FluentCheck;

public readonly struct Assertion<T>
{
    public T Subject { get; }
    public string Expression { get; }

    internal Assertion(T subject, string expression)
    {
        Subject = subject;
        Expression = expression;
    }

    // Allows chaining: .Should().NotBeNull().And.BeOfType<User>()
    public Assertion<T> And => this; 
    
    // Allows extracting properties: .Should().BeOfType<User>().Which.Name.Should()...
    public T Which => Subject; 
}