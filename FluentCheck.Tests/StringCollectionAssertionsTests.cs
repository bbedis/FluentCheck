namespace FluentCheck.Tests;

public class StringCollectionAssertionsTests
{
    [Fact]
    public void Contain_WhenPresent_Succeeds()
    {
        new[] { "apple", "banana", "cherry" }.Should().Contain("banana");
    }

    [Fact]
    public void Contain_WhenNotPresent_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            new[] { "apple", "banana" }.Should().Contain("cherry"));
    }

    [Fact]
    public void NotContain_WhenAbsent_Succeeds()
    {
        new[] { "apple", "banana" }.Should().NotContain("cherry");
    }

    [Fact]
    public void NotContain_WhenPresent_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            new[] { "apple", "banana" }.Should().NotContain("banana"));
    }

    [Fact]
    public void HaveCount_WhenCorrect_Succeeds()
    {
        new[] { "a", "b", "c" }.Should().HaveCount(3);
    }

    [Fact]
    public void HaveCount_WhenWrong_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            new[] { "a", "b" }.Should().HaveCount(5));
    }

    [Fact]
    public void NotHaveCount_WhenWrong_Succeeds()
    {
        new[] { "a", "b" }.Should().NotHaveCount(5);
    }

    [Fact]
    public void NotHaveCount_WhenCorrect_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            new[] { "a", "b" }.Should().NotHaveCount(2));
    }

    [Fact]
    public void ContainSingle_WhenSingleElement_Succeeds()
    {
        new[] { "single" }.Should().ContainSingle();
    }

    [Fact]
    public void ContainSingle_WhenEmpty_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            Array.Empty<string>().Should().ContainSingle());
    }

    [Fact]
    public void ContainSingle_WhenMultiple_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            new[] { "a", "b" }.Should().ContainSingle());
    }

    [Fact]
    public void ContainSingle_WhenSingleMatch_Succeeds()
    {
        new[] { "apple", "banana" }.Should().ContainSingle("apple");
    }

    [Fact]
    public void ContainSingle_WhenNoMatch_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            new[] { "apple", "banana" }.Should().ContainSingle("cherry"));
    }

    [Fact]
    public void ContainSingle_WhenMultipleMatches_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            new[] { "a", "a", "a" }.Should().ContainSingle("a"));
    }

    [Fact]
    public void StartWith_WhenFirstElementStartsWith_Succeeds()
    {
        new[] { "apple", "banana" }.Should().StartWith("app");
    }

    [Fact]
    public void StartWith_WhenEmptyCollection_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            Array.Empty<string>().Should().StartWith("test"));
    }

    [Fact]
    public void EndWith_WhenLastElementEndsWith_Succeeds()
    {
        new[] { "apple", "banana" }.Should().EndWith("na");
    }

    [Fact]
    public void EndWith_WhenEmptyCollection_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            Array.Empty<string>().Should().EndWith("test"));
    }

    [Fact]
    public void NotStartWith_WhenFirstElementDoesNotStartWith_Succeeds()
    {
        new[] { "apple", "banana" }.Should().NotStartWith("ban");
    }

    [Fact]
    public void NotStartWith_WhenFirstElementStartsWith_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            new[] { "apple", "banana" }.Should().NotStartWith("app"));
    }

    [Fact]
    public void NotEndWith_WhenLastElementDoesNotEndWith_Succeeds()
    {
        new[] { "apple", "banana" }.Should().NotEndWith("pple");
    }

    [Fact]
    public void NotEndWith_WhenLastElementEndsWith_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            new[] { "apple", "banana" }.Should().NotEndWith("na"));
    }

    [Fact]
    public void HaveElement_WhenCorrectIndexAndValue_Succeeds()
    {
        new[] { "a", "b", "c" }.Should().HaveElement("b", 1);
    }

    [Fact]
    public void HaveElement_WhenWrongIndex_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            new[] { "a", "b" }.Should().HaveElement("c", 5));
    }

    [Fact]
    public void HaveElement_WhenWrongValue_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            new[] { "a", "b" }.Should().HaveElement("c", 0));
    }

    [Fact]
    public void HaveElement_WithPredicate_Succeeds()
    {
        new[] { "apple", "banana" }.Should().HaveElement(s => s.Contains("ban"));
    }

    [Fact]
    public void HaveElement_WithPredicate_NoMatch_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            new[] { "apple", "banana" }.Should().HaveElement(s => s.Contains("cherry")));
    }

    [Fact]
    public void Satisfy_WithPredicate_Succeeds()
    {
        new[] { "apple", "banana" }.Should().Satisfy(s => s.Length > 4);
    }

    [Fact]
    public void HaveElements_WhenMatching_Succeeds()
    {
        new[] { "a", "b", "c" }.Should().HaveElements("a", "b", "c");
    }

    [Fact]
    public void HaveElements_WhenMissingElement_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            new[] { "a", "b" }.Should().HaveElements("a", "b", "c"));
    }

    [Fact]
    public void HaveSameCount_WhenSameCount_Succeeds()
    {
        new[] { "a", "b" }.Should().HaveSameCount(new[] { 1, 2 }.Cast<int>());
    }

    [Fact]
    public void HaveSameCount_WhenDifferentCount_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            new[] { "a" }.Should().HaveSameCount(new[] { 1, 2, 3 }.Cast<int>()));
    }

    // --- Ordering ---

    [Fact]
    public void ContainInOrder_WhenSequenceFound_Succeeds()
    {
        new[] { "a", "b", "c", "d" }.Should().ContainInOrder("b", "d");
    }

    [Fact]
    public void ContainInOrder_WhenOutOfOrder_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            new[] { "a", "b", "c" }.Should().ContainInOrder("c", "a"));
    }

    [Fact]
    public void ContainInOrder_WhenNotContained_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            new[] { "a", "b" }.Should().ContainInOrder("a", "z"));
    }

    [Fact]
    public void ContainInConsecutiveOrder_WhenConsecutive_Succeeds()
    {
        new[] { "a", "b", "c", "d" }.Should().ContainInConsecutiveOrder("b", "c");
    }

    [Fact]
    public void ContainInConsecutiveOrder_WhenNotConsecutive_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            new[] { "a", "b", "d", "c" }.Should().ContainInConsecutiveOrder("b", "c"));
    }

    [Fact]
    public void BeInOrder_WhenAlreadyOrdered_Succeeds()
    {
        new[] { "a", "b", "c" }.Should().BeInOrder();
    }

    [Fact]
    public void BeInOrder_WhenNotOrdered_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            new[] { "c", "a", "b" }.Should().BeInOrder());
    }

    [Fact]
    public void BeInExactOrder_WhenExactMatch_Succeeds()
    {
        new[] { "a", "b", "c" }.Should().BeInExactOrder("a", "b", "c");
    }

    [Fact]
    public void BeInExactOrder_WhenDifferentLength_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            new[] { "a", "b" }.Should().BeInExactOrder("a", "b", "c"));
    }

    [Fact]
    public void BeInExactOrder_WhenDifferentValues_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            new[] { "a", "b", "c" }.Should().BeInExactOrder("a", "x", "c"));
    }

    // --- Set operations ---

    [Fact]
    public void ContainAll_WhenAllPresent_Succeeds()
    {
        new[] { "a", "b", "c" }.Should().ContainAll("a", "b");
    }

    [Fact]
    public void ContainAll_WhenOneMissing_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            new[] { "a", "b" }.Should().ContainAll("a", "b", "c"));
    }

    [Fact]
    public void ContainAny_WhenAnyPresent_Succeeds()
    {
        new[] { "a", "b", "c" }.Should().ContainAny("x", "b", "z");
    }

    [Fact]
    public void ContainAny_WhenNonePresent_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            new[] { "a", "b" }.Should().ContainAny("x", "y", "z"));
    }

    [Fact]
    public void BeEmpty_WhenEmpty_Succeeds()
    {
        Array.Empty<string>().Should().BeEmpty();
    }

    [Fact]
    public void BeEmpty_WhenNonEmpty_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            new[] { "a" }.Should().BeEmpty());
    }

    [Fact]
    public void NotBeEmpty_WhenNonEmpty_Succeeds()
    {
        new[] { "a" }.Should().NotBeEmpty();
    }

    [Fact]
    public void NotBeEmpty_WhenEmpty_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            Array.Empty<string>().Should().NotBeEmpty());
    }

    // --- Negation ordering ---

    [Fact]
    public void NotContainInOrder_WhenNotInOrder_Succeeds()
    {
        new[] { "d", "c", "b" }.Should().NotContainInOrder("a", "b", "c");
    }

    [Fact]
    public void NotContainInOrder_WhenInOrder_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            new[] { "a", "b", "c" }.Should().NotContainInOrder("a", "c"));
    }

    [Fact]
    public void NotContainInConsecutiveOrder_WhenNotConsecutive_Succeeds()
    {
        new[] { "a", "c", "b" }.Should().NotContainInConsecutiveOrder("b", "c");
    }

    [Fact]
    public void NotContainInConsecutiveOrder_WhenConsecutive_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            new[] { "a", "b", "c" }.Should().NotContainInConsecutiveOrder("a", "b"));
    }

    [Fact]
    public void NotContainAll_WhenNotAllPresent_Succeeds()
    {
        new[] { "a", "b" }.Should().NotContainAll("a", "b", "c");
    }

    [Fact]
    public void NotContainAll_WhenAllPresent_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            new[] { "a", "b", "c" }.Should().NotContainAll("a", "b", "c"));
    }

    [Fact]
    public void NotContainAny_WhenNonePresent_Succeeds()
    {
        new[] { "a", "b" }.Should().NotContainAny("x", "y", "z");
    }

    [Fact]
    public void NotContainAny_WhenAnyPresent_Fails()
    {
        Assert.Throws<AssertionFailedException>(() =>
            new[] { "a", "b" }.Should().NotContainAny("x", "b", "z"));
    }

    [Fact]
    public void And_ReturnsSubjectAssertion()
    {
        new[] { "a" }.Should().And.Should().BeEquivalentTo(new[] { "a" });
    }

    [Fact]
    public void AssertionScope_CollectsAllFailures()
    {
        var scope = new AssertionScope();
        new[] { "a", "b" }.Should().Contain("c");
        new[] { "a", "b" }.Should().HaveCount(5);
        new[] { "a", "b" }.Should().BeEmpty();
        var ex = Assert.Throws<AssertionFailedException>(() => scope.Dispose());
        Assert.Contains("Multiple assertion failures (3)", ex.Message);
    }
}
