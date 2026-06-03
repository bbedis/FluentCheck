using BenchmarkDotNet.Attributes;
using FluentCheck;

namespace FluentCheck.Benchmarks;

public class StringAssertionsBenchmarks
{
    private string _longString = null!;
    private string _match = null!;
    private string[] _substrings = null!;

    [GlobalSetup]
    public void Setup()
    {
        _longString = new string('a', 10_000) + "needle" + new string('b', 10_000);
        _match = "Hello, World!";
        _substrings = new[] { "aaa", "Hello", "World!", "needle", "bbb" };
    }

    [Benchmark(Baseline = true)]
    public void BePass() => _match.Should().Be("Hello, World!");

    [Benchmark]
    public void BeFail() => new Action(() => _match.Should().Be("Goodbye, World!")).Should().Throw<AssertionFailedException>();

    [Benchmark]
    public void ContainPass() => _longString.Should().Contain("needle");

    [Benchmark]
    public void ContainFail() => new Action(() => _longString.Should().Contain("zzz")).Should().Throw<AssertionFailedException>();

    [Benchmark]
    public void StartWithPass() => _match.Should().StartWith("Hello");

    [Benchmark]
    public void StartWithFail() => new Action(() => _match.Should().StartWith("World")).Should().Throw<AssertionFailedException>();

    [Benchmark]
    public void EndWithPass() => _match.Should().EndWith("World!");

    [Benchmark]
    public void EndWithFail() => new Action(() => _match.Should().EndWith("Hello")).Should().Throw<AssertionFailedException>();

    [Benchmark]
    public void BeOneOf() => _match.Should().BeOneOf("Hello, World!", "Goodbye, World!", "Hello, Universe!");

    [Benchmark]
    public void ContainAll() => _longString.Should().ContainAll(_substrings);

    [Benchmark]
    public void ContainAny() => _longString.Should().ContainAny(_substrings);

    [Benchmark]
    public void BeLowerCased() => "hello world".Should().BeLowerCased();

    [Benchmark]
    public void BeUpperCased() => "HELLO WORLD".Should().BeUpperCased();

    [Benchmark]
    public void MatchRegex() => "user123@test.com".Should().MatchRegex("^[a-z]+\\d+@[a-z]+\\.[a-z]+$");

    [Benchmark]
    public void MatchRegexFail() => new Action(() => "user123@test.com".Should().MatchRegex("^[A-Z]+\\d+@[a-z]+\\.[a-z]+$")).Should().Throw<AssertionFailedException>();

    [Benchmark]
    public void BeEquivalentOf() => _match.Should().BeEquivalentOf("hello, world!", StringComparison.OrdinalIgnoreCase);

    [Benchmark]
    public void ContainEquivalentOf() => "Hello, WORLD!".Should().ContainEquivalentOf("world", StringComparison.OrdinalIgnoreCase);

    [Benchmark]
    public void HaveLengthPass() => _match.Should().HaveLength(13);

    [Benchmark]
    public void HaveLengthFail() => new Action(() => _match.Should().HaveLength(20)).Should().Throw<AssertionFailedException>();

    [Benchmark]
    public void NotBePass() => _match.Should().NotBe("Goodbye, World!");

    [Benchmark]
    public void NotContainPass() => _match.Should().NotContain("xyz");

    [Benchmark]
    public void NotStartWithPass() => _match.Should().NotStartWith("Goodbye");

    [Benchmark]
    public void NotEndWithPass() => _match.Should().NotEndWith("Universe");

    [Benchmark]
    public void NotMatchRegex() => "user123@test.com".Should().NotMatchRegex("^[0-9]+$");
}
