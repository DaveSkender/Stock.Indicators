namespace StreamHubs;

[TestClass]
public class RsiHubStateTests : StreamHubTestBase
{
    private const int lookbackPeriods = 5;

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(Quotes.Take(20));
        
        RsiHubState observer = quoteHub.ToRsiHubState(lookbackPeriods);

        observer.ToString().Should().Be($"RSI({lookbackPeriods})");

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        // Setup quote provider hub
        QuoteHub quoteHub = new();

        // Prefill quotes at provider
        quoteHub.Add(Quotes.Take(20));

        // Initialize observer
        RsiHubState observer = quoteHub.ToRsiHubState(lookbackPeriods);

        // Fetch initial results (early)
        IReadOnlyList<RsiResult> sut = observer.Results;

        // Emulate adding quotes to provider hub
        for (int i = 20; i < quotesCount; i++)
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

        IReadOnlyList<RsiResult> expectedOriginal = Quotes.ToRsi(lookbackPeriods);
        sut.IsExactly(expectedOriginal);

        // Delete, should equal series (revised)
        quoteHub.Remove(Quotes[removeAtIndex]);

        IReadOnlyList<RsiResult> expectedRevised = RevisedQuotes.ToRsi(lookbackPeriods);
        sut.IsExactly(expectedRevised);
        sut.Should().HaveCount(quotesCount - 1);

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
        
        RsiHubState observer = quoteHub.ToRsiHubState(lookbackPeriods);

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
        for (int i = lookbackPeriods; i < observer.StateCache.Count; i++)
        {
            RsiState state = observer.StateCache[i];
            
            // State should have valid gain/loss values after warmup
            state.AvgGain.Should().NotBe(double.NaN);
            state.AvgLoss.Should().NotBe(double.NaN);
            state.AvgGain.Should().BeGreaterOrEqualTo(0);
            state.AvgLoss.Should().BeGreaterOrEqualTo(0);
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
        
        RsiHubState observer = quoteHub.ToRsiHubState(lookbackPeriods);

        int initialCount = observer.Results.Count;
        int initialStateCount = observer.StateCache.Count;

        // Remove some entries (triggers rollback)
        DateTime rollbackTimestamp = observer.Results[80].Timestamp;
        double? rsiAt79 = observer.Results[79].Rsi;
        
        observer.RemoveRange(rollbackTimestamp, notify: false);

        // Verify state was rolled back
        observer.Results.Should().HaveCount(80);
        observer.StateCache.Should().HaveCount(80);

        // Rebuild from provider
        observer.Rebuild(rollbackTimestamp);

        // Verify rebuild worked and results are consistent
        observer.Results.Should().HaveCount(initialCount);
        observer.StateCache.Should().HaveCount(initialStateCount);

        // Verify RSI value at position 79 didn't change
        observer.Results[79].Rsi.Should().Be(rsiAt79);

        // Cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void RsiHubState_ProducesSameResultsAsRsiHub()
    {
        // Setup both hub types
        QuoteHub quoteHub1 = new();
        QuoteHub quoteHub2 = new();
        
        quoteHub1.Add(Quotes);
        quoteHub2.Add(Quotes);

        RsiHub rsiHub = quoteHub1.ToRsiHub(lookbackPeriods);
        RsiHubState rsiHubState = quoteHub2.ToRsiHubState(lookbackPeriods);

        // Results should be identical
        rsiHub.Results.Should().HaveCount(rsiHubState.Results.Count);

        for (int i = 0; i < rsiHub.Results.Count; i++)
        {
            RsiResult expected = rsiHub.Results[i];
            RsiResult actual = rsiHubState.Results[i];

            actual.Timestamp.Should().Be(expected.Timestamp);
            actual.Rsi.Should().Be(expected.Rsi);
        }

        // Verify state cache is populated
        rsiHubState.StateCache.Should().HaveCount(rsiHubState.Results.Count);

        // Cleanup
        rsiHub.Unsubscribe();
        rsiHubState.Unsubscribe();
        quoteHub1.EndTransmission();
        quoteHub2.EndTransmission();
    }
}
