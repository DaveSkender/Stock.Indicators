namespace BufferLists;

[TestClass]
public class Pmo : BufferListTestBase, ITestChainBufferList, ITestCustomBufferListCache
{
    private const int timePeriods = 35;
    private const int smoothPeriods = 20;
    private const int signalPeriods = 10;

    private static readonly IReadOnlyList<IReusable> reusables
       = Quotes
        .Cast<IReusable>()
        .ToList();

    private static readonly IReadOnlyList<PmoResult> series
       = Quotes.ToPmo(timePeriods, smoothPeriods, signalPeriods);

    [TestMethod]
    public void AddDiscreteValues()
    {
        PmoList sut = new(timePeriods, smoothPeriods, signalPeriods);

        foreach (IReusable item in reusables)
        {
            sut.Add(item.Timestamp, item.Value);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddReusableItems()
    {
        PmoList sut = new(timePeriods, smoothPeriods, signalPeriods);

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
        PmoList sut = new(timePeriods, smoothPeriods, signalPeriods) { reusables };

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddQuotes()
    {
        PmoList sut = new(timePeriods, smoothPeriods, signalPeriods);

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
        PmoList sut = Quotes.ToPmoList(timePeriods, smoothPeriods, signalPeriods);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void WithQuotesCtor()
    {
        PmoList sut = new(timePeriods, smoothPeriods, signalPeriods, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<PmoResult> expected = subset.ToPmo(timePeriods, smoothPeriods, signalPeriods);

        PmoList sut = new(timePeriods, smoothPeriods, signalPeriods, subset);

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

        PmoList sut = new(timePeriods, smoothPeriods, signalPeriods) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<PmoResult> expected
            = series.Skip(series.Count - maxListSize).ToList();

        sut.Should().HaveCount(maxListSize);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void CustomBufferPruning()
    {
        const int maxListSize = 120;

        PmoList sut = new(timePeriods, smoothPeriods, signalPeriods) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<PmoResult> expected
            = series.Skip(series.Count - maxListSize).ToList();

        sut.Should().HaveCount(maxListSize);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());

        // Add more quotes to verify continued operation after pruning
        List<Quote> moreQuotes = Quotes.TakeLast(50).ToList();
        sut.Add(moreQuotes);

        IReadOnlyList<PmoResult> allSeries = Quotes.Concat(moreQuotes).ToList()
            .ToPmo(timePeriods, smoothPeriods, signalPeriods);

        IReadOnlyList<PmoResult> expectedAfterMore
            = allSeries.Skip(allSeries.Count - maxListSize).ToList();

        sut.Should().HaveCount(maxListSize);
        sut.Should().BeEquivalentTo(expectedAfterMore, options => options.WithStrictOrdering());
    }
}
