namespace BufferList;

[TestClass]
public class VolatilityStop : BufferListTestBase, ITestQuoteBufferList
{
    private const int lookbackPeriods = 14;
    private const double multiplier = 3;

    [TestMethod]
    public void AddDiscreteValues()
    {
        VolatilityStopList sut = new(lookbackPeriods, multiplier);

        foreach (IQuote quote in Quotes)
        {
            sut.Add(quote);
        }

        sut.Should().HaveCount(Quotes.Count);

        // Verify against series results
        IReadOnlyList<VolatilityStopResult> series = Quotes.ToVolatilityStop(lookbackPeriods, multiplier);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddQuotes()
    {
        VolatilityStopList sut = new(lookbackPeriods, multiplier);
        sut.Add(Quotes);

        sut.Should().HaveCount(Quotes.Count);

        // Verify against series results
        IReadOnlyList<VolatilityStopResult> series = Quotes.ToVolatilityStop(lookbackPeriods, multiplier);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddQuotesBatch()
    {
        VolatilityStopList sut = new(lookbackPeriods, multiplier) { Quotes };

        sut.Should().HaveCount(Quotes.Count);

        // Verify against series results
        IReadOnlyList<VolatilityStopResult> series = Quotes.ToVolatilityStop(lookbackPeriods, multiplier);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void WithQuotesCtor()
    {
        VolatilityStopList sut = new(lookbackPeriods, multiplier, Quotes);

        sut.Should().HaveCount(Quotes.Count);

        // Verify against series results
        IReadOnlyList<VolatilityStopResult> series = Quotes.ToVolatilityStop(lookbackPeriods, multiplier);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<VolatilityStopResult> expected = subset.ToVolatilityStop(lookbackPeriods, multiplier);

        VolatilityStopList sut = new(lookbackPeriods, multiplier, subset);

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
        VolatilityStopList sut = new(lookbackPeriods, multiplier) {
            MaxListSize = 100
        };

        sut.Add(Quotes);

        sut.Should().HaveCount(100);

        // Verify the last 100 items match series results
        IReadOnlyList<VolatilityStopResult> series = Quotes.ToVolatilityStop(lookbackPeriods, multiplier);
        IReadOnlyList<VolatilityStopResult> expectedLast100 = series.Skip(series.Count - 100).ToList();

        sut.Should().BeEquivalentTo(expectedLast100, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void ExtensionMethod()
    {
        VolatilityStopList sut = Quotes.ToVolatilityStopList(lookbackPeriods, multiplier);

        sut.Should().HaveCount(Quotes.Count);

        // Verify against series results
        IReadOnlyList<VolatilityStopResult> series = Quotes.ToVolatilityStop(lookbackPeriods, multiplier);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void DefaultParameters()
    {
        // Test with default parameters
        VolatilityStopList sut = new();
        sut.Add(Quotes);

        sut.Should().HaveCount(Quotes.Count);

        // Verify against series results with defaults
        IReadOnlyList<VolatilityStopResult> series = Quotes.ToVolatilityStop(7, 3);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void RepaintingBehavior()
    {
        // This test specifically validates that the indicator properly nullifies
        // results before the first stop, demonstrating the repainting capability
        VolatilityStopList sut = new(lookbackPeriods, multiplier);

        // Add quotes incrementally
        for (int i = 0; i < Quotes.Count; i++)
        {
            sut.Add(Quotes[i]);
        }

        // Verify against series results - this will validate that the repainting
        // (nullification of results before first stop) matches the Series implementation
        IReadOnlyList<VolatilityStopResult> series = Quotes.ToVolatilityStop(lookbackPeriods, multiplier);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }
}
