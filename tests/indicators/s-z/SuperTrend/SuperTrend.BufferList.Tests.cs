namespace BufferLists;

[TestClass]
public class SuperTrend : BufferListTestBase
{
    private const int lookbackPeriods = 10;
    private const double multiplier = 3;

    private static readonly IReadOnlyList<SuperTrendResult> series
       = Quotes.ToSuperTrend(lookbackPeriods, multiplier);

    [TestMethod]
    public void AddQuotes()
    {
        SuperTrendList sut = new(lookbackPeriods, multiplier);

        foreach (Quote quote in Quotes)
        {
            sut.Add(quote);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddQuotesBatch()
    {
        SuperTrendList sut = Quotes.ToSuperTrendList(lookbackPeriods, multiplier);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void WithQuotesCtor()
    {
        SuperTrendList sut = new(lookbackPeriods, multiplier, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<SuperTrendResult> expected = subset.ToSuperTrend(lookbackPeriods, multiplier);

        SuperTrendList sut = new(lookbackPeriods, multiplier, subset);

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

        SuperTrendList sut = new(lookbackPeriods, multiplier) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<SuperTrendResult> expected = series
            .Skip(series.Count - maxListSize)
            .ToList();

        sut.Should().HaveCount(maxListSize);
        sut.Should().BeEquivalentTo(expected, static options => options.WithStrictOrdering());
    }
}
