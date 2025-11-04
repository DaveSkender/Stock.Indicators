namespace StreamHub;

[TestClass]
public class EpmaStreamHubTests : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    private const int lookbackPeriods = 20;

    private static readonly IReadOnlyList<EpmaResult> series
        = Quotes.ToEpma(lookbackPeriods);

    [TestMethod]
    public void QuoteObserver()
    {
        // Test streaming with state maintenance
        QuoteHub quoteHub = new();
        EpmaHub epmaHub = quoteHub.ToEpmaHub(lookbackPeriods);

        foreach (Quote quote in Quotes)
        {
            quoteHub.Add(quote);
        }

        IReadOnlyList<EpmaResult> results = epmaHub.Results;

        // Verify results match series calculation exactly
        results.Should().HaveCount(Quotes.Count);
        results.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public override void CustomToString()
    {
        QuoteHub quoteHub = new();
        EpmaHub epmaHub = quoteHub.ToEpmaHub(lookbackPeriods);

        epmaHub.ToString().Should().Be($"EPMA({lookbackPeriods})");
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
        streamResults.Should().BeEquivalentTo(seriesResults);
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
    public void ChainObserver()
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
        streamResults.Should().BeEquivalentTo(seriesResults, options => options.WithStrictOrdering());

        epmaHub.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider()
    {
        // Test EPMA chaining to other indicators
        const int smaPeriods = 10;

        QuoteHub quoteHub = new();
        SmaHub smaHub = quoteHub
            .ToEpmaHub(lookbackPeriods)
            .ToSmaHub(smaPeriods);

        foreach (Quote quote in Quotes)
        {
            quoteHub.Add(quote);
        }

        IReadOnlyList<SmaResult> chainedResults = smaHub.Results;
        IReadOnlyList<SmaResult> expectedChained = Quotes
            .ToEpma(lookbackPeriods)
            .ToSma(smaPeriods);

        chainedResults.Should().HaveCount(Quotes.Count);
        chainedResults.Should().BeEquivalentTo(expectedChained, options => options.WithStrictOrdering());

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
        chainedResults.Should().BeEquivalentTo(expectedChained);
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

        IReadOnlyList<EpmaResult> results = epmaHub.Results;
        results.Should().HaveCount(Quotes.Count);
        results.Should().BeEquivalentTo(series);
    }
}
