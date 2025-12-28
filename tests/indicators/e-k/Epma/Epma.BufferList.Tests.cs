namespace BufferLists;

[TestClass]
public class Epma : BufferListTestBase, ITestChainBufferList, ITestCustomBufferListCache
{
    private const int lookbackPeriods = 20;

    private static readonly IReadOnlyList<IReusable> reusables
       = Quotes
        .Cast<IReusable>()
        .ToList();

    private static readonly IReadOnlyList<EpmaResult> series
       = Quotes.ToEpma(lookbackPeriods);

    [TestMethod]
    public void AddQuote_IncrementsResults()
    {
        EpmaList sut = new(lookbackPeriods);

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
        EpmaList sut = new(lookbackPeriods) { Quotes };

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void QuotesCtor_OnInstantiation_IncrementsResults()
    {
        EpmaList sut = new(lookbackPeriods, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddReusableItem_IncrementsResults()
    {
        EpmaList sut = new(lookbackPeriods);

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
        EpmaList sut = new(lookbackPeriods) { reusables };

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddDateAndValue_IncrementsResults()
    {
        EpmaList sut = new(lookbackPeriods);

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
        IReadOnlyList<EpmaResult> expected = subset.ToEpma(lookbackPeriods);

        EpmaList sut = new(lookbackPeriods, subset);

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
        const int maxListSize = 120;

        EpmaList sut = new(lookbackPeriods) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<EpmaResult> expected = series
            .Skip(series.Count - maxListSize)
            .ToList();

        sut.Should().HaveCount(maxListSize);
        sut.IsExactly(expected);
    }

    [TestMethod]
    public void CustomBuffer_OverMaxListSize_AutoAdjustsListAndBuffers()
    {
        const int maxListSize = 150;
        const int quotesSize = 1250;

        // Use a test data that exceeds all cache size thresholds
        List<Quote> quotes = LongishQuotes
            .Take(quotesSize)
            .ToList();

        // Expected results after pruning (tail end)
        IReadOnlyList<EpmaResult> expected = quotes
            .ToEpma(lookbackPeriods)
            .Skip(quotesSize - maxListSize)
            .ToList();

        // Generate buffer list
        EpmaList sut = new(lookbackPeriods, quotes) {
            MaxListSize = maxListSize
        };

        // Verify expected results matching equivalent series values
        sut.Count.Should().Be(maxListSize);
        sut.IsExactly(expected);
    }
}
