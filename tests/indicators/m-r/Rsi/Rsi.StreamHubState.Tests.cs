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

        // Cleanup
        rsiHub.Unsubscribe();
        rsiHubState.Unsubscribe();
        quoteHub1.EndTransmission();
        quoteHub2.EndTransmission();
    }
}
