namespace BufferLists;

[TestClass]
public class BollingerBands : BufferListTestBase, ITestChainBufferList
{
    private const int lookbackPeriods = 20;
    private const double standardDeviations = 2;

    private static readonly IReadOnlyList<IReusable> reusables
        = Quotes
            .Cast<IReusable>()
            .ToList();

    private static readonly IReadOnlyList<BollingerBandsResult> series
        = Quotes.ToBollingerBands(lookbackPeriods, standardDeviations);

    [TestMethod]
    public void AddQuote_IncrementsResults()
    {
        BollingerBandsList sut = new(lookbackPeriods, standardDeviations);

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
        BollingerBandsList sut = new(lookbackPeriods, standardDeviations) { Quotes };

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void QuotesCtor_OnInstantiation_IncrementsResults()
    {
        BollingerBandsList sut = new(lookbackPeriods, standardDeviations, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddReusableItem_IncrementsResults()
    {
        BollingerBandsList sut = new(lookbackPeriods, standardDeviations);

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
        BollingerBandsList sut = new(lookbackPeriods, standardDeviations) { reusables };

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddDateAndValue_IncrementsResults()
    {
        BollingerBandsList sut = new(lookbackPeriods, standardDeviations);

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
        IReadOnlyList<BollingerBandsResult> expected = subset.ToBollingerBands(lookbackPeriods, standardDeviations);

        BollingerBandsList sut = new(lookbackPeriods, standardDeviations, subset);

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

        BollingerBandsList sut = new(lookbackPeriods, standardDeviations) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<BollingerBandsResult> expected = series
            .Skip(series.Count - maxListSize)
            .ToList();

        sut.Should().HaveCount(maxListSize);
        sut.IsExactly(expected);
    }
}
