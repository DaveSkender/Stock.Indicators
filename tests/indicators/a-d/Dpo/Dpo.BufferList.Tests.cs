namespace BufferLists;

[TestClass]
public class Dpo : BufferListTestBase, ITestChainBufferList
{
    private const int lookbackPeriods = 20;

    private static readonly IReadOnlyList<IReusable> reusables
       = Quotes
        .Cast<IReusable>()
        .ToList();

    private static readonly IReadOnlyList<DpoResult> series
       = Quotes.ToDpo(lookbackPeriods);

    [TestMethod]
    public void AddDateAndValue_IncrementsResults()
    {
        DpoList sut = new(lookbackPeriods);

        foreach (IReusable item in reusables)
        {
            sut.Add(item.Timestamp, item.Value);
        }

        // DPO results are delayed by offset periods
        const int offset = (lookbackPeriods / 2) + 1;
        int expectedCount = Quotes.Count - offset;

        sut.Should().HaveCount(expectedCount);
        sut.Should().BeEquivalentTo(series.Take(expectedCount), static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddReusableItem_IncrementsResults()
    {
        DpoList sut = new(lookbackPeriods);

        foreach (IReusable item in reusables)
        {
            sut.Add(item);
        }

        const int offset = (lookbackPeriods / 2) + 1;
        int expectedCount = Quotes.Count - offset;

        sut.Should().HaveCount(expectedCount);
        sut.Should().BeEquivalentTo(series.Take(expectedCount), static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddReusableItemBatch_IncrementsResults()
    {
        DpoList sut = new(lookbackPeriods) { reusables };

        const int offset = (lookbackPeriods / 2) + 1;
        int expectedCount = Quotes.Count - offset;

        sut.Should().HaveCount(expectedCount);
        sut.Should().BeEquivalentTo(series.Take(expectedCount), static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddQuote_IncrementsResults()
    {
        DpoList sut = new(lookbackPeriods);

        foreach (Quote quote in Quotes)
        {
            sut.Add(quote);
        }

        const int offset = (lookbackPeriods / 2) + 1;
        int expectedCount = Quotes.Count - offset;

        sut.Should().HaveCount(expectedCount);
        sut.Should().BeEquivalentTo(series.Take(expectedCount), static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddQuotesBatch_IncrementsResults()
    {
        DpoList sut = Quotes.ToDpoList(lookbackPeriods);

        const int offset = (lookbackPeriods / 2) + 1;
        int expectedCount = Quotes.Count - offset;

        sut.Should().HaveCount(expectedCount);
        sut.Should().BeEquivalentTo(series.Take(expectedCount), static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void QuotesCtor_OnInstantiation_IncrementsResults()
    {
        DpoList sut = new(lookbackPeriods, Quotes);

        const int offset = (lookbackPeriods / 2) + 1;
        int expectedCount = Quotes.Count - offset;

        sut.Should().HaveCount(expectedCount);
        sut.Should().BeEquivalentTo(series.Take(expectedCount), static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<DpoResult> expected = subset.ToDpo(lookbackPeriods);

        DpoList sut = new(lookbackPeriods, subset);

        const int offset = (lookbackPeriods / 2) + 1;
        int expectedCount = subset.Count - offset;

        sut.Should().HaveCount(expectedCount);
        sut.Should().BeEquivalentTo(expected.Take(expectedCount), static options => options.WithStrictOrdering());

        sut.Clear();

        sut.Should().BeEmpty();

        sut.Add(subset);

        sut.Should().HaveCount(expectedCount);
        sut.Should().BeEquivalentTo(expected.Take(expectedCount), static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void PruneList_OverMaxListSize_AutoAdjustsListAndBuffers()
    {
        const int maxListSize = 120;

        DpoList sut = new(lookbackPeriods) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        const int offset = (lookbackPeriods / 2) + 1;
        int totalResults = Quotes.Count - offset;
        int expectedCount = Math.Min(maxListSize, totalResults);

        sut.Should().HaveCount(expectedCount);

        // Compare with the last expectedCount results from series
        IReadOnlyList<DpoResult> expected = series
            .Skip(totalResults - expectedCount)
            .Take(expectedCount)
            .ToList();

        sut.Should().BeEquivalentTo(expected, static options => options.WithStrictOrdering());
    }
}
