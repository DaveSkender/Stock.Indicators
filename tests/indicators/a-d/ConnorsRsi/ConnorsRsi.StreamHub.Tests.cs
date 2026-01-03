namespace StreamHubs;

[TestClass]
public class ConnorsRsiHubTests : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    private const int rsiPeriods = 3;
    private const int streakPeriods = 2;
    private const int rankPeriods = 100;

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        ConnorsRsiHub hub = Quotes.ToConnorsRsiHub(rsiPeriods, streakPeriods, rankPeriods);
        string actual = hub.ToString();
        string expected = $"CRSI({rsiPeriods},{streakPeriods},{rankPeriods})";

        actual.Should().Be(expected);
    }

    [TestMethod]
    public void Results_AreAlwaysBounded()
    {
        IReadOnlyList<ConnorsRsiResult> sut = Quotes.ToConnorsRsiHub(3, 2, 100).Results;
        sut.IsBetween(static x => x.ConnorsRsi, 0, 100);
    }

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        for (int i = 0; i < 20; i++)
        {
            quoteHub.Add(Quotes[i]);
        }

        // initialize observer
        ConnorsRsiHub observer = quoteHub
            .ToConnorsRsiHub(rsiPeriods, streakPeriods, rankPeriods);

        // fetch initial results (early)
        IReadOnlyList<ConnorsRsiResult> actuals
            = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 20; i < quotesCount; i++)
        {
            // skip one (add later)
            if (i == 80)
            {
                continue;
            }

            Quote q = Quotes[i];
            quoteHub.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105)
            {
                quoteHub.Add(q);
            }
        }

        // late arrival
        quoteHub.Insert(Quotes[80]);

        // delete
        quoteHub.Remove(Quotes[removeAtIndex]);

        // time-series, for comparison
        IReadOnlyList<ConnorsRsiResult> expected = RevisedQuotes.ToConnorsRsi(rsiPeriods, streakPeriods, rankPeriods);

        // assert, should equal series
        actuals.Should().HaveCount(quotesCount - 1);
        actuals.IsExactly(expected);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver_ChainedProvider_MatchesSeriesExactly()
    {
        const int emaPeriods = 12;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        ConnorsRsiHub observer = quoteHub
            .ToEmaHub(emaPeriods)
            .ToConnorsRsiHub(rsiPeriods, streakPeriods, rankPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // final results
        IReadOnlyList<ConnorsRsiResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<ConnorsRsiResult> seriesList
           = quotesList
            .ToEma(emaPeriods)
            .ToConnorsRsi(rsiPeriods, streakPeriods, rankPeriods);

        // assert, should equal series
        streamList.Should().HaveCount(length);
        streamList.IsExactly(seriesList);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider_MatchesSeriesExactly()
    {
        const int emaPeriods = 12;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        EmaHub observer = quoteHub
            .ToConnorsRsiHub(rsiPeriods, streakPeriods, rankPeriods)
            .ToEmaHub(emaPeriods);

        // emulate quote stream
        for (int i = 0; i < quotesCount; i++)
        {
            if (i == 80) { continue; }  // Skip for late arrival

            Quote q = Quotes[i];
            quoteHub.Add(q);

            if (i is > 100 and < 105) { quoteHub.Add(q); }  // Duplicate quotes
        }

        quoteHub.Insert(Quotes[80]);  // Late arrival
        quoteHub.Remove(Quotes[removeAtIndex]);  // Remove

        // final results
        IReadOnlyList<EmaResult> sut = observer.Results;

        // time-series, for comparison (revised)
        IReadOnlyList<EmaResult> expected = RevisedQuotes
            .ToConnorsRsi(rsiPeriods, streakPeriods, rankPeriods)
            .ToEma(emaPeriods);

        // assert, should equal series
        sut.Should().HaveCount(quotesCount - 1);
        sut.IsExactly(expected);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }
}
