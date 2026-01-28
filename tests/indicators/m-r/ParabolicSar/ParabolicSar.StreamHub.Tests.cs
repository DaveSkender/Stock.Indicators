namespace StreamHubs;

[TestClass]
public class ParabolicSarHubTests : StreamHubTestBase, ITestQuoteObserver, ITestChainProvider
{
    private const double accelerationStep = 0.02;
    private const double maxAccelerationFactor = 0.2;

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        List<Quote> quotes = Quotes.ToList();
        int length = quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        for (int i = 0; i < 50; i++)
        {
            quoteHub.Add(quotes[i]);
        }

        // initialize observer
        ParabolicSarHub observer = quoteHub
            .ToParabolicSarHub(accelerationStep, maxAccelerationFactor);

        // fetch initial results (early)
        IReadOnlyList<ParabolicSarResult> streamList = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 50; i < length; i++)
        {
            // skip one (add later)
            if (i == 80)
            {
                continue;
            }

            Quote q = quotes[i];
            quoteHub.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105)
            {
                quoteHub.Add(q);
            }
        }

        // late arrival
        quoteHub.Add(quotes[80]);

        // Should match series after all quotes added
        IReadOnlyList<ParabolicSarResult> expectedOriginal = quotes
            .ToParabolicSar(accelerationStep, maxAccelerationFactor);

        streamList.IsExactly(expectedOriginal);

        // delete
        quoteHub.RemoveAt(removeAtIndex);
        quotes.RemoveAt(removeAtIndex);

        // time-series, for comparison (revised)
        IReadOnlyList<ParabolicSarResult> seriesList = quotes
            .ToParabolicSar(accelerationStep, maxAccelerationFactor);

        // assert, should equal series (revised)
        streamList.Should().HaveCount(501);
        streamList.IsExactly(seriesList);

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
        IReadOnlyList<ParabolicSarResult> expected = quotes
            .ToParabolicSar(accelerationStep, maxAccelerationFactor)
            .TakeLast(maxCacheSize)
            .ToList();

        // Setup with cache limit
        QuoteHub quoteHub = new(maxCacheSize);
        ParabolicSarHub observer = quoteHub.ToParabolicSarHub(accelerationStep, maxAccelerationFactor);

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
    public void ChainProvider_MatchesSeriesExactly()
    {
        const int smaPeriods = 10;
        List<Quote> quotes = Quotes.ToList();
        int length = quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        SmaHub observer = quoteHub
            .ToParabolicSarHub(accelerationStep, maxAccelerationFactor)
            .ToSmaHub(smaPeriods);

        // emulate adding quotes to provider hub
        for (int i = 0; i < length; i++)
        {
            // skip one (add later)
            if (i == 80)
            {
                continue;
            }

            Quote q = quotes[i];
            quoteHub.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105)
            {
                quoteHub.Add(q);
            }
        }

        // late arrival
        quoteHub.Add(quotes[80]);

        // delete
        quoteHub.RemoveAt(removeAtIndex);
        quotes.RemoveAt(removeAtIndex);

        // final results
        IReadOnlyList<SmaResult> streamList = observer.Results;

        // time-series, for comparison (revised)
        IReadOnlyList<SmaResult> seriesList = quotes
            .ToParabolicSar(accelerationStep, maxAccelerationFactor)
            .ToSma(smaPeriods);

        // assert, should equal series
        streamList.Should().HaveCount(501);
        streamList.IsExactly(seriesList);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        ParabolicSarHub hub = new(new QuoteHub(), 0.02, 0.2);
        hub.ToString().Should().Be("PSAR(0.02,0.2,0.02)");
    }

    [TestMethod]
    public void CustomInitialFactor()
    {
        const double customInitialFactor = 0.05;
        List<Quote> quotesList = Quotes.ToList();

        // setup quote provider hub
        QuoteHub quoteHub = new();
        quoteHub.Add(quotesList);

        // initialize observer with custom initial factor
        ParabolicSarHub observer = quoteHub
            .ToParabolicSarHub(accelerationStep, maxAccelerationFactor, customInitialFactor);

        // fetch results
        IReadOnlyList<ParabolicSarResult> streamList = observer.Results;

        // time-series, for comparison
        IReadOnlyList<ParabolicSarResult> seriesList = quotesList
            .ToParabolicSar(accelerationStep, maxAccelerationFactor, customInitialFactor);

        // assert, should equal series
        streamList.IsExactly(seriesList);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }
}
