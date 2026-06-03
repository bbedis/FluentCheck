namespace FluentCheck;

public readonly struct AndWhichConstraint<TSubject, TAssertions> where TAssertions : struct
{
    public TSubject Subject { get; }
    public TAssertions Assertions { get; }

    internal AndWhichConstraint(TSubject subject, TAssertions assertions)
    {
        Subject = subject;
        Assertions = assertions;
    }

    public AndWhichConstraint<TSubject, TAssertions> And => this;

    public AndWhichConstraint<TSubject, TAssertions> Which => this;
}
