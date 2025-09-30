namespace BufferLists;
#pragma warning disable IDE0028 // required for test case when no params

[TestClass]
public class Adl : BufferListTestBase
{
    private static readonly IReadOnlyList<AdlResult> series
       = Quotes.ToAdl();

    [TestMethod]
    public override void FromQuote()
    {
        AdlList sut = new();

        foreach (Quote q in Quotes) { sut.Add(q); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public override void FromQuoteBatch()
    {
        AdlList sut = new() { Quotes };

        IReadOnlyList<AdlResult> series
            = Quotes.ToAdl();

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();

        AdlList sut = new();

        foreach (Quote q in subset) { sut.Add(q); }

        sut.Should().HaveCount(subset.Count);

        sut.Clear();

        sut.Should().BeEmpty();

        foreach (Quote quote in subset)
        {
            sut.Add(quote);
        }

        IReadOnlyList<AdlResult> expected = subset.ToAdl();

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected);
    }
}
