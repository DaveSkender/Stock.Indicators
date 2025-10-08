namespace BufferLists;

[TestClass]
public class Dpo : BufferListTestBase, ITestReusableBufferList
{
    private const int lookbackPeriods = 20;

    private static readonly IReadOnlyList<IReusable> reusables
       = Quotes
        .Cast<IReusable>()
        .ToList();

    private static readonly IReadOnlyList<DpoResult> series
       = Quotes.ToDpo(lookbackPeriods);

    [TestMethod]
    public void AddDiscreteValues()
    {
        DpoList sut = new(lookbackPeriods);

        foreach (IReusable item in reusables)
        {
            sut.Add(item.Timestamp, item.Value);
        }

        // DPO results are delayed by offset periods
        int offset = (lookbackPeriods / 2) + 1;
        int expectedCount = Quotes.Count - offset;

        sut.Should().HaveCount(expectedCount);
        sut.Should().BeEquivalentTo(series.Take(expectedCount), options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddReusableItems()
    {
        DpoList sut = new(lookbackPeriods);

        foreach (IReusable item in reusables)
        {
            sut.Add(item);
        }

        int offset = (lookbackPeriods / 2) + 1;
        int expectedCount = Quotes.Count - offset;

        sut.Should().HaveCount(expectedCount);
        sut.Should().BeEquivalentTo(series.Take(expectedCount), options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddReusableItemsBatch()
    {
        DpoList sut = new(lookbackPeriods) { reusables };

        int offset = (lookbackPeriods / 2) + 1;
        int expectedCount = Quotes.Count - offset;

        sut.Should().HaveCount(expectedCount);
        sut.Should().BeEquivalentTo(series.Take(expectedCount), options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void AddQuotes()
    {
        DpoList sut = new(lookbackPeriods);

        foreach (Quote quote in Quotes)
        {
            sut.Add(quote);
        }

        int offset = (lookbackPeriods / 2) + 1;
        int expectedCount = Quotes.Count - offset;

        sut.Should().HaveCount(expectedCount);
        sut.Should().BeEquivalentTo(series.Take(expectedCount), options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void AddQuotesBatch()
    {
        DpoList sut = Quotes.ToDpoList(lookbackPeriods);

        int offset = (lookbackPeriods / 2) + 1;
        int expectedCount = Quotes.Count - offset;

        sut.Should().HaveCount(expectedCount);
        sut.Should().BeEquivalentTo(series.Take(expectedCount), options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void WithQuotesCtor()
    {
        DpoList sut = new(lookbackPeriods, Quotes);

        int offset = (lookbackPeriods / 2) + 1;
        int expectedCount = Quotes.Count - offset;

        sut.Should().HaveCount(expectedCount);
        sut.Should().BeEquivalentTo(series.Take(expectedCount), options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<DpoResult> expected = subset.ToDpo(lookbackPeriods);

        DpoList sut = new(lookbackPeriods, subset);

        int offset = (lookbackPeriods / 2) + 1;
        int expectedCount = subset.Count - offset;

        sut.Should().HaveCount(expectedCount);
        sut.Should().BeEquivalentTo(expected.Take(expectedCount), options => options.WithStrictOrdering());

        sut.Clear();

        sut.Should().BeEmpty();

        sut.Add(subset);

        sut.Should().HaveCount(expectedCount);
        sut.Should().BeEquivalentTo(expected.Take(expectedCount), options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void AutoListPruning()
    {
        const int maxListSize = 120;

        DpoList sut = new(lookbackPeriods) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        int offset = (lookbackPeriods / 2) + 1;
        int totalResults = Quotes.Count - offset;
        int expectedCount = Math.Min(maxListSize, totalResults);

        sut.Should().HaveCount(expectedCount);

        // Compare with the last expectedCount results from series
        IReadOnlyList<DpoResult> expected = series
            .Skip(totalResults - expectedCount)
            .Take(expectedCount)
            .ToList();

        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }
}
