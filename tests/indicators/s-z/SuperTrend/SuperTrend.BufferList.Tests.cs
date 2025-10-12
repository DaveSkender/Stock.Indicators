namespace BufferLists;

[TestClass]
public class SuperTrend : BufferListTestBase
{
    private const int lookbackPeriods = 10;
    private const double multiplier = 3;

    private static readonly IReadOnlyList<SuperTrendResult> series
       = Quotes.ToSuperTrend(lookbackPeriods, multiplier);

    [TestMethod]
    public override void AddQuotes()
    {
        SuperTrendList sut = new(lookbackPeriods, multiplier);

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
        SuperTrendList sut = Quotes.ToSuperTrendList(lookbackPeriods, multiplier);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void WithQuotesCtor()
    {
        SuperTrendList sut = new(lookbackPeriods, multiplier, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<SuperTrendResult> expected = subset.ToSuperTrend(lookbackPeriods, multiplier);

        SuperTrendList sut = new(lookbackPeriods, multiplier, subset);

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

        SuperTrendList sut = new(lookbackPeriods, multiplier) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<SuperTrendResult> expected = series
            .Skip(series.Count - maxListSize)
            .ToList();

        sut.Should().HaveCount(maxListSize);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }
}
