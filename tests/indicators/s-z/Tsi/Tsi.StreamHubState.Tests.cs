namespace StreamHubs;

[TestClass]
public class TsiHubStateTests : StreamHubTestBase
{
    private const int lookbackPeriods = 25;
    private const int smoothPeriods = 13;
    private const int signalPeriods = 7;

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(Quotes.Take(20));

        TsiHubState observer = quoteHub.ToTsiHubState(lookbackPeriods, smoothPeriods, signalPeriods);

        observer.ToString().Should().Be($"TSI({lookbackPeriods},{smoothPeriods},{signalPeriods})");

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void Results_AreAlwaysBounded()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(Quotes);

        IReadOnlyList<TsiResult> sut = quoteHub.ToTsiHubState(lookbackPeriods, smoothPeriods, signalPeriods).Results;
        sut.IsBetween(x => x.Tsi, -100, 100);
        sut.IsBetween(x => x.Signal, -100, 100);

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
        TsiHubState observer = quoteHub.ToTsiHubState(lookbackPeriods, smoothPeriods, signalPeriods);

        // Fetch initial results (early)
        IReadOnlyList<TsiResult> sut = observer.Results;

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

        IReadOnlyList<TsiResult> expectedOriginal = Quotes.ToTsi(lookbackPeriods, smoothPeriods, signalPeriods);
        sut.IsExactly(expectedOriginal);

        // Delete, should equal series (revised)
        quoteHub.Remove(Quotes[removeAtIndex]);

        IReadOnlyList<TsiResult> expectedRevised = RevisedQuotes.ToTsi(lookbackPeriods, smoothPeriods, signalPeriods);
        sut.IsExactly(expectedRevised);
        sut.Should().HaveCount(quotesCount - 1);

        // Cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void TsiHubState_ProducesSameResultsAsTsiHub()
    {
        // Setup both hub types
        QuoteHub quoteHub1 = new();
        QuoteHub quoteHub2 = new();

        quoteHub1.Add(Quotes);
        quoteHub2.Add(Quotes);

        TsiHub tsiHub = quoteHub1.ToTsiHub(lookbackPeriods, smoothPeriods, signalPeriods);
        TsiHubState tsiHubState = quoteHub2.ToTsiHubState(lookbackPeriods, smoothPeriods, signalPeriods);

        // Results should be identical
        tsiHub.Results.Should().HaveCount(tsiHubState.Results.Count);

        for (int i = 0; i < tsiHub.Results.Count; i++)
        {
            TsiResult expected = tsiHub.Results[i];
            TsiResult actual = tsiHubState.Results[i];

            actual.Timestamp.Should().Be(expected.Timestamp);
            actual.Tsi.Should().Be(expected.Tsi);
            actual.Signal.Should().Be(expected.Signal);
        }

        // Cleanup
        tsiHub.Unsubscribe();
        tsiHubState.Unsubscribe();
        quoteHub1.EndTransmission();
        quoteHub2.EndTransmission();
    }
}
