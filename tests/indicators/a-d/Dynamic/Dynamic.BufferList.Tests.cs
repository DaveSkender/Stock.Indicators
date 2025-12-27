namespace BufferLists;

[TestClass]
public class MgDynamic : BufferListTestBase, ITestChainBufferList
{
    private const int lookbackPeriods = 14;
    private const double kFactor = 0.6;

    private static readonly IReadOnlyList<IReusable> reusables
       = Quotes
        .Cast<IReusable>()
        .ToList();

    private static readonly IReadOnlyList<DynamicResult> series
       = Quotes.ToDynamic(lookbackPeriods, kFactor);

    [TestMethod]
    public void AddQuote_IncrementsResults()
    {
        DynamicList sut = new(lookbackPeriods, kFactor);

        foreach (Quote q in Quotes) { sut.Add(q); }

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddQuotesBatch_IncrementsResults()
    {
        DynamicList sut = new(lookbackPeriods, kFactor) { Quotes };

        IReadOnlyList<DynamicResult> series
            = Quotes.ToDynamic(lookbackPeriods, kFactor);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void QuotesCtor_OnInstantiation_IncrementsResults()
    {
        DynamicList sut = new(lookbackPeriods, kFactor, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddReusableItem_IncrementsResults()
    {
        DynamicList sut = new(lookbackPeriods, kFactor);

        foreach (IReusable item in reusables) { sut.Add(item); }

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddReusableItemBatch_IncrementsResults()
    {
        DynamicList sut = new(lookbackPeriods, kFactor) { reusables };

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddDateAndValue_IncrementsResults()
    {
        DynamicList sut = new(lookbackPeriods, kFactor);

        foreach (IReusable item in reusables)
        {
            sut.Add(item.Timestamp, item.Value);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public override void PruneList_OverMaxListSize_AutoAdjustsListAndBuffers()
    {
        const int maxListSize = 120;

        DynamicList sut = new(lookbackPeriods, kFactor) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<DynamicResult> expected
            = series.Skip(series.Count - maxListSize).ToList();

        sut.Should().HaveCount(maxListSize);
        sut.IsExactly(expected);
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<DynamicResult> expected = subset.ToDynamic(lookbackPeriods, kFactor);

        DynamicList sut = new(lookbackPeriods, kFactor, subset);

        sut.Should().HaveCount(subset.Count);
        sut.IsExactly(expected);

        sut.Clear();

        sut.Should().BeEmpty();

        sut.Add(subset);

        sut.Should().HaveCount(expected.Count);
        sut.IsExactly(expected);
    }
}
