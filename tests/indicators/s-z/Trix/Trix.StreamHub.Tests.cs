namespace StreamHubs;

[TestClass]
public class TrixHubTests : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        quoteHub.Add(Quotes.Take(20));

        // initialize observer
        TrixHub observer = quoteHub.ToTrixHub(14);

        // fetch initial results (early)
        IReadOnlyList<TrixResult> sut = observer.Results;

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

        IReadOnlyList<TrixResult> expectedOriginal = Quotes.ToTrix(14);
        sut.IsExactly(expectedOriginal);

        // delete, should equal series (revised)
        quoteHub.Remove(Quotes[removeAtIndex]);

        IReadOnlyList<TrixResult> expectedRevised = RevisedQuotes.ToTrix(14);
        sut.IsExactly(expectedRevised);
        sut.Should().HaveCount(quotesCount - 1);

        // cleanup
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

        // cleanup
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
