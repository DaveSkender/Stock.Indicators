namespace BufferLists;

[TestClass]
public class MgDynamic : BufferListTestBase, ITestChainBufferList
{
    private const int lookbackPeriods = 14;
    private const double kFactor = 0.6;

    private static readonly IReadOnlyList<IReusable> reusables
       = Quotes
        .Cast<IReusable>()
        .ToList();

    private static readonly IReadOnlyList<DynamicResult> series
       = Quotes.ToDynamic(lookbackPeriods, kFactor);

    [TestMethod]
    public void AddQuotes()
    {
        DynamicList sut = new(lookbackPeriods, kFactor);

        foreach (Quote q in Quotes) { sut.Add(q); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddQuotesBatch()
    {
        DynamicList sut = new(lookbackPeriods, kFactor) { Quotes };

        IReadOnlyList<DynamicResult> series
            = Quotes.ToDynamic(lookbackPeriods, kFactor);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void WithQuotesCtor()
    {
        DynamicList sut = new(lookbackPeriods, kFactor, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddReusableItems()
    {
        DynamicList sut = new(lookbackPeriods, kFactor);

        foreach (IReusable item in reusables) { sut.Add(item); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddReusableItemsBatch()
    {
        DynamicList sut = new(lookbackPeriods, kFactor) { reusables };

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddDiscreteValues()
    {
        DynamicList sut = new(lookbackPeriods, kFactor);

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

        DynamicList sut = new(lookbackPeriods, kFactor) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<DynamicResult> expected
            = series.Skip(series.Count - maxListSize).ToList();

        sut.Should().HaveCount(maxListSize);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<DynamicResult> expected = subset.ToDynamic(lookbackPeriods, kFactor);

        DynamicList sut = new(lookbackPeriods, kFactor, subset);

        sut.Should().HaveCount(subset.Count);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());

        sut.Clear();

        sut.Should().BeEmpty();

        sut.Add(subset);

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }
}
