namespace BufferLists;

[TestClass]
public class Atr : BufferListTestBase
{
    private const int lookbackPeriods = 14;

    private static readonly IReadOnlyList<AtrResult> series
       = Quotes.ToAtr(lookbackPeriods);

    [TestMethod]
    public override void FromQuote()
    {
        AtrList sut = new(lookbackPeriods);

        foreach (Quote q in Quotes) { sut.Add(q); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public override void FromQuoteBatch()
    {
        AtrList sut = new(lookbackPeriods) { Quotes };

        IReadOnlyList<AtrResult> series
            = Quotes.ToAtr(lookbackPeriods);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();

        AtrList sut = new(lookbackPeriods);

        foreach (Quote quote in subset)
        {
            sut.Add(quote);
        }

        sut.Should().HaveCount(subset.Count);

        sut.Clear();

        sut.Should().BeEmpty();

        foreach (Quote quote in subset)
        {
            sut.Add(quote);
        }

        IReadOnlyList<AtrResult> expected = subset.ToAtr(lookbackPeriods);

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected);
    }
}
