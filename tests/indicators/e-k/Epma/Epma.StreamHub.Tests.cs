namespace StreamHubs;

[TestClass]
public class EpmaHubTests : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    private const int lookbackPeriods = 20;

    private static readonly IReadOnlyList<EpmaResult> series
        = Quotes.ToEpma(lookbackPeriods);

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider (warmup coverage)
        quoteHub.Add(Quotes.Take(20));

        // initialize observer
        EpmaHub observer = quoteHub.ToEpmaHub(lookbackPeriods);

        // fetch initial results (early)
        IReadOnlyList<EpmaResult> actuals = observer.Results;

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

        actuals.Should().HaveCount(length);
        actuals.IsExactly(series);

        // delete, should equal series (revised)
        quoteHub.Remove(Quotes[removeAtIndex]);

        IReadOnlyList<EpmaResult> expectedRevised = RevisedQuotes.ToEpma(lookbackPeriods);

        actuals.Should().HaveCount(501);
        actuals.IsExactly(expectedRevised);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        EpmaHub hub = new(new QuoteHub(), lookbackPeriods);
        hub.ToString().Should().Be($"EPMA({lookbackPeriods})");
    }

    [TestMethod]
    public void ConsistencyWithSeries()
    {
        // Compare stream results with series results
        QuoteHub quoteHub = new();
        EpmaHub epmaHub = quoteHub.ToEpmaHub(lookbackPeriods);

        foreach (Quote quote in Quotes)
        {
            quoteHub.Add(quote);
        }

        IReadOnlyList<EpmaResult> streamResults = epmaHub.Results;
        IReadOnlyList<EpmaResult> seriesResults = Quotes.ToEpma(lookbackPeriods);

        streamResults.Should().HaveCount(seriesResults.Count);
        streamResults.IsExactly(seriesResults);
    }

    [TestMethod]
    public void RealTimeSimulation()
    {
        // Simulate real-time data processing
        QuoteHub quoteHub = new();
        EpmaHub epmaHub = quoteHub.ToEpmaHub(lookbackPeriods);

        // Process initial historical data
        foreach (Quote quote in Quotes.Take(100))
        {
            quoteHub.Add(quote);
        }

        int initialCount = epmaHub.Results.Count;
        initialCount.Should().Be(100);

        // Process new incoming quotes
        foreach (Quote quote in Quotes.Skip(100).Take(10))
        {
            quoteHub.Add(quote);

            IReadOnlyList<EpmaResult> currentResults = epmaHub.Results;
            EpmaResult latestResult = currentResults[^1];

            // Verify real-time calculations
            if (latestResult.Epma.HasValue)
            {
                latestResult.Epma.Should().BeOfType(typeof(double));
                latestResult.Timestamp.Should().Be(quote.Timestamp);
            }
        }

        epmaHub.Results.Should().HaveCount(110);
    }

    [TestMethod]
    public void ChainObserver_ChainedProvider_MatchesSeriesExactly()
    {
        // Test EPMA observing another indicator chain
        const int smaPeriods = 10;

        QuoteHub quoteHub = new();
        EpmaHub epmaHub = quoteHub
            .ToSmaHub(smaPeriods)
            .ToEpmaHub(lookbackPeriods);

        foreach (Quote quote in Quotes)
        {
            quoteHub.Add(quote);
        }

        IReadOnlyList<EpmaResult> streamResults = epmaHub.Results;
        IReadOnlyList<EpmaResult> seriesResults = Quotes
            .ToSma(smaPeriods)
            .ToEpma(lookbackPeriods);

        streamResults.Should().HaveCount(Quotes.Count);
        streamResults.IsExactly(seriesResults);
    }

    [TestMethod]
    public void ChainProvider_MatchesSeriesExactly()
    {
        // Test EPMA chaining to other indicators
        const int smaPeriods = 10;

        QuoteHub quoteHub = new();
        SmaHub smaHub = quoteHub
            .ToEpmaHub(lookbackPeriods)
            .ToSmaHub(smaPeriods);

        // emulate quote stream
        for (int i = 0; i < quotesCount; i++)
        {
            if (i == 80) { continue; }  // Skip for late arrival

            Quote q = Quotes[i];
            quoteHub.Add(q);

            if (i is > 100 and < 105) { quoteHub.Add(q); }  // Duplicate quotes
        }

        quoteHub.Insert(Quotes[80]);  // Late arrival
        quoteHub.Remove(Quotes[removeAtIndex]);  // Remove

        IReadOnlyList<SmaResult> sut = smaHub.Results;
        IReadOnlyList<SmaResult> expected = RevisedQuotes
            .ToEpma(lookbackPeriods)
            .ToSma(smaPeriods);

        sut.Should().HaveCount(quotesCount - 1);
        sut.IsExactly(expected);

        smaHub.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainableWithOtherIndicators()
    {
        // Test EPMA chaining with other indicators
        QuoteHub quoteHub = new();
        EpmaHub epmaHub = quoteHub.ToEpmaHub(lookbackPeriods);
        SmaHub smaHub = epmaHub.ToSmaHub(10);

        foreach (Quote quote in Quotes)
        {
            quoteHub.Add(quote);
        }

        IReadOnlyList<SmaResult> chainedResults = smaHub.Results;
        chainedResults.Should().HaveCount(Quotes.Count);

        // Verify chained calculation matches expected with tolerance
        IReadOnlyList<SmaResult> expectedChained = Quotes
            .ToEpma(lookbackPeriods)
            .ToSma(10);

        chainedResults.Should().HaveCount(expectedChained.Count);
        chainedResults.IsExactly(expectedChained);
    }

    [TestMethod]
    public void Add()
    {
        // Additional test for streaming functionality
        QuoteHub quoteHub = new();
        EpmaHub epmaHub = quoteHub.ToEpmaHub(lookbackPeriods);

        foreach (Quote quote in Quotes)
        {
            quoteHub.Add(quote);
        }

        IReadOnlyList<EpmaResult> sut = epmaHub.Results;
        sut.Should().HaveCount(Quotes.Count);
        sut.IsExactly(series);
    }
}
