namespace StreamHub;

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

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Results_AreAlwaysBounded()
    {
        ConnorsRsiHub sut = Quotes.ToConnorsRsiHub(rsiPeriods, streakPeriods, rankPeriods);

        TestAssert.IsBetween(sut.Results, static x => x.ConnorsRsi, 0d, 100d);
    }

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        for (int i = 0; i < 20; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // initialize observer
        ConnorsRsiHub observer = quoteHub
            .ToConnorsRsiHub(rsiPeriods, streakPeriods, rankPeriods);

        // fetch initial results (early)
        IReadOnlyList<ConnorsRsiResult> streamList
            = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 20; i < length; i++)
        {
            // skip one (add later)
            if (i == 80)
            {
                continue;
            }

            Quote q = quotesList[i];
            quoteHub.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105)
            {
                quoteHub.Add(q);
            }
        }

        // late arrival
        quoteHub.Insert(quotesList[80]);

        // delete
        quoteHub.Remove(quotesList[400]);
        quotesList.RemoveAt(400);

        // time-series, for comparison
        IReadOnlyList<ConnorsRsiResult> seriesList = quotesList.ToConnorsRsi(rsiPeriods, streakPeriods, rankPeriods);

        // assert, should equal series
        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList, options => options.WithStrictOrdering());

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
        streamList.Should().BeEquivalentTo(seriesList, options => options.WithStrictOrdering());

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider_MatchesSeriesExactly()
    {
        const int emaPeriods = 12;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        EmaHub observer = quoteHub
            .ToConnorsRsiHub(rsiPeriods, streakPeriods, rankPeriods)
            .ToEmaHub(emaPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // final results
        IReadOnlyList<EmaResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<EmaResult> seriesList
           = quotesList
            .ToConnorsRsi(rsiPeriods, streakPeriods, rankPeriods)
            .ToEma(emaPeriods);

        // assert, should equal series
        streamList.Should().HaveCount(length);
        streamList.Should().BeEquivalentTo(seriesList, options => options.WithStrictOrdering());

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }
}
