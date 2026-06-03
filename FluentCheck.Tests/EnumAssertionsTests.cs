using System.Globalization;

namespace FluentCheck.Tests;

public class EnumAssertionsTests
{
    private enum Colors { Red = 1, Green = 2, Blue = 4 }

    [Fact]
    public void Be_WhenEqual_Succeeds()
    {
        Colors.Red.Should().Be(Colors.Red);
    }

    [Fact]
    public void Be_WhenNotEqual_Fails()
    {
        Assert.Throws<AssertionFailedException>(() => Colors.Red.Should().Be(Colors.Green));
    }

    [Fact]
    public void NotBe_WhenNotEqual_Succeeds()
    {
        Colors.Red.Should().NotBe(Colors.Green);
    }

    [Fact]
    public void NotBe_WhenEqual_Fails()
    {
        Assert.Throws<AssertionFailedException>(() => Colors.Red.Should().NotBe(Colors.Red));
    }

    [Fact]
    public void BeOneOf_WhenMatchFound_Succeeds()
    {
        Colors.Green.Should().BeOneOf(Colors.Red, Colors.Green, Colors.Blue);
    }

    [Fact]
    public void BeOneOf_WhenNoMatch_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            Colors.Red.Should().BeOneOf(Colors.Green, Colors.Blue));
    }

    [Fact]
    public void HaveFlag_WhenFlagPresent_Succeeds()
    {
        ((Colors.Red | Colors.Green)).Should().HaveFlag(Colors.Red);
    }

    [Fact]
    public void HaveFlag_WhenFlagAbsent_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            Colors.Red.Should().HaveFlag(Colors.Green));
    }

    [Fact]
    public void NotHaveFlag_WhenFlagAbsent_Succeeds()
    {
        Colors.Red.Should().NotHaveFlag(Colors.Green);
    }

    [Fact]
    public void NotHaveFlag_WhenFlagPresent_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            ((Colors.Red | Colors.Green)).Should().NotHaveFlag(Colors.Red));
    }

    [Fact]
    public void HaveValue_WhenMatchingUnderlyingValue_Succeeds()
    {
        ((Colors.Red | Colors.Green)).Should().HaveValue((Colors.Red | Colors.Green));
    }

    [Fact]
    public void HaveValue_WhenMismatchingValue_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            Colors.Red.Should().HaveValue(Colors.Green));
    }

    [Fact]
    public void AssertionScope_CollectsAllFailures()
    {
        var scope = new AssertionScope();
        Colors.Red.Should().Be(Colors.Green);
        Colors.Red.Should().NotBe(Colors.Red);
        var ex = Assert.Throws<AssertionFailedException>(() => scope.Dispose());
        Assert.Contains("Multiple assertion failures (2)", ex.Message);
    }
}
