using BenchmarkDotNet.Attributes;
using FluentCheck;
using FluentCheck.Equivalency;

namespace FluentCheck.Benchmarks;

[MemoryDiagnoser]
public class DeepEquivalencyBenchmarks
{
    private Order _order1 = null!, _order2 = null!;
    private Order _orderModified = null!;
    private Receipt _receipt1 = null!, _receipt2 = null!;
    private User _user1 = null!, _user2 = null!;
    private List<User> _users1 = null!, _users2 = null!;
    private Receipt _receiptNested1 = null!, _receiptNested2 = null!;

    [GlobalSetup]
    public void Setup()
    {
        _order1 = new Order { Id = Guid.NewGuid(), Total = 100m, Items = new[] { "a", "b", "c" }, PaymentToken = "tok_123" };
        _order2 = new Order { Id = _order1.Id, Total = _order1.Total, Items = (string[])_order1.Items.Clone(), PaymentToken = _order1.PaymentToken };
        _orderModified = new Order { Id = _order1.Id, Total = _order1.Total + 1, Items = _order1.Items, PaymentToken = _order1.PaymentToken };

        _receipt1 = new Receipt { Id = Guid.NewGuid(), Total = 50m, Items = new[] { "x", "y" }, Audit = new AuditLog { Action = "created", UserId = 1 } };
        _receipt2 = new Receipt { Id = _receipt1.Id, Total = _receipt1.Total, Items = (string[])_receipt1.Items.Clone(), Audit = new AuditLog { Action = _receipt1.Audit!.Action, UserId = _receipt1.Audit!.UserId } };

        _user1 = new User { Username = "alice", LastLogin = new DateTime(2024, 6, 1), Roles = new[] { "admin", "user" } };
        _user2 = new User { Username = "alice", LastLogin = _user1.LastLogin, Roles = (string[])_user1.Roles.Clone() };

        _users1 = Enumerable.Range(0, 100).Select(i => new User { Username = $"user{i}", LastLogin = DateTime.UtcNow, Roles = new[] { "user" } }).ToList();
        _users2 = _users1.Select(u => new User { Username = u.Username, LastLogin = u.LastLogin, Roles = (string[])u.Roles.Clone() }).ToList();

        _receiptNested1 = new Receipt { Id = Guid.NewGuid(), Total = 200m, Items = new[] { "item1" }, Audit = new AuditLog { Action = "updated", UserId = 42 } };
        _receiptNested2 = new Receipt { Id = _receiptNested1.Id, Total = _receiptNested1.Total, Items = (string[])_receiptNested1.Items.Clone(), Audit = new AuditLog { Action = _receiptNested1.Audit!.Action, UserId = _receiptNested1.Audit!.UserId } };
    }

    [Benchmark(Baseline = true)]
    public void SameTypePass() => _order1.Should().BeEquivalentTo(_order2);

    [Benchmark]
    public void SameTypeFail() => new Action(() => _order1.Should().BeEquivalentTo(_orderModified)).Should().Throw<AssertionFailedException>();

    [Benchmark]
    public void NestedPass() => _receiptNested1.Should().BeEquivalentTo(_receiptNested2);

    [Benchmark]
    public void WithExclusion() => _order1.Should().BeEquivalentTo(_order2, opts => opts.ExcludingProperty("PaymentToken"));

    [Benchmark]
    public void WithIgnoringCase() => new { Name = "Alice" }.Should().BeEquivalentTo(new { Name = "alice" }, opts => opts.IgnoringCase());

    [Benchmark]
    public void WithExcludingProperty() => _order1.Should().BeEquivalentTo(_order2, opts => opts.ExcludingProperty("Id"));
}
