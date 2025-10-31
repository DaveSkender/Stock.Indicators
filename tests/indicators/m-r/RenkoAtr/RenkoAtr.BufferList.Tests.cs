namespace BufferLists;

[TestClass]
public class RenkoAtr : BufferListTestBase, ITestQuoteBufferList, ITestCustomBufferListCache
{
    private const int atrPeriods = 14;
    private const EndType endType = EndType.Close;

    private static readonly IReadOnlyList<RenkoResult> series
       = Quotes.ToRenkoAtr(atrPeriods, endType);

    [TestMethod]
    public void AddQuote_IncrementsResults()
    {
        RenkoAtrList sut = new(atrPeriods, endType);

        foreach (Quote quote in Quotes)
        {
            sut.Add(quote);
        }

        sut.Should().HaveCount(series.Count);
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddQuotesBatch_IncrementsResults()
    {
        RenkoAtrList sut = new(atrPeriods, endType) { Quotes };

        sut.Should().HaveCount(series.Count);
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void QuotesCtor_OnInstantiation_IncrementsResults()
    {
        RenkoAtrList sut = new(atrPeriods, endType, Quotes);

        sut.Should().HaveCount(series.Count);
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();

        RenkoAtrList sut = new(atrPeriods, endType, subset);

        sut.Should().HaveCount(subset.ToRenkoAtr(atrPeriods, endType).Count);

        sut.Clear();

        sut.Should().BeEmpty();

        foreach (Quote quote in subset)
        {
            sut.Add(quote);
        }

        IReadOnlyList<RenkoResult> expected = subset.ToRenkoAtr(atrPeriods, endType);

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void PruneList_OverMaxListSize_AutoAdjustsListAndBuffers()
    {
        const int maxListSize = 80;

        RenkoAtrList sut = new(atrPeriods, endType) {
            MaxListSize = maxListSize
        };

        foreach (Quote quote in Quotes)
        {
            sut.Add(quote);
        }

        // RenkoAtr produces fewer bricks than maxListSize, so we expect all of them
        int expectedCount = Math.Min(maxListSize, series.Count);
        IReadOnlyList<RenkoResult> expected
            = series.Skip(Math.Max(0, series.Count - maxListSize)).ToList();

        sut.Should().HaveCount(expectedCount);
        sut.Should().BeEquivalentTo(expected, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void CustomBuffer_OverMaxListSize_AutoAdjustsListAndBuffers()
    {
        const int maxListSize = 20;

        RenkoAtrList sut = new(atrPeriods, endType, Quotes) {
            MaxListSize = maxListSize
        };

        // RenkoAtr produces 29 bricks total, which is more than maxListSize
        // So we expect the list to be pruned to the last 20 bricks
        int expectedCount = Math.Min(maxListSize, series.Count);
        IReadOnlyList<RenkoResult> expected
            = series.Skip(Math.Max(0, series.Count - maxListSize)).ToList();

        sut.Should().HaveCount(expectedCount);
        sut.Should().BeEquivalentTo(expected, static options => options.WithStrictOrdering());
    }
}
