namespace BufferLists;

[TestClass]
public class Adx : BufferListTestBase
{
    private const int lookbackPeriods = 14;

    private static readonly IReadOnlyList<AdxResult> series
       = Quotes.ToAdx(lookbackPeriods);

    [TestMethod]
    public override void FromQuote()
    {
        AdxList sut = new(lookbackPeriods);

        foreach (Quote q in Quotes) { sut.Add(q); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public override void FromQuoteBatch()
    {
        AdxList sut = new(lookbackPeriods) { Quotes };

        IReadOnlyList<AdxResult> series
            = Quotes.ToAdx(lookbackPeriods);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();

        AdxList sut = new(lookbackPeriods);

        foreach (Quote quote in subset)
        {
            sut.Add(quote);
        }

        sut.Should().HaveCount(subset.Count);

        sut.Clear();

        sut.Should().BeEmpty();

        foreach (Quote quote in subset)
        {
            sut.Add(quote);
        }

        IReadOnlyList<AdxResult> expected = subset.ToAdx(lookbackPeriods);

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void AutoPruning()
    {
        const int maxListSize = 100;

        // Create buffer list with small MaxListSize for testing
        AdxList sut = new(lookbackPeriods) { MaxListSize = maxListSize };

        // Add more quotes than MaxListSize
        for (int i = 0; i < 150; i++)
        {
            Quote quote = Quotes[i % Quotes.Count];
            sut.Add(quote);
        }

    // Verify list was pruned to stay at MaxListSize
    sut.Count.Should().Be(maxListSize);

        // Verify most recent results are retained
        AdxResult lastResult = sut[^1];
        lastResult.Should().NotBeNull();
    }
}
