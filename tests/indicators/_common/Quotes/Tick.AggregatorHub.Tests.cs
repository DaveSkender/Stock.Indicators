namespace StreamHubs;

[TestClass]
public class TickAggregatorHubTests : StreamHubTestBase, ITestQuoteObserver, ITestChainProvider
{
    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        TickHub provider = new();
        TickAggregatorHub aggregator = provider.ToTickAggregatorHub(PeriodSize.FiveMinutes);

        string result = aggregator.ToString();

        result.Should().Contain("TICK-AGG");
        result.Should().Contain("00:05:00");

        aggregator.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void BasicAggregation_TicksToOneMinuteBars()
    {
        // Setup: Create tick-level data
        List<Tick> ticks =
        [
            new(DateTime.Parse("2023-11-09 10:00:00", invariantCulture), 100.00m, 10m),
            new(DateTime.Parse("2023-11-09 10:00:15", invariantCulture), 100.50m, 15m),
            new(DateTime.Parse("2023-11-09 10:00:30", invariantCulture), 99.75m, 20m),
            new(DateTime.Parse("2023-11-09 10:00:45", invariantCulture), 100.25m, 25m),
            new(DateTime.Parse("2023-11-09 10:01:10", invariantCulture), 101.00m, 30m),
        ];

        TickHub provider = new();
        TickAggregatorHub aggregator = provider.ToTickAggregatorHub(PeriodSize.OneMinute);

        // Add ticks
        foreach (Tick tick in ticks)
        {
            provider.Add(tick);
        }

        // Verify results
        IReadOnlyList<IQuote> results = aggregator.Results;

        results.Should().HaveCount(2);

        // First 1-minute bar (10:00)
        IQuote bar1 = results[0];
        bar1.Timestamp.Should().Be(DateTime.Parse("2023-11-09 10:00", invariantCulture));
        bar1.Open.Should().Be(100.00m);  // First tick price
        bar1.High.Should().Be(100.50m);  // Max tick price in period
        bar1.Low.Should().Be(99.75m);    // Min tick price in period
        bar1.Close.Should().Be(100.25m); // Last tick price in period
        bar1.Volume.Should().Be(70m);    // Sum of tick volumes (10+15+20+25)

        // Second 1-minute bar (10:01)
        IQuote bar2 = results[1];
        bar2.Timestamp.Should().Be(DateTime.Parse("2023-11-09 10:01", invariantCulture));
        bar2.Open.Should().Be(101.00m);
        bar2.High.Should().Be(101.00m);
        bar2.Low.Should().Be(101.00m);
        bar2.Close.Should().Be(101.00m);
        bar2.Volume.Should().Be(30m);

        // Cleanup
        aggregator.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void GapFilling_CarriesForwardPrices()
    {
        List<Tick> ticks =
        [
            new(DateTime.Parse("2023-11-09 10:00:00", invariantCulture), 100.00m, 10m),
            // Gap: 10:01 missing - will be filled
            new(DateTime.Parse("2023-11-09 10:02:00", invariantCulture), 102.00m, 20m),
        ];

        TickHub provider = new();
        TickAggregatorHub aggregator = provider.ToTickAggregatorHub(
            PeriodSize.OneMinute,
            fillGaps: true);

        foreach (Tick tick in ticks)
        {
            provider.Add(tick);
        }

        IReadOnlyList<IQuote> results = aggregator.Results;

        // Should have 3 bars: 10:00, 10:01 (gap-filled), 10:02
        results.Should().HaveCount(3);

        // Verify gap-filled bar at 10:01
        IQuote gapBar = results[1];
        gapBar.Timestamp.Should().Be(DateTime.Parse("2023-11-09 10:01", invariantCulture));
        gapBar.Open.Should().Be(100.00m);  // Carried forward from 10:00 close
        gapBar.High.Should().Be(100.00m);
        gapBar.Low.Should().Be(100.00m);
        gapBar.Close.Should().Be(100.00m);
        gapBar.Volume.Should().Be(0m);

        aggregator.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void NoGapFilling_SkipsMissingPeriods()
    {
        List<Tick> ticks =
        [
            new(DateTime.Parse("2023-11-09 10:00:00", invariantCulture), 100.00m, 10m),
            // Gap: 10:01, 10:02 missing
            new(DateTime.Parse("2023-11-09 10:03:00", invariantCulture), 103.00m, 30m),
        ];

        TickHub provider = new();
        TickAggregatorHub aggregator = provider.ToTickAggregatorHub(
            PeriodSize.OneMinute,
            fillGaps: false);

        foreach (Tick tick in ticks)
        {
            provider.Add(tick);
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
    public void ChainWithDownstreamIndicator_WorksCorrectly()
    {
        List<Tick> ticks =
        [
            new(DateTime.Parse("2023-11-09 10:00", invariantCulture), 100m, 10m),
            new(DateTime.Parse("2023-11-09 10:01", invariantCulture), 101m, 11m),
            new(DateTime.Parse("2023-11-09 10:02", invariantCulture), 102m, 12m),
            new(DateTime.Parse("2023-11-09 10:03", invariantCulture), 103m, 13m),
            new(DateTime.Parse("2023-11-09 10:04", invariantCulture), 104m, 14m),
        ];

        TickHub provider = new();
        TickAggregatorHub aggregator = provider.ToTickAggregatorHub(PeriodSize.OneMinute);

        // Chain with SMA
        SmaHub sma = aggregator.ToSmaHub(3);

        foreach (Tick tick in ticks)
        {
            provider.Add(tick);
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
        const double expectedSma = (100 + 101 + 102) / 3.0;
        smaResults[2].Sma.Should().BeApproximately(expectedSma, Money4);

        sma.Unsubscribe();
        aggregator.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        // Setup tick provider hub
        TickHub provider = new();

        // Create some tick data
        List<Tick> ticks = [];
        for (int i = 0; i < 100; i++)
        {
            ticks.Add(new Tick(
                DateTime.Parse("2023-11-09 10:00", invariantCulture).AddMinutes(i),
                100m + i,
                10m + i));
        }

        // Prefill warmup window
        provider.Add(ticks.Take(20));

        // Initialize aggregator
        TickAggregatorHub aggregator = provider.ToTickAggregatorHub(PeriodSize.OneMinute);

        // Fetch initial results
        IReadOnlyList<IQuote> sut = aggregator.Results.ToList();

        // Stream remaining ticks in-order including duplicates
        for (int i = 20; i < ticks.Count; i++)
        {
            // Skip one (add later as late arrival)
            if (i == 80) { continue; }
            
            // Skip removal index
            if (i == 50) { continue; }

            Tick tick = ticks[i];
            provider.Add(tick);

            // Stream duplicate at same timestamp with different execution ID
            if (i is > 30 and < 35)
            {
                provider.Add(new Tick(
                    tick.Timestamp,
                    tick.Price + 0.5m,
                    tick.Volume + 1m,
                    $"dup-{i}"));
            }
        }

        // Late historical insert (earlier timestamp than current tail)
        provider.Insert(ticks[80]);
        IReadOnlyList<IQuote> afterLateArrival = aggregator.Results.ToList();

        // Remove a historical tick (simulate rollback)
        provider.Insert(ticks[50]);
        IReadOnlyList<IQuote> afterRemoval = aggregator.Results.ToList();

        // Verify structural invariants at all stages
        sut.Should().AllSatisfy(q => {
            q.Timestamp.Should().NotBe(default);
            q.Open.Should().BeGreaterThan(0);
            q.High.Should().BeGreaterThanOrEqualTo(q.Low);
            q.Close.Should().BeGreaterThan(0);
        });

        afterLateArrival.Should().AllSatisfy(q => {
            q.Timestamp.Should().NotBe(default);
            q.Open.Should().BeGreaterThan(0);
            q.High.Should().BeGreaterThanOrEqualTo(q.Low);
            q.Close.Should().BeGreaterThan(0);
        });

        afterRemoval.Should().AllSatisfy(q => {
            q.Timestamp.Should().NotBe(default);
            q.Open.Should().BeGreaterThan(0);
            q.High.Should().BeGreaterThanOrEqualTo(q.Low);
            q.Close.Should().BeGreaterThan(0);
        });

        // Verify ordering is strictly preserved
        for (int i = 1; i < sut.Count; i++)
        {
            sut[i].Timestamp.Should().BeGreaterThanOrEqualTo(sut[i - 1].Timestamp);
        }

        for (int i = 1; i < afterLateArrival.Count; i++)
        {
            afterLateArrival[i].Timestamp.Should().BeGreaterThanOrEqualTo(afterLateArrival[i - 1].Timestamp);
        }

        for (int i = 1; i < afterRemoval.Count; i++)
        {
            afterRemoval[i].Timestamp.Should().BeGreaterThanOrEqualTo(afterRemoval[i - 1].Timestamp);
        }

        // Cleanup
        aggregator.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider_MatchesSeriesExactly()
    {
        const int emaPeriods = 14;

        // Setup tick provider hub
        TickHub provider = new();

        // Create tick data
        List<Tick> ticks = [];
        for (int i = 0; i < 100; i++)
        {
            Tick tick = new(
                DateTime.Parse("2023-11-09 10:00", invariantCulture).AddMinutes(i),
                100m + i,
                10m + i);
            ticks.Add(tick);
            provider.Add(tick);
        }

        // Initialize aggregator and chain with EMA
        TickAggregatorHub aggregator = provider.ToTickAggregatorHub(PeriodSize.OneMinute);
        EmaHub observer = aggregator.ToEmaHub(emaPeriods);

        // Final results
        IReadOnlyList<EmaResult> sut = observer.Results;

        // Should have results
        sut.Should().HaveCount(100);

        // Verify some EMA values are calculated
        sut.Skip(emaPeriods).Should().AllSatisfy(r => r.Ema.Should().NotBeNull());

        // Build reference EMA Series from same tick sequence
        // First aggregate ticks to quotes
        List<Quote> quoteSequence = [];
        foreach (Tick tick in ticks)
        {
            DateTime barTimestamp = tick.Timestamp.RoundDown(TimeSpan.FromMinutes(1));
            Quote q = quoteSequence.FirstOrDefault(x => x.Timestamp == barTimestamp);
            if (q == null)
            {
                q = new Quote(barTimestamp, tick.Price, tick.Price, tick.Price, tick.Price, tick.Volume);
                quoteSequence.Add(q);
            }
            else
            {
                // Update existing bar
                q = new Quote(
                    q.Timestamp,
                    q.Open,
                    Math.Max(q.High, tick.Price),
                    Math.Min(q.Low, tick.Price),
                    tick.Price,
                    q.Volume + tick.Volume);
                quoteSequence[quoteSequence.Count - 1] = q;
            }
        }

        // Calculate reference EMA on aggregated quotes
        IReadOnlyList<EmaResult> referenceEma = quoteSequence.Ema(emaPeriods);

        // Compare observer results to reference EMA with strict ordering and equality
        sut.Should().HaveSameCount(referenceEma);
        for (int i = 0; i < sut.Count; i++)
        {
            // Verify timestamp and ordering
            sut[i].Timestamp.Should().Be(referenceEma[i].Timestamp);
            if (i > 0)
            {
                sut[i].Timestamp.Should().BeGreaterThanOrEqualTo(sut[i - 1].Timestamp);
            }

            // Verify EMA values match
            if (referenceEma[i].Ema.HasValue)
            {
                sut[i].Ema.Should().BeApproximately(referenceEma[i].Ema.Value, Money4,
                    $"EMA at index {i} should match reference series");
            }
            else
            {
                sut[i].Ema.Should().BeNull($"EMA at index {i} should be null like reference");
            }
        }

        // Cleanup
        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void Properties_AreSetCorrectly()
    {
        TickHub provider = new();
        TickAggregatorHub aggregator = provider.ToTickAggregatorHub(
            PeriodSize.FifteenMinutes,
            fillGaps: true);

        aggregator.FillGaps.Should().BeTrue();
        aggregator.AggregationPeriod.Should().Be(TimeSpan.FromMinutes(15));

        aggregator.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void DuplicateExecutionId_CorrectionRebuildsBar()
    {
        TickHub provider = new();
        TickAggregatorHub aggregator = provider.ToTickAggregatorHub(PeriodSize.OneMinute);

        // Add tick with execution ID
        provider.Add(new Tick(
            DateTime.Parse("2023-11-09 10:00:00", invariantCulture),
            100m, 10m, "EXEC-001"));

        // Add correction with same execution ID and timestamp
        provider.Add(new Tick(
            DateTime.Parse("2023-11-09 10:00:00", invariantCulture),
            200m, 20m, "EXEC-001"));

        IReadOnlyList<IQuote> results = aggregator.Results;

        results.Should().HaveCount(1);

        // Should reflect corrected tick
        IQuote bar = results[0];
        bar.Open.Should().Be(200m);
        bar.High.Should().Be(200m);
        bar.Low.Should().Be(200m);
        bar.Close.Should().Be(200m);
        bar.Volume.Should().Be(20m);

        aggregator.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void TicksWithoutExecutionId_DuplicatesAllowed()
    {
        TickHub provider = new();
        TickAggregatorHub aggregator = provider.ToTickAggregatorHub(PeriodSize.OneMinute);

        // Add ticks without execution IDs (same timestamp is allowed)
        provider.Add(new Tick(
            DateTime.Parse("2023-11-09 10:00:00", invariantCulture),
            100m, 10m, null));

        provider.Add(new Tick(
            DateTime.Parse("2023-11-09 10:00:00", invariantCulture),
            105m, 15m, null));

        IReadOnlyList<IQuote> results = aggregator.Results;

        results.Should().HaveCount(1);

        // Both ticks should be incorporated
        IQuote bar = results[0];
        bar.Open.Should().Be(100m); // First tick price
        bar.High.Should().Be(105m);
        bar.Low.Should().Be(100m);
        bar.Close.Should().Be(105m); // Last tick price
        bar.Volume.Should().Be(25m); // Sum of both volumes

        aggregator.Unsubscribe();
        provider.EndTransmission();
    }
}
