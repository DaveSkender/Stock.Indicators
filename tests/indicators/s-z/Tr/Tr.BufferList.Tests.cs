namespace BufferLists;

[TestClass]
public class Tr : BufferListTestBase
{
    private static readonly IReadOnlyList<TrResult> series
       = Quotes.ToTr();

    [TestMethod]
    public override void FromQuote()
    {
        TrList sut = new();

        foreach (Quote q in Quotes) { sut.Add(q); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public override void FromQuoteBatch()
    {
        TrList sut = new() { Quotes };

        IReadOnlyList<TrResult> series
            = Quotes.ToTr();

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void FromQuotesCtor()
    {
        TrList sut = new(Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();

        TrList sut = new(subset);

        sut.Should().HaveCount(subset.Count);

        sut.Clear();

        sut.Should().BeEmpty();

        foreach (Quote quote in subset)
        {
            sut.Add(quote);
        }

        IReadOnlyList<TrResult> expected = subset.ToTr();

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected);
    }
}
