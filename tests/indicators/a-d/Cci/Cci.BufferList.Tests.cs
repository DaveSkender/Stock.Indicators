namespace BufferLists;

[TestClass]
public class Cci : BufferListTestBase
{
    private const int lookbackPeriods = 20;

    private static readonly IReadOnlyList<CciResult> series
       = Quotes.ToCci(lookbackPeriods);

    [TestMethod]
    public override void FromQuote()
    {
        CciList sut = new(lookbackPeriods);

        foreach (Quote q in Quotes) { sut.Add(q); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public override void FromQuoteBatch()
    {
        CciList sut = new(lookbackPeriods) { Quotes };

        IReadOnlyList<CciResult> series
            = Quotes.ToCci(lookbackPeriods);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void FromQuotesCtor()
    {
        CciList sut = new(lookbackPeriods, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();

        CciList sut = new(lookbackPeriods, subset);

        sut.Should().HaveCount(subset.Count);

        sut.Clear();

        sut.Should().BeEmpty();

        foreach (Quote quote in subset)
        {
            sut.Add(quote);
        }

        IReadOnlyList<CciResult> expected = subset.ToCci(lookbackPeriods);

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected);
    }
}
