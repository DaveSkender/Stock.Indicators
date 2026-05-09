namespace BufferLists;

[TestClass]
public class Ichimoku : BufferListTestBase, ITestQuoteBufferList, ITestCustomBufferListCache
{
    private const int tenkanPeriods = 9;
    private const int kijunPeriods = 26;
    private const int senkouBPeriods = 52;
    private const int senkouOffset = 26;
    private const int chikouOffset = 26;

    private static readonly IReadOnlyList<IchimokuResult> series
        = Quotes.ToIchimoku(tenkanPeriods, kijunPeriods, senkouBPeriods, senkouOffset, chikouOffset);

    [TestMethod]
    public void AddQuote_IncrementsResults()
    {
        IchimokuList sut = new(tenkanPeriods, kijunPeriods, senkouBPeriods, senkouOffset, chikouOffset);

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
        IchimokuList sut = Quotes.ToIchimokuList(tenkanPeriods, kijunPeriods, senkouBPeriods, senkouOffset, chikouOffset);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void QuotesCtor_OnInstantiation_IncrementsResults()
    {
        IchimokuList sut = new(tenkanPeriods, kijunPeriods, senkouBPeriods, senkouOffset, chikouOffset, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<IchimokuResult> expected = subset.ToIchimoku(tenkanPeriods, kijunPeriods, senkouBPeriods, senkouOffset, chikouOffset);

        IchimokuList sut = new(tenkanPeriods, kijunPeriods, senkouBPeriods, senkouOffset, chikouOffset, subset);

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

        IchimokuList sut = new(tenkanPeriods, kijunPeriods, senkouBPeriods, senkouOffset, chikouOffset) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<IchimokuResult> expected
            = series.Skip(series.Count - maxListSize).ToList();

        sut.Should().HaveCount(maxListSize);
        sut.IsExactly(expected);
    }

    [TestMethod]
    public void CustomBuffer_OverMaxListSize_AutoAdjustsListAndBuffers()
    {
        const int maxListSize = 150;
        const int quotesSize = 1000;

        // Use a test data that exceeds all cache size thresholds
        List<Quote> quotes = LongishQuotes
            .Take(quotesSize)
            .ToList();

        // Expected results after pruning (tail end)
        IReadOnlyList<IchimokuResult> expected = quotes
            .ToIchimoku(tenkanPeriods, kijunPeriods, senkouBPeriods, senkouOffset, chikouOffset)
            .Skip(quotesSize - maxListSize)
            .ToList();

        // Generate buffer list
        IchimokuList sut = new(tenkanPeriods, kijunPeriods, senkouBPeriods, senkouOffset, chikouOffset, quotes) {
            MaxListSize = maxListSize
        };

        // Verify expected results matching equivalent series values
        sut.Count.Should().Be(maxListSize);
        sut.IsExactly(expected);
    }
}
