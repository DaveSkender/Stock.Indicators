namespace BufferLists;

[TestClass]
public class UlcerIndex : BufferListTestBase, ITestChainBufferList
{
    private const int lookbackPeriods = 14;

    private static readonly IReadOnlyList<IReusable> reusables
       = Quotes
        .Cast<IReusable>()
        .ToList();

    private static readonly IReadOnlyList<UlcerIndexResult> series
       = Quotes.ToUlcerIndex(lookbackPeriods);

    [TestMethod]
    public void AddDateAndValue_IncrementsResults()
    {
        UlcerIndexList sut = new(lookbackPeriods);

        foreach (IReusable item in reusables)
        {
            sut.Add(item.Timestamp, item.Value);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddReusableItem_IncrementsResults()
    {
        UlcerIndexList sut = new(lookbackPeriods);

        foreach (IReusable item in reusables)
        {
            sut.Add(item);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddReusableItemBatch_IncrementsResults()
    {
        UlcerIndexList sut = new(lookbackPeriods) { reusables };

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddQuote_IncrementsResults()
    {
        UlcerIndexList sut = new(lookbackPeriods);

        foreach (Quote quote in Quotes)
        {
            sut.Add(quote);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddQuotesBatch_IncrementsResults()
    {
        UlcerIndexList sut = Quotes.ToUlcerIndexList(lookbackPeriods);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void QuotesCtor_OnInstantiation_IncrementsResults()
    {
        UlcerIndexList sut = new(lookbackPeriods, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<UlcerIndexResult> expected = subset.ToUlcerIndex(lookbackPeriods);

        UlcerIndexList sut = new(lookbackPeriods, subset);

        sut.Should().HaveCount(subset.Count);
        sut.Should().BeEquivalentTo(expected, static options => options.WithStrictOrdering());

        sut.Clear();

        sut.Should().BeEmpty();

        sut.Add(subset);

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void PruneList_OverMaxListSize_AutoAdjustsListAndBuffers()
    {
        const int maxListSize = 120;

        UlcerIndexList sut = new(lookbackPeriods) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<UlcerIndexResult> expected = series
            .Skip(series.Count - maxListSize)
            .ToList();

        sut.Should().HaveCount(maxListSize);
        sut.Should().BeEquivalentTo(expected, static options => options.WithStrictOrdering());
    }
}
