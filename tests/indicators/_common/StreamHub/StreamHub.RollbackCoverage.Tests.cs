namespace Observables;

/// <summary>
/// Rollback-engine coverage for the aggregator hubs and through-aggregator
/// chains that the generic catalog-driven rollback contract does not reach.
/// <c>QuoteAggregatorHub</c>/<c>TickAggregatorHub</c> override
/// <c>RollbackState</c>/<c>Rebuild</c> but are not catalog-registered, so they
/// are exercised here directly; and a late arrival routed through an aggregator
/// into a downstream indicator must converge to the same results as a fresh
/// chain.
/// </summary>
[TestClass]
public class RollbackCoverage : TestBase
{
    private const int TickCount = 500;

    [TestMethod]
    public void QuoteAggregator_AfterRebuild_MatchesFreshStreamAndBatch()
    {
        IReadOnlyList<Quote> quotes = Quotes.Take(500).ToList();
        DateTime rollback = quotes[300].Timestamp;

        // a hub that was rolled back mid-stream and replayed
        QuoteHub providerA = new();
        QuoteAggregatorHub aggA = providerA.ToQuoteAggregatorHub(PeriodSize.Week);
        providerA.Add(quotes);
        aggA.Rebuild(rollback);

        // a fresh hub fed the same quotes once
        QuoteHub providerB = new();
        QuoteAggregatorHub aggB = providerB.ToQuoteAggregatorHub(PeriodSize.Week);
        providerB.Add(quotes);

        // independent batch oracle: the streamed aggregation (fresh or rebuilt)
        // must equal the batch Aggregate — this upgrades the rollback-equivalence
        // check to rollback-and-correctness
        IReadOnlyList<Quote> batch = quotes.Aggregate(PeriodSize.Week);

        aggA.Results.Should().NotBeEmpty();
        aggB.Results.IsExactly(batch);
        aggA.Results.IsExactly(aggB.Results);

        aggA.Unsubscribe();
        aggB.Unsubscribe();
        providerA.EndTransmission();
        providerB.EndTransmission();
    }

    [TestMethod]
    public void TickAggregator_AfterRebuild_MatchesFreshStream()
    {
        List<Tick> ticks = BuildTicks(TickCount);
        DateTime rollback = ticks[300].Timestamp;

        TickHub providerA = new();
        TickAggregatorHub aggA = providerA.ToTickAggregatorHub(PeriodSize.FifteenMinutes);
        providerA.Add(ticks);
        aggA.Rebuild(rollback);

        TickHub providerB = new();
        TickAggregatorHub aggB = providerB.ToTickAggregatorHub(PeriodSize.FifteenMinutes);
        providerB.Add(ticks);

        aggA.Results.Should().NotBeEmpty();
        aggB.Results.Should().NotBeEmpty();
        aggA.Results.IsExactly(aggB.Results);

        aggA.Unsubscribe();
        aggB.Unsubscribe();
        providerA.EndTransmission();
        providerB.EndTransmission();
    }

    [TestMethod]
    public void QuoteHub_Aggregator_Ema_LateArrival_MatchesFreshChain()
    {
        const int total = 500;
        const int skipIndex = 200;
        const int emaPeriods = 5;

        IReadOnlyList<Quote> quotes = Quotes.Take(total).ToList();

        // late chain: feed all but one bar, then add it late so the correction
        // rolls back through the aggregator and replays into the EMA
        QuoteHub lateProvider = new();
        QuoteAggregatorHub lateAgg = lateProvider.ToQuoteAggregatorHub(PeriodSize.Week);
        EmaHub lateEma = lateAgg.ToEmaHub(emaPeriods);

        for (int i = 0; i < total; i++)
        {
            if (i == skipIndex) { continue; }

            lateProvider.Add(quotes[i]);
        }

        lateProvider.Add(quotes[skipIndex]);  // late arrival → rollback through the aggregator

        // fresh chain: in-order oracle
        QuoteHub freshProvider = new();
        QuoteAggregatorHub freshAgg = freshProvider.ToQuoteAggregatorHub(PeriodSize.Week);
        EmaHub freshEma = freshAgg.ToEmaHub(emaPeriods);

        freshProvider.Add(quotes);

        // Aggregator-level equality is the sensitive pin: a bucket's Volume is a
        // sum, so a dropped or duplicated quote in the rollback replay diverges
        // here even when the EMA (which sees only the bucket close) would not.
        // The downstream EMA equality then confirms the correction propagates
        // through the aggregator into the chained indicator.
        lateAgg.Results.IsExactly(freshAgg.Results);

        lateEma.Results.Should().NotBeEmpty();
        freshEma.Results.Should().NotBeEmpty();
        lateEma.Results.IsExactly(freshEma.Results);

        lateEma.Unsubscribe();
        lateAgg.Unsubscribe();
        lateProvider.EndTransmission();
        freshEma.Unsubscribe();
        freshAgg.Unsubscribe();
        freshProvider.EndTransmission();
    }

    [TestMethod]
    public void TickHub_Aggregator_Ema_LateArrival_MatchesFreshChain()
    {
        const int skipIndex = 200;
        const int emaPeriods = 5;

        List<Tick> ticks = BuildTicks(TickCount);

        TickHub lateProvider = new();
        TickAggregatorHub lateAgg = lateProvider.ToTickAggregatorHub(PeriodSize.FifteenMinutes);
        EmaHub lateEma = lateAgg.ToEmaHub(emaPeriods);

        for (int i = 0; i < ticks.Count; i++)
        {
            if (i == skipIndex) { continue; }

            lateProvider.Add(ticks[i]);
        }

        lateProvider.Add(ticks[skipIndex]);  // late arrival

        TickHub freshProvider = new();
        TickAggregatorHub freshAgg = freshProvider.ToTickAggregatorHub(PeriodSize.FifteenMinutes);
        EmaHub freshEma = freshAgg.ToEmaHub(emaPeriods);

        freshProvider.Add(ticks);

        // aggregator-level equality is the sensitive pin (see the quote analog)
        lateAgg.Results.IsExactly(freshAgg.Results);

        lateEma.Results.Should().NotBeEmpty();
        freshEma.Results.Should().NotBeEmpty();
        lateEma.Results.IsExactly(freshEma.Results);

        lateEma.Unsubscribe();
        lateAgg.Unsubscribe();
        lateProvider.EndTransmission();
        freshEma.Unsubscribe();
        freshAgg.Unsubscribe();
        freshProvider.EndTransmission();
    }

    /// <summary>
    /// Synthetic one-minute ticks with strictly increasing timestamps and a mild
    /// deterministic wave so OHLC aggregation is non-trivial.
    /// </summary>
    private static List<Tick> BuildTicks(int count)
    {
        DateTime start = new(2023, 11, 9, 9, 30, 0, DateTimeKind.Utc);
        List<Tick> ticks = new(count);

        for (int i = 0; i < count; i++)
        {
            decimal price = 100m + (i % 37) - (i % 13);
            ticks.Add(new Tick(start.AddMinutes(i), price, 10m + (i % 5)));
        }

        return ticks;
    }
}
