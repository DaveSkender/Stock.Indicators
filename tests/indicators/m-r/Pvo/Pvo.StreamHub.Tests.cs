namespace StreamHubs;

[TestClass]
public class PvoHubTests : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    public override void ToStringOverride_ReturnsExpectedName()
    {
        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        PvoHub observer = quoteHub.ToPvoHub(12, 26, 9);

        // test string output
        observer.ToString().Should().Be("PVO(12,26,9)");

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
        PvoHub observer = quoteHub
            .ToPvoHub(12, 26, 9);

        // fetch initial results (early)
        IReadOnlyList<PvoResult> actuals
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
        IReadOnlyList<PvoResult> expected = RevisedQuotes.ToPvo(12, 26, 9);

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
        const int pvoFast = 12;
        const int pvoSlow = 26;
        const int pvoSignal = 9;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer from QuotePartHub(Volume)
        PvoHub observer = quoteHub
            .ToQuotePartHub(CandlePart.Volume)
            .ToPvoHub(pvoFast, pvoSlow, pvoSignal);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // final results
        IReadOnlyList<PvoResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<PvoResult> seriesList
           = quotesList
            .ToPvo(pvoFast, pvoSlow, pvoSignal);

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
        const int pvoFast = 12;
        const int pvoSlow = 26;
        const int pvoSignal = 9;
        const int emaPeriods = 10;

        // setup chain quoteHub
        QuoteHub quoteProvider = new();
        PvoHub pvoHub = quoteProvider.ToPvoHub(pvoFast, pvoSlow, pvoSignal);

        // initialize observer (EMA of PVO)
        EmaHub observer = pvoHub
            .ToEmaHub(emaPeriods);

        // emulate live quotes
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
        IReadOnlyList<EmaResult> sut = observer.Results;

        // time-series, for comparison (revised)
        IReadOnlyList<EmaResult> expected = RevisedQuotes
            .ToPvo(pvoFast, pvoSlow, pvoSignal)
            .ToEma(emaPeriods);

        // assert
        sut.Should().HaveCount(quotesCount - 1);
        sut.IsExactly(expected);

        // cleanup
        observer.Unsubscribe();
        pvoHub.EndTransmission();
        quoteProvider.EndTransmission();
    }

    [TestMethod]
    public void StreamingAccuracy()
    {
        const int fastPeriods = 12;
        const int slowPeriods = 26;
        const int signalPeriods = 9;

        List<Quote> quotesList = Quotes.ToList();

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        PvoHub observer = quoteHub
            .ToPvoHub(fastPeriods, slowPeriods, signalPeriods);

        // stream first 100 quotes
        for (int i = 0; i < 100; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // get streaming results
        IReadOnlyList<PvoResult> streamResults = observer.Results;

        // time-series for comparison
        IReadOnlyList<PvoResult> seriesResults = quotesList.Take(100).ToList().ToPvo(fastPeriods, slowPeriods, signalPeriods);

        // validate specific data points
        PvoResult streamResult = streamResults[50];
        PvoResult seriesResult = seriesResults[50];

        streamResult.Pvo.Should().Be(seriesResult.Pvo);
        streamResult.Signal.Should().Be(seriesResult.Signal);
        streamResult.Histogram.Should().Be(seriesResult.Histogram);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void Parameters()
    {
        List<Quote> quotesList = Quotes.ToList();

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer with custom parameters
        PvoHub observer = quoteHub
            .ToPvoHub(8, 21, 5);

        // verify parameters
        observer.FastPeriods.Should().Be(8);
        observer.SlowPeriods.Should().Be(21);
        observer.SignalPeriods.Should().Be(5);

        // process some quotes
        for (int i = 0; i < 50; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // verify results consistency
        IReadOnlyList<PvoResult> streamResults = observer.Results;
        IReadOnlyList<PvoResult> seriesResults = quotesList.Take(50).ToList().ToPvo(8, 21, 5);

        streamResults.IsExactly(seriesResults);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }
}
