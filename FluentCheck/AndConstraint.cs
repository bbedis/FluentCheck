namespace FluentCheck;

public readonly struct AndConstraint<TAssertions> where TAssertions : struct
{
    public TAssertions Assertions { get; }

    internal AndConstraint(TAssertions assertions) => Assertions = assertions;

    public AndConstraint<TAssertions> And => this;
}
