namespace BufferLists;

[TestClass]
public class HeikinAshi : BufferListTestBase
{
    private static readonly IReadOnlyList<HeikinAshiResult> series
       = Quotes.ToHeikinAshi();

    [TestMethod]
    public override void AddQuotes()
    {
        HeikinAshiList sut = [];

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
        HeikinAshiList sut = Quotes.ToHeikinAshiList();

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void WithQuotesCtor()
    {
        HeikinAshiList sut = new(Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<HeikinAshiResult> expected = subset.ToHeikinAshi();

        HeikinAshiList sut = new(subset);

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

        HeikinAshiList sut = new() {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<HeikinAshiResult> expected
            = series.Skip(series.Count - maxListSize).ToList();

        sut.Should().HaveCount(maxListSize);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }
}
