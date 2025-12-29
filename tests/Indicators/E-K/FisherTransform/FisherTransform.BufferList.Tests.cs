namespace BufferLists;

[TestClass]
public class FisherTransform : BufferListTestBase, ITestChainBufferList
{
    private const int lookbackPeriods = 10;

    private static readonly IReadOnlyList<IReusable> reusables
       = Quotes
        .Cast<IReusable>()
        .ToList();

    private static readonly IReadOnlyList<FisherTransformResult> series
       = Quotes.ToFisherTransform(lookbackPeriods);

    [TestMethod]
    public void AddQuote_IncrementsResults()
    {
        FisherTransformList sut = new(lookbackPeriods);

        foreach (Quote q in Quotes) { sut.Add(q); }

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddQuotesBatch_IncrementsResults()
    {
        FisherTransformList sut = new(lookbackPeriods) { Quotes };

        IReadOnlyList<FisherTransformResult> series
            = Quotes.ToFisherTransform(lookbackPeriods);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void QuotesCtor_OnInstantiation_IncrementsResults()
    {
        FisherTransformList sut = new(lookbackPeriods, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public override void PruneList_OverMaxListSize_AutoAdjustsListAndBuffers()
    {
        const int maxListSize = 120;

        FisherTransformList sut = new(lookbackPeriods) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<FisherTransformResult> expected
            = series.Skip(series.Count - maxListSize).ToList();

        sut.Should().HaveCount(maxListSize);
        sut.IsExactly(expected);
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<FisherTransformResult> expected = subset.ToFisherTransform(lookbackPeriods);

        FisherTransformList sut = new(lookbackPeriods, subset);

        sut.Should().HaveCount(subset.Count);
        sut.IsExactly(expected);

        sut.Clear();

        sut.Should().BeEmpty();

        sut.Add(subset);

        sut.Should().HaveCount(expected.Count);
        sut.IsExactly(expected);
    }

    [TestMethod]
    public void AddDateAndValue_IncrementsResults()
    {
        FisherTransformList sut = new(lookbackPeriods);

        foreach (IReusable item in reusables)
        {
            sut.Add(item.Timestamp, item.Value);
        }

        // For FisherTransform with IReusable, we're using Close values
        // whereas with IQuote we use HL2, so we need to compare to reusable series
        IReadOnlyList<FisherTransformResult> reusableSeries = reusables.ToFisherTransform(lookbackPeriods);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(reusableSeries);
    }

    [TestMethod]
    public void AddReusableItem_IncrementsResults()
    {
        FisherTransformList sut = new(lookbackPeriods);

        foreach (IReusable item in reusables)
        {
            sut.Add(item);
        }

        // FisherTransform with IReusable derived from IQuote should use HL2, same as IQuote
        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddReusableItemBatch_IncrementsResults()
    {
        FisherTransformList sut = new(lookbackPeriods) { reusables };

        // FisherTransform with IReusable derived from IQuote should use HL2, same as IQuote
        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }
}
