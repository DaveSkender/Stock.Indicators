namespace BufferLists;

[TestClass]
public class StochRsi : BufferListTestBase
{
    private const int rsiPeriods = 14;
    private const int stochPeriods = 14;
    private const int signalPeriods = 3;
    private const int smoothPeriods = 1;

    private static readonly IReadOnlyList<IReusable> reusables
       = Quotes
        .Cast<IReusable>()
        .ToList();

    private static readonly IReadOnlyList<StochRsiResult> series
       = Quotes.ToStochRsi(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);

    [TestMethod]
    public void FromReusableSplit()
    {
        StochRsiList sut = new(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);

        foreach (IReusable item in reusables)
        {
            sut.Add(item.Timestamp, item.Value);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void FromReusableItem()
    {
        StochRsiList sut = new(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);

        foreach (IReusable item in reusables) { sut.Add(item); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void FromReusableBatch()
    {
        StochRsiList sut = new(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods) { reusables };

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public override void FromQuote()
    {
        StochRsiList sut = new(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);

        foreach (Quote q in Quotes) { sut.Add(q); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public override void FromQuoteBatch()
    {
        StochRsiList sut = new(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods) { Quotes };

        IReadOnlyList<StochRsiResult> series
            = Quotes.ToStochRsi(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void FromQuotesCtor()
    {
        StochRsiList sut = new(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();

        StochRsiList sut = new(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods, subset);

        sut.Should().HaveCount(subset.Count);

        sut.Clear();

        sut.Should().BeEmpty();

        foreach (Quote quote in subset)
        {
            sut.Add(quote);
        }

        IReadOnlyList<StochRsiResult> expected = subset.ToStochRsi(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected);
    }
}
