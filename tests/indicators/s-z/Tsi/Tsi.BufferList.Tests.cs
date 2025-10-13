namespace BufferLists;

[TestClass]
public class Tsi : BufferListTestBase, ITestChainBufferList
{
    private const int lookbackPeriods = 25;
    private const int smoothPeriods = 13;
    private const int signalPeriods = 7;

    private static readonly IReadOnlyList<IReusable> reusables
       = Quotes.Cast<IReusable>().ToList();

    private static readonly IReadOnlyList<TsiResult> series
       = Quotes.ToTsi(lookbackPeriods, smoothPeriods, signalPeriods);

    [TestMethod]
    public void AddQuotes()
    {
        TsiList sut = new(lookbackPeriods, smoothPeriods, signalPeriods);

        foreach (Quote q in Quotes) { sut.Add(q); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddQuotesBatch()
    {
        TsiList sut = Quotes.ToTsiList(lookbackPeriods, smoothPeriods, signalPeriods);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void WithQuotesCtor()
    {
        TsiList sut = new(lookbackPeriods, smoothPeriods, signalPeriods, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddReusableItems()
    {
        TsiList sut = new(lookbackPeriods, smoothPeriods, signalPeriods);

        foreach (IReusable item in reusables) { sut.Add(item); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddReusableItemsBatch()
    {
        TsiList sut = new(lookbackPeriods, smoothPeriods, signalPeriods) { reusables };

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddDiscreteValues()
    {
        TsiList sut = new(lookbackPeriods, smoothPeriods, signalPeriods);

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

        TsiList sut = new(lookbackPeriods, smoothPeriods, signalPeriods) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<TsiResult> expected
            = series.Skip(series.Count - maxListSize).ToList();

        sut.Should().HaveCount(maxListSize);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<TsiResult> expected = subset.ToTsi(lookbackPeriods, smoothPeriods, signalPeriods);

        TsiList sut = new(lookbackPeriods, smoothPeriods, signalPeriods, subset);

        sut.Should().HaveCount(subset.Count);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());

        sut.Clear();

        sut.Should().BeEmpty();

        sut.Add(subset);

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }
}
