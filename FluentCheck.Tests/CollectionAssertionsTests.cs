namespace FluentCheck.Tests;

public class CollectionAssertionsTests
{
    [Fact] public void BeEmpty_empty() => Array.Empty<int>().Should().BeEmpty();
    [Fact] public void BeEmpty_fails() => Assert.Throws<AssertionFailedException>(() => new[] { 1 }.Should().BeEmpty());

    [Fact] public void Contain_found() => new[] { 1, 2 }.Should().Contain(1);
    [Fact] public void Contain_notFound() => Assert.Throws<AssertionFailedException>(() => new[] { 1 }.Should().Contain(2));

    [Fact] public void NotContain_notFound() => new[] { 1 }.Should().NotContain(2);
    [Fact] public void NotContain_found() => Assert.Throws<AssertionFailedException>(() => new[] { 1 }.Should().NotContain(1));

    [Fact] public void HaveCount_correct() => new[] { 1, 2 }.Should().HaveCount(2);
    [Fact] public void HaveCount_wrong() => Assert.Throws<AssertionFailedException>(() => new[] { 1, 2 }.Should().HaveCount(3));

    [Fact] public void NotHaveCount_wrong() => new[] { 1 }.Should().NotHaveCount(2);
    [Fact] public void NotHaveCount_same() => Assert.Throws<AssertionFailedException>(() => new[] { 1 }.Should().NotHaveCount(1));

    [Fact] public void OnlyContain_allMatch() => new[] { 2, 4 }.Should().OnlyContain(x => x % 2 == 0);
    [Fact] public void OnlyContain_someFail() => Assert.Throws<AssertionFailedException>(() => new[] { 1, 2 }.Should().OnlyContain(x => x % 2 == 0));

    [Fact] public void NotOnlyContain_someFail() => new[] { 1, 2 }.Should().NotOnlyContain(x => x > 1);
    [Fact] public void NotOnlyContain_allMatch() => Assert.Throws<AssertionFailedException>(() => new[] { 1, 2 }.Should().NotOnlyContain(x => x > 0));

    [Fact] public void ContainInOrder_found() => new[] { 1, 2, 3 }.Should().ContainInOrder(1, 3);
    [Fact] public void ContainInOrder_notFound() => Assert.Throws<AssertionFailedException>(() => new[] { 3, 2, 1 }.Should().ContainInOrder(1, 3));

    [Fact] public void ContainInConsecutiveOrder_found() => new[] { 1, 2, 3 }.Should().ContainInConsecutiveOrder(2, 3);
    [Fact] public void ContainInConsecutiveOrder_notFound() => Assert.Throws<AssertionFailedException>(() => new[] { 1, 3, 2 }.Should().ContainInConsecutiveOrder(2, 3));

    [Fact] public void NotContainInOrder_found() => Assert.Throws<AssertionFailedException>(() => new[] { 1, 2, 3 }.Should().NotContainInOrder(1, 3));
    [Fact] public void NotContainInConsecutiveOrder_found() => Assert.Throws<AssertionFailedException>(() => new[] { 1, 2, 3 }.Should().NotContainInConsecutiveOrder(1, 2));

    [Fact] public void ContainSingle_exact() => new[] { 1 }.Should().ContainSingle();
    [Fact] public void ContainSingle_predicate() => new[] { 1, 3, 5 }.Should().ContainSingle(x => x > 4);
    [Fact] public void ContainSingle_empty() => Assert.Throws<AssertionFailedException>(() => Array.Empty<int>().Should().ContainSingle());

    [Fact] public void NotContainSingle_multiple() => new[] { 1, 2 }.Should().NotContainSingle();
    [Fact] public void NotContainSingle_single() => Assert.Throws<AssertionFailedException>(() => new[] { 1 }.Should().NotContainSingle());

    [Fact] public void StartWith_match() => new[] { 1, 2 }.Should().StartWith(1);
    [Fact] public void StartWith_mismatch() => Assert.Throws<AssertionFailedException>(() => new[] { 2 }.Should().StartWith(1));

    [Fact] public void NotStartWith_match() => Assert.Throws<AssertionFailedException>(() => new[] { 1 }.Should().NotStartWith(1));

    [Fact] public void EndWith_match() => new[] { 1, 2 }.Should().EndWith(2);
    [Fact] public void NotEndWith_match() => Assert.Throws<AssertionFailedException>(() => new[] { 1, 2 }.Should().NotEndWith(2));

    [Fact] public void Satisfy_found() => new[] { 1, 2 }.Should().Satisfy(x => x == 2);
    [Fact] public void Satisfy_notFound() => Assert.Throws<AssertionFailedException>(() => new[] { 1 }.Should().Satisfy(x => x == 2));

    [Fact] public void NotSatisfy_notFound() => new[] { 1 }.Should().NotSatisfy(x => x == 2);
    [Fact] public void NotSatisfy_found() => Assert.Throws<AssertionFailedException>(() => new[] { 1, 2 }.Should().NotSatisfy(x => x == 2));

    [Fact] public void OnlyHaveUniqueItems_allUnique() => new[] { 1, 2, 3 }.Should().OnlyHaveUniqueItems();
    [Fact] public void OnlyHaveUniqueItems_duplicates() => Assert.Throws<AssertionFailedException>(() => new[] { 1, 2, 2 }.Should().OnlyHaveUniqueItems());

    [Fact] public void OnlyHaveUniqueItems_byKey() => new object[] { "a", "bb" }.Should().OnlyHaveUniqueItems(x => ((string)x).Length);
    [Fact] public void OnlyHaveUniqueItems_byKey_duplicates() => Assert.Throws<AssertionFailedException>(() => new object[] { "a", "b" }.Should().OnlyHaveUniqueItems(x => ((string)x).Length));

    [Fact] public void BeSubsetOf_isSubset() => new[] { 1, 2 }.Should().BeSubsetOf(new[] { 1, 2, 3 });
    [Fact] public void BeSubsetOf_notSubset() => Assert.Throws<AssertionFailedException>(() => new[] { 1, 4 }.Should().BeSubsetOf(new[] { 1, 2, 3 }));

    [Fact] public void HaveElementPreceding() => new[] { 1, 2, 3 }.Should().HaveElementPreceding(3, 2);
    [Fact] public void HaveElementSucceeding() => new[] { 1, 2, 3 }.Should().HaveElementSucceeding(2, 3);

    [Fact] public void Equal_same() => new[] { 1, 2 }.Should().Equal(new[] { 1, 2 });
    [Fact] public void Equal_different() => Assert.Throws<AssertionFailedException>(() => new[] { 1, 2 }.Should().Equal(new[] { 1, 3 }));
    [Fact] public void Equal_differentCount() => Assert.Throws<AssertionFailedException>(() => new[] { 1 }.Should().Equal(new[] { 1, 2 }));
}
