namespace StreamHubs;

[TestClass]
public class PmoHubStateTests : StreamHubTestBase
{
    private const int timePeriods = 35;
    private const int smoothPeriods = 20;
    private const int signalPeriods = 10;

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(Quotes.Take(20));

        PmoHubState observer = quoteHub.ToPmoHubState(timePeriods, smoothPeriods, signalPeriods);

        observer.ToString().Should().Be($"PMO({timePeriods},{smoothPeriods},{signalPeriods})");

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
        PmoHubState observer = quoteHub.ToPmoHubState(timePeriods, smoothPeriods, signalPeriods);

        // Fetch initial results (early)
        IReadOnlyList<PmoResult> sut = observer.Results;

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

        IReadOnlyList<PmoResult> expectedOriginal = Quotes.ToPmo(timePeriods, smoothPeriods, signalPeriods);
        sut.IsExactly(expectedOriginal);

        // Delete, should equal series (revised)
        quoteHub.Remove(Quotes[removeAtIndex]);

        IReadOnlyList<PmoResult> expectedRevised = RevisedQuotes.ToPmo(timePeriods, smoothPeriods, signalPeriods);
        sut.IsExactly(expectedRevised);
        sut.Should().HaveCount(quotesCount - 1);

        // Cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void PmoHubState_ProducesSameResultsAsPmoHub()
    {
        // Setup both hub types
        QuoteHub quoteHub1 = new();
        QuoteHub quoteHub2 = new();

        quoteHub1.Add(Quotes);
        quoteHub2.Add(Quotes);

        PmoHub pmoHub = quoteHub1.ToPmoHub(timePeriods, smoothPeriods, signalPeriods);
        PmoHubState pmoHubState = quoteHub2.ToPmoHubState(timePeriods, smoothPeriods, signalPeriods);

        // Results should be identical
        pmoHub.Results.Should().HaveCount(pmoHubState.Results.Count);

        for (int i = 0; i < pmoHub.Results.Count; i++)
        {
            PmoResult expected = pmoHub.Results[i];
            PmoResult actual = pmoHubState.Results[i];

            actual.Timestamp.Should().Be(expected.Timestamp);
            actual.Pmo.Should().Be(expected.Pmo);
            actual.Signal.Should().Be(expected.Signal);
        }

        // Cleanup
        pmoHub.Unsubscribe();
        pmoHubState.Unsubscribe();
        quoteHub1.EndTransmission();
        quoteHub2.EndTransmission();
    }
}
