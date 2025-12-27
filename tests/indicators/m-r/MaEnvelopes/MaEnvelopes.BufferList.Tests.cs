namespace BufferLists;

[TestClass]
public class MaEnvelopes : BufferListTestBase, ITestChainBufferList
{
    private const int lookbackPeriods = 20;
    private const double percentOffset = 2.5;

    private static readonly IReadOnlyList<IReusable> reusables
       = Quotes
        .Cast<IReusable>()
        .ToList();

    private static readonly IReadOnlyList<MaEnvelopeResult> series
       = Quotes.ToMaEnvelopes(lookbackPeriods, percentOffset);

    [TestMethod]
    public void AddQuote_IncrementsResults()
    {
        MaEnvelopesList sut = new(lookbackPeriods, percentOffset);

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
        MaEnvelopesList sut = Quotes.ToMaEnvelopesList(lookbackPeriods, percentOffset);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void QuotesCtor_OnInstantiation_IncrementsResults()
    {
        MaEnvelopesList sut = new(lookbackPeriods, percentOffset, MaType.SMA, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddReusableItem_IncrementsResults()
    {
        MaEnvelopesList sut = new(lookbackPeriods, percentOffset);

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
        MaEnvelopesList sut = new(lookbackPeriods, percentOffset) { reusables };

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddDateAndValue_IncrementsResults()
    {
        MaEnvelopesList sut = new(lookbackPeriods, percentOffset);

        foreach (IReusable item in reusables)
        {
            sut.Add(item.Timestamp, item.Value);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void WithEmaType()
    {
        IReadOnlyList<MaEnvelopeResult> expected = Quotes.ToMaEnvelopes(
            lookbackPeriods, percentOffset, MaType.EMA);

        MaEnvelopesList sut = Quotes.ToMaEnvelopesList(
            lookbackPeriods, percentOffset, MaType.EMA);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(expected);
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<MaEnvelopeResult> expected = subset.ToMaEnvelopes(lookbackPeriods, percentOffset);

        MaEnvelopesList sut = new(lookbackPeriods, percentOffset, MaType.SMA, subset);

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

        MaEnvelopesList sut = new(lookbackPeriods, percentOffset) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<MaEnvelopeResult> expected = series
            .Skip(series.Count - maxListSize)
            .ToList();

        sut.Should().HaveCount(maxListSize);
        sut.IsExactly(expected);
    }
}
