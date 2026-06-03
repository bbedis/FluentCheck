namespace FluentCheck.Tests;

public class NullableAssertionsTests
{
    // --- NullableDateTime ---

    [Fact]
    public void NullableDateTime_HaveValue_WhenHasValue_Succeeds()
    {
        DateTime? now = DateTime.UtcNow;
        now.Should().HaveValue();
    }

    [Fact]
    public void NullableDateTime_HaveValue_WhenNull_Fails()
    {
        DateTime? nullDt = null;
        Assert.Throws<AssertionFailedException>(() => nullDt.Should().HaveValue());
    }

    [Fact]
    public void NullableDateTime_NotHaveValue_WhenNull_Succeeds()
    {
        DateTime? nullDt = null;
        nullDt.Should().NotHaveValue();
    }

    [Fact]
    public void NullableDateTime_NotHaveValue_WhenHasValue_Fails()
    {
        DateTime? now = DateTime.UtcNow;
        Assert.Throws<AssertionFailedException>(() => now.Should().NotHaveValue());
    }

    [Fact]
    public void NullableDateTime_Which_ReturnsNonNullableAssertion()
    {
        DateTime? now = DateTime.UtcNow;
        now.Should().HaveValue();
        now.Should().Which.Should().Be(now.Value);
    }

    // --- NullableDateOnly ---

    [Fact]
    public void NullableDateOnly_HaveValue_WhenHasValue_Succeeds()
    {
        DateOnly? now = DateOnly.FromDateTime(DateTime.UtcNow);
        now.Should().HaveValue();
    }

    [Fact]
    public void NullableDateOnly_HaveValue_WhenNull_Fails()
    {
        DateOnly? nullDate = null;
        Assert.Throws<AssertionFailedException>(() => nullDate.Should().HaveValue());
    }

    [Fact]
    public void NullableDateOnly_NotHaveValue_WhenNull_Succeeds()
    {
        DateOnly? nullDate = null;
        nullDate.Should().NotHaveValue();
    }

    [Fact]
    public void NullableDateOnly_NotHaveValue_WhenHasValue_Fails()
    {
        DateOnly? now = DateOnly.FromDateTime(DateTime.UtcNow);
        Assert.Throws<AssertionFailedException>(() => now.Should().NotHaveValue());
    }

    // --- NullableGuid ---

    [Fact]
    public void NullableGuid_HaveValue_WhenHasValue_Succeeds()
    {
        Guid? id = Guid.NewGuid();
        id.Should().HaveValue();
    }

    [Fact]
    public void NullableGuid_HaveValue_WhenNull_Fails()
    {
        Guid? nullGuid = null;
        Assert.Throws<AssertionFailedException>(() => nullGuid.Should().HaveValue());
    }

    [Fact]
    public void NullableGuid_NotHaveValue_WhenNull_Succeeds()
    {
        Guid? nullGuid = null;
        nullGuid.Should().NotHaveValue();
    }

    [Fact]
    public void NullableGuid_NotHaveValue_WhenHasValue_Fails()
    {
        Guid? id = Guid.NewGuid();
        Assert.Throws<AssertionFailedException>(() => id.Should().NotHaveValue());
    }

    [Fact]
    public void NullableGuid_Which_ReturnsNonNullableAssertion()
    {
        Guid? id = Guid.NewGuid();
        id.Should().HaveValue();
        id.Should().Which.Should().Be(id.Value);
    }

    [Fact]
    public void AssertionScope_CollectsAllFailures()
    {
        var scope = new AssertionScope();
        DateTime? nullDt = null;
        nullDt.Should().HaveValue();
        Guid? nullGuid = null;
        nullGuid.Should().HaveValue();
        ((DateTime?)DateTime.UtcNow).Should().NotHaveValue();
        var ex = Assert.Throws<AssertionFailedException>(() => scope.Dispose());
        Assert.Contains("Multiple assertion failures (3)", ex.Message);
    }
}
