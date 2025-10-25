namespace BufferLists;

[TestClass]
public class Doji : BufferListTestBase
{
    private const double maxPriceChangePercent = 0.1;

    private static readonly IReadOnlyList<CandleResult> series
       = Quotes.ToDoji(maxPriceChangePercent);

    [TestMethod]
    public void AddQuotes()
    {
        DojiList sut = new(maxPriceChangePercent);

        foreach (Quote quote in Quotes)
        {
            sut.Add(quote);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddQuotesBatch()
    {
        DojiList sut = Quotes.ToDojiList(maxPriceChangePercent);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void WithQuotesCtor()
    {
        DojiList sut = new(maxPriceChangePercent, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<CandleResult> expected = subset.ToDoji(maxPriceChangePercent);

        DojiList sut = new(maxPriceChangePercent, subset);

        sut.Should().HaveCount(subset.Count);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());

        sut.Clear();

        sut.Should().BeEmpty();

        sut.Add(subset);

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void PruneList_OverMaxListSize_AutoAdjustsListAndBuffers()
    {
        const int maxListSize = 120;

        DojiList sut = new(maxPriceChangePercent) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<CandleResult> expected = series
            .Skip(series.Count - maxListSize)
            .ToList();

        sut.Should().HaveCount(maxListSize);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }
}
