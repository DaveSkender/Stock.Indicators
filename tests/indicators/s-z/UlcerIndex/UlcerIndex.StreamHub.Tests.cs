namespace StreamHubs;

[TestClass]
public class UlcerIndexHubTests : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    private const int lookbackPeriods = 14;
    private readonly IReadOnlyList<UlcerIndexResult> expectedOriginal = Quotes.ToUlcerIndex(lookbackPeriods);

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        quoteHub.Add(Quotes.Take(20));

        // initialize observer
        UlcerIndexHub observer = quoteHub.ToUlcerIndexHub(lookbackPeriods);

        // fetch initial results (early)
        IReadOnlyList<UlcerIndexResult> actuals = observer.Results;

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

        IReadOnlyList<UlcerIndexResult> expectedRevised = RevisedQuotes.ToUlcerIndex(lookbackPeriods);

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
        IReadOnlyList<UlcerIndexResult> expected = quotes
            .ToUlcerIndex(lookbackPeriods)
            .TakeLast(maxCacheSize)
            .ToList();

        // Setup with cache limit
        QuoteHub quoteHub = new() { MaxCacheSize = maxCacheSize };
        UlcerIndexHub observer = quoteHub.ToUlcerIndexHub(lookbackPeriods);

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
    public void ChainObserver_ChainedProvider_MatchesSeriesExactly()
    {
        const int ulcerPeriods = 14;
        const int smaPeriods = 8;
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        UlcerIndexHub observer = quoteHub
            .ToSmaHub(smaPeriods)
            .ToUlcerIndexHub(ulcerPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++) { quoteHub.Add(Quotes[i]); }

        // final results
        IReadOnlyList<UlcerIndexResult> actuals = observer.Results;

        // time-series, for comparison
        IReadOnlyList<UlcerIndexResult> expected = Quotes
            .ToSma(smaPeriods)
            .ToUlcerIndex(ulcerPeriods);

        // assert, should equal series
        actuals.Should().HaveCount(length);
        actuals.IsExactly(expected);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider_MatchesSeriesExactly()
    {
        const int ulcerPeriods = 14;
        const int smaPeriods = 10;
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        SmaHub observer = quoteHub
            .ToUlcerIndexHub(ulcerPeriods)
            .ToSmaHub(smaPeriods);

        // emulate adding quotes to provider hub
        for (int i = 0; i < length; i++)
        {
            // skip one (add later)
            if (i == 80) { continue; }

            Quote q = Quotes[i];
            quoteHub.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105) { quoteHub.Add(q); }
        }

        // late arrival
        quoteHub.Insert(Quotes[80]);

        // delete
        quoteHub.RemoveAt(removeAtIndex);

        // final results
        IReadOnlyList<SmaResult> actuals
            = observer.Results;

        // time-series, for comparison (revised)
        IReadOnlyList<SmaResult> seriesList = RevisedQuotes
            .ToUlcerIndex(ulcerPeriods)
            .ToSma(smaPeriods);

        // assert, should equal series
        actuals.Should().HaveCount(length - 1);
        actuals.IsExactly(seriesList);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        UlcerIndexHub hub = new(new QuoteHub(), 14);
        hub.ToString().Should().Be("ULCER(14)");
    }

    [TestMethod]
    public void Results_AreAlwaysBounded()
    {
        IReadOnlyList<UlcerIndexResult> sut = Quotes.ToUlcerIndexHub(14).Results;
        sut.IsBetween(static x => x.UlcerIndex, 0, 100);
    }
}
