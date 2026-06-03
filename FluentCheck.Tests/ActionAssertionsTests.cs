namespace FluentCheck.Tests;

public class ActionAssertionsTests
{
    [Fact] public void Throw_correctType()
    {
        Action act = () => { throw new InvalidOperationException("bad"); };
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact] public void Throw_wrongType()
    {
        Action act = () => { throw new ArgumentException(); };
        Assert.Throws<AssertionFailedException>(() => act.Should().Throw<InvalidOperationException>());
    }

    [Fact] public void Throw_noException()
    {
        Action act = () => { };
        Assert.Throws<AssertionFailedException>(() => act.Should().Throw<InvalidOperationException>());
    }

    [Fact] public void ThrowExactly_exact()
    {
        Action act = () => { throw new InvalidOperationException("bad"); };
        act.Should().ThrowExactly<InvalidOperationException>();
    }

    [Fact] public void ThrowExactly_inherited()
    {
        Action act = () => { throw new Exception("base"); };
        Assert.Throws<AssertionFailedException>(() => act.Should().ThrowExactly<InvalidOperationException>());
    }

    [Fact] public async Task ThrowAsync_correctType()
    {
        Func<Task> act = async () => { throw new InvalidOperationException("bad"); };
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact] public async Task ThrowAsync_noException()
    {
        Func<Task> act = async () => { };
        await Assert.ThrowsAsync<AssertionFailedException>(async () => await act.Should().ThrowAsync<InvalidOperationException>());
    }

    [Fact] public async Task ThrowAsyncExactly_exact()
    {
        Func<Task> act = async () => { throw new InvalidOperationException("bad"); };
        await act.Should().ThrowAsyncExactly<InvalidOperationException>();
    }

    [Fact]
    public void WithMessage_match()
    {
        Action act = () => { throw new ArgumentException("user not found"); };
        act.Should().Throw<ArgumentException>().WithMessage("*not found*");
    }

    [Fact]
    public void WithMessage_containing()
    {
        Action act = () => { throw new ArgumentException("user not found"); };
        act.Should().Throw<ArgumentException>().WithMessageContaining("not found");
    }

    [Fact]
    public void WithInnerException_found()
    {
        var inner = new InvalidOperationException("inner");
        Action act = () => throw new ArgumentException("outer", inner);
        act.Should().Throw<ArgumentException>().WithInnerException<InvalidOperationException>();
    }

    [Fact]
    public void WithInnerException_wrongType()
    {
        var inner = new ArgumentNullException();
        Action act = () => throw new ArgumentException("outer", inner);
        Assert.Throws<AssertionFailedException>(() =>
            act.Should().Throw<ArgumentException>().WithInnerException<InvalidOperationException>());
    }

    [Fact]
    public void WithParameterName_match()
    {
        Action act = () => throw new ArgumentException("msg", "myParam");
        act.Should().Throw<ArgumentException>().WithParameterName("myParam");
    }

    [Fact]
    public void WithParameterName_wrong()
    {
        Action act = () => throw new ArgumentException("msg", "myParam");
        Assert.Throws<AssertionFailedException>(() =>
            act.Should().Throw<ArgumentException>().WithParameterName("otherParam"));
    }

    [Fact]
    public void WithParameterName_nonArgException()
    {
        Action act = () => throw new InvalidOperationException("msg");
        Assert.Throws<AssertionFailedException>(() =>
            act.Should().Throw<ArgumentException>().WithParameterName("myParam"));
    }

    [Fact]
    public void Where_match()
    {
        Action act = () => throw new ArgumentException("msg");
        act.Should().Throw<ArgumentException>().Where(ex => ex.Message == "msg");
    }

    [Fact]
    public void Where_noMatch()
    {
        Action act = () => throw new ArgumentException("msg");
        Assert.Throws<AssertionFailedException>(() =>
            act.Should().Throw<ArgumentException>().Where(ex => ex.Message == "other"));
    }

    [Fact]
    public void ThrowAndWhich_chain()
    {
        Action act = () => throw new ArgumentException("msg", "param");
        var caught = act.Should().Throw<ArgumentException>();
        caught.WithParameterName("param");
    }

    [Fact]
    public void ThrowAndWhich_Subject()
    {
        Action act = () => throw new ArgumentException("msg", "param");
        var caught = act.Should().Throw<ArgumentException>();
        Assert.Equal("msg (Parameter 'param')", caught.Subject!.Message);
        Assert.Equal("param", caught.Subject!.ParamName);
    }
}
