namespace StreamHubs;

[TestClass]
public class SuperTrendHubTests : StreamHubTestBase, ITestQuoteObserver
{
    private const int lookbackPeriods = 14;
    private const double multiplier = 3;
    private readonly IReadOnlyList<SuperTrendResult> expectedOriginal
        = Quotes.ToSuperTrend(lookbackPeriods, multiplier);

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        quoteHub.Add(Quotes.Take(20));

        // initialize observer
        SuperTrendHub observer = quoteHub.ToSuperTrendHub(lookbackPeriods, multiplier);

        // fetch initial results (early)
        IReadOnlyList<SuperTrendResult> actuals = observer.Results;

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
        quoteHub.Add(Quotes[80]);
        actuals.IsExactly(expectedOriginal);

        // delete, should equal series (revised)
        quoteHub.RemoveAt(removeAtIndex);

        IReadOnlyList<SuperTrendResult> expectedRevised = RevisedQuotes.ToSuperTrend(lookbackPeriods, multiplier);

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
        IReadOnlyList<SuperTrendResult> expected = quotes
            .ToSuperTrend(lookbackPeriods, multiplier)
            .TakeLast(maxCacheSize)
            .ToList();

        // Setup with cache limit
        QuoteHub quoteHub = new(maxCacheSize);
        SuperTrendHub observer = quoteHub.ToSuperTrendHub(lookbackPeriods, multiplier);

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
    public void LateArrival_MidStream_MatchesFreshStream()
    {
        const int totalQuotes = 300;
        const int lateIndex = 150;

        IReadOnlyList<Quote> quotes = Quotes.Take(totalQuotes).ToList();

        QuoteHub lateSource = new();
        SuperTrendHub lateHub = lateSource.ToSuperTrendHub(lookbackPeriods, multiplier);
        for (int i = 0; i < totalQuotes; i++)
        {
            if (i == lateIndex) { continue; }

            lateSource.Add(quotes[i]);
        }

        lateSource.Add(quotes[lateIndex]);

        QuoteHub freshSource = new();
        SuperTrendHub freshHub = freshSource.ToSuperTrendHub(lookbackPeriods, multiplier);
        freshSource.Add(quotes);

        lateHub.Results.Should().HaveCount(totalQuotes);
        lateHub.Results.IsExactly(freshHub.Results);

        lateHub.Unsubscribe();
        freshHub.Unsubscribe();
        lateSource.EndTransmission();
        freshSource.EndTransmission();
    }

    [TestMethod]
    public void LateArrival_AtAtrWarmupBoundary_MatchesFreshStream()
    {
        // SuperTrend emits first non-null result at lookback (= 14); index
        // 20 forces the rollback path to replay across the ATR-seeded
        // direction-state transition that determines stop placement.
        const int totalQuotes = 300;
        const int lateIndex = 20;

        IReadOnlyList<Quote> quotes = Quotes.Take(totalQuotes).ToList();

        QuoteHub lateSource = new();
        SuperTrendHub lateHub = lateSource.ToSuperTrendHub(lookbackPeriods, multiplier);
        for (int i = 0; i < totalQuotes; i++)
        {
            if (i == lateIndex) { continue; }

            lateSource.Add(quotes[i]);
        }

        lateSource.Add(quotes[lateIndex]);

        QuoteHub freshSource = new();
        SuperTrendHub freshHub = freshSource.ToSuperTrendHub(lookbackPeriods, multiplier);
        freshSource.Add(quotes);

        lateHub.Results.Should().HaveCount(totalQuotes);
        lateHub.Results.IsExactly(freshHub.Results);

        lateHub.Unsubscribe();
        freshHub.Unsubscribe();
        lateSource.EndTransmission();
        freshSource.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        SuperTrendHub hub = new(new QuoteHub(), 14, 3.0);
        hub.ToString().Should().Be("SUPERTREND(14,3)");
    }
}
