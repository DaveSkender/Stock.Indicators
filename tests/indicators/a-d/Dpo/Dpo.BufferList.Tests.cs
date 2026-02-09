namespace BufferLists;

[TestClass]
public class Dpo : BufferListTestBase, ITestChainBufferList, ITestCustomBufferListCache
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

        // DPO maintains 1:1 correspondence - retroactively updates as lookahead becomes available
        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddReusableItem_IncrementsResults()
    {
        DpoList sut = new(lookbackPeriods);

        foreach (IReusable item in reusables)
        {
            sut.Add(item);
        }

        // DPO maintains 1:1 correspondence - retroactively updates as lookahead becomes available
        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddReusableItemBatch_IncrementsResults()
    {
        DpoList sut = new(lookbackPeriods) { reusables };

        // DPO maintains 1:1 correspondence - retroactively updates as lookahead becomes available
        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddQuote_IncrementsResults()
    {
        DpoList sut = new(lookbackPeriods);

        foreach (Quote quote in Quotes)
        {
            sut.Add(quote);
        }

        // DPO maintains 1:1 correspondence - retroactively updates as lookahead becomes available
        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void AddQuotesBatch_IncrementsResults()
    {
        DpoList sut = Quotes.ToDpoList(lookbackPeriods);

        // DPO maintains 1:1 correspondence - retroactively updates as lookahead becomes available
        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public void QuotesCtor_OnInstantiation_IncrementsResults()
    {
        DpoList sut = new(lookbackPeriods, Quotes);

        // DPO maintains 1:1 correspondence - retroactively updates as lookahead becomes available
        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<DpoResult> expected = subset.ToDpo(lookbackPeriods);

        DpoList sut = new(lookbackPeriods, subset);

        // DPO maintains 1:1 correspondence - retroactively updates as lookahead becomes available
        sut.Should().HaveCount(subset.Count);
        sut.IsExactly(expected);

        sut.Clear();

        sut.Should().BeEmpty();

        sut.Add(subset);

        sut.Should().HaveCount(subset.Count);
        sut.IsExactly(expected);
    }

    [TestMethod]
    public override void PruneList_OverMaxListSize_AutoAdjustsListAndBuffers()
    {
        const int maxListSize = 120;

        DpoList sut = new(lookbackPeriods) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        // DPO maintains 1:1 correspondence with maxListSize pruning
        int expectedCount = Math.Min(maxListSize, Quotes.Count);

        sut.Should().HaveCount(expectedCount);

        // Compare with the last expectedCount results from series
        IReadOnlyList<DpoResult> expected = series
            .Skip(Quotes.Count - expectedCount)
            .Take(expectedCount)
            .ToList();

        sut.IsExactly(expected);
    }

    [TestMethod]
    public void CustomBuffer_OverMaxListSize_AutoAdjustsListAndBuffers()
    {
        const int maxListSize = 120;

        DpoList sut = new(lookbackPeriods) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        // DPO maintains 1:1 correspondence with maxListSize pruning
        int expectedCount = Math.Min(maxListSize, Quotes.Count);

        sut.Should().HaveCount(expectedCount);

        // Compare with the last expectedCount results from series
        IReadOnlyList<DpoResult> expected = series
            .Skip(Quotes.Count - expectedCount)
            .Take(expectedCount)
            .ToList();

        sut.IsExactly(expected);
    }
}
