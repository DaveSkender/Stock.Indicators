namespace BufferLists;

[TestClass]
public class StochRsi : BufferListTestBase, ITestChainBufferList
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
    public void AddQuote_IncrementsResults()
    {
        StochRsiList sut = new(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);

        foreach (Quote quote in Quotes)
        {
            sut.Add(quote);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddQuotesBatch_IncrementsResults()
    {
        StochRsiList sut = Quotes.ToStochRsiList(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void QuotesCtor_OnInstantiation_IncrementsResults()
    {
        StochRsiList sut = new(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<StochRsiResult> expected = subset.ToStochRsi(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);

        StochRsiList sut = new(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods, subset);

        sut.Should().HaveCount(subset.Count);
        sut.Should().BeEquivalentTo(expected, static options => options.WithStrictOrdering());

        sut.Clear();

        sut.Should().BeEmpty();

        sut.Add(subset);

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddReusableItem_IncrementsResults()
    {
        StochRsiList sut = new(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);

        foreach (IReusable item in reusables)
        {
            sut.Add(item);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddReusableItemBatch_IncrementsResults()
    {
        StochRsiList sut = new(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods) { reusables };

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddDateAndValue_IncrementsResults()
    {
        StochRsiList sut = new(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);

        foreach (IReusable item in reusables)
        {
            sut.Add(item.Timestamp, item.Value);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void PruneList_OverMaxListSize_AutoAdjustsListAndBuffers()
    {
        const int maxListSize = 120;

        StochRsiList sut = new(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<StochRsiResult> expected = series
            .Skip(series.Count - maxListSize)
            .ToList();

        sut.Should().HaveCount(maxListSize);
        sut.Should().BeEquivalentTo(expected, static options => options.WithStrictOrdering());
    }
}
