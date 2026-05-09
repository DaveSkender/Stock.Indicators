namespace BufferLists;

[TestClass]
public class Stc : BufferListTestBase, ITestChainBufferList
{
    private const int cyclePeriods = 9;
    private const int fastPeriods = 12;
    private const int slowPeriods = 26;

    private static readonly IReadOnlyList<IReusable> reusables
       = Quotes
        .Cast<IReusable>()
        .ToList();

    private static readonly IReadOnlyList<StcResult> series
       = Quotes.ToStc(cyclePeriods, fastPeriods, slowPeriods);

    [TestMethod]
    public void AddQuote_IncrementsResults()
    {
        StcList sut = new(cyclePeriods, fastPeriods, slowPeriods);

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
        StcList sut = Quotes.ToStcList(cyclePeriods, fastPeriods, slowPeriods);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void QuotesCtor_OnInstantiation_IncrementsResults()
    {
        StcList sut = new(cyclePeriods, fastPeriods, slowPeriods, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddReusableItem_IncrementsResults()
    {
        StcList sut = new(cyclePeriods, fastPeriods, slowPeriods);

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
        StcList sut = new(cyclePeriods, fastPeriods, slowPeriods) { reusables };

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddDateAndValue_IncrementsResults()
    {
        StcList sut = new(cyclePeriods, fastPeriods, slowPeriods);

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
        List<Quote> subset = Quotes.Take(100).ToList();
        IReadOnlyList<StcResult> expected = subset.ToStc(cyclePeriods, fastPeriods, slowPeriods);

        StcList sut = new(cyclePeriods, fastPeriods, slowPeriods, subset);

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

        StcList sut = new(cyclePeriods, fastPeriods, slowPeriods) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<StcResult> expected = series
            .Skip(series.Count - maxListSize)
            .ToList();

        sut.Should().HaveCount(maxListSize);
        sut.IsExactly(expected);
    }

    [TestMethod]
    public void Results_AreAlwaysBounded()
    {
        StcList sut = new(9, 12, 26, Quotes);
        sut.IsBetween(static x => x.Stc, 0, 100);
    }
}
