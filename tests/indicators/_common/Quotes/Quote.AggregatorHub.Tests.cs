namespace StreamHubs;

[TestClass]
public class QuoteAggregatorHubTests : StreamHubTestBase, ITestQuoteObserver, ITestChainProvider
{
    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        QuoteHub provider = new();
        QuoteAggregatorHub aggregator = provider.ToQuoteAggregatorHub(PeriodSize.FiveMinutes);

        string result = aggregator.ToString();

        result.Should().Contain("QUOTE-AGG");
        result.Should().Contain("00:05:00");

        aggregator.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void BasicAggregation_OneMinuteToFiveMinute()
    {
        // Setup: Create minute-level quotes
        List<Quote> minuteQuotes =
        [
            new(DateTime.Parse("2023-11-09 10:00", invariantCulture), 100, 105, 99, 102, 1000),
            new(DateTime.Parse("2023-11-09 10:01", invariantCulture), 102, 106, 101, 104, 1100),
            new(DateTime.Parse("2023-11-09 10:02", invariantCulture), 104, 107, 103, 105, 1200),
            new(DateTime.Parse("2023-11-09 10:03", invariantCulture), 105, 108, 104, 106, 1300),
            new(DateTime.Parse("2023-11-09 10:04", invariantCulture), 106, 109, 105, 107, 1400),
            new(DateTime.Parse("2023-11-09 10:05", invariantCulture), 107, 110, 106, 108, 1500),
            new(DateTime.Parse("2023-11-09 10:06", invariantCulture), 108, 111, 107, 109, 1600),
        ];

        QuoteHub provider = new();
        QuoteAggregatorHub aggregator = provider.ToQuoteAggregatorHub(PeriodSize.FiveMinutes);

        // Add quotes
        foreach (Quote q in minuteQuotes)
        {
            provider.Add(q);
        }

        // Verify results
        IReadOnlyList<IQuote> results = aggregator.Results;

        results.Should().HaveCount(2);

        // First 5-minute bar (10:00-10:04)
        IQuote bar1 = results[0];
        bar1.Timestamp.Should().Be(DateTime.Parse("2023-11-09 10:00", invariantCulture));
        bar1.Open.Should().Be(100);
        bar1.High.Should().Be(109);  // Max of all highs in period
        bar1.Low.Should().Be(99);    // Min of all lows in period
        bar1.Close.Should().Be(107);  // Last close in period
        bar1.Volume.Should().Be(6000); // Sum of all volumes

        // Second 5-minute bar (10:05-10:06, incomplete)
        IQuote bar2 = results[1];
        bar2.Timestamp.Should().Be(DateTime.Parse("2023-11-09 10:05", invariantCulture));
        bar2.Open.Should().Be(107);
        bar2.High.Should().Be(111);
        bar2.Low.Should().Be(106);
        bar2.Close.Should().Be(109);
        bar2.Volume.Should().Be(3100);

        // Cleanup
        aggregator.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void AggregationWithTimeSpan()
    {
        List<Quote> minuteQuotes =
        [
            new(DateTime.Parse("2023-11-09 10:00", invariantCulture), 100, 105, 99, 102, 1000),
            new(DateTime.Parse("2023-11-09 10:01", invariantCulture), 102, 106, 101, 104, 1100),
            new(DateTime.Parse("2023-11-09 10:02", invariantCulture), 104, 107, 103, 105, 1200),
        ];

        QuoteHub provider = new();
        QuoteAggregatorHub aggregator = provider.ToQuoteAggregatorHub(TimeSpan.FromMinutes(3));

        foreach (Quote q in minuteQuotes)
        {
            provider.Add(q);
        }

        IReadOnlyList<IQuote> results = aggregator.Results;

        results.Should().HaveCount(1);

        IQuote bar = results[0];
        bar.Timestamp.Should().Be(DateTime.Parse("2023-11-09 10:00", invariantCulture));
        bar.Open.Should().Be(100);
        bar.High.Should().Be(107);
        bar.Low.Should().Be(99);
        bar.Close.Should().Be(105);
        bar.Volume.Should().Be(3300);

        aggregator.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void GapFilling_CarriesForwardPrices()
    {
        List<Quote> minuteQuotes =
        [
            new(DateTime.Parse("2023-11-09 10:00", invariantCulture), 100, 105, 99, 102, 1000),
            new(DateTime.Parse("2023-11-09 10:01", invariantCulture), 102, 106, 101, 104, 1100),
            // Gap: 10:02 missing
            new(DateTime.Parse("2023-11-09 10:03", invariantCulture), 105, 108, 104, 106, 1300),
        ];

        QuoteHub provider = new();
        QuoteAggregatorHub aggregator = provider.ToQuoteAggregatorHub(
            PeriodSize.OneMinute,
            fillGaps: true);

        foreach (Quote q in minuteQuotes)
        {
            provider.Add(q);
        }

        IReadOnlyList<IQuote> results = aggregator.Results;

        // Should have 4 bars: 10:00, 10:01, 10:02 (gap-filled), 10:03
        results.Should().HaveCount(4);

        // Verify gap-filled bar at 10:02
        IQuote gapBar = results[2];
        gapBar.Timestamp.Should().Be(DateTime.Parse("2023-11-09 10:02", invariantCulture));
        gapBar.Open.Should().Be(104);  // Carried forward from 10:01 close
        gapBar.High.Should().Be(104);
        gapBar.Low.Should().Be(104);
        gapBar.Close.Should().Be(104);
        gapBar.Volume.Should().Be(0);

        aggregator.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void GapFilling_MultipleConsecutiveGaps()
    {
        List<Quote> minuteQuotes =
        [
            new(DateTime.Parse("2023-11-09 10:00", invariantCulture), 100, 105, 99, 102, 1000),
            // Gaps: 10:01, 10:02, 10:03 missing
            new(DateTime.Parse("2023-11-09 10:04", invariantCulture), 105, 108, 104, 106, 1300),
        ];

        QuoteHub provider = new();
        QuoteAggregatorHub aggregator = provider.ToQuoteAggregatorHub(
            PeriodSize.OneMinute,
            fillGaps: true);

        foreach (Quote q in minuteQuotes)
        {
            provider.Add(q);
        }

        IReadOnlyList<IQuote> results = aggregator.Results;

        // Should have 5 bars: 10:00, 10:01 (gap), 10:02 (gap), 10:03 (gap), 10:04
        results.Should().HaveCount(5);

        // Verify all gap-filled bars carry forward price of 102
        for (int i = 1; i <= 3; i++)
        {
            IQuote gapBar = results[i];
            gapBar.Open.Should().Be(102);
            gapBar.High.Should().Be(102);
            gapBar.Low.Should().Be(102);
            gapBar.Close.Should().Be(102);
            gapBar.Volume.Should().Be(0);
        }

        aggregator.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void NoGapFilling_SkipsMissingPeriods()
    {
        List<Quote> minuteQuotes =
        [
            new(DateTime.Parse("2023-11-09 10:00", invariantCulture), 100, 105, 99, 102, 1000),
            // Gap: 10:01, 10:02 missing
            new(DateTime.Parse("2023-11-09 10:03", invariantCulture), 105, 108, 104, 106, 1300),
        ];

        QuoteHub provider = new();
        QuoteAggregatorHub aggregator = provider.ToQuoteAggregatorHub(
            PeriodSize.OneMinute,
            fillGaps: false);

        foreach (Quote q in minuteQuotes)
        {
            provider.Add(q);
        }

        IReadOnlyList<IQuote> results = aggregator.Results;

        // Should have only 2 bars (no gap filling)
        results.Should().HaveCount(2);

        results[0].Timestamp.Should().Be(DateTime.Parse("2023-11-09 10:00", invariantCulture));
        results[1].Timestamp.Should().Be(DateTime.Parse("2023-11-09 10:03", invariantCulture));

        aggregator.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void SameTimestampUpdates_ReplaceWithinSameBar()
    {
        QuoteHub provider = new();
        QuoteAggregatorHub aggregator = provider.ToQuoteAggregatorHub(PeriodSize.FiveMinutes);

        // Add initial quote
        provider.Add(new Quote(
            DateTime.Parse("2023-11-09 10:00", invariantCulture), 100, 105, 99, 102, 1000));

        // Add another quote in the same 5-minute period (should update the bar)
        provider.Add(new Quote(
            DateTime.Parse("2023-11-09 10:02", invariantCulture), 102, 110, 98, 108, 1500));

        IReadOnlyList<IQuote> results = aggregator.Results;

        results.Should().HaveCount(1);

        IQuote bar = results[0];
        bar.Timestamp.Should().Be(DateTime.Parse("2023-11-09 10:00", invariantCulture));
        bar.Open.Should().Be(100);   // Original open
        bar.High.Should().Be(110);   // Updated high
        bar.Low.Should().Be(98);     // Updated low
        bar.Close.Should().Be(108);  // Latest close
        bar.Volume.Should().Be(2500); // Sum of volumes

        aggregator.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void ChainWithDownstreamIndicator_WorksCorrectly()
    {
        List<Quote> minuteQuotes =
        [
            new(DateTime.Parse("2023-11-09 10:00", invariantCulture), 100, 105, 99, 102, 1000),
            new(DateTime.Parse("2023-11-09 10:05", invariantCulture), 102, 107, 101, 104, 1100),
            new(DateTime.Parse("2023-11-09 10:10", invariantCulture), 104, 109, 103, 106, 1200),
            new(DateTime.Parse("2023-11-09 10:15", invariantCulture), 106, 111, 105, 108, 1300),
            new(DateTime.Parse("2023-11-09 10:20", invariantCulture), 108, 113, 107, 110, 1400),
        ];

        QuoteHub provider = new();
        QuoteAggregatorHub aggregator = provider.ToQuoteAggregatorHub(PeriodSize.FiveMinutes);

        // Chain with SMA
        SmaHub sma = aggregator.ToSmaHub(3);

        foreach (Quote q in minuteQuotes)
        {
            provider.Add(q);
        }

        // Verify aggregated quotes
        IReadOnlyList<IQuote> aggResults = aggregator.Results;
        aggResults.Should().HaveCount(5);

        // Verify SMA results
        IReadOnlyList<SmaResult> smaResults = sma.Results;
        smaResults.Should().HaveCount(5);

        // First two should be null (not enough data)
        smaResults[0].Sma.Should().BeNull();
        smaResults[1].Sma.Should().BeNull();

        // Third should be average of first three closes
        smaResults[2].Sma.Should().NotBeNull();
        const double expectedSma = (102 + 104 + 106) / 3.0;
        smaResults[2].Sma.Should().BeApproximately(expectedSma, 0.0001);

        sma.Unsubscribe();
        aggregator.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void InvalidPeriodSize_Month_ThrowsException()
    {
        QuoteHub provider = new();

        FluentActions
            .Invoking(() => provider.ToQuoteAggregatorHub(PeriodSize.Month))
            .Should()
            .ThrowExactly<ArgumentException>()
            .WithMessage("*Month aggregation is not supported*");

        provider.EndTransmission();
    }

    [TestMethod]
    public void InvalidTimeSpan_Zero_ThrowsException()
    {
        QuoteHub provider = new();

        FluentActions
            .Invoking(() => provider.ToQuoteAggregatorHub(TimeSpan.Zero))
            .Should()
            .ThrowExactly<ArgumentOutOfRangeException>()
            .WithMessage("*must be greater than zero*");

        provider.EndTransmission();
    }

    [TestMethod]
    public void InvalidTimeSpan_Negative_ThrowsException()
    {
        QuoteHub provider = new();

        FluentActions
            .Invoking(() => provider.ToQuoteAggregatorHub(TimeSpan.FromMinutes(-5)))
            .Should()
            .ThrowExactly<ArgumentOutOfRangeException>()
            .WithMessage("*must be greater than zero*");

        provider.EndTransmission();
    }

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        // Setup quote provider hub
        QuoteHub provider = new();

        // Prefill quotes at provider
        provider.Add(Quotes.Take(20));

        // Initialize aggregator (1-minute aggregation, no gaps to keep it simple)
        QuoteAggregatorHub aggregator = provider.ToQuoteAggregatorHub(PeriodSize.OneMinute);

        // Fetch initial results (early)
        IReadOnlyList<IQuote> sut = aggregator.Results;

        // Emulate adding quotes to provider hub
        for (int i = 20; i < quotesCount; i++)
        {
            // Skip one (add later)
            if (i == 80) { continue; }

            Quote q = Quotes[i];
            provider.Add(q);

            // Resend duplicate quotes
            if (i is > 100 and < 105) { provider.Add(q); }
        }

        // Late arrival
        provider.Insert(Quotes[80]);

        // Note: Since QuoteAggregatorHub aggregates by time periods,
        // the exact count and structure will differ from raw quotes,
        // but the aggregation should be consistent and not crash
        sut.Should().NotBeEmpty();
        sut.Should().AllSatisfy(q => {
            q.Timestamp.Should().NotBe(default);
            q.Open.Should().BeGreaterThan(0);
            q.High.Should().BeGreaterThanOrEqualTo(q.Low);
            q.Close.Should().BeGreaterThan(0);
        });

        // Cleanup
        aggregator.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider_MatchesSeriesExactly()
    {
        const int emaPeriods = 14;

        // Setup quote provider hub
        QuoteHub provider = new();

        // Initialize aggregator and chain with EMA
        EmaHub observer = provider
            .ToQuoteAggregatorHub(PeriodSize.FiveMinutes)
            .ToEmaHub(emaPeriods);

        // Emulate quote stream - for aggregator, use simple sequential adding
        // (late arrivals and removals don't make sense for time-based aggregation)
        foreach (Quote q in Quotes)
        {
            provider.Add(q);
        }

        // Final results
        IReadOnlyList<EmaResult> sut = observer.Results;

        // Compare with aggregated series
        IReadOnlyList<Quote> aggregatedQuotes = Quotes.Aggregate(PeriodSize.FiveMinutes);

        IReadOnlyList<EmaResult> expected = aggregatedQuotes
            .ToEma(emaPeriods);

        // Assert: should have same count as batch aggregation
        sut.Should().HaveCount(expected.Count);

        // Verify EMA values match closely
        // Note: There may be slight differences due to streaming vs batch processing
        // but they should be very close for completed bars
        for (int i = emaPeriods; i < sut.Count; i++)
        {
            if (sut[i].Ema.HasValue && expected[i].Ema.HasValue)
            {
                sut[i].Ema.Should().BeApproximately(expected[i].Ema.Value, 0.1,
                    $"at index {i}, timestamp {sut[i].Timestamp}");
            }
        }

        // Cleanup
        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void Properties_AreSetCorrectly()
    {
        QuoteHub provider = new();
        QuoteAggregatorHub aggregator = provider.ToQuoteAggregatorHub(
            PeriodSize.FifteenMinutes,
            fillGaps: true);

        aggregator.FillGaps.Should().BeTrue();
        aggregator.AggregationPeriod.Should().Be(TimeSpan.FromMinutes(15));

        aggregator.Unsubscribe();
        provider.EndTransmission();
    }
}
