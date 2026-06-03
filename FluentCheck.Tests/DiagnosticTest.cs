using FluentCheck.Equivalency;

namespace FluentCheck.Tests;

public class DiagnosticTest
{
    [Fact]
    public void DebugDeepEquiv()
    {
        var options = new EquivalencyOptions();

        // string[] comparison
        var arr1 = new string[] { "a" };
        var arr2 = new string[] { "b" };

        Console.WriteLine($"string[] IsSealed: {typeof(string[]).IsSealed}");
        Console.WriteLine($"string[] IsArray: {typeof(string[]).IsArray}");

        var ienum = typeof(System.Collections.IEnumerable);
        Console.WriteLine($"IEnumerable.IsAssignableFrom(string[]): {ienum.IsAssignableFrom(typeof(string[]))}");

        Console.WriteLine("string[] properties:");
        foreach (var p in typeof(string[]).GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
            Console.WriteLine($"  {p.Name}: {p.PropertyType}");

        Console.WriteLine($"string[] deep equals: {DeepEqualityComparer.AreEquivalent(arr1, arr2, options, out var msg)}");
        Console.WriteLine($"msg: {msg}");
        Console.WriteLine($"Order deep equals (diff): {DeepEqualityComparer.AreEquivalent(new Order { Total = 100 }, new Order { Total = 200 }, options, out _)}");
        Console.WriteLine($"Order deep equals (same): {DeepEqualityComparer.AreEquivalent(new Order { Total = 100 }, new Order { Total = 100 }, options, out _)}");
    }
}
