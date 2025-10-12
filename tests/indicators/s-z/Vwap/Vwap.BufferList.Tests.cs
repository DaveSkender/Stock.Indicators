namespace BufferLists;

[TestClass]
public class Vwap : BufferListTestBase
{
    private static readonly DateTime startDate = new(2018, 12, 31);

    private static readonly IReadOnlyList<VwapResult> series
       = Quotes.ToVwap(startDate);

    [TestMethod]
    public override void AddQuotes()
    {
        VwapList sut = new(startDate);

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
        VwapList sut = Quotes.ToVwapList(startDate);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void WithQuotesCtor()
    {
        VwapList sut = new(startDate, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void WithDefaultStartDate()
    {
        VwapList sut = Quotes.ToVwapList();
        IReadOnlyList<VwapResult> expected = Quotes.ToVwap();

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<VwapResult> expected = subset.ToVwap(startDate);

        VwapList sut = new(startDate, subset);

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

        VwapList sut = new(startDate) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<VwapResult> expected = series
            .Skip(series.Count - maxListSize)
            .ToList();

        sut.Should().HaveCount(maxListSize);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }
}
