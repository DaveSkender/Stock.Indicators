namespace BufferLists;

[TestClass]
public class StochRsi : BufferListTestBase, ITestReusableBufferList
{
    private const int rsiPeriods = 14;
    private const int stochPeriods = 14;
    private const int signalPeriods = 3;
    private const int smoothPeriods = 1;

    private static readonly IReadOnlyList<IReusable> reusables
       = Quotes
        .Cast<IReusable>()
        .ToList();

    private static readonly IReadOnlyList<StochRsiResult> series
       = Quotes.ToStochRsi(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);

    [TestMethod]
    public override void AddQuotes()
    {
        StochRsiList sut = new(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);

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
        StochRsiList sut = Quotes.ToStochRsiList(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void WithQuotesCtor()
    {
        StochRsiList sut = new(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<StochRsiResult> expected = subset.ToStochRsi(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);

        StochRsiList sut = new(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods, subset);

        sut.Should().HaveCount(subset.Count);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());

        sut.Clear();

        sut.Should().BeEmpty();

        sut.Add(subset);

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddReusableItems()
    {
        StochRsiList sut = new(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);

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
        StochRsiList sut = new(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods) { reusables };

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddDiscreteValues()
    {
        StochRsiList sut = new(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);

        foreach (IReusable item in reusables)
        {
            sut.Add(item.Timestamp, item.Value);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void AutoListPruning()
    {
        const int maxListSize = 120;

        StochRsiList sut = new(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<StochRsiResult> expected = series
            .Skip(series.Count - maxListSize)
            .ToList();

        sut.Should().HaveCount(maxListSize);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }
}
