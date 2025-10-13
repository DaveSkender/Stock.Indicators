namespace BufferLists;

[TestClass]
public class StarcBands : BufferListTestBase, ITestQuoteBufferList
{
    private const int smaPeriods = 5;
    private const double multiplier = 2;
    private const int atrPeriods = 10;

    private static readonly IReadOnlyList<StarcBandsResult> series
       = Quotes.ToStarcBands(smaPeriods, multiplier, atrPeriods);

    [TestMethod]
    public void AddQuotes()
    {
        StarcBandsList sut = new(smaPeriods, multiplier, atrPeriods);

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
        StarcBandsList sut = new(smaPeriods, multiplier, atrPeriods) { Quotes };

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void WithQuotesCtor()
    {
        StarcBandsList sut = new(smaPeriods, multiplier, atrPeriods, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<StarcBandsResult> expected = subset.ToStarcBands(smaPeriods, multiplier, atrPeriods);

        StarcBandsList sut = new(smaPeriods, multiplier, atrPeriods, subset);

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
        const int maxListSize = 100;

        StarcBandsList sut = new(smaPeriods, multiplier, atrPeriods) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<StarcBandsResult> expected = series
            .Skip(series.Count - maxListSize)
            .ToList();

        sut.Should().HaveCount(maxListSize);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }
}
