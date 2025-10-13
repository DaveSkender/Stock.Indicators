namespace BufferLists;

[TestClass]
public class Renko : BufferListTestBase
{
    private const decimal brickSize = 1.0m;
    private const EndType endType = EndType.Close;

    private static readonly IReadOnlyList<RenkoResult> series
       = Quotes.ToRenko(brickSize, endType);

    [TestMethod]
    public void AddQuotes()
    {
        RenkoList sut = new(brickSize, endType);

        foreach (Quote quote in Quotes)
        {
            sut.Add(quote);
        }

        sut.Should().HaveCount(series.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddQuotesBatch()
    {
        RenkoList sut = new(brickSize, endType) { Quotes };

        sut.Should().HaveCount(series.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void WithQuotesCtor()
    {
        RenkoList sut = new(brickSize, endType, Quotes);

        sut.Should().HaveCount(series.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();

        RenkoList sut = new(brickSize, endType, subset);

        sut.Should().HaveCount(subset.ToRenko(brickSize, endType).Count);

        sut.Clear();

        sut.Should().BeEmpty();

        foreach (Quote quote in subset)
        {
            sut.Add(quote);
        }

        IReadOnlyList<RenkoResult> expected = subset.ToRenko(brickSize, endType);

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void AutoListPruning()
    {
        const int maxListSize = 80;

        RenkoList sut = new(brickSize, endType) {
            MaxListSize = maxListSize
        };

        foreach (Quote quote in Quotes)
        {
            sut.Add(quote);
        }

        IReadOnlyList<RenkoResult> expected
            = series.Skip(series.Count - maxListSize).ToList();

        sut.Should().HaveCount(maxListSize);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }
}
