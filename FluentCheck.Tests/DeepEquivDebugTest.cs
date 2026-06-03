using FluentCheck.Equivalency;

namespace FluentCheck.Tests;

public class DeepEquivDebugTest
{
    [Fact]
    public void DebugExcludingProperty()
    {
        var builder = new EquivalencyOptionsBuilder();
        builder.ExcludingProperty("Id");
        var options = builder.Build();

        // Check the built options
        Assert.NotNull(options.ExcludedProperties);
        Assert.Contains("Id", options.ExcludedProperties);
        Assert.Single(options.ExcludedProperties);

        // Test comparison directly
        var obj = new Order { Id = Guid.NewGuid(), Total = 100 };
        var expected = new Order { Id = Guid.NewGuid(), Total = 100 };

        var result = DeepEqualityComparer.AreEquivalent(obj, expected, options, out var msg);
        Assert.True(result, $"Should be equivalent but failed: {msg}");
    }

    [Fact]
    public void DebugWithExclusionViaConfigure()
    {
        var obj = new Order { Id = Guid.NewGuid(), Total = 100 };
        var expected = new Order { Id = Guid.NewGuid(), Total = 100 };

        var options = EquivalencyOptions.Configure(opts => opts.ExcludingProperty("Id"));

        Assert.NotNull(options.ExcludedProperties);
        Assert.Contains("Id", options.ExcludedProperties);

        var result = DeepEqualityComparer.AreEquivalent(obj, expected, options, out var msg);
        Assert.True(result, $"Should be equivalent but failed: {msg}");
    }

    [Fact]
    public void DebugNoExclusion()
    {
        var obj = new Order { Id = Guid.NewGuid(), Total = 100 };
        var expected = new Order { Id = obj.Id, Total = 100 };

        // Same Id, same Total
        var result = DeepEqualityComparer.AreEquivalent(obj, expected, EquivalencyOptions.Default, out var msg);
        Assert.True(result, $"Should be equivalent but failed: {msg}");
    }

    [Fact]
    public void DebugDifferentTotal()
    {
        var obj = new Order { Id = Guid.NewGuid(), Total = 100 };
        var expected = new Order { Id = Guid.NewGuid(), Total = 200 };

        var result = DeepEqualityComparer.AreEquivalent(obj, expected, EquivalencyOptions.Default, out var msg);
        Assert.False(result);
    }
}
