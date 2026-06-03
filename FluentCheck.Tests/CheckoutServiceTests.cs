using FluentCheck; // <-- Crucial: Brings in your new Should() extensions
using Xunit;

namespace FluentCheck.Tests;

public class CheckoutServiceTests
{
    private readonly CheckoutService _service = new();

    [Fact]
    public void ProcessCheckout_ValidOrder_ReturnsReceipt()
    {
        // Arrange
        var order = new Order { Id = Guid.NewGuid(), Total = 150.50m, Items = ["Apple", "Banana"] };

        // Act
        var receipt = _service.Process(order);

        // Assert (Zero-alloc happy path, beautiful chaining)
        receipt.Should().NotBeNull();
        
        receipt.Id.Should().NotBeEmpty();
        
        receipt.Total.Should().BeGreaterThan(0m);
        
        receipt.Items.Should().HaveCount(2);
        receipt.Items.Should().Contain("Apple");
        
        // Deep structural equivalence
        var expectedAudit = new AuditLog { Action = "Checkout", UserId = 1 };
        receipt.Audit.Should().BeEquivalentTo(expectedAudit);
    }

    [Fact]
    public async Task ProcessCheckout_InvalidPayment_ThrowsAndLogs()
    {
        var order = new Order { PaymentToken = "invalid" };

        // Async Exception checking
        Func<Task> act = async () => await _service.ProcessAsync(order);
        
        var exceptionAssert = await act.Should().ThrowAsync<PaymentDeclinedException>();
        
        // Check the message
        exceptionAssert.WithMessage("Payment*declined*");
    }

    [Fact]
    public void ComplexValidation_UsingScope()
    {
        var user = _service.GetUser(123);

        // Batches all failures and throws once at the end of the 'using' block
        using (new AssertionScope())
        {
            user.Should().NotBeNull();
            user.Username.Should().StartWith("admin_");
            user.LastLogin.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(5));
            user.Roles.Should().Contain("SuperAdmin");
        }
    }
}