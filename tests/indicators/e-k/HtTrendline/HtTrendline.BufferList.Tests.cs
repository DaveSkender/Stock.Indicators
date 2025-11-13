namespace BufferLists;

[TestClass]
public class HtTrendline : BufferListTestBase, ITestChainBufferList, ITestCustomBufferListCache
{
    private static readonly IReadOnlyList<IReusable> reusables
       = Quotes
        .Cast<IReusable>()
        .ToList();

    private static readonly IReadOnlyList<HtlResult> series
       = Quotes.ToHtTrendline();

    [TestMethod]
    public void AddQuote_IncrementsResults()
    {
        HtlList sut = new();

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
        HtlList sut = new() { Quotes };

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void QuotesCtor_OnInstantiation_IncrementsResults()
    {
        HtlList sut = new(Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddReusableItem_IncrementsResults()
    {
        HtlList sut = new();

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
        HtlList sut = new() { reusables };

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddDateAndValue_IncrementsResults()
    {
        HtlList sut = new();

        foreach (IReusable item in reusables)
        {
            sut.Add(item.Timestamp, item.Hl2OrValue());
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<HtlResult> expected = subset.ToHtTrendline();

        HtlList sut = new(subset);

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

        HtlList sut = new() { MaxListSize = maxListSize };

        sut.Add(Quotes);

        IReadOnlyList<HtlResult> expected = series
            .Skip(series.Count - maxListSize)
            .ToList();

        sut.Should().HaveCount(maxListSize);
        sut.Should().BeEquivalentTo(expected, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void CustomBuffer_OverMaxListSize_AutoAdjustsListAndBuffers()
    {
        const int maxListSize = 200;
        const int quotesSize = 750;

        // Use test data that exceeds custom cache thresholds
        List<Quote> quotes = LongishQuotes
            .Take(quotesSize)
            .ToList();

        // Expected results after pruning (tail end)
        IReadOnlyList<HtlResult> expected = quotes
            .ToHtTrendline()
            .Skip(quotesSize - maxListSize)
            .ToList();

        // Generate buffer list
        HtlList sut = new(quotes) { MaxListSize = maxListSize };

        sut.Should().HaveCount(maxListSize);
        sut.Should().BeEquivalentTo(expected, static options => options.WithStrictOrdering());

        // Add more quotes to verify continued operation after pruning
        List<Quote> moreQuotes = LongishQuotes.Skip(quotesSize).Take(50).ToList();
        sut.Add(moreQuotes);

        IReadOnlyList<HtlResult> allSeries = quotes
            .Concat(moreQuotes)
            .ToList()
            .ToHtTrendline();

        IReadOnlyList<HtlResult> finalExpected = allSeries
            .Skip(allSeries.Count - maxListSize)
            .ToList();

        sut.Should().HaveCount(maxListSize);
        sut.Should().BeEquivalentTo(finalExpected, static options => options.WithStrictOrdering());
    }
}
