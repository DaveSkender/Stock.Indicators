namespace BufferLists;

[TestClass]
public class Awesome : BufferListTestBase, ITestChainBufferList
{
    private const int fastPeriods = 5;
    private const int slowPeriods = 34;

    private static readonly IReadOnlyList<IReusable> reusables
        = Quotes
            .ToQuotePart(CandlePart.HL2)
            .ToList();

    private static readonly IReadOnlyList<AwesomeResult> series
        = Quotes.ToAwesome(fastPeriods, slowPeriods);

    [TestMethod]
    public void AddDiscreteValues()
    {
        AwesomeList sut = new(fastPeriods, slowPeriods);

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
        AwesomeList sut = new(fastPeriods, slowPeriods);

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
        AwesomeList sut = new(fastPeriods, slowPeriods) { reusables };

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddQuotes()
    {
        AwesomeList sut = new(fastPeriods, slowPeriods);

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
        AwesomeList sut = new(fastPeriods, slowPeriods) { Quotes };

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void WithQuotesCtor()
    {
        AwesomeList sut = new(fastPeriods, slowPeriods, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<AwesomeResult> expected = subset.ToAwesome(fastPeriods, slowPeriods);

        AwesomeList sut = new(fastPeriods, slowPeriods, subset);

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

        AwesomeList sut = new(fastPeriods, slowPeriods) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<AwesomeResult> expected
            = series.Skip(series.Count - maxListSize).ToList();

        sut.Should().HaveCount(maxListSize);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }
}
