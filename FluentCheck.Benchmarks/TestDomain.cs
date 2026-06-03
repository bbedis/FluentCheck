namespace FluentCheck.Benchmarks;

// POCOs for benchmarking — mirrors FluentCheck.Tests.TestDomain but local to benchmarks.
public class Order
{
    public Guid Id { get; set; }
    public decimal Total { get; set; }
    public string[] Items { get; set; } = [];
    public string? PaymentToken { get; set; }
}

public class Receipt
{
    public Guid Id { get; set; }
    public decimal Total { get; set; }
    public string[] Items { get; set; } = [];
    public AuditLog? Audit { get; set; }
}

public class AuditLog
{
    public string Action { get; set; } = "";
    public int UserId { get; set; }
}

public class User
{
    public string Username { get; set; } = "";
    public DateTime LastLogin { get; set; }
    public string[] Roles { get; set; } = [];
}
