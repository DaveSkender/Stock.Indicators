namespace BufferLists;

[TestClass]
public class Marubozu : BufferListTestBase
{
    private const double minBodyPercent = 95;

    private static readonly IReadOnlyList<CandleResult> series
       = Quotes.ToMarubozu(minBodyPercent);

    [TestMethod]
    public void AddQuotes()
    {
        MarubozuList sut = new(minBodyPercent);

        foreach (Quote quote in Quotes)
        {
            sut.Add(quote);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddQuotesBatch()
    {
        MarubozuList sut = new(minBodyPercent) { Quotes };

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void WithQuotesCtor()
    {
        MarubozuList sut = new(minBodyPercent, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<CandleResult> expected = subset.ToMarubozu(minBodyPercent);

        MarubozuList sut = new(minBodyPercent, subset);

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

        MarubozuList sut = new(minBodyPercent) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<CandleResult> expected = series
            .Skip(series.Count - maxListSize)
            .ToList();

        sut.Should().HaveCount(maxListSize);
        sut.IsExactly(expected);
    }
}
