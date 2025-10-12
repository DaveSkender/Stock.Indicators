namespace BufferLists;

[TestClass]
public class Marubozu : BufferListTestBase
{
    private const double minBodyPercent = 95;

    private static readonly IReadOnlyList<CandleResult> series
       = Quotes.ToMarubozu(minBodyPercent);

    [TestMethod]
    public override void AddQuotes()
    {
        MarubozuList sut = new(minBodyPercent);

        foreach (Quote quote in Quotes)
        {
            sut.Add(quote);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void AddQuotesBatch()
    {
        MarubozuList sut = Quotes.ToMarubozuList(minBodyPercent);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void WithQuotesCtor()
    {
        MarubozuList sut = new(minBodyPercent, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<CandleResult> expected = subset.ToMarubozu(minBodyPercent);

        MarubozuList sut = new(minBodyPercent, subset);

        sut.Should().HaveCount(subset.Count);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());

        sut.Clear();

        sut.Should().BeEmpty();

        sut.Add(subset);

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void AutoListPruning()
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
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }
}
