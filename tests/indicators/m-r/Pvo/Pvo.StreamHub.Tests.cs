namespace StreamHub;

[TestClass]
public class PvoHubTests : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    public override void CustomToString()
    {
        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        PvoHub observer = quoteHub.ToPvoHub(12, 26, 9);

        // test string output
        observer.ToString().Should().Be("PVO(12,26,9)");

        observer.Unsubscribe();
        quoteHub.EndTransmission();
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
        PvoHub observer = quoteHub
            .ToPvoHub(12, 26, 9);

        // fetch initial results (early)
        IReadOnlyList<PvoResult> streamList
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
        IReadOnlyList<PvoResult> seriesList = quotesList.ToPvo(12, 26, 9);

        // assert, should equal series
        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver()
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
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider()
    {
        const int pvoFast = 12;
        const int pvoSlow = 26;
        const int pvoSignal = 9;
        const int emaPeriods = 10;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup chain quoteHub
        QuoteHub quoteProvider = new();
        PvoHub pvoHub = quoteProvider.ToPvoHub(pvoFast, pvoSlow, pvoSignal);

        // initialize observer (EMA of PVO)
        EmaHub observer = pvoHub
            .ToEmaHub(emaPeriods);

        // emulate live quotes
        for (int i = 0; i < length; i++)
        {
            quoteProvider.Add(quotesList[i]);
        }

        // final results
        IReadOnlyList<EmaResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<EmaResult> seriesList
           = quotesList
            .ToPvo(pvoFast, pvoSlow, pvoSignal)
            .ToEma(emaPeriods);

        // assert
        streamList.Should().HaveCount(length);
        streamList.Should().BeEquivalentTo(seriesList);

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

        streamResults.Should().BeEquivalentTo(seriesResults);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }
}
