namespace BufferLists;

[TestClass]
public class FisherTransform : BufferListTestBase
{
    private const int lookbackPeriods = 10;

    private static readonly IReadOnlyList<FisherTransformResult> series
       = Quotes.ToFisherTransform(lookbackPeriods);

    [TestMethod]
    public override void AddQuotes()
    {
        FisherTransformList sut = new(lookbackPeriods);

        foreach (Quote q in Quotes) { sut.Add(q); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void AddQuotesBatch()
    {
        FisherTransformList sut = new(lookbackPeriods) { Quotes };

        IReadOnlyList<FisherTransformResult> series
            = Quotes.ToFisherTransform(lookbackPeriods);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void WithQuotesCtor()
    {
        FisherTransformList sut = new(lookbackPeriods, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void AutoListPruning()
    {
        const int maxListSize = 120;

        FisherTransformList sut = new(lookbackPeriods) {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<FisherTransformResult> expected
            = series.Skip(series.Count - maxListSize).ToList();

        sut.Should().HaveCount(maxListSize);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<FisherTransformResult> expected = subset.ToFisherTransform(lookbackPeriods);

        FisherTransformList sut = new(lookbackPeriods, subset);

        sut.Should().HaveCount(subset.Count);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());

        sut.Clear();

        sut.Should().BeEmpty();

        sut.Add(subset);

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }
}
