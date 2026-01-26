namespace StreamHubs;

[TestClass]
public class VortexHubTests : StreamHubTestBase, ITestQuoteObserver
{
    private const int lookbackPeriods = 14;
    private readonly IReadOnlyList<VortexResult> expectedOriginal = Quotes.ToVortex(lookbackPeriods);

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        quoteHub.Add(Quotes.Take(20));

        // initialize observer
        VortexHub observer = quoteHub.ToVortexHub(lookbackPeriods);

        // fetch initial results (early)
        IReadOnlyList<VortexResult> actuals = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 20; i < length; i++)
        {
            // skip one (add later)
            if (i == 80) { continue; }

            Quote q = Quotes[i];
            quoteHub.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105) { quoteHub.Add(q); }
        }

        // late arrival, should equal series
        quoteHub.Insert(Quotes[80]);
        actuals.IsExactly(expectedOriginal);

        // delete, should equal series (revised)
        quoteHub.RemoveAt(removeAtIndex);

        IReadOnlyList<VortexResult> expectedRevised = RevisedQuotes.ToVortex(lookbackPeriods);

        actuals.Should().HaveCount(501);
        actuals.IsExactly(expectedRevised);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void WithCachePruning_MatchesSeriesExactly()
    {
        const int maxCacheSize = 50;
        const int totalQuotes = 100;

        IReadOnlyList<Quote> quotes = Quotes.Take(totalQuotes).ToList();
        IReadOnlyList<VortexResult> expected = quotes
            .ToVortex(lookbackPeriods)
            .TakeLast(maxCacheSize)
            .ToList();

        // Setup with cache limit
        QuoteHub quoteHub = new() { MaxCacheSize = maxCacheSize };
        VortexHub observer = quoteHub.ToVortexHub(lookbackPeriods);

        // Stream more quotes than cache can hold
        quoteHub.Add(quotes);

        // Verify cache was pruned
        quoteHub.Quotes.Should().HaveCount(maxCacheSize);
        observer.Results.Should().HaveCount(maxCacheSize);

        // Streaming results should match last N from full series (original series with front chopped off)
        // NOT recomputation on just the cached quotes (which would have different warmup)
        observer.Results.IsExactly(expected);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        VortexHub hub = new(new QuoteHub(), 14);
        hub.ToString().Should().Be("VORTEX(14)");
    }
}
