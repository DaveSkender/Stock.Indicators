namespace BufferLists;

[TestClass]
public class SuperTrend : BufferListTestBase, ITestQuoteBufferList
{
    private const int lookbackPeriods = 10;
    private const double multiplier = 3;

    private static readonly IReadOnlyList<SuperTrendResult> series
       = Quotes.ToSuperTrend(lookbackPeriods, multiplier);

    [TestMethod]
    public void AddQuote_IncrementsResults()
    {
        SuperTrendList sut = new(lookbackPeriods, multiplier);

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
        SuperTrendList sut = new(lookbackPeriods, multiplier) { Quotes };

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void QuotesCtor_OnInstantiation_IncrementsResults()
    {
        SuperTrendList sut = new(lookbackPeriods, multiplier, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<SuperTrendResult> expected = subset.ToSuperTrend(lookbackPeriods, multiplier);

        SuperTrendList sut = new(lookbackPeriods, multiplier, subset);

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

        SuperTrendList sut = new(lookbackPeriods, multiplier) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<SuperTrendResult> expected = series
            .Skip(series.Count - maxListSize)
            .ToList();

        sut.Should().HaveCount(maxListSize);
        sut.IsExactly(expected);
    }
}
