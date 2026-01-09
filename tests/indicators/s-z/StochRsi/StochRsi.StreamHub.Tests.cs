namespace StreamHubs;

[TestClass]
public class StochRsiHubTests : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    [TestMethod]
    public void Results_AreAlwaysBounded()
    {
        IReadOnlyList<StochRsiResult> sut = Quotes.ToStochRsiHub(14, 14, 3, 1).Results;
        sut.IsBetween(static x => x.StochRsi, 0, 100);
        sut.IsBetween(static x => x.Signal, 0, 100);
    }

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        quoteHub.Add(Quotes.Take(20));

        // initialize observer
        StochRsiHub observer = quoteHub.ToStochRsiHub(14, 14, 3, 1);

        // fetch initial results (early)
        IReadOnlyList<StochRsiResult> sut = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 20; i < quotesCount; i++)
        {
            // skip one (add later)
            if (i == 80) { continue; }

            Quote q = Quotes[i];
            quoteHub.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105) { quoteHub.Add(q); }
        }

        // late arrival, should equal series
        quoteHub.Insert(Quotes[80]);

        IReadOnlyList<StochRsiResult> expectedOriginal = Quotes.ToStochRsi(14, 14, 3, 1);
        sut.IsExactly(expectedOriginal);

        // delete, should equal series (revised)
        quoteHub.Remove(Quotes[removeAtIndex]);

        IReadOnlyList<StochRsiResult> expectedRevised = RevisedQuotes.ToStochRsi(14, 14, 3, 1);
        sut.IsExactly(expectedRevised);
        sut.Should().HaveCount(quotesCount - 1);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver_ChainedProvider_MatchesSeriesExactly()
    {
        const int emaPeriods = 12;
        const int rsiPeriods = 14;
        const int stochPeriods = 14;
        const int signalPeriods = 3;
        const int smoothPeriods = 1;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        StochRsiHub observer = quoteHub
            .ToEmaHub(emaPeriods)
            .ToStochRsiHub(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // final results
        IReadOnlyList<StochRsiResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<StochRsiResult> seriesList
           = quotesList
            .ToEma(emaPeriods)
            .ToStochRsi(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);

        // assert, should equal series
        streamList.Should().HaveCount(length);
        streamList.IsExactly(seriesList);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider_MatchesSeriesExactly()
    {
        const int rsiPeriods = 14;
        const int stochPeriods = 14;
        const int signalPeriods = 3;
        const int smoothPeriods = 1;
        const int emaPeriods = 12;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        EmaHub observer = quoteHub
            .ToStochRsiHub(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods)
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
            .ToStochRsi(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods)
            .ToEma(emaPeriods);

        // assert, should equal series
        sut.Should().HaveCount(quotesCount - 1);
        sut.IsExactly(expected);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        StochRsiHub hub = new(new QuoteHub(), 14, 14, 3, 1);
        hub.ToString().Should().Be("STOCH-RSI(14,14,3,1)");
    }
}
