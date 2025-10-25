namespace BufferLists;

[TestClass]
public class ZigZag : BufferListTestBase, ITestQuoteBufferList
{
    private const EndType endType = EndType.Close;
    private const decimal percentChange = 5;

    private static readonly IReadOnlyList<ZigZagResult> series
       = Quotes.ToZigZag(endType, percentChange);

    [TestMethod]
    public void AddQuote_IncrementsResults()
    {
        ZigZagList sut = new(endType, percentChange);

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
        ZigZagList sut = Quotes.ToZigZagList(endType, percentChange);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void QuotesCtor_OnInstantiation_IncrementsResults()
    {
        ZigZagList sut = new(endType, percentChange, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, static options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void Clear_WithState_ResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<ZigZagResult> expected = subset.ToZigZag(endType, percentChange);

        ZigZagList sut = new(endType, percentChange, subset);

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

        ZigZagList sut = new(endType, percentChange) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<ZigZagResult> expected = series
            .Skip(series.Count - maxListSize)
            .ToList();

        sut.Should().HaveCount(maxListSize);
        sut.Should().BeEquivalentTo(expected, static options => options.WithStrictOrdering());
    }
}
