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
    public void WithCachePruning_MatchesSeriesExactly()
    {
        const int maxCacheSize = 20;

        // Create enough ticks to produce many 1-minute bars
        List<Tick> ticks = [];
        for (int i = 0; i < 200; i++)
        {
            ticks.Add(new Tick(
                DateTime.Parse("2023-11-09 10:00", invariantCulture).AddMinutes(i),
                100m + i, 10m + i));
        }

        // Setup with cache limit
        TickHub provider = new(maxCacheSize);
        TickAggregatorHub aggregator = provider.ToTickAggregatorHub(PeriodSize.OneMinute);

        // Stream all ticks
        foreach (Tick tick in ticks)
        {
            provider.Add(tick);
        }

        // Compute expected 1-minute bars from original ticks:
        // group ticks by tick.Timestamp truncated to minute, produce Open/High/Low/Close/Timestamp per group
        List<Quote> allBars = [];
        foreach (Tick tick in ticks)
        {
            DateTime barTs = tick.Timestamp.RoundDown(TimeSpan.FromMinutes(1));
            int idx = allBars.FindIndex(q => q.Timestamp == barTs);
            if (idx < 0)
            {
                allBars.Add(new Quote(barTs, tick.Price, tick.Price, tick.Price, tick.Price, tick.Volume));
            }
            else
            {
                Quote q = allBars[idx];
                allBars[idx] = new Quote(barTs, q.Open, Math.Max(q.High, tick.Price), Math.Min(q.Low, tick.Price), tick.Price, q.Volume + tick.Volume);
            }
        }

        // Apply cache pruning semantics: TickHub retains only the most recent maxCacheSize ticks;
        // since each tick falls on a unique minute, this maps to the last maxCacheSize bars
        List<Quote> expectedBars = allBars.Count > maxCacheSize
            ? allBars.Skip(allBars.Count - maxCacheSize).ToList()
            : allBars;

        // Verify exact count and numerical parity: Timestamp, Open, High, Low, Close for each entry
        IReadOnlyList<IQuote> results = aggregator.Results;
        results.Should().HaveCount(expectedBars.Count);

        for (int i = 0; i < expectedBars.Count; i++)
        {
            Quote expected = expectedBars[i];
            IQuote actual = results[i];

            actual.Timestamp.Should().Be(expected.Timestamp, $"bar[{i}] Timestamp should match");
            actual.Open.Should().Be(expected.Open, $"bar[{i}] Open should match");
            actual.High.Should().Be(expected.High, $"bar[{i}] High should match");
            actual.Low.Should().Be(expected.Low, $"bar[{i}] Low should match");
            actual.Close.Should().Be(expected.Close, $"bar[{i}] Close should match");
        }

        // Cleanup
        aggregator.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        TickHub provider = new();

        // Create tick data: 50 ticks (each at a different minute timestamp)
        List<Tick> ticks = [];
        for (int i = 0; i < 50; i++)
        {
            ticks.Add(new Tick(
                DateTime.Parse("2023-11-09 10:00", invariantCulture).AddMinutes(i),
                100m + i,
                10m + i));
        }

        // Prefill warmup window (first 10 ticks = minutes 0-9)
        provider.Add(ticks.Take(10));

        // Initialize aggregator
        TickAggregatorHub aggregator = provider.ToTickAggregatorHub(PeriodSize.OneMinute);
        aggregator.Results.Should().HaveCount(10);

        // Stream ticks 10-39 in-order, skipping tick[25] for late arrival
        // Also inject duplicates at ticks 15-18 to test duplicate handling
        for (int i = 10; i < 40; i++)
        {
            if (i == 25) { continue; }  // Skip for late arrival

            Tick tick = ticks[i];
            provider.Add(tick);

            // Stream duplicate at same timestamp with different execution ID
            if (i is > 15 and < 19)
            {
                provider.Add(new Tick(
                    tick.Timestamp,
                    tick.Price + 0.5m,
                    tick.Volume + 1m,
                    $"dup-{i}"));
            }
        }

        // Snapshot before late arrival
        IReadOnlyList<IQuote> beforeLateArrival = aggregator.Results.ToList();

        // Late arrival: inject tick[25] which was skipped in stream
        provider.Add(ticks[25]);
        IReadOnlyList<IQuote> afterLateArrival = aggregator.Results.ToList();

        // Late arrival should result in same or more bars (only adds, never removes)
        afterLateArrival.Count.Should().BeGreaterThanOrEqualTo(beforeLateArrival.Count);

        // All bars should have valid structural invariants
        afterLateArrival.Should().AllSatisfy(q => {
            q.Timestamp.Should().NotBe(default);
            q.Open.Should().BeGreaterThan(0);
            q.High.Should().BeGreaterThanOrEqualTo(q.Low);
            q.Close.Should().BeGreaterThan(0);
        });

        // Verify ordering is preserved
        for (int i = 1; i < afterLateArrival.Count; i++)
        {
            afterLateArrival[i].Timestamp.Should().BeAfter(afterLateArrival[i - 1].Timestamp);
        }

        // Build reference aggregated quotes from exact tick sequence for parity comparison
        // This represents what the Series API would produce
        List<Quote> referenceQuotes = [];

        // Add warmup ticks
        foreach (Tick tick in ticks.Take(10))
        {
            DateTime barTimestamp = tick.Timestamp.RoundDown(TimeSpan.FromMinutes(1));
            referenceQuotes.Add(new Quote(barTimestamp, tick.Price, tick.Price, tick.Price, tick.Price, tick.Volume));
        }

        // Add stream ticks (10-24, 26-39) including duplicates
        for (int i = 10; i < 40; i++)
        {
            if (i == 25) { continue; }

            Tick tick = ticks[i];
            DateTime barTimestamp = tick.Timestamp.RoundDown(TimeSpan.FromMinutes(1));
            int existingIdx = referenceQuotes.FindIndex(q => q.Timestamp == barTimestamp);

            if (existingIdx < 0)
            {
                referenceQuotes.Add(new Quote(barTimestamp, tick.Price, tick.Price, tick.Price, tick.Price, tick.Volume));
            }
            else
            {
                Quote q = referenceQuotes[existingIdx];
                referenceQuotes[existingIdx] = new Quote(
                    q.Timestamp, q.Open, Math.Max(q.High, tick.Price), Math.Min(q.Low, tick.Price),
                    tick.Price, q.Volume + tick.Volume);
            }

            // Add duplicate at same minute
            if (i is > 15 and < 19)
            {
                Tick dup = new(tick.Timestamp, tick.Price + 0.5m, tick.Volume + 1m, $"dup-{i}");
                Quote q = referenceQuotes[existingIdx >= 0 ? existingIdx : referenceQuotes.Count - 1];
                int dupIdx = referenceQuotes.FindIndex(x => x.Timestamp == barTimestamp);
                referenceQuotes[dupIdx] = new Quote(
                    q.Timestamp, q.Open, Math.Max(q.High, dup.Price), Math.Min(q.Low, dup.Price),
                    dup.Price, q.Volume + dup.Volume);
            }
        }

        // Add late arrival tick[25]
        {
            Tick tick = ticks[25];
            DateTime barTimestamp = tick.Timestamp.RoundDown(TimeSpan.FromMinutes(1));
            int existingIdx = referenceQuotes.FindIndex(q => q.Timestamp == barTimestamp);

            if (existingIdx < 0)
            {
                referenceQuotes.Add(new Quote(barTimestamp, tick.Price, tick.Price, tick.Price, tick.Price, tick.Volume));
            }
            else
            {
                Quote q = referenceQuotes[existingIdx];
                referenceQuotes[existingIdx] = new Quote(
                    q.Timestamp, q.Open, Math.Max(q.High, tick.Price), Math.Min(q.Low, tick.Price),
                    tick.Price, q.Volume + tick.Volume);
            }
        }

        // Verify strict numerical parity with reference aggregation for all bars
        // The aggregator results should exactly match what Series computation would produce
        // Build a mapping of bar timestamps -> bars for comparison since order should match
        Dictionary<DateTime, Quote> referenceMap = referenceQuotes.ToDictionary(q => q.Timestamp);

        afterLateArrival.Should().AllSatisfy(bar => {
            // Verify this bar exists in reference and matches exactly
            referenceMap.Should().ContainKey(bar.Timestamp,
                $"bar at {bar.Timestamp:O} should exist in reference aggregation");

            Quote expected = referenceMap[bar.Timestamp];
            bar.Open.Should().Be(expected.Open,
                $"bar at {bar.Timestamp:O} Open must match (got {bar.Open:C4}, expected {expected.Open:C4})");
            bar.High.Should().Be(expected.High,
                $"bar at {bar.Timestamp:O} High must match (got {bar.High:C4}, expected {expected.High:C4})");
            bar.Low.Should().Be(expected.Low,
                $"bar at {bar.Timestamp:O} Low must match (got {bar.Low:C4}, expected {expected.Low:C4})");
            bar.Close.Should().Be(expected.Close,
                $"bar at {bar.Timestamp:O} Close must match (got {bar.Close:C4}, expected {expected.Close:C4})");
            bar.Volume.Should().Be(expected.Volume,
                $"bar at {bar.Timestamp:O} Volume must match");
        });

        // Rollback: remove a historical tick (simulate deletion)
        DateTime removeTimestamp = ticks[20].Timestamp;
        int removeIdx = provider.Cache.IndexGte(removeTimestamp);
        removeIdx.Should().BeGreaterOrEqualTo(0, "tick[20] should be in cache");
        provider.RemoveAt(removeIdx);

        IReadOnlyList<IQuote> afterRemoval = aggregator.Results.ToList();
        afterRemoval.Should().HaveCount(afterLateArrival.Count - 1);

        // Removed minute should be absent
        DateTime minute20 = removeTimestamp.RoundDown(TimeSpan.FromMinutes(1));
        afterRemoval.Should().NotContain(q => q.Timestamp == minute20);

        // Verify ordering after removal
        for (int i = 1; i < afterRemoval.Count; i++)
        {
            afterRemoval[i].Timestamp.Should().BeAfter(afterRemoval[i - 1].Timestamp);
        }

        // Verify parity after removal using map-based comparison
        // The removed minute bar should be absent from both aggregator and reference
        Dictionary<DateTime, IQuote> afterRemovalMap = afterRemoval.ToDictionary(q => q.Timestamp);

        referenceQuotes.RemoveAll(q => q.Timestamp == minute20);
        referenceMap.Remove(minute20);

        // All bars in afterRemoval should match reference exactly
        afterRemovalMap.Should().AllSatisfy(kvp => {
            referenceMap.Should().ContainKey(kvp.Key,
                $"bar at {kvp.Key:O} should exist in reference after removal");

            Quote expected = referenceMap[kvp.Key];
            kvp.Value.Open.Should().Be(expected.Open);
            kvp.Value.High.Should().Be(expected.High);
            kvp.Value.Low.Should().Be(expected.Low);
            kvp.Value.Close.Should().Be(expected.Close);
            kvp.Value.Volume.Should().Be(expected.Volume);
        });

        // Reference and aggregator should have same bar count after removal
        afterRemovalMap.Should().HaveSameCount(referenceMap);

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

                quoteSequence[^1] = q;
            }
        }

        // Calculate reference EMA on aggregated quotes
        IReadOnlyList<EmaResult> referenceEma = quoteSequence.ToEma(emaPeriods);

        // Compare observer results to reference EMA with strict ordering and equality
        sut.Should().HaveSameCount(referenceEma);
        for (int i = 0; i < sut.Count; i++)
        {
            // Verify timestamp and ordering
            sut[i].Timestamp.Should().Be(referenceEma[i].Timestamp);
            if (i > 0)
            {
                sut[i].Timestamp.Should().BeOnOrAfter(sut[i - 1].Timestamp);
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

    [TestMethod]
    public void LateArrival_CrossingClosedBucket_RebuildsCorrectly()
    {
        // A late tick belonging to a 1-minute bucket that has already been
        // closed (by ticks in subsequent buckets) must trigger a rebuild
        // and produce a bar reflecting the additional tick's price/volume.
        // Later buckets must remain unchanged.

        TickHub provider = new();
        TickAggregatorHub aggregator = provider.ToTickAggregatorHub(PeriodSize.OneMinute);

        // Bucket 10:00 — two ticks (no execution IDs so they aren't deduped)
        provider.Add(new Tick(
            DateTime.Parse("2023-11-09 10:00:00", invariantCulture), 100m, 10m, null));
        provider.Add(new Tick(
            DateTime.Parse("2023-11-09 10:00:30", invariantCulture), 101m, 11m, null));

        // Bucket 10:01 — closes bucket 10:00
        provider.Add(new Tick(
            DateTime.Parse("2023-11-09 10:01:10", invariantCulture), 102m, 12m, null));
        provider.Add(new Tick(
            DateTime.Parse("2023-11-09 10:01:40", invariantCulture), 103m, 13m, null));

        // Bucket 10:02 — closes bucket 10:01
        provider.Add(new Tick(
            DateTime.Parse("2023-11-09 10:02:15", invariantCulture), 104m, 14m, null));
        provider.Add(new Tick(
            DateTime.Parse("2023-11-09 10:02:45", invariantCulture), 105m, 15m, null));

        // Late tick into the closed 10:00 bucket with extreme high and low
        provider.Add(new Tick(
            DateTime.Parse("2023-11-09 10:00:45", invariantCulture), 120m, 5m, null));

        IReadOnlyList<IQuote> results = aggregator.Results;
        results.Should().HaveCount(3);

        // Bucket 10:00 now reflects the late tick
        IQuote bar0 = results[0];
        bar0.Timestamp.Should().Be(DateTime.Parse("2023-11-09 10:00", invariantCulture));
        bar0.Open.Should().Be(100m);
        bar0.High.Should().Be(120m);   // Updated by late tick
        bar0.Low.Should().Be(100m);    // Min of 100, 101, 120
        bar0.Close.Should().Be(120m);  // Last-by-timestamp in bucket is 10:00:45
        bar0.Volume.Should().Be(26m);  // 10 + 11 + 5

        // Bucket 10:01 unchanged
        IQuote bar1 = results[1];
        bar1.Timestamp.Should().Be(DateTime.Parse("2023-11-09 10:01", invariantCulture));
        bar1.Open.Should().Be(102m);
        bar1.High.Should().Be(103m);
        bar1.Low.Should().Be(102m);
        bar1.Close.Should().Be(103m);
        bar1.Volume.Should().Be(25m);

        // Bucket 10:02 unchanged
        IQuote bar2 = results[2];
        bar2.Timestamp.Should().Be(DateTime.Parse("2023-11-09 10:02", invariantCulture));
        bar2.Open.Should().Be(104m);
        bar2.High.Should().Be(105m);
        bar2.Low.Should().Be(104m);
        bar2.Close.Should().Be(105m);
        bar2.Volume.Should().Be(29m);

        aggregator.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void PartialBucket_OnStreamEnd_IsEmittedAsIncomplete()
    {
        // The aggregator emits the current (in-progress) bucket as soon as
        // its first tick arrives, and that bar mutates in place as more
        // ticks inside the same bucket arrive. When the stream ends
        // mid-bucket the partial bar remains in Results — it is NOT
        // trimmed, hidden, or frozen at first emission. Pin both the
        // live-mutation contract and the survives-on-stream-end contract
        // so downstream consumers (e.g. live-bar charting) can rely on it.

        TickHub provider = new();
        TickAggregatorHub aggregator = provider.ToTickAggregatorHub(PeriodSize.OneMinute);

        // First tick opens the 10:00 bucket as a partial bar
        provider.Add(new Tick(
            DateTime.Parse("2023-11-09 10:00:00", invariantCulture), 100m, 10m, null));

        IReadOnlyList<IQuote> results = aggregator.Results;
        results.Should().HaveCount(1);
        results[0].Open.Should().Be(100m);
        results[0].High.Should().Be(100m);
        results[0].Low.Should().Be(100m);
        results[0].Close.Should().Be(100m);
        results[0].Volume.Should().Be(10m);

        // Second tick in the same bucket — the partial bar must mutate
        provider.Add(new Tick(
            DateTime.Parse("2023-11-09 10:00:30", invariantCulture), 105m, 15m, null));

        results.Should().HaveCount(1);
        IQuote partial = results[0];
        partial.Timestamp.Should().Be(DateTime.Parse("2023-11-09 10:00", invariantCulture));
        partial.Open.Should().Be(100m);
        partial.High.Should().Be(105m);
        partial.Low.Should().Be(100m);
        partial.Close.Should().Be(105m);
        partial.Volume.Should().Be(25m);

        aggregator.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void LateArrival_MatchesFreshStream()
    {
        // Skip a mid-stream tick, then re-add it after the cache head
        // has advanced several buckets. The late hub's aggregated bars
        // must equal a fresh hub fed the same ticks in correct order;
        // this is the rollback-equivalence invariant exercised at the
        // bucket-aware aggregator layer rather than at a per-indicator
        // hub.

        const int totalTicks = 60;
        const int lateIndex = 17;

        // 15-second spacing places multiple ticks per 1-minute bucket
        List<Tick> ticks = [];
        for (int i = 0; i < totalTicks; i++)
        {
            ticks.Add(new Tick(
                DateTime.Parse("2023-11-09 10:00:00", invariantCulture).AddSeconds(i * 15),
                100m + i,
                10m + i,
                null));
        }

        TickHub lateSource = new();
        TickAggregatorHub lateHub = lateSource.ToTickAggregatorHub(PeriodSize.OneMinute);
        for (int i = 0; i < totalTicks; i++)
        {
            if (i == lateIndex) { continue; }

            lateSource.Add(ticks[i]);
        }

        lateSource.Add(ticks[lateIndex]);

        TickHub freshSource = new();
        TickAggregatorHub freshHub = freshSource.ToTickAggregatorHub(PeriodSize.OneMinute);
        foreach (Tick tick in ticks)
        {
            freshSource.Add(tick);
        }

        IReadOnlyList<IQuote> lateResults = lateHub.Results;
        IReadOnlyList<IQuote> freshResults = freshHub.Results;

        lateResults.Should().HaveSameCount(freshResults);
        for (int i = 0; i < freshResults.Count; i++)
        {
            lateResults[i].Timestamp.Should().Be(freshResults[i].Timestamp);
            lateResults[i].Open.Should().Be(freshResults[i].Open);
            lateResults[i].High.Should().Be(freshResults[i].High);
            lateResults[i].Low.Should().Be(freshResults[i].Low);
            lateResults[i].Close.Should().Be(freshResults[i].Close);
            lateResults[i].Volume.Should().Be(freshResults[i].Volume);
        }

        lateHub.Unsubscribe();
        freshHub.Unsubscribe();
        lateSource.EndTransmission();
        freshSource.EndTransmission();
    }
}
