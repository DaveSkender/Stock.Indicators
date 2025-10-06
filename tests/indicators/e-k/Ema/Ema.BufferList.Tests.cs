namespace BufferLists;

[TestClass]
public class Ema : BufferListTestBase, ITestReusableBufferList
{
    private const int lookbackPeriods = 14;

    private static readonly IReadOnlyList<IReusable> reusables
       = Quotes
        .Cast<IReusable>()
        .ToList();

    private static readonly IReadOnlyList<EmaResult> series
       = Quotes.ToEma(lookbackPeriods);

    [TestMethod]
    public override void AddQuotes()
    {
        EmaList sut = new(lookbackPeriods);

        foreach (Quote q in Quotes) { sut.Add(q); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void AddQuotesBatch()
    {
        EmaList sut = new(lookbackPeriods) { Quotes };

        IReadOnlyList<EmaResult> series
            = Quotes.ToEma(lookbackPeriods);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void WithQuotesCtor()
    {
        EmaList sut = new(lookbackPeriods, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddReusableItems()
    {
        EmaList sut = new(lookbackPeriods);

        foreach (IReusable item in reusables) { sut.Add(item); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddReusableItemsBatch()
    {
        EmaList sut = new(lookbackPeriods) { reusables };

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddDiscreteValues()
    {
        EmaList sut = new(lookbackPeriods);

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

        EmaList sut = new(lookbackPeriods) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<EmaResult> expected
            = series.Skip(series.Count - maxListSize).ToList();

        sut.Should().HaveCount(maxListSize);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<EmaResult> expected = subset.ToEma(lookbackPeriods);

        EmaList sut = new(lookbackPeriods, subset);

        sut.Should().HaveCount(subset.Count);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());

        sut.Clear();

        sut.Should().BeEmpty();

        sut.Add(subset);

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }
}
