namespace BufferLists;

[TestClass]
public class Chandelier : BufferListTestBase
{
    private const int lookbackPeriods = 22;
    private const double multiplier = 3;
    private const Direction type = Direction.Long;

    private static readonly IReadOnlyList<ChandelierResult> series
       = Quotes.ToChandelier(lookbackPeriods, multiplier, type);

    [TestMethod]
    public void AddQuotes()
    {
        ChandelierList sut = new(lookbackPeriods, multiplier, type);

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
        ChandelierList sut = new(lookbackPeriods, multiplier, type) { Quotes };

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void WithQuotesCtor()
    {
        ChandelierList sut = new(lookbackPeriods, multiplier, type, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<ChandelierResult> expected = subset.ToChandelier(lookbackPeriods, multiplier, type);

        ChandelierList sut = new(lookbackPeriods, multiplier, type, subset);

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

        ChandelierList sut = new(lookbackPeriods, multiplier, type) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<ChandelierResult> expected = series
            .Skip(series.Count - maxListSize)
            .ToList();

        sut.Should().HaveCount(maxListSize);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }
}
