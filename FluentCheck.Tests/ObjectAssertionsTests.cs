namespace FluentCheck.Tests;

public class ObjectAssertionsTests
{
    [Fact] public void NotBeNull() => new object().Should().NotBeNull();
    [Fact] public void BeNull() => ((object?)null!).Should().BeNull();
    [Fact] public void NotBeNull_fails() => Assert.Throws<AssertionFailedException>(() => ((object?)null!).Should().NotBeNull());

    [Fact] public void BeOfType_sameType() => new Order().Should().BeOfType<Order>();
    [Fact] public void BeOfType_wrongType() => Assert.Throws<AssertionFailedException>(() => new Order().Should().BeOfType<Receipt>());

    [Fact] public void Be_sameValues()
    {
        var o1 = new Order { Total = 100 };
        var o2 = new Order { Total = 100 };
        o1.Should().Be(o2);
    }

    [Fact] public void Be_differentValues() => Assert.Throws<AssertionFailedException>(() =>
        new Order { Total = 100 }.Should().Be(new Order { Total = 200 }));

    [Fact] public void BeEquivalentTo_same()
    {
        var obj = new Order { Id = Guid.NewGuid(), Total = 100, Items = ["a", "b"] };
        var expected = new Order { Id = obj.Id, Total = 100, Items = ["a", "b"] };
        obj.Should().BeEquivalentTo(expected);
    }

    [Fact] public void BeEquivalentTo_differentTotal() => Assert.Throws<AssertionFailedException>(() =>
        new Order { Total = 100 }.Should().BeEquivalentTo(new Order { Total = 200 }));

    [Fact] public void BeEquivalentTo_excludingId()
    {
        var obj = new Order { Id = Guid.NewGuid(), Total = 100 };
        obj.Should().BeEquivalentTo(new Order { Id = Guid.NewGuid(), Total = 100 },
            opts => opts.ExcludingProperty("Id"));
    }

    [Fact] public void BeEquivalentTo_ignoringCase()
    {
        var obj = new User { Username = "Admin_Bob" };
        obj.Should().BeEquivalentTo(new User { Username = "admin_bob" }, opts => opts.IgnoringCase());
    }

    [Fact] public void BeEquivalentTo_strictTyping_crossType() => Assert.Throws<AssertionFailedException>(() =>
        new Receipt { Id = Guid.NewGuid(), Total = 100 }.Should().BeEquivalentTo(
            new Order { Id = Guid.NewGuid(), Total = 100 }, opts => opts.WithStrictTyping()));

    [Fact] public void BeEquivalentTo_nestedObject()
    {
        var receipt = new Receipt { Id = Guid.NewGuid(), Total = 100, Audit = new AuditLog { Action = "Checkout", UserId = 1 } };
        var expected = new Receipt { Id = receipt.Id, Total = 100, Audit = new AuditLog { Action = "Checkout", UserId = 1 } };
        receipt.Should().BeEquivalentTo(expected);
    }

    [Fact] public void NotBeEquivalentTo_different() =>
        new Order { Total = 100 }.Should().NotBeEquivalentTo(new Order { Total = 200 });

    [Fact] public void NotBeEquivalentTo_same() => Assert.Throws<AssertionFailedException>(() =>
        new Order { Total = 100 }.Should().NotBeEquivalentTo(new Order { Total = 100 }));

    [Fact] public void And_chain() => new Order().Should().And.Should().NotBeNull();
}
