namespace BufferLists;

[TestClass]
public class Vwma : BufferListTestBase
{
    private const int lookbackPeriods = 10;

    private static readonly IReadOnlyList<VwmaResult> series
       = Quotes.ToVwma(lookbackPeriods);

    [TestMethod]
    public override void FromQuote()
    {
        VwmaList sut = new(lookbackPeriods);

        foreach (Quote q in Quotes) { sut.Add(q); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public override void FromQuoteBatch()
    {
        VwmaList sut = new(lookbackPeriods) { Quotes };

        IReadOnlyList<VwmaResult> series
            = Quotes.ToVwma(lookbackPeriods);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void FromQuotesCtor()
    {
        VwmaList sut = new(lookbackPeriods, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();

        VwmaList sut = new(lookbackPeriods, subset);

        sut.Should().HaveCount(subset.Count);

        sut.Clear();

        sut.Should().BeEmpty();

        foreach (Quote quote in subset)
        {
            sut.Add(quote);
        }

        IReadOnlyList<VwmaResult> expected = subset.ToVwma(lookbackPeriods);

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected);
    }
}
