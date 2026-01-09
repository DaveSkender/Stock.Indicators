namespace StreamHubs;

[TestClass]
public class PmoHubTests : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        List<Quote> quotesList = Quotes.ToList();

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        PmoHub observer = quoteHub
            .ToPmoHub(35, 20, 10);

        // test string output
        observer.ToString().Should().Be("PMO(35,20,10)");

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
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
        PmoHub observer = quoteHub
            .ToPmoHub(35, 20, 10);

        // fetch initial results (early)
        IReadOnlyList<PmoResult> actuals
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
        IReadOnlyList<PmoResult> expected = RevisedQuotes.ToPmo(35, 20, 10);

        // assert, should equal series
        actuals.Should().HaveCount(quotesCount - 1);
        actuals.IsExactly(expected);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver_ChainedProvider_MatchesSeriesExactly()
    {
        const int emaPeriods = 12;
        const int timePeriods = 35;
        const int smoothPeriods = 20;
        const int signalPeriods = 10;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        PmoHub observer = quoteHub
            .ToEmaHub(emaPeriods)
            .ToPmoHub(timePeriods, smoothPeriods, signalPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // final results
        IReadOnlyList<PmoResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<PmoResult> seriesList
           = quotesList
            .ToEma(emaPeriods)
            .ToPmo(timePeriods, smoothPeriods, signalPeriods);

        // assert, should equal series
        streamList.Should().HaveCount(length);
        streamList.IsExactly(seriesList);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider_MatchesSeriesExactly()
    {
        const int smaPeriods = 10;
        const int timePeriods = 35;
        const int smoothPeriods = 20;
        const int signalPeriods = 10;

        // setup chain quoteHub
        QuoteHub quoteProvider = new();
        SmaHub quoteHub = quoteProvider.ToSmaHub(smaPeriods);

        // initialize observer
        SmaHub observer = quoteHub
            .ToPmoHub(timePeriods, smoothPeriods, signalPeriods)
            .ToSmaHub(smaPeriods);

        // emulate quote stream
        for (int i = 0; i < quotesCount; i++)
        {
            if (i == 80) { continue; }  // Skip for late arrival

            Quote q = Quotes[i];
            quoteProvider.Add(q);

            if (i is > 100 and < 105) { quoteProvider.Add(q); }  // Duplicate quotes
        }

        quoteProvider.Insert(Quotes[80]);  // Late arrival
        quoteProvider.Remove(Quotes[removeAtIndex]);  // Remove

        // final results
        IReadOnlyList<SmaResult> sut = observer.Results;

        // time-series, for comparison (revised)
        IReadOnlyList<SmaResult> expected = RevisedQuotes
            .ToSma(smaPeriods)
            .ToPmo(timePeriods, smoothPeriods, signalPeriods)
            .ToSma(smaPeriods);

        // assert, should equal series
        sut.Should().HaveCount(quotesCount - 1);
        sut.IsExactly(expected);

        // cleanup
        observer.Unsubscribe();
        quoteHub.Unsubscribe();
        quoteProvider.EndTransmission();
    }
}
