namespace BufferLists;

[TestClass]
public class Vwap : BufferListTestBase, ITestQuoteBufferList
{
    private static readonly DateTime startDate = DateTime.Parse("2018-12-31", invariantCulture);

    private static readonly IReadOnlyList<VwapResult> series
       = Quotes.ToVwap(startDate);

    private static readonly IReadOnlyList<VwapResult> seriesDefault
       = Quotes.ToVwap();

    [TestMethod]
    public void AddQuote_IncrementsResults()
    {
        VwapList sut = new(startDate);

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
        VwapList sut = Quotes.ToVwapList(startDate);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void QuotesCtor_OnInstantiation_IncrementsResults()
    {
        VwapList sut = new(startDate, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void DefaultStartDate()
    {
        VwapList sut = Quotes.ToVwapList();

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(seriesDefault);
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<VwapResult> expected = subset.ToVwap(startDate);

        VwapList sut = new(startDate, subset);

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

        VwapList sut = new(startDate) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<VwapResult> expected = series
            .Skip(series.Count - maxListSize)
            .ToList();

        sut.Should().HaveCount(maxListSize);
        sut.IsExactly(expected);
    }
}
