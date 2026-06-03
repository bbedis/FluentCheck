namespace FluentCheck.Tests;

public class BooleanAssertionsTests
{
    [Fact] public void BeTrue_true() => true.Should().BeTrue();
    [Fact] public void BeTrue_false() => Assert.Throws<AssertionFailedException>(() => false.Should().BeTrue());

    [Fact] public void BeFalse_false() => false.Should().BeFalse();
    [Fact] public void BeFalse_true() => Assert.Throws<AssertionFailedException>(() => true.Should().BeFalse());

    [Fact] public void Be_equal() => true.Should().Be(true);
    [Fact] public void Be_notEqual() => Assert.Throws<AssertionFailedException>(() => true.Should().Be(false));

    [Fact] public void NotBe_notEqual() => true.Should().NotBe(false);
    [Fact] public void NotBe_equal() => Assert.Throws<AssertionFailedException>(() => true.Should().NotBe(true));
}
