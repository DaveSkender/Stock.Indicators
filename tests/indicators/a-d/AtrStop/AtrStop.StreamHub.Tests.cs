namespace StreamHubs;

[TestClass]
public class AtrStopHubTests : StreamHubTestBase, ITestQuoteObserver
{
    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider (batch)
        quoteHub.Add(Quotes.Take(20));

        // initialize observer
        AtrStopHub observer = quoteHub
            .ToAtrStopHub();

        observer.Results.Should().HaveCount(20);

        // fetch initial results (early)
        IReadOnlyList<AtrStopResult> actuals
            = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 20; i < quotesCount; i++)
        {
            // skip one (add later)
            if (i is 30 or 80)
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

        // late arrivals
        quoteHub.Add(Quotes[30]);  // rebuilds complete series
        quoteHub.Add(Quotes[80]);  // rebuilds from last reversal

        // delete
        quoteHub.RemoveAt(removeAtIndex);

        // time-series, for comparison
        IReadOnlyList<AtrStopResult> expected = RevisedQuotes.ToAtrStop();

        // assert, should equal series
        actuals.Should().HaveCount(quotesCount - 1);
        actuals.IsExactly(expected);

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
        IReadOnlyList<AtrStopResult> expected = quotes
            .ToAtrStop()
            .TakeLast(maxCacheSize)
            .ToList();

        // Setup with cache limit
        QuoteHub quoteHub = new(maxCacheSize);
        AtrStopHub observer = quoteHub.ToAtrStopHub();

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
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactlyHighLow()
    {
        // simple test, just to check High/Low variant

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        AtrStopHub observer = quoteHub
            .ToAtrStopHub(endType: EndType.HighLow);

        // add quotes to quoteHub
        quoteHub.Add(Quotes);

        // stream results
        IReadOnlyList<AtrStopResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<AtrStopResult> seriesList
           = Quotes
            .ToAtrStop(endType: EndType.HighLow);

        // assert, should equal series
        streamList.Should().HaveCount(Quotes.Count);
        streamList.IsExactly(seriesList);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void LateArrival_MidStream_MatchesFreshStream()
    {
        // ATR-Stop carries reversal state across bars; a late arrival
        // landing inside an established trend must rebuild the same
        // stop sequence as the fresh stream.
        const int totalQuotes = 300;
        const int lateIndex = 150;

        IReadOnlyList<Quote> quotes = Quotes.Take(totalQuotes).ToList();

        QuoteHub lateSource = new();
        AtrStopHub lateHub = lateSource.ToAtrStopHub();
        for (int i = 0; i < totalQuotes; i++)
        {
            if (i == lateIndex) { continue; }
            lateSource.Add(quotes[i]);
        }
        lateSource.Add(quotes[lateIndex]);

        QuoteHub freshSource = new();
        AtrStopHub freshHub = freshSource.ToAtrStopHub();
        freshSource.Add(quotes);

        lateHub.Results.Should().HaveCount(totalQuotes);
        lateHub.Results.IsExactly(freshHub.Results);

        lateHub.Unsubscribe();
        freshHub.Unsubscribe();
        lateSource.EndTransmission();
        freshSource.EndTransmission();
    }

    [TestMethod]
    public void LateArrival_AtAtrStopSeedBoundary_MatchesFreshStream()
    {
        // ATR-Stop emits first non-null result at lookback (= 14); index
        // 20 forces replay across the ATR seed + initial direction
        // transition that anchors the trailing-stop level.
        const int totalQuotes = 300;
        const int lateIndex = 20;

        IReadOnlyList<Quote> quotes = Quotes.Take(totalQuotes).ToList();

        QuoteHub lateSource = new();
        AtrStopHub lateHub = lateSource.ToAtrStopHub();
        for (int i = 0; i < totalQuotes; i++)
        {
            if (i == lateIndex) { continue; }
            lateSource.Add(quotes[i]);
        }
        lateSource.Add(quotes[lateIndex]);

        QuoteHub freshSource = new();
        AtrStopHub freshHub = freshSource.ToAtrStopHub();
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
        AtrStopHub hub = new(new QuoteHub(), 14, 3, EndType.Close);
        hub.ToString().Should().Be("ATR-STOP(14,3,CLOSE)");
    }
}
