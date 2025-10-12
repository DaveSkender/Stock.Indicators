namespace BufferLists;

[TestClass]
public class Pvo : BufferListTestBase
{
    private const int fastPeriods = 12;
    private const int slowPeriods = 26;
    private const int signalPeriods = 9;

    private static readonly IReadOnlyList<PvoResult> series
       = Quotes.ToPvo(fastPeriods, slowPeriods, signalPeriods);

    [TestMethod]
    public override void AddQuotes()
    {
        PvoList sut = new(fastPeriods, slowPeriods, signalPeriods);

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
        PvoList sut = Quotes.ToPvoList(fastPeriods, slowPeriods, signalPeriods);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void WithQuotesCtor()
    {
        PvoList sut = new(fastPeriods, slowPeriods, signalPeriods, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<PvoResult> expected = subset.ToPvo(fastPeriods, slowPeriods, signalPeriods);

        PvoList sut = new(fastPeriods, slowPeriods, signalPeriods, subset);

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

        PvoList sut = new(fastPeriods, slowPeriods, signalPeriods) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<PvoResult> expected
            = series.Skip(series.Count - maxListSize).ToList();

        sut.Should().HaveCount(maxListSize);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }
}
