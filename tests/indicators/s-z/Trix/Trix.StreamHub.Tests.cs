namespace StreamHubs;

[TestClass]
public class TrixHubTests : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
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
        TrixHub observer = quoteHub
            .ToTrixHub(14);

        // fetch initial results (early)
        IReadOnlyList<TrixResult> streamList
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
        IReadOnlyList<TrixResult> seriesList = quotesList.ToTrix(14);

        // assert, should equal series
        streamList.Should().HaveCount(length - 1);
        streamList.IsExactly(seriesList);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver_ChainedProvider_MatchesSeriesExactly()
    {
        const int trixPeriods = 14;
        const int smaPeriods = 8;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        TrixHub observer = quoteHub
            .ToSmaHub(smaPeriods)
            .ToTrixHub(trixPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // final results
        IReadOnlyList<TrixResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<TrixResult> seriesList
           = quotesList
            .ToSma(smaPeriods)
            .ToTrix(trixPeriods);

        // assert
        streamList.Should().HaveCount(length);
        streamList.IsExactly(seriesList);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider_MatchesSeriesExactly()
    {
        const int trixPeriods = 14;
        const int emaPeriods = 10;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize chain with TRIX as input to EMA
        EmaHub emaOfTrix = quoteHub
            .ToTrixHub(trixPeriods)
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
        IReadOnlyList<EmaResult> sut = emaOfTrix.Results;

        // time-series, for comparison (revised)
        IReadOnlyList<EmaResult> expected = RevisedQuotes
            .ToTrix(trixPeriods)
            .ToEma(emaPeriods);

        // assert
        sut.Should().HaveCount(quotesCount - 1);
        sut.IsExactly(expected);

        emaOfTrix.Unsubscribe();
        quoteHub.EndTransmission();
    }

    public override void ToStringOverride_ReturnsExpectedName()
    {
        TrixHub hub = new(new QuoteHub(), 14);
        hub.ToString().Should().Be("TRIX(14)");
    }
}
