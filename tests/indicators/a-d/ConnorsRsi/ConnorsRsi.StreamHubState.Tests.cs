namespace StreamHubs;

[TestClass]
public class ConnorsRsiHubStateTests : StreamHubTestBase
{
    private const int rsiPeriods = 3;
    private const int streakPeriods = 2;
    private const int rankPeriods = 100;

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(Quotes.Take(20));

        ConnorsRsiHubState observer = quoteHub.ToConnorsRsiHubState(rsiPeriods, streakPeriods, rankPeriods);

        observer.ToString().Should().Be($"CRSI({rsiPeriods},{streakPeriods},{rankPeriods})");

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void Results_AreAlwaysBounded()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(Quotes);

        IReadOnlyList<ConnorsRsiResult> sut = quoteHub.ToConnorsRsiHubState(rsiPeriods, streakPeriods, rankPeriods).Results;
        sut.IsBetween(x => x.ConnorsRsi, 0, 100);

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
        ConnorsRsiHubState observer = quoteHub.ToConnorsRsiHubState(rsiPeriods, streakPeriods, rankPeriods);

        // Fetch initial results (early)
        IReadOnlyList<ConnorsRsiResult> sut = observer.Results;

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

        IReadOnlyList<ConnorsRsiResult> expectedOriginal = Quotes.ToConnorsRsi(rsiPeriods, streakPeriods, rankPeriods);
        sut.IsExactly(expectedOriginal);

        // Delete, should equal series (revised)
        quoteHub.Remove(Quotes[removeAtIndex]);

        IReadOnlyList<ConnorsRsiResult> expectedRevised = RevisedQuotes.ToConnorsRsi(rsiPeriods, streakPeriods, rankPeriods);
        sut.IsExactly(expectedRevised);
        sut.Should().HaveCount(quotesCount - 1);

        // Cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ConnorsRsiHubState_ProducesSameResultsAsConnorsRsiHub()
    {
        // Setup both hub types
        QuoteHub quoteHub1 = new();
        QuoteHub quoteHub2 = new();

        quoteHub1.Add(Quotes);
        quoteHub2.Add(Quotes);

        ConnorsRsiHub connorsRsiHub = quoteHub1.ToConnorsRsiHub(rsiPeriods, streakPeriods, rankPeriods);
        ConnorsRsiHubState connorsRsiHubState = quoteHub2.ToConnorsRsiHubState(rsiPeriods, streakPeriods, rankPeriods);

        // Results should be identical
        connorsRsiHub.Results.Should().HaveCount(connorsRsiHubState.Results.Count);

        for (int i = 0; i < connorsRsiHub.Results.Count; i++)
        {
            ConnorsRsiResult expected = connorsRsiHub.Results[i];
            ConnorsRsiResult actual = connorsRsiHubState.Results[i];

            actual.Timestamp.Should().Be(expected.Timestamp);
            actual.Rsi.Should().Be(expected.Rsi);
            actual.RsiStreak.Should().Be(expected.RsiStreak);
            actual.PercentRank.Should().Be(expected.PercentRank);
            actual.ConnorsRsi.Should().Be(expected.ConnorsRsi);
        }

        // Cleanup
        connorsRsiHub.Unsubscribe();
        connorsRsiHubState.Unsubscribe();
        quoteHub1.EndTransmission();
        quoteHub2.EndTransmission();
    }
}
