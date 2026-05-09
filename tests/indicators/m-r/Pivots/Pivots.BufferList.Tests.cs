namespace BufferLists;

[TestClass]
public class Pivots : BufferListTestBase, ITestQuoteBufferList
{
    private const int leftSpan = 4;
    private const int rightSpan = 4;
    private const int maxTrendPeriods = 20;

    private static readonly IReadOnlyList<PivotsResult> series
       = Quotes.ToPivots(leftSpan, rightSpan, maxTrendPeriods);

    [TestMethod]
    public void AddQuote_IncrementsResults()
    {
        PivotsList sut = new(leftSpan, rightSpan, maxTrendPeriods);

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
        PivotsList sut = Quotes.ToPivotsList(leftSpan, rightSpan, maxTrendPeriods);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void QuotesCtor_OnInstantiation_IncrementsResults()
    {
        PivotsList sut = new(leftSpan, rightSpan, maxTrendPeriods, EndType.HighLow, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<PivotsResult> expected = subset.ToPivots(leftSpan, rightSpan, maxTrendPeriods);

        PivotsList sut = new(leftSpan, rightSpan, maxTrendPeriods, EndType.HighLow, subset);

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

        PivotsList sut = new(leftSpan, rightSpan, maxTrendPeriods) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<PivotsResult> expected
            = series.Skip(series.Count - maxListSize).ToList();

        sut.Should().HaveCount(maxListSize);
        sut.IsExactly(expected);
    }
}
