namespace BufferLists;

[TestClass]
public class QuoteParts : BufferListTestBase, ITestQuoteBufferList
{
    private const CandlePart candlePart = CandlePart.Close;

    private static readonly IReadOnlyList<TimeValue> series
       = Quotes.ToQuotePart(candlePart);

    [TestMethod]
    public void AddQuote_IncrementsResults()
    {
        QuotePartList sut = new(candlePart);

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
        QuotePartList sut = Quotes.ToQuotePartList(candlePart);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void QuotesCtor_OnInstantiation_IncrementsResults()
    {
        QuotePartList sut = new(candlePart, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<TimeValue> expected = subset.ToQuotePart(candlePart);

        QuotePartList sut = new(candlePart, subset);

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

        QuotePartList sut = new(candlePart) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<TimeValue> expected = series
            .Skip(series.Count - maxListSize)
            .ToList();

        sut.Should().HaveCount(maxListSize);
        sut.IsExactly(expected);
    }

    [TestMethod]
    public void MultipleCandleParts()
    {
        // Test different candle parts
        IReadOnlyList<TimeValue> openSeries = Quotes.ToQuotePart(CandlePart.Open);
        QuotePartList openList = new(CandlePart.Open, Quotes);
        openList.IsExactly(openSeries);

        IReadOnlyList<TimeValue> highSeries = Quotes.ToQuotePart(CandlePart.High);
        QuotePartList highList = new(CandlePart.High, Quotes);
        highList.IsExactly(highSeries);

        IReadOnlyList<TimeValue> hl2Series = Quotes.ToQuotePart(CandlePart.HL2);
        QuotePartList hl2List = new(CandlePart.HL2, Quotes);
        hl2List.IsExactly(hl2Series);
    }
}
