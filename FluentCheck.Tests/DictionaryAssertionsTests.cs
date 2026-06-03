namespace FluentCheck.Tests;

public class DictionaryAssertionsTests
{
    private readonly IDictionary<string, int> _dict = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };

    [Fact] public void ContainKey_found() => _dict.Should().ContainKey("a");
    [Fact] public void ContainKey_notFound() => Assert.Throws<AssertionFailedException>(() => _dict.Should().ContainKey("z"));

    [Fact] public void ContainKeyAndValue_found() => _dict.Should().ContainKeyAndValue("a", 1);
    [Fact] public void ContainKeyAndValue_wrongValue() => Assert.Throws<AssertionFailedException>(() => _dict.Should().ContainKeyAndValue("a", 99));

    [Fact] public void NotContainKey_notFound() => _dict.Should().NotContainKey("z");
    [Fact] public void NotContainKey_found() => Assert.Throws<AssertionFailedException>(() => _dict.Should().NotContainKey("a"));

    [Fact] public void NotContainValue_notFound() => _dict.Should().NotContainValue(99);
    [Fact] public void NotContainValue_found() => Assert.Throws<AssertionFailedException>(() => _dict.Should().NotContainValue(1));

    [Fact] public void NotContainKeyAndValue_notMatch() => _dict.Should().NotContainKeyAndValue("a", 99);
    [Fact] public void NotContainKeyAndValue_matches() => Assert.Throws<AssertionFailedException>(() => _dict.Should().NotContainKeyAndValue("a", 1));

    [Fact] public void ContainKeys_allFound() => _dict.Should().ContainKeys("a", "b");
    [Fact] public void ContainKeys_missing() => Assert.Throws<AssertionFailedException>(() => _dict.Should().ContainKeys("a", "z"));
    [Fact] public void ContainValues_allFound() => _dict.Should().ContainValues(1, 2);
    [Fact] public void ContainValues_missing() => Assert.Throws<AssertionFailedException>(() => _dict.Should().ContainValues(1, 99));

    [Fact] public void Contain_alias() => _dict.Should().Contain("a", 1);

    [Fact] public void Equal_same() => _dict.Should().Equal(new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 });
    [Fact] public void Equal_different() => Assert.Throws<AssertionFailedException>(() =>
        _dict.Should().Equal(new Dictionary<string, int> { ["a"] = 1, ["b"] = 3 }));
    [Fact] public void Equal_differentCount() => Assert.Throws<AssertionFailedException>(() =>
        _dict.Should().Equal(new Dictionary<string, int> { ["a"] = 1, ["b"] = 2, ["c"] = 3 }));

    [Fact] public void HaveSameCount_same() => _dict.Should().HaveSameCount(new Dictionary<string, int> { ["x"] = 1, ["y"] = 2 });
    [Fact] public void HaveSameCount_different() => Assert.Throws<AssertionFailedException>(() =>
        _dict.Should().HaveSameCount(new Dictionary<string, int> { ["x"] = 1 }));

    [Fact] public void NotContain_empty() => new Dictionary<string, int>().Should().NotContain();
    [Fact] public void NotContain_notEmpty() => Assert.Throws<AssertionFailedException>(() => _dict.Should().NotContain());

    [Fact] public void ContainKey_nullDict() => Assert.Throws<AssertionFailedException>(() =>
        ((Dictionary<string, int>?)null!).Should().ContainKey("a"));

    [Fact] public void ContainKey_readOnlyDictionary()
    {
        ((IDictionary<string, int>)new Dictionary<string, int> { ["x"] = 1 }).Should().ContainKey("x");
    }

    [Fact] public void ContainKey_readOnlyDictionary_viaCast()
    {
        IReadOnlyDictionary<string, int> readOnly = new Dictionary<string, int> { ["x"] = 1 };
        ((IDictionary<string, int>?)readOnly).Should().ContainKey("x");
    }

    [Fact] public void HaveSameCount_readOnlyDictionary()
    {
        var dict = (IDictionary<string, int>)new Dictionary<string, int> { ["a"] = 1 };
        var expected = (IDictionary<string, int>)new Dictionary<string, int> { ["b"] = 2 };
        dict.Should().HaveSameCount(expected);
    }

    [Fact] public void Equal_viaExplicitCast()
    {
        var dict = (IDictionary<string, int>)new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };
        var expected = (IDictionary<string, int>)new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };
        dict.Should().Equal(expected);
    }
}
