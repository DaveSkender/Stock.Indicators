namespace BufferLists;

[TestClass]
public class BollingerBands : BufferListTestBase, ITestChainBufferList
{
    private const int lookbackPeriods = 20;
    private const double standardDeviations = 2;

    private static readonly IReadOnlyList<IReusable> reusables
        = Quotes
            .Cast<IReusable>()
            .ToList();

    private static readonly IReadOnlyList<BollingerBandsResult> series
        = Quotes.ToBollingerBands(lookbackPeriods, standardDeviations);

    [TestMethod]
    public void AddQuotes()
    {
        BollingerBandsList sut = new(lookbackPeriods, standardDeviations);

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
        BollingerBandsList sut = new(lookbackPeriods, standardDeviations) { Quotes };

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void WithQuotesCtor()
    {
        BollingerBandsList sut = new(lookbackPeriods, standardDeviations, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddReusableItems()
    {
        BollingerBandsList sut = new(lookbackPeriods, standardDeviations);

        foreach (IReusable item in reusables)
        {
            sut.Add(item);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddReusableItemsBatch()
    {
        BollingerBandsList sut = new(lookbackPeriods, standardDeviations) { reusables };

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddDiscreteValues()
    {
        BollingerBandsList sut = new(lookbackPeriods, standardDeviations);

        foreach (IReusable item in reusables)
        {
            sut.Add(item.Timestamp, item.Value);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<BollingerBandsResult> expected = subset.ToBollingerBands(lookbackPeriods, standardDeviations);

        BollingerBandsList sut = new(lookbackPeriods, standardDeviations, subset);

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

        BollingerBandsList sut = new(lookbackPeriods, standardDeviations) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<BollingerBandsResult> expected = series
            .Skip(series.Count - maxListSize)
            .ToList();

        sut.Should().HaveCount(maxListSize);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }
}
