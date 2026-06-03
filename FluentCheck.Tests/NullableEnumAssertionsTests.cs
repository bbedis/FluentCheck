namespace FluentCheck.Tests;

public class NullableEnumAssertionsTests
{
    private enum Priority { Low = 1, Medium = 2, High = 3 }

    [Fact]
    public void HaveValue_WhenHasValue_Succeeds()
    {
        Priority? nullablePriority = Priority.High;
        nullablePriority.Should().HaveValue();
    }

    [Fact]
    public void HaveValue_WhenNull_Fails()
    {
        Priority? nullP = null;
        Assert.Throws<AssertionFailedException>(() => nullP.Should().HaveValue());
    }

    [Fact]
    public void NotHaveValue_WhenNull_Succeeds()
    {
        Priority? nullP = null;
        nullP.Should().NotHaveValue();
    }

    [Fact]
    public void NotHaveValue_WhenHasValue_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            ((Priority?)Priority.High).Should().NotHaveValue());
    }

    [Fact]
    public void Which_ReturnsNonNullableAssertion()
    {
        Priority? nullablePriority = Priority.Medium;
        nullablePriority.Should().HaveValue();
        nullablePriority.Should().Which.Should().Be(Priority.Medium);
    }

    [Fact]
    public void BeWithExpectedValue_Succeeds()
    {
        Priority.High.Should().Be(Priority.High);
    }

    [Fact]
    public void BeWithWrongValue_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            Priority.High.Should().Be(Priority.Low));
    }

    [Fact]
    public void Be_WhenNull_Fails()
    {
        Priority? nullP = null;
        Assert.Throws<AssertionFailedException>(() => nullP.Should().Be(Priority.High));
    }

    [Fact]
    public void NotBe_WhenNotEqual_Succeeds()
    {
        Priority.Low.Should().NotBe(Priority.High);
    }

    [Fact]
    public void NotBe_WhenEqual_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            Priority.High.Should().NotBe(Priority.High));
    }

    [Fact]
    public void HaveFlag_WhenFlagPresent_Succeeds()
    {
        ((Priority.Low | Priority.High)).Should().HaveFlag(Priority.Low);
    }

    [Fact]
    public void NotHaveFlag_WhenFlagAbsent_Succeeds()
    {
        Priority.Low.Should().NotHaveFlag(Priority.High);
    }

    [Fact]
    public void AssertionScope_CollectsAllFailures()
    {
        var scope = new AssertionScope();
        Priority.High.Should().HaveValue(Priority.Low);
        Priority? nullP = null;
        nullP.Should().HaveValue();
        var ex = Assert.Throws<AssertionFailedException>(() => scope.Dispose());
        Assert.Contains("Multiple assertion failures (2)", ex.Message);
    }
}
