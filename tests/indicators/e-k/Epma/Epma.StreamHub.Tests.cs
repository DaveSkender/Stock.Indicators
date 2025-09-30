namespace StreamHub;

[TestClass]
public class EpmaStreamHubTests : StreamHubTestBase
{
    private const int lookbackPeriods = 20;

    private static readonly IReadOnlyList<EpmaResult> series
        = Quotes.ToEpma(lookbackPeriods);

    [TestMethod]
    public override void QuoteObserver()
    {
        // Test streaming with state maintenance
        QuoteHub<Quote> quoteHub = new();
        EpmaHub<Quote> epmaHub = quoteHub.ToEpma(lookbackPeriods);

        foreach (Quote quote in Quotes)
        {
            quoteHub.Add(quote);
        }

        IReadOnlyList<EpmaResult> results = epmaHub.Results;

        // Verify results match series calculation with tolerance for floating-point precision
        results.Should().HaveCount(Quotes.Count);

        for (int i = 0; i < results.Count; i++)
        {
            EpmaResult actual = results[i];
            EpmaResult expected = series[i];

            actual.Timestamp.Should().Be(expected.Timestamp);

            if (expected.Epma.HasValue && actual.Epma.HasValue)
            {
                actual.Epma.Should().BeApproximately(expected.Epma.Value, 6, $"at index {i}");
            }
            else
            {
                actual.Epma.Should().Be(expected.Epma, $"at index {i}");
            }
        }

        // Verify specific values
        EpmaResult last = results[^1];
        EpmaResult expectedLast = series[^1];
        last.Epma.Should().BeApproximately(expectedLast.Epma, 6);
    }

    [TestMethod]
    public override void CustomToString()
    {
        QuoteHub<Quote> quoteHub = new();
        EpmaHub<Quote> epmaHub = quoteHub.ToEpma(lookbackPeriods);

        epmaHub.ToString().Should().Be($"EPMA({lookbackPeriods})");
    }

    [TestMethod]
    public void ConsistencyWithSeries()
    {
        // Compare stream results with series results
        QuoteHub<Quote> quoteHub = new();
        EpmaHub<Quote> epmaHub = quoteHub.ToEpma(lookbackPeriods);

        foreach (Quote quote in Quotes)
        {
            quoteHub.Add(quote);
        }

        IReadOnlyList<EpmaResult> streamResults = epmaHub.Results;
        IReadOnlyList<EpmaResult> seriesResults = Quotes.ToEpma(lookbackPeriods);

        streamResults.Should().HaveCount(seriesResults.Count);

        for (int i = 0; i < streamResults.Count; i++)
        {
            EpmaResult actual = streamResults[i];
            EpmaResult expected = seriesResults[i];

            actual.Timestamp.Should().Be(expected.Timestamp);

            if (expected.Epma.HasValue && actual.Epma.HasValue)
            {
                actual.Epma.Should().BeApproximately(expected.Epma.Value, 6, $"at index {i}");
            }
            else
            {
                actual.Epma.Should().Be(expected.Epma, $"at index {i}");
            }
        }
    }

    [TestMethod]
    public void RealTimeSimulation()
    {
        // Simulate real-time data processing
        QuoteHub<Quote> quoteHub = new();
        EpmaHub<Quote> epmaHub = quoteHub.ToEpma(lookbackPeriods);

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
    public void ChainableWithOtherIndicators()
    {
        // Test EPMA chaining with other indicators
        QuoteHub<Quote> quoteHub = new();
        EpmaHub<Quote> epmaHub = quoteHub.ToEpma(lookbackPeriods);
        SmaHub<EpmaResult> smaHub = epmaHub.ToSma(10);

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

        for (int i = 0; i < chainedResults.Count; i++)
        {
            SmaResult actual = chainedResults[i];
            SmaResult expected = expectedChained[i];

            actual.Timestamp.Should().Be(expected.Timestamp);

            if (expected.Sma.HasValue && actual.Sma.HasValue)
            {
                actual.Sma.Should().BeApproximately(expected.Sma.Value, 6, $"at index {i}");
            }
            else
            {
                actual.Sma.Should().Be(expected.Sma, $"at index {i}");
            }
        }
    }

    [TestMethod]
    public void Add()
    {
        // Additional test for streaming functionality
        QuoteHub<Quote> quoteHub = new();
        EpmaHub<Quote> epmaHub = quoteHub.ToEpma(lookbackPeriods);

        foreach (Quote quote in Quotes)
        {
            quoteHub.Add(quote);
        }

        IReadOnlyList<EpmaResult> results = epmaHub.Results;
        results.Should().HaveCount(Quotes.Count);

        for (int i = 0; i < results.Count; i++)
        {
            EpmaResult actual = results[i];
            EpmaResult expected = series[i];

            actual.Timestamp.Should().Be(expected.Timestamp);

            if (expected.Epma.HasValue && actual.Epma.HasValue)
            {
                actual.Epma.Should().BeApproximately(expected.Epma.Value, 6, $"at index {i}");
            }
            else
            {
                actual.Epma.Should().Be(expected.Epma, $"at index {i}");
            }
        }
    }
}
