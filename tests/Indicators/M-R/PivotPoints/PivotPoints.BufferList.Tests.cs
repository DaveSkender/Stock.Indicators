namespace BufferLists;

[TestClass]
public class PivotPoints : BufferListTestBase, ITestQuoteBufferList
{
    private const PeriodSize windowSize = PeriodSize.Month;
    private const PivotPointType pointType = PivotPointType.Standard;

    private static readonly IReadOnlyList<PivotPointsResult> series
       = Quotes.ToPivotPoints(windowSize, pointType);

    [TestMethod]
    public void AddQuote_IncrementsResults()
    {
        PivotPointsList sut = new(windowSize, pointType);

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
        PivotPointsList sut = Quotes.ToPivotPointsList(windowSize, pointType);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void QuotesCtor_OnInstantiation_IncrementsResults()
    {
        PivotPointsList sut = new(windowSize, pointType, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<PivotPointsResult> expected = subset.ToPivotPoints(windowSize, pointType);

        PivotPointsList sut = new(windowSize, pointType, subset);

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

        PivotPointsList sut = new(windowSize, pointType) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<PivotPointsResult> expected
            = series.Skip(series.Count - maxListSize).ToList();

        sut.Should().HaveCount(maxListSize);
        sut.IsExactly(expected);
    }
}
