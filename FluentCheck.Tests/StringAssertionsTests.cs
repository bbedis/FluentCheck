using System.Text.RegularExpressions;

namespace FluentCheck.Tests;

public class StringAssertionsTests
{
    [Fact] public void Be_same_string() => "hello".Should().Be("hello");
    [Fact] public void Be_fails_on_mismatch() => Assert.Throws<AssertionFailedException>(() => "hello".Should().Be("world"));

    [Fact] public void NotBe_different() => "hello".Should().NotBe("world");
    [Fact] public void NotBe_fails_on_match() => Assert.Throws<AssertionFailedException>(() => "hello".Should().NotBe("hello"));

    [Fact] public void Contain_found() => "hello world".Should().Contain("world");
    [Fact] public void Contain_not_found() => Assert.Throws<AssertionFailedException>(() => "hello".Should().Contain("world"));

    [Fact] public void NotContain_not_found() => "hello".Should().NotContain("world");
    [Fact] public void NotContain_found() => Assert.Throws<AssertionFailedException>(() => "hello world".Should().NotContain("world"));

    [Fact] public void StartWith_found() => "hello".Should().StartWith("hel");
    [Fact] public void StartWith_not_found() => Assert.Throws<AssertionFailedException>(() => "hello".Should().StartWith("world"));

    [Fact] public void NotStartWith_found() => Assert.Throws<AssertionFailedException>(() => "hello".Should().NotStartWith("hel"));

    [Fact] public void EndWith_found() => "hello".Should().EndWith("lo");
    [Fact] public void EndWith_not_found() => Assert.Throws<AssertionFailedException>(() => "hello".Should().EndWith("world"));

    [Fact] public void NotEndWith_found() => Assert.Throws<AssertionFailedException>(() => "hello".Should().NotEndWith("lo"));

    [Fact] public void MatchRegex_matches() => "test@example.com".Should().MatchRegex(".*@.*\\.com");
    [Fact] public void MatchRegex_does_not_match() => Assert.Throws<AssertionFailedException>(() => "hello".Should().MatchRegex(".*@.*\\.com"));

    [Fact] public void NotMatchRegex_no_match() => "hello".Should().NotMatchRegex("@.*");
    [Fact] public void NotMatchRegex_matches() => Assert.Throws<AssertionFailedException>(() => "a@b.com".Should().NotMatchRegex(".*@.*"));

    [Fact] public void BeNullOrEmpty_empty() => "".Should().BeNullOrEmpty();
    [Fact] public void BeNullOrEmpty_null() => ((string?)null!).Should().BeNullOrEmpty();
    [Fact] public void BeNullOrEmpty_fails() => Assert.Throws<AssertionFailedException>(() => "x".Should().BeNullOrEmpty());

    [Fact] public void NotBeNullOrEmpty_nonEmpty() => "x".Should().NotBeNullOrEmpty();
    [Fact] public void NotBeNullOrEmpty_fails() => Assert.Throws<AssertionFailedException>(() => "".Should().NotBeNullOrEmpty());

    [Fact] public void NotBeNullOrWhiteSpace_nonEmpty() => "x".Should().NotBeNullOrWhiteSpace();
    [Fact] public void NotBeNullOrWhiteSpace_fails() => Assert.Throws<AssertionFailedException>(() => "  ".Should().NotBeNullOrWhiteSpace());

    [Fact] public void BeOneOf_match() => "a".Should().BeOneOf("a", "b", "c");
    [Fact] public void BeOneOf_fail() => Assert.Throws<AssertionFailedException>(() => "x".Should().BeOneOf("a", "b"));

    [Fact] public void ContainAll_allPresent() => "abc".Should().ContainAll("a", "b", "c");
    [Fact] public void ContainAll_missing() => Assert.Throws<AssertionFailedException>(() => "ab".Should().ContainAll("a", "b", "c"));

    [Fact] public void ContainAny_onePresent() => "abc".Should().ContainAny("x", "b", "z");
    [Fact] public void ContainAny_nonePresent() => Assert.Throws<AssertionFailedException>(() => "ab".Should().ContainAny("x", "y", "z"));

    [Fact] public void BeLowerCased_lower() => "abc".Should().BeLowerCased();
    [Fact] public void BeLowerCased_mixed() => Assert.Throws<AssertionFailedException>(() => "aBc".Should().BeLowerCased());

    [Fact] public void BeUpperCased_upper() => "ABC".Should().BeUpperCased();
    [Fact] public void BeUpperCased_mixed() => Assert.Throws<AssertionFailedException>(() => "AbC".Should().BeUpperCased());

    [Fact] public void HaveLength_correct() => "abc".Should().HaveLength(3);
    [Fact] public void HaveLength_wrong() => Assert.Throws<AssertionFailedException>(() => "abc".Should().HaveLength(4));

    [Fact] public void Be_caseInsensitive() => "Hello".Should().Be("hello", StringComparison.OrdinalIgnoreCase);
    [Fact] public void ContainEquivalentOf() => "Hello World".Should().ContainEquivalentOf("hello", StringComparison.OrdinalIgnoreCase);
    [Fact] public void StartWithEquivalentOf() => "Hello".Should().StartWithEquivalentOf("hello", StringComparison.OrdinalIgnoreCase);
    [Fact] public void EndWithEquivalentOf() => "Hello".Should().EndWithEquivalentOf("ELLO", StringComparison.OrdinalIgnoreCase);
    [Fact] public void MatchEquivalentOf() => "HELLO".Should().MatchEquivalentOf("hello", RegexOptions.IgnoreCase);
    [Fact] public void NotContainEquivalentOf() => "abc".Should().NotContainEquivalentOf("XYZ", StringComparison.OrdinalIgnoreCase);
    [Fact] public void NotMatchEquivalentOf() => "abc".Should().NotMatchEquivalentOf("[0-9]+", RegexOptions.IgnoreCase);

    [Fact] public void Contain_withOccurrenceConstraint_exactOnce() => "abc".Should().Contain("a", OccurrenceConstraint.Once);
    [Fact] public void Contain_withOccurrenceConstraint_atLeast2() => "aaa".Should().Contain("a", OccurrenceConstraint.AtLeast(2));
    [Fact] public void Contain_withOccurrenceConstraint_atMost2() => "aba".Should().Contain("a", OccurrenceConstraint.AtMost(2));
    [Fact] public void Contain_withOccurrenceConstraint_twice() => "aa".Should().Contain("a", OccurrenceConstraint.Twice);
    [Fact] public void Contain_withOccurrenceConstraint_fails() => Assert.Throws<AssertionFailedException>(() => "a".Should().Contain("a", OccurrenceConstraint.Twice));
    [Fact] public void Contain_withOccurrenceConstraint_nullSubject() => Assert.Throws<AssertionFailedException>(() =>
        ((string?)null!).Should().Contain("a", OccurrenceConstraint.Once));
}
