namespace BufferLists;

[TestClass]
public class Obv : BufferListTestBase
{
    private static readonly IReadOnlyList<ObvResult> series = Quotes.ToObv();

    [TestMethod]
    public override void AddQuotes()
    {
#pragma warning disable IDE0028 // Collection expression incompatible with IQuote Add overloads
        ObvList sut = new();
#pragma warning restore IDE0028

        sut.Add(Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void AddQuotesBatch()
    {
        ObvList sut = new() { Quotes };

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void WithQuotesCtor()
    {
        ObvList sut = new(Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void WithQuotesCtorPartial()
    {
        // Test split initialization: half on construction, half after
        int splitPoint = Quotes.Count / 2;
        List<Quote> firstHalf = Quotes.Take(splitPoint).ToList();
        List<Quote> secondHalf = Quotes.Skip(splitPoint).ToList();

        ObvList sut = new(firstHalf);

        foreach (Quote quote in secondHalf)
        {
            sut.Add(quote);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<ObvResult> expected = subset.ToObv();

        ObvList sut = new(subset);

        sut.Should().HaveCount(subset.Count);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());

        sut.Clear();

        sut.Should().BeEmpty();

        sut.Add(subset);

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AutoPrunesAtConfiguredMax()
    {
        const int maxListSize = 120;

        ObvList sut = new() {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<ObvResult> expected = series
            .Skip(series.Count - maxListSize)
            .ToList();

        sut.Should().HaveCount(maxListSize);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }

    public override void AutoListPruning() => throw new NotImplementedException();
}
