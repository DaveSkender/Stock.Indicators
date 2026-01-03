namespace StreamHubs;

[TestClass]
public class SmaHubStateTests : StreamHubTestBase
{
    private const int lookbackPeriods = 5;

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(Quotes.Take(20));

        SmaHubState observer = quoteHub.ToSmaHubState(lookbackPeriods);

        observer.ToString().Should().Be($"SMA({lookbackPeriods})");

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
        SmaHubState observer = quoteHub.ToSmaHubState(lookbackPeriods);

        // Fetch initial results (early)
        IReadOnlyList<SmaResult> sut = observer.Results;

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

        IReadOnlyList<SmaResult> expectedOriginal = Quotes.ToSma(lookbackPeriods);
        sut.IsExactly(expectedOriginal);

        // Delete, should equal series (revised)
        quoteHub.Remove(Quotes[removeAtIndex]);

        IReadOnlyList<SmaResult> expectedRevised = RevisedQuotes.ToSma(lookbackPeriods);
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

        SmaHubState observer = quoteHub.ToSmaHubState(lookbackPeriods);

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
            SmaState state = observer.StateCache[i];

            // State should have valid count and sum after warmup
            state.Count.Should().Be(lookbackPeriods);
            state.RollingSum.Should().NotBe(0);
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

        SmaHubState observer = quoteHub.ToSmaHubState(lookbackPeriods);

        int initialCount = observer.Results.Count;
        int initialStateCount = observer.StateCache.Count;

        // Remove some entries (triggers rollback)
        DateTime rollbackTimestamp = observer.Results[80].Timestamp;
        double? smaAt79 = observer.Results[79].Sma;

        observer.RemoveRange(rollbackTimestamp, notify: false);

        // Verify state was rolled back
        observer.Results.Should().HaveCount(80);
        observer.StateCache.Should().HaveCount(80);

        // Rebuild from provider
        observer.Rebuild(rollbackTimestamp);

        // Verify rebuild worked and results are consistent
        observer.Results.Should().HaveCount(initialCount);
        observer.StateCache.Should().HaveCount(initialStateCount);

        // Verify SMA value at position 79 didn't change
        observer.Results[79].Sma.Should().Be(smaAt79);

        // Cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void SmaHubState_ProducesSameResultsAsSmaHub()
    {
        // Setup both hub types
        QuoteHub quoteHub1 = new();
        QuoteHub quoteHub2 = new();

        quoteHub1.Add(Quotes);
        quoteHub2.Add(Quotes);

        SmaHub smaHub = quoteHub1.ToSmaHub(lookbackPeriods);
        SmaHubState smaHubState = quoteHub2.ToSmaHubState(lookbackPeriods);

        // Results should be identical
        smaHub.Results.Should().HaveCount(smaHubState.Results.Count);

        for (int i = 0; i < smaHub.Results.Count; i++)
        {
            SmaResult expected = smaHub.Results[i];
            SmaResult actual = smaHubState.Results[i];

            actual.Timestamp.Should().Be(expected.Timestamp);
            actual.Sma.Should().Be(expected.Sma);
        }

        // Verify state cache is populated
        smaHubState.StateCache.Should().HaveCount(smaHubState.Results.Count);

        // Cleanup
        smaHub.Unsubscribe();
        smaHubState.Unsubscribe();
        quoteHub1.EndTransmission();
        quoteHub2.EndTransmission();
    }
}
