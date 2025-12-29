namespace BufferLists;

[TestClass]
public class StarcBands : BufferListTestBase, ITestQuoteBufferList
{
    private const int smaPeriods = 5;
    private const double multiplier = 2;
    private const int atrPeriods = 10;

    private static readonly IReadOnlyList<StarcBandsResult> series
       = Quotes.ToStarcBands(smaPeriods, multiplier, atrPeriods);

    [TestMethod]
    public void AddQuote_IncrementsResults()
    {
        StarcBandsList sut = new(smaPeriods, multiplier, atrPeriods);

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
        StarcBandsList sut = new(smaPeriods, multiplier, atrPeriods) { Quotes };

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void QuotesCtor_OnInstantiation_IncrementsResults()
    {
        StarcBandsList sut = new(smaPeriods, multiplier, atrPeriods, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<StarcBandsResult> expected = subset.ToStarcBands(smaPeriods, multiplier, atrPeriods);

        StarcBandsList sut = new(smaPeriods, multiplier, atrPeriods, subset);

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
        const int maxListSize = 100;

        StarcBandsList sut = new(smaPeriods, multiplier, atrPeriods) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<StarcBandsResult> expected = series
            .Skip(series.Count - maxListSize)
            .ToList();

        sut.Should().HaveCount(maxListSize);
        sut.IsExactly(expected);
    }
}
