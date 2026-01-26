namespace StreamHubs;

[TestClass]
public class WmaHubTests : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    private const int LookbackPeriods = 14;

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        quoteHub.Add(Quotes.Take(20));

        // initialize observer
        WmaHub observer = quoteHub.ToWmaHub(LookbackPeriods);

        // fetch initial results (early)
        IReadOnlyList<WmaResult> sut = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 20; i < quotesCount; i++)
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

        IReadOnlyList<WmaResult> expectedOriginal = Quotes.ToWma(LookbackPeriods);
        sut.IsExactly(expectedOriginal);

        // delete, should equal series (revised)
        quoteHub.RemoveAt(removeAtIndex);
        IReadOnlyList<WmaResult> expectedRevised = RevisedQuotes.ToWma(LookbackPeriods);
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
        IReadOnlyList<WmaResult> expected = quotes
            .ToWma(LookbackPeriods)
            .TakeLast(maxCacheSize)
            .ToList();

        // Setup with cache limit
        QuoteHub quoteHub = new() { MaxCacheSize = maxCacheSize };
        WmaHub observer = quoteHub.ToWmaHub(LookbackPeriods);

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
        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        WmaHub observer = quoteHub
            .ToQuotePartHub(CandlePart.HL2)
            .ToWmaHub(LookbackPeriods);

        // emulate quote stream
        for (int i = 0; i < quotesCount; i++) { quoteHub.Add(Quotes[i]); }

        // final results
        IReadOnlyList<WmaResult> sut = observer.Results;

        // time-series, for comparison
        IReadOnlyList<WmaResult> expected = Quotes
            .Use(CandlePart.HL2)
            .ToWma(LookbackPeriods);

        // assert, should equal series
        sut.IsExactly(expected);
        sut.Should().HaveCount(quotesCount);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider_MatchesSeriesExactly()
    {
        const int wmaPeriods = LookbackPeriods;
        const int emaPeriods = 10;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        EmaHub observer = quoteHub
            .ToWmaHub(wmaPeriods)
            .ToEmaHub(emaPeriods);

        // emulate adding quotes to provider hub
        for (int i = 0; i < quotesCount; i++)
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
        IReadOnlyList<EmaResult> sut = observer.Results;

        // time-series, for comparison (revised)
        IReadOnlyList<EmaResult> expected = RevisedQuotes
            .ToWma(wmaPeriods)
            .ToEma(emaPeriods);

        // assert, should equal series
        sut.Should().HaveCount(quotesCount - 1);
        sut.IsExactly(expected);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        WmaHub hub = new(new QuoteHub(), LookbackPeriods);
        hub.ToString().Should().Be($"WMA({LookbackPeriods})");
    }
}
