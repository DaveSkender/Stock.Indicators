namespace BufferLists;

[TestClass]
public class Ema : BufferListTestBase, ITestChainBufferList
{
    private const int lookbackPeriods = 14;

    private static readonly IReadOnlyList<IReusable> reusables
       = Quotes.Cast<IReusable>().ToList();

    private static readonly IReadOnlyList<EmaResult> series
       = Quotes.ToEma(lookbackPeriods);

    [TestMethod]
    public void AddQuote_IncrementsResults()
    {
        EmaList sut = new(lookbackPeriods);

        foreach (Quote q in Quotes) { sut.Add(q); }

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddQuotesBatch_IncrementsResults()
    {
        EmaList sut = Quotes.ToEmaList(lookbackPeriods);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void QuotesCtor_OnInstantiation_IncrementsResults()
    {
        EmaList sut = new(lookbackPeriods, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddReusableItem_IncrementsResults()
    {
        EmaList sut = new(lookbackPeriods);

        foreach (IReusable item in reusables) { sut.Add(item); }

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddReusableItemBatch_IncrementsResults()
    {
        EmaList sut = new(lookbackPeriods) { reusables };

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddDateAndValue_IncrementsResults()
    {
        EmaList sut = new(lookbackPeriods);

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

        EmaList sut = new(lookbackPeriods) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<EmaResult> expected
            = series.Skip(series.Count - maxListSize).ToList();

        sut.Should().HaveCount(maxListSize);
        sut.IsExactly(expected);
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<EmaResult> expected = subset.ToEma(lookbackPeriods);

        EmaList sut = new(lookbackPeriods, subset);

        sut.Should().HaveCount(subset.Count);
        sut.IsExactly(expected);

        sut.Clear();

        sut.Should().BeEmpty();

        sut.Add(subset);

        sut.Should().HaveCount(expected.Count);
        sut.IsExactly(expected);
    }
}
