namespace BufferLists;

[TestClass]
public class Mama : BufferListTestBase, ITestChainBufferList, ITestCustomBufferListCache
{
    private const double fastLimit = 0.5;
    private const double slowLimit = 0.05;

    private static readonly IReadOnlyList<IReusable> reusables
       = Quotes
        .Use(CandlePart.HL2)  // HL2 values (not Close) for comparables
        .Cast<IReusable>()
        .ToList();

    private static readonly IReadOnlyList<MamaResult> series
       = Quotes.ToMama(fastLimit, slowLimit);

    [TestMethod]
    public void AddQuote_IncrementsResults()
    {
        MamaList sut = new(fastLimit, slowLimit);

        foreach (Quote quote in Quotes)
        {
            sut.Add(quote);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddQuotesBatch_IncrementsResults()
    {
        MamaList sut = new(fastLimit, slowLimit) { Quotes };

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void QuotesCtor_OnInstantiation_IncrementsResults()
    {
        MamaList sut = new(fastLimit, slowLimit, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddReusableItem_IncrementsResults()
    {
        MamaList sut = new(fastLimit, slowLimit);

        foreach (IReusable item in reusables)
        {
            sut.Add(item);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddReusableItemBatch_IncrementsResults()
    {
        MamaList sut = new(fastLimit, slowLimit) { reusables };

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddDateAndValue_IncrementsResults()
    {
        MamaList sut = new(fastLimit, slowLimit);

        foreach (IReusable item in reusables)
        {
            sut.Add(item.Timestamp, item.Value);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<MamaResult> expected = subset.ToMama(fastLimit, slowLimit);

        MamaList sut = new(fastLimit, slowLimit, subset);

        sut.Should().HaveCount(subset.Count);
        sut.IsExactly(expected);

        sut.Clear();

        sut.Should().BeEmpty();

        sut.Add(subset);

        sut.Should().HaveCount(expected.Count);
        sut.IsExactly(expected);
    }

    [TestMethod]
    public override void PruneList_OverMaxListSize_AutoAdjustsListAndBuffers()
    {
        const int maxListSize = 150;

        MamaList sut = new(fastLimit, slowLimit) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<MamaResult> expected = series
            .Skip(series.Count - maxListSize)
            .ToList();

        sut.Should().HaveCount(maxListSize);
        sut.IsExactly(expected);
    }

    [TestMethod]
    public void CustomBuffer_OverMaxListSize_AutoAdjustsListAndBuffers()
    {
        const int maxListSize = 200;
        const int quotesSize = 1250;

        // Use a test data that exceeds all cache size thresholds
        List<Quote> quotes = LongishQuotes
            .Take(quotesSize)
            .ToList();

        // Expected results after pruning (tail end)
        IReadOnlyList<MamaResult> expected = quotes
            .ToMama(fastLimit, slowLimit)
            .Skip(quotesSize - maxListSize)
            .ToList();

        // Generate buffer list
        MamaList sut = new(fastLimit, slowLimit, quotes) {
            MaxListSize = maxListSize
        };

        // Verify expected results matching equivalent series values
        sut.Count.Should().Be(maxListSize);
        sut.IsExactly(expected);
    }
}
