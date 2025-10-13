namespace BufferLists;

[TestClass]
public class ZigZag : BufferListTestBase, ITestQuoteBufferList
{
    private const EndType endType = EndType.Close;
    private const decimal percentChange = 5;

    private static readonly IReadOnlyList<ZigZagResult> series
       = Quotes.ToZigZag(endType, percentChange);

    [TestMethod]
    public void AddQuotes()
    {
        ZigZagList sut = new(endType, percentChange);

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
        ZigZagList sut = Quotes.ToZigZagList(endType, percentChange);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void WithQuotesCtor()
    {
        ZigZagList sut = new(endType, percentChange, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<ZigZagResult> expected = subset.ToZigZag(endType, percentChange);

        ZigZagList sut = new(endType, percentChange, subset);

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

        ZigZagList sut = new(endType, percentChange) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<ZigZagResult> expected = series
            .Skip(series.Count - maxListSize)
            .ToList();

        sut.Should().HaveCount(maxListSize);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }
}
