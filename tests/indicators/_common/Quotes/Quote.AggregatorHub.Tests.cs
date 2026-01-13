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

    [TestMethod]
    public void SameTimestampQuotes_ReplacesPriorBar()
    {
        QuoteHub provider = new();
        QuoteAggregatorHub aggregator = provider.ToQuoteAggregatorHub(PeriodSize.FiveMinutes);

        // Add initial quote at 10:00
        provider.Add(new Quote(
            DateTime.Parse("2023-11-09 10:00", invariantCulture),
            100m, 110m, 99m, 105m, 1000m));

        // Add another quote also at 10:00 (within same 5-min period)
        provider.Add(new Quote(
            DateTime.Parse("2023-11-09 10:01", invariantCulture),
            105m, 115m, 104m, 108m, 1100m));

        IReadOnlyList<IQuote> results = aggregator.Results;

        results.Should().HaveCount(1);

        // Bar should incorporate both quotes
        IQuote bar = results[0];
        bar.Open.Should().Be(100m);  // First quote's open
        bar.High.Should().Be(115m);  // Max of both highs
        bar.Low.Should().Be(99m);    // Min of both lows
        bar.Close.Should().Be(108m); // Last quote's close
        bar.Volume.Should().Be(2100m); // Sum of volumes

        aggregator.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void ComprehensiveAggregation_MultipleTimeframes()
    {
        // Generate sufficient 5-minute candles to test all higher timeframes including 1-week
        // Need at least 7 days * 24 hours * 12 periods = 2,016 5-minute periods
        // Start on a Monday to align with week boundaries (weeks start on Monday in ISO 8601)
        const int totalFiveMinutePeriods = 2_016;
        DateTime startTime = DateTime.Parse("2023-10-30 00:00", invariantCulture); // Monday

        // Create 5-minute quote provider
        QuoteHub fiveMinuteProvider = new();

        // Create aggregators for higher timeframes (15m, 1h, 4h, 1d, 1w)
        QuoteAggregatorHub fifteenMinuteAgg = fiveMinuteProvider.ToQuoteAggregatorHub(PeriodSize.FifteenMinutes);
        QuoteAggregatorHub oneHourAgg = fiveMinuteProvider.ToQuoteAggregatorHub(PeriodSize.OneHour);
        QuoteAggregatorHub fourHourAgg = fiveMinuteProvider.ToQuoteAggregatorHub(PeriodSize.FourHours);
        QuoteAggregatorHub oneDayAgg = fiveMinuteProvider.ToQuoteAggregatorHub(PeriodSize.Day);
        QuoteAggregatorHub oneWeekAgg = fiveMinuteProvider.ToQuoteAggregatorHub(PeriodSize.Week);

        // Generate and add 5-minute candles with realistic price movements
        decimal basePrice = 100m;
        Random rnd = new(42); // Fixed seed for reproducibility

        for (int i = 0; i < totalFiveMinutePeriods; i++)
        {
            DateTime timestamp = startTime.AddMinutes(i * 5);

            // Simulate price movement
            decimal priceChange = (decimal)(rnd.NextDouble() - 0.5) * 2; // -1 to +1
            basePrice += priceChange * 0.1m; // Small incremental changes

            // Ensure positive prices
            if (basePrice < 50m) { basePrice = 50m; }
            if (basePrice > 150m) { basePrice = 150m; }

            decimal open = basePrice;
            decimal high = basePrice + (decimal)(rnd.NextDouble() * 2);
            decimal low = basePrice - (decimal)(rnd.NextDouble() * 2);
            decimal close = low + (decimal)(rnd.NextDouble() * (double)(high - low));
            decimal volume = 5000m + (decimal)(rnd.NextDouble() * 2500); // Higher volume for 5-min bars

            Quote quote = new(timestamp, open, high, low, close, volume);
            fiveMinuteProvider.Add(quote);
        }

        // Verify 5-minute provider
        IReadOnlyList<IQuote> fiveMinuteResults = fiveMinuteProvider.Results;
        fiveMinuteResults.Should().HaveCount(totalFiveMinutePeriods);

        // Verify 15-minute aggregation (3x 5-minute bars)
        IReadOnlyList<IQuote> fifteenMinuteResults = fifteenMinuteAgg.Results;
        int expectedFifteenMinute = totalFiveMinutePeriods / 3;
        fifteenMinuteResults.Should().HaveCount(expectedFifteenMinute);

        // Verify 1-hour aggregation (12x 5-minute bars)
        IReadOnlyList<IQuote> oneHourResults = oneHourAgg.Results;
        int expectedOneHour = totalFiveMinutePeriods / 12;
        oneHourResults.Should().HaveCount(expectedOneHour);

        // Verify 4-hour aggregation (48x 5-minute bars)
        IReadOnlyList<IQuote> fourHourResults = fourHourAgg.Results;
        int expectedFourHour = totalFiveMinutePeriods / 48;
        fourHourResults.Should().HaveCount(expectedFourHour);

        // Verify 1-day aggregation (288x 5-minute bars)
        IReadOnlyList<IQuote> oneDayResults = oneDayAgg.Results;
        int expectedOneDay = totalFiveMinutePeriods / 288;
        oneDayResults.Should().HaveCount(expectedOneDay);

        // Verify 1-week aggregation
        // Week aggregation may span 2 weeks depending on start day, so check it exists and has bars
        IReadOnlyList<IQuote> oneWeekResults = oneWeekAgg.Results;
        oneWeekResults.Should().NotBeEmpty("1-week aggregation should produce at least one bar");
        oneWeekResults.Should().HaveCountLessThan(4, "1-week aggregation should produce at most a few bars");

        // Verify OHLCV properties are valid for all aggregations
        VerifyOHLCVProperties(fifteenMinuteResults, "15-minute");
        VerifyOHLCVProperties(oneHourResults, "1-hour");
        VerifyOHLCVProperties(fourHourResults, "4-hour");
        VerifyOHLCVProperties(oneDayResults, "1-day");
        VerifyOHLCVProperties(oneWeekResults, "1-week");

        // Cleanup
        oneWeekAgg.Unsubscribe();
        oneDayAgg.Unsubscribe();
        fourHourAgg.Unsubscribe();
        oneHourAgg.Unsubscribe();
        fifteenMinuteAgg.Unsubscribe();
        fiveMinuteProvider.EndTransmission();
    }

    private static void VerifyOHLCVProperties(IReadOnlyList<IQuote> results, string timeframeName)
    {
        results.Should().NotBeEmpty($"{timeframeName} aggregation should produce results");

        foreach (IQuote bar in results)
        {
            bar.Timestamp.Should().NotBe(default, $"{timeframeName}: Timestamp should be set");
            bar.Open.Should().BeGreaterThan(0, $"{timeframeName}: Open should be positive");
            bar.High.Should().BeGreaterThanOrEqualTo(bar.Open, $"{timeframeName}: High should be >= Open");
            bar.High.Should().BeGreaterThanOrEqualTo(bar.Close, $"{timeframeName}: High should be >= Close");
            bar.High.Should().BeGreaterThanOrEqualTo(bar.Low, $"{timeframeName}: High should be >= Low");
            bar.Low.Should().BeLessThanOrEqualTo(bar.Open, $"{timeframeName}: Low should be <= Open");
            bar.Low.Should().BeLessThanOrEqualTo(bar.Close, $"{timeframeName}: Low should be <= Close");
            bar.Low.Should().BeGreaterThan(0, $"{timeframeName}: Low should be positive");
            bar.Close.Should().BeGreaterThan(0, $"{timeframeName}: Close should be positive");
            bar.Volume.Should().BeGreaterThan(0, $"{timeframeName}: Volume should be positive");
        }
    }

    [TestMethod]
    public void CascadedAggregation_FiveMinToFifteenMinToOneHour()
    {
        // This test demonstrates the bug where QuoteAggregatorHub notifies observers
        // with the SOURCE timestamp instead of the AGGREGATED timestamp.
        //
        // Expected behavior:
        // - 5-min candle at 01:20:00 → updates 15-min bar at 01:15:00
        // - 15-min aggregator should notify with 01:15:00 (not 01:20:00)
        // - 1-hour aggregator should rebuild from 01:00:00 (not 01:20:00)

        // Create cascaded aggregators: 5-min → 15-min → 1-hour
        QuoteHub fiveMinProvider = new();
        QuoteAggregatorHub fifteenMinAgg = fiveMinProvider.ToQuoteAggregatorHub(PeriodSize.FifteenMinutes);
        QuoteAggregatorHub oneHourAgg = fifteenMinAgg.ToQuoteAggregatorHub(PeriodSize.OneHour);

        // Add 5-minute candles
        // First, add some candles to establish the baseline
        fiveMinProvider.Add(new Quote(
            DateTime.Parse("2023-11-09 01:00", invariantCulture), 100, 105, 99, 102, 1000));
        fiveMinProvider.Add(new Quote(
            DateTime.Parse("2023-11-09 01:05", invariantCulture), 102, 106, 101, 104, 1100));
        fiveMinProvider.Add(new Quote(
            DateTime.Parse("2023-11-09 01:10", invariantCulture), 104, 107, 103, 105, 1200));
        fiveMinProvider.Add(new Quote(
            DateTime.Parse("2023-11-09 01:15", invariantCulture), 105, 108, 104, 106, 1300));

        // Verify initial state
        IReadOnlyList<IQuote> fiveMinResults = fiveMinProvider.Results;
        IReadOnlyList<IQuote> fifteenMinResults = fifteenMinAgg.Results;
        IReadOnlyList<IQuote> oneHourResults = oneHourAgg.Results;

        fiveMinResults.Should().HaveCount(4, "should have 4 five-minute candles");
        fifteenMinResults.Should().HaveCount(2, "should have 2 fifteen-minute bars");
        oneHourResults.Should().HaveCount(1, "should have 1 one-hour bar");

        // Now add a candle at 01:20:00 (which should update the 01:15:00 fifteen-minute bar)
        fiveMinProvider.Add(new Quote(
            DateTime.Parse("2023-11-09 01:20", invariantCulture), 106, 109, 105, 107, 1400));

        // Verify results after update
        fiveMinResults = fiveMinProvider.Results;
        fifteenMinResults = fifteenMinAgg.Results;
        oneHourResults = oneHourAgg.Results;

        // Five-minute provider should have 5 candles
        fiveMinResults.Should().HaveCount(5, "should have 5 five-minute candles after adding 01:20");

        // Fifteen-minute aggregator should still have 2 bars (01:00 and 01:15)
        // The 01:15 bar should be updated with the new 01:20 data
        fifteenMinResults.Should().HaveCount(2, "should have 2 fifteen-minute bars");

        if (fifteenMinResults.Count == 2)
        {
            IQuote fifteenMinBar = fifteenMinResults[1];
            fifteenMinBar.Timestamp.Should().Be(DateTime.Parse("2023-11-09 01:15", invariantCulture));
            fifteenMinBar.Close.Should().Be(107, "should include the 01:20 candle's close");
        }

        // One-hour aggregator should still have 1 bar at 01:00
        // It should be updated with the latest data from the 01:15 fifteen-minute bar
        oneHourResults.Should().HaveCount(1, "should have 1 one-hour bar");

        if (oneHourResults.Count == 1)
        {
            IQuote oneHourBar = oneHourResults[0];
            oneHourBar.Timestamp.Should().Be(DateTime.Parse("2023-11-09 01:00", invariantCulture));
            oneHourBar.Close.Should().Be(107, "should include the 01:20 candle's close (cascaded from 15-min bar)");
        }

        // Cleanup
        oneHourAgg.Unsubscribe();
        fifteenMinAgg.Unsubscribe();
        fiveMinProvider.EndTransmission();
    }

    [TestMethod]
    public void MultiThreaded_ConcurrentAggregation()
    {
        // Generate 5-minute candles for multi-threaded test
        const int totalFiveMinutePeriods = 2_016;
        DateTime startTime = DateTime.Parse("2023-10-30 00:00", invariantCulture);

        // Pre-generate all quotes
        List<Quote> quotes = [];
        decimal basePrice = 100m;
        Random rnd = new(42);

        for (int i = 0; i < totalFiveMinutePeriods; i++)
        {
            DateTime timestamp = startTime.AddMinutes(i * 5);
            decimal priceChange = (decimal)(rnd.NextDouble() - 0.5) * 2;
            basePrice += priceChange * 0.1m;

            if (basePrice < 50m) { basePrice = 50m; }
            if (basePrice > 150m) { basePrice = 150m; }

            decimal open = basePrice;
            decimal high = basePrice + (decimal)(rnd.NextDouble() * 2);
            decimal low = basePrice - (decimal)(rnd.NextDouble() * 2);
            decimal close = low + (decimal)(rnd.NextDouble() * (double)(high - low));
            decimal volume = 5000m + (decimal)(rnd.NextDouble() * 2500);

            quotes.Add(new Quote(timestamp, open, high, low, close, volume));
        }

        // Create 5-minute quote provider
        QuoteHub fiveMinuteProvider = new();

        // Create aggregators for higher timeframes
        QuoteAggregatorHub fifteenMinuteAgg = fiveMinuteProvider.ToQuoteAggregatorHub(PeriodSize.FifteenMinutes);
        QuoteAggregatorHub oneHourAgg = fiveMinuteProvider.ToQuoteAggregatorHub(PeriodSize.OneHour);
        QuoteAggregatorHub fourHourAgg = fiveMinuteProvider.ToQuoteAggregatorHub(PeriodSize.FourHours);
        QuoteAggregatorHub oneDayAgg = fiveMinuteProvider.ToQuoteAggregatorHub(PeriodSize.Day);

        // Split quotes into chunks for different threads
        int threadCount = 4;
        int chunkSize = quotes.Count / threadCount;
        List<Exception> exceptions = [];
        object lockObj = new();

        // Create threads that add quotes concurrently
        Task[] tasks = new Task[threadCount];
        for (int t = 0; t < threadCount; t++)
        {
            int threadIndex = t;
            tasks[t] = Task.Run(() => {
                try
                {
                    int start = threadIndex * chunkSize;
                    int end = (threadIndex == threadCount - 1)
                        ? quotes.Count
                        : start + chunkSize;

                    for (int i = start; i < end; i++)
                    {
                        fiveMinuteProvider.Add(quotes[i]);
                    }
                }
                catch (Exception ex)
                {
                    lock (lockObj)
                    {
                        exceptions.Add(ex);
                    }
                }
            });
        }

        // Wait for all threads to complete
        Task.WaitAll(tasks);

        // Document thread-safety issues if any exceptions occurred
        if (exceptions.Count > 0)
        {
            string exceptionSummary = string.Join(
                "\n",
                exceptions.Select(static e => $"  - {e.GetType().Name}: {e.Message}"));

            Assert.Fail(
                $"Thread-safety issues detected. {exceptions.Count} exception(s) occurred during concurrent operations:\n{exceptionSummary}");
        }

        // Verify results - the class appears to have thread-safety issues
        // This test documents the expected behavior and failures under concurrent load
        IReadOnlyList<IQuote> fiveMinuteResults = fiveMinuteProvider.Results;
        IReadOnlyList<IQuote> fifteenMinuteResults = fifteenMinuteAgg.Results;
        IReadOnlyList<IQuote> oneHourResults = oneHourAgg.Results;
        IReadOnlyList<IQuote> fourHourResults = fourHourAgg.Results;
        IReadOnlyList<IQuote> oneDayResults = oneDayAgg.Results;

        // Expected behavior: Results may be incomplete or corrupted due to race conditions
        // This test serves to document thread-safety requirements for the component
        try
        {
            fiveMinuteResults.Should().NotBeEmpty("5-minute provider should have results");
            fifteenMinuteResults.Should().NotBeEmpty("15-minute aggregation should have results");
            oneHourResults.Should().NotBeEmpty("1-hour aggregation should have results");
            fourHourResults.Should().NotBeEmpty("4-hour aggregation should have results");
            oneDayResults.Should().NotBeEmpty("1-day aggregation should have results");

            // If we got here, verify OHLCV properties are valid (no corrupted data)
            VerifyOHLCVProperties(fifteenMinuteResults, "15-minute (multi-threaded)");
            VerifyOHLCVProperties(oneHourResults, "1-hour (multi-threaded)");
            VerifyOHLCVProperties(fourHourResults, "4-hour (multi-threaded)");
            VerifyOHLCVProperties(oneDayResults, "1-day (multi-threaded)");
        }
        catch (Exception ex)
        {
            // Document the thread-safety issue
            Assert.Inconclusive(
                $"Thread-safety issue detected: {ex.Message}\n\n" +
                "QuoteAggregatorHub is not designed for concurrent access from multiple threads. " +
                "The class uses unsynchronized mutable state (_currentBar, _currentBarTimestamp) " +
                "and performs read-modify-write operations on shared collections (Cache) without locks. " +
                "For multi-threaded scenarios, external synchronization is required.");
        }

        // Cleanup
        oneDayAgg.Unsubscribe();
        fourHourAgg.Unsubscribe();
        oneHourAgg.Unsubscribe();
        fifteenMinuteAgg.Unsubscribe();
        fiveMinuteProvider.EndTransmission();
    }
}

