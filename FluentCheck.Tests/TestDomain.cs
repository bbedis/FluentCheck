namespace FluentCheck.Tests;

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

public class PaymentDeclinedException : Exception
{
    public PaymentDeclinedException(string message) : base(message) { }
}

public class CheckoutService
{
    public Receipt Process(Order order)
    {
        return new Receipt
        {
            Id = Guid.NewGuid(),
            Total = order.Total,
            Items = order.Items,
            Audit = new AuditLog { Action = "Checkout", UserId = 1 }
        };
    }

    public async Task<Receipt> ProcessAsync(Order order)
    {
        await Task.Delay(10);
        if (order.PaymentToken == "invalid")
        {
            throw new PaymentDeclinedException("Payment was declined by the bank.");
        }
        return Process(order);
    }

    public User GetUser(int id) 
    {
        return new User 
        { 
            Username = "admin_bob", 
            LastLogin = DateTime.UtcNow.AddMinutes(-2), // Simulating a recent login
            Roles = ["User", "SuperAdmin"] 
        };
    }
}