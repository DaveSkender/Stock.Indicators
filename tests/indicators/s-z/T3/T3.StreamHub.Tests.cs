namespace StreamHubs;

[TestClass]
public class T3HubTests : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        const int lookbackPeriods = 5;
        const double volumeFactor = 0.7;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        quoteHub.Add(Quotes.Take(20));

        // initialize observer
        T3Hub observer = quoteHub.ToT3Hub(lookbackPeriods, volumeFactor);

        // fetch initial results (early)
        IReadOnlyList<T3Result> sut = observer.Results;

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

        IReadOnlyList<T3Result> expectedOriginal = Quotes.ToT3(lookbackPeriods, volumeFactor);
        sut.IsExactly(expectedOriginal);

        // delete, should equal series (revised)
        quoteHub.RemoveAt(removeAtIndex);
        IReadOnlyList<T3Result> expectedRevised = RevisedQuotes.ToT3(lookbackPeriods, volumeFactor);
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
        const int lookbackPeriods = 5;
        const double volumeFactor = 0.7;

        IReadOnlyList<Quote> quotes = Quotes.Take(totalQuotes).ToList();
        IReadOnlyList<T3Result> expected = quotes
            .ToT3(lookbackPeriods, volumeFactor)
            .TakeLast(maxCacheSize)
            .ToList();

        // Setup with cache limit
        QuoteHub quoteHub = new() { MaxCacheSize = maxCacheSize };
        T3Hub observer = quoteHub.ToT3Hub(lookbackPeriods, volumeFactor);

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
        const int smaPeriods = 10;
        const int t3Periods = 5;
        const double volumeFactor = 0.7;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        T3Hub observer = quoteHub
            .ToSmaHub(smaPeriods)
            .ToT3Hub(t3Periods, volumeFactor);

        // emulate quote stream
        for (int i = 0; i < quotesCount; i++) { quoteHub.Add(Quotes[i]); }

        // final results
        IReadOnlyList<T3Result> sut = observer.Results;

        // time-series, for comparison
        IReadOnlyList<T3Result> expected = Quotes
            .ToSma(smaPeriods)
            .ToT3(t3Periods, volumeFactor);

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
        const int t3Periods = 5;
        const double volumeFactor = 0.7;
        const int smaPeriods = 10;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        SmaHub observer = quoteHub
            .ToT3Hub(t3Periods, volumeFactor)
            .ToSmaHub(smaPeriods);

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
        IReadOnlyList<SmaResult> sut = observer.Results;

        // time-series, for comparison (revised)
        IReadOnlyList<SmaResult> expected = RevisedQuotes
            .ToT3(t3Periods, volumeFactor)
            .ToSma(smaPeriods);

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
        T3Hub hub = new(new QuoteHub(), 5, 0.7);
        hub.ToString().Should().Be("T3(5,0.7)");
    }

    [TestMethod]
    public void SettingsInheritance()
    {
        // setup quote hub (1st level)
        QuoteHub quoteHub = new();

        // setup t3 hub (2nd level)
        T3Hub t3Hub = quoteHub
            .ToT3Hub(lookbackPeriods: 5, volumeFactor: 0.7);

        // setup child hub (3rd level)
        SmaHub childHub = t3Hub
            .ToSmaHub(lookbackPeriods: 5);

        // note: despite `quoteHub` being parentless,
        // it has default properties; it should not
        // inherit its own empty quoteHub settings

        // assert
        quoteHub.Properties.Settings.Should().Be(0b00000000, "it has default settings, not inherited");
        t3Hub.Properties.Settings.Should().Be(0b00000000, "it has default properties");
        childHub.Properties.Settings.Should().Be(0b00000000, "it inherits default properties");
    }
}
