namespace StreamHubs;

[TestClass]
public class StdDevHubStateTests : StreamHubTestBase
{
    private const int lookbackPeriods = 10;

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(Quotes.Take(20));

        StdDevHubState observer = quoteHub.ToStdDevHubState(lookbackPeriods);

        observer.ToString().Should().Be($"STDDEV({lookbackPeriods})");

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        int length = Quotes.Count;

        // Setup quote provider hub
        QuoteHub quoteHub = new();

        // Prefill quotes at provider
        quoteHub.Add(Quotes.Take(20));

        // Initialize observer
        StdDevHubState observer = quoteHub.ToStdDevHubState(lookbackPeriods);

        // Fetch initial results (early)
        IReadOnlyList<StdDevResult> actuals = observer.Results;

        // Emulate adding quotes to provider hub
        for (int i = 20; i < length; i++)
        {
            // Skip one (add later)
            if (i == 80) { continue; }

            Quote q = Quotes[i];
            quoteHub.Add(q);

            // Resend duplicate quotes
            if (i is > 100 and < 105) { quoteHub.Add(q); }
        }

        // Late arrival, should equal series
        quoteHub.Insert(Quotes[80]);

        IReadOnlyList<StdDevResult> expectedOriginal = Quotes.ToStdDev(lookbackPeriods);
        actuals.IsExactly(expectedOriginal);

        // Delete, should equal series (revised)
        quoteHub.Remove(Quotes[removeAtIndex]);

        IReadOnlyList<StdDevResult> expectedRevised = RevisedQuotes.ToStdDev(lookbackPeriods);

        actuals.Should().HaveCount(501);
        actuals.IsExactly(expectedRevised);

        // Cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void StateCache_SynchronizedWithResults()
    {
        // Setup
        QuoteHub quoteHub = new();
        quoteHub.Add(Quotes.Take(50));

        StdDevHubState observer = quoteHub.ToStdDevHubState(lookbackPeriods);

        // Verify state cache is synchronized
        observer.Results.Should().HaveCount(50);
        observer.StateCache.Should().HaveCount(50);

        // Add more quotes
        for (int i = 50; i < quotesCount; i++)
        {
            quoteHub.Add(Quotes[i]);
        }

        // Verify state cache stays synchronized
        observer.Results.Should().HaveCount(quotesCount);
        observer.StateCache.Should().HaveCount(quotesCount);

        // Verify state contains valid values after warmup
        for (int i = lookbackPeriods - 1; i < observer.StateCache.Count; i++)
        {
            StdDevState state = observer.StateCache[i];

            // State should have valid count after warmup
            state.Count.Should().Be(lookbackPeriods);
            state.Mean.Should().NotBe(0);
        }

        // Cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void StateCache_RollbackRestoresCorrectly()
    {
        // Setup
        QuoteHub quoteHub = new();
        quoteHub.Add(Quotes);

        StdDevHubState observer = quoteHub.ToStdDevHubState(lookbackPeriods);

        int initialCount = observer.Results.Count;
        int initialStateCount = observer.StateCache.Count;

        // Remove some entries (triggers rollback)
        DateTime rollbackTimestamp = observer.Results[80].Timestamp;
        double? stdDevAt79 = observer.Results[79].StdDev;

        observer.RemoveRange(rollbackTimestamp, notify: false);

        // Verify state was rolled back
        observer.Results.Should().HaveCount(80);
        observer.StateCache.Should().HaveCount(80);

        // Rebuild from provider
        observer.Rebuild(rollbackTimestamp);

        // Verify rebuild worked and results are consistent
        observer.Results.Should().HaveCount(initialCount);
        observer.StateCache.Should().HaveCount(initialStateCount);

        // Verify StdDev value at position 79 didn't change
        observer.Results[79].StdDev.Should().Be(stdDevAt79);

        // Cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void StdDevHubState_ProducesSameResultsAsStdDevHub()
    {
        // Setup both hub types
        QuoteHub quoteHub1 = new();
        QuoteHub quoteHub2 = new();

        quoteHub1.Add(Quotes);
        quoteHub2.Add(Quotes);

        StdDevHub stdDevHub = quoteHub1.ToStdDevHub(lookbackPeriods);
        StdDevHubState stdDevHubState = quoteHub2.ToStdDevHubState(lookbackPeriods);

        // Results should be identical
        stdDevHub.Results.Should().HaveCount(stdDevHubState.Results.Count);

        for (int i = 0; i < stdDevHub.Results.Count; i++)
        {
            StdDevResult expected = stdDevHub.Results[i];
            StdDevResult actual = stdDevHubState.Results[i];

            actual.Timestamp.Should().Be(expected.Timestamp);
            actual.StdDev.Should().Be(expected.StdDev);
            actual.Mean.Should().Be(expected.Mean);
            actual.ZScore.Should().Be(expected.ZScore);
        }

        // Verify state cache is populated
        stdDevHubState.StateCache.Should().HaveCount(stdDevHubState.Results.Count);

        // Cleanup
        stdDevHub.Unsubscribe();
        stdDevHubState.Unsubscribe();
        quoteHub1.EndTransmission();
        quoteHub2.EndTransmission();
    }
}
