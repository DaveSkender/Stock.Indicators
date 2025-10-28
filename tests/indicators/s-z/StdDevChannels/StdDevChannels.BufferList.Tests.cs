namespace BufferLists;

[TestClass]
public class StdDevChannels : BufferListTestBase, ITestChainBufferList
{
    private const int lookbackPeriods = 20;
    private const double stdDeviations = 2;

    private static readonly IReadOnlyList<IReusable> reusables
       = Quotes
        .Cast<IReusable>()
        .ToList();

    private static readonly IReadOnlyList<StdDevChannelsResult> series
       = Quotes.ToStdDevChannels(lookbackPeriods, stdDeviations);

    [TestMethod]
    public void AddQuote_IncrementsResults()
    {
        StdDevChannelsList sut = new(lookbackPeriods, stdDeviations);

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
        StdDevChannelsList sut = Quotes.ToStdDevChannelsList(lookbackPeriods, stdDeviations);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void QuotesCtor_OnInstantiation_IncrementsResults()
    {
        StdDevChannelsList sut = new(lookbackPeriods, stdDeviations, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddReusableItem_IncrementsResults()
    {
        StdDevChannelsList sut = new(lookbackPeriods, stdDeviations);

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
        StdDevChannelsList sut = new(lookbackPeriods, stdDeviations) { reusables };

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddDateAndValue_IncrementsResults()
    {
        StdDevChannelsList sut = new(lookbackPeriods, stdDeviations);

        foreach (IReusable item in reusables)
        {
            sut.Add(item.Timestamp, item.Value);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<StdDevChannelsResult> expected = subset.ToStdDevChannels(lookbackPeriods, stdDeviations);

        StdDevChannelsList sut = new(lookbackPeriods, stdDeviations, subset);

        sut.Should().HaveCount(subset.Count);
        sut.Should().BeEquivalentTo(expected, static options => options.WithStrictOrdering());

        sut.Clear();

        sut.Should().BeEmpty();

        sut.Add(subset);

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void PruneList_OverMaxListSize_AutoAdjustsListAndBuffers()
    {
        const int maxListSize = 120;

        StdDevChannelsList sut = new(lookbackPeriods, stdDeviations) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<StdDevChannelsResult> expected = series
            .Skip(series.Count - maxListSize)
            .ToList();

        sut.Should().HaveCount(maxListSize);
        sut.Should().BeEquivalentTo(expected, static options => options.WithStrictOrdering());
    }
}
