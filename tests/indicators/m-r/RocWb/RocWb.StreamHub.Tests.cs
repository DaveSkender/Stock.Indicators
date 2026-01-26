namespace StreamHubs;

[TestClass]
public class RocWbHubTests : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    private const int lookbackPeriods = 20;
    private const int emaPeriods = 5;
    private const int stdDevPeriods = 5;

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        quoteHub.Add(Quotes.Take(25));

        // initialize observer
        RocWbHub observer = quoteHub
            .ToRocWbHub(lookbackPeriods, emaPeriods, stdDevPeriods);

        // fetch initial results (early)
        IReadOnlyList<RocWbResult> sut = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 25; i < quotesCount; i++)
        {
            if (i == 80) { continue; }  // Skip for late arrival

            Quote q = Quotes[i];
            quoteHub.Add(q);

            if (i is > 100 and < 105) { quoteHub.Add(q); }  // Duplicate quotes
        }

        // late arrival, should equal series
        quoteHub.Insert(Quotes[80]);

        IReadOnlyList<RocWbResult> expectedOriginal = Quotes.ToRocWb(lookbackPeriods, emaPeriods, stdDevPeriods);
        sut.IsExactly(expectedOriginal);
        sut.Should().HaveCount(quotesCount);

        // delete, should equal series (revised)
        quoteHub.RemoveAt(removeAtIndex);

        IReadOnlyList<RocWbResult> expectedRevised = RevisedQuotes.ToRocWb(lookbackPeriods, emaPeriods, stdDevPeriods);
        sut.IsExactly(expectedRevised);
        sut.Should().HaveCount(quotesCount - 1);

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
        IReadOnlyList<RocWbResult> expected = quotes
            .ToRocWb(lookbackPeriods, emaPeriods, stdDevPeriods)
            .TakeLast(maxCacheSize)
            .ToList();

        // Setup with cache limit
        QuoteHub quoteHub = new() { MaxCacheSize = maxCacheSize };
        RocWbHub observer = quoteHub.ToRocWbHub(lookbackPeriods, emaPeriods, stdDevPeriods);

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
        const int emaInnerPeriods = 12;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        RocWbHub observer = quoteHub
            .ToEmaHub(emaInnerPeriods)
            .ToRocWbHub(lookbackPeriods, emaPeriods, stdDevPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // final results
        IReadOnlyList<RocWbResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<RocWbResult> seriesList
           = quotesList
            .ToEma(emaInnerPeriods)
            .ToRocWb(lookbackPeriods, emaPeriods, stdDevPeriods);

        // assert, should equal series
        streamList.Should().HaveCount(length);
        streamList.IsExactly(seriesList);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider_MatchesSeriesExactly()
    {
        const int emaOuterPeriods = 12;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        EmaHub observer = quoteHub
            .ToRocWbHub(lookbackPeriods, emaPeriods, stdDevPeriods)
            .ToEmaHub(emaOuterPeriods);

        // emulate quote stream
        for (int i = 0; i < quotesCount; i++)
        {
            if (i == 80) { continue; }  // Skip for late arrival

            Quote q = Quotes[i];
            quoteHub.Add(q);

            if (i is > 100 and < 105) { quoteHub.Add(q); }  // Duplicate quotes
        }

        quoteHub.Insert(Quotes[80]);  // Late arrival
        quoteHub.RemoveAt(removeAtIndex);  // Remove

        // final results
        IReadOnlyList<EmaResult> sut = observer.Results;

        // time-series, for comparison (revised)
        IReadOnlyList<EmaResult> expected = RevisedQuotes
            .ToRocWb(lookbackPeriods, emaPeriods, stdDevPeriods)
            .ToEma(emaOuterPeriods);

        // assert, should equal series
        sut.Should().HaveCount(quotesCount - 1);
        sut.IsExactly(expected);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        RocWbHub hub = new(new QuoteHub(), lookbackPeriods, emaPeriods, stdDevPeriods);
        hub.ToString().Should().Be($"ROCWB({lookbackPeriods},{emaPeriods},{stdDevPeriods})");
    }
}
