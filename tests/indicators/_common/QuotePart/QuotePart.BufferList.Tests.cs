namespace BufferLists;

[TestClass]
public class QuoteParts : BufferListTestBase, ITestQuoteBufferList
{
    private const CandlePart candlePart = CandlePart.Close;

    private static readonly IReadOnlyList<QuotePart> series
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
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddQuotesBatch_IncrementsResults()
    {
        QuotePartList sut = Quotes.ToQuotePartList(candlePart);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void QuotesCtor_OnInstantiation_IncrementsResults()
    {
        QuotePartList sut = new(candlePart, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<QuotePart> expected = subset.ToQuotePart(candlePart);

        QuotePartList sut = new(candlePart, subset);

        sut.Should().HaveCount(subset.Count);
        sut.Should().BeEquivalentTo(expected, static options => options.WithStrictOrdering());

        sut.Clear();

        sut.Should().BeEmpty();

        sut.Add(subset);

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void PruneList_OverMaxListSize_AutoAdjustsListAndBuffers()
    {
        const int maxListSize = 120;

        QuotePartList sut = new(candlePart) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<QuotePart> expected = series
            .Skip(series.Count - maxListSize)
            .ToList();

        sut.Should().HaveCount(maxListSize);
        sut.Should().BeEquivalentTo(expected, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void MultipleCandleParts()
    {
        // Test different candle parts
        IReadOnlyList<QuotePart> openSeries = Quotes.ToQuotePart(CandlePart.Open);
        QuotePartList openList = new(CandlePart.Open, Quotes);
        openList.Should().BeEquivalentTo(openSeries, static options => options.WithStrictOrdering());

        IReadOnlyList<QuotePart> highSeries = Quotes.ToQuotePart(CandlePart.High);
        QuotePartList highList = new(CandlePart.High, Quotes);
        highList.Should().BeEquivalentTo(highSeries, static options => options.WithStrictOrdering());

        IReadOnlyList<QuotePart> hl2Series = Quotes.ToQuotePart(CandlePart.HL2);
        QuotePartList hl2List = new(CandlePart.HL2, Quotes);
        hl2List.Should().BeEquivalentTo(hl2Series, static options => options.WithStrictOrdering());
    }
}
