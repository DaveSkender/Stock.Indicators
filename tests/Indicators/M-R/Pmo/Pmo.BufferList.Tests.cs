namespace BufferLists;

[TestClass]
public class Pmo : BufferListTestBase, ITestChainBufferList, ITestCustomBufferListCache
{
    private const int timePeriods = 35;
    private const int smoothPeriods = 20;
    private const int signalPeriods = 10;

    private static readonly IReadOnlyList<IReusable> reusables
       = Quotes
        .Cast<IReusable>()
        .ToList();

    private static readonly IReadOnlyList<PmoResult> series
       = Quotes.ToPmo(timePeriods, smoothPeriods, signalPeriods);

    [TestMethod]
    public void AddDateAndValue_IncrementsResults()
    {
        PmoList sut = new(timePeriods, smoothPeriods, signalPeriods);

        foreach (IReusable item in reusables)
        {
            sut.Add(item.Timestamp, item.Value);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddReusableItem_IncrementsResults()
    {
        PmoList sut = new(timePeriods, smoothPeriods, signalPeriods);

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
        PmoList sut = new(timePeriods, smoothPeriods, signalPeriods) { reusables };

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddQuote_IncrementsResults()
    {
        PmoList sut = new(timePeriods, smoothPeriods, signalPeriods);

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
        PmoList sut = Quotes.ToPmoList(timePeriods, smoothPeriods, signalPeriods);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void QuotesCtor_OnInstantiation_IncrementsResults()
    {
        PmoList sut = new(timePeriods, smoothPeriods, signalPeriods, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<PmoResult> expected = subset.ToPmo(timePeriods, smoothPeriods, signalPeriods);

        PmoList sut = new(timePeriods, smoothPeriods, signalPeriods, subset);

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

        PmoList sut = new(timePeriods, smoothPeriods, signalPeriods) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<PmoResult> expected
            = series.Skip(series.Count - maxListSize).ToList();

        sut.Should().HaveCount(maxListSize);
        sut.IsExactly(expected);
    }

    [TestMethod]
    public void CustomBuffer_OverMaxListSize_AutoAdjustsListAndBuffers()
    {
        const int maxListSize = 120;

        PmoList sut = new(timePeriods, smoothPeriods, signalPeriods) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<PmoResult> expected
            = series.Skip(series.Count - maxListSize).ToList();

        sut.Should().HaveCount(maxListSize);
        sut.IsExactly(expected);

        // Add more quotes to verify continued operation after pruning
        List<Quote> moreQuotes = Quotes.TakeLast(50).ToList();
        sut.Add(moreQuotes);

        IReadOnlyList<PmoResult> allSeries = Quotes.Concat(moreQuotes).ToList()
            .ToPmo(timePeriods, smoothPeriods, signalPeriods);

        IReadOnlyList<PmoResult> expectedAfterMore
            = allSeries.Skip(allSeries.Count - maxListSize).ToList();

        sut.Should().HaveCount(maxListSize);
        sut.IsExactly(expectedAfterMore);
    }
}
