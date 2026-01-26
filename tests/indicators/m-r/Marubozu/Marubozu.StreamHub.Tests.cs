namespace StreamHubs;

[TestClass]
public class MarubozuHubTests : StreamHubTestBase, ITestQuoteObserver
{
    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        // setup quote provider
        QuoteHub quoteHub = new();

        // prefill quotes to provider
        for (int i = 0; i < 20; i++)
        {
            quoteHub.Add(Quotes[i]);
        }

        // initialize observer
        MarubozuHub marubozuHub = quoteHub
            .ToMarubozuHub(95);

        // fetch initial results (early)
        IReadOnlyList<CandleResult> actuals
            = marubozuHub.Results;

        // emulate adding quotes to provider
        for (int i = 20; i < quotesCount; i++)
        {
            // skip one (add later)
            if (i == 80)
            {
                continue;
            }

            Quote q = Quotes[i];
            quoteHub.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105)
            {
                quoteHub.Add(q);
            }
        }

        // late arrival
        quoteHub.Insert(Quotes[80]);

        // delete
        quoteHub.RemoveAt(removeAtIndex);

        // time-series, for comparison
        IReadOnlyList<CandleResult> expected = RevisedQuotes.ToMarubozu(95);

        // assert, should equal series
        actuals.Should().HaveCount(quotesCount - 1);
        actuals.IsExactly(expected);

        marubozuHub.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void WithCachePruning_MatchesSeriesExactly()
    {
        const int maxCacheSize = 50;
        const int totalQuotes = 100;

        IReadOnlyList<Quote> quotes = Quotes.Take(totalQuotes).ToList();
        IReadOnlyList<CandleResult> expected = quotes
            .ToMarubozu(95)
            .TakeLast(maxCacheSize)
            .ToList();

        // Setup with cache limit
        QuoteHub quoteHub = new() { MaxCacheSize = maxCacheSize };
        MarubozuHub observer = quoteHub.ToMarubozuHub(95);

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
        MarubozuHub hub = new(new QuoteHub(), 95);
        hub.ToString().Should().Be("MARUBOZU(95.0)");
    }
}
