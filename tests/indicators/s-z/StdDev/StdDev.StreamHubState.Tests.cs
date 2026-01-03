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

        // Cleanup
        stdDevHub.Unsubscribe();
        stdDevHubState.Unsubscribe();
        quoteHub1.EndTransmission();
        quoteHub2.EndTransmission();
    }
}
