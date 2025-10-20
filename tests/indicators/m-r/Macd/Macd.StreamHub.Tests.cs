namespace StreamHub;

[TestClass]
public class MacdHubTests : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    public override void CustomToString()
    {
        List<Quote> quotesList = Quotes.ToList();

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        MacdHub observer = quoteHub
            .ToMacdHub(12, 26, 9);

        // emulate quote stream
        for (int i = 0; i < 20; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // test string output
        observer.ToString().Should().Be("MACD(12,26,9)");

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }
    [TestMethod]
    public void QuoteObserver()
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
        MacdHub observer = quoteHub
            .ToMacdHub(12, 26, 9);

        // fetch initial results (early)
        IReadOnlyList<MacdResult> streamList
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
        IReadOnlyList<MacdResult> seriesList = quotesList.ToMacd(12, 26, 9);

        // assert, should equal series
        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver()
    {
        const int emaPeriods = 14;
        const int macdFast = 12;
        const int macdSlow = 26;
        const int macdSignal = 9;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        MacdHub observer = quoteHub
            .ToEmaHub(emaPeriods)
            .ToMacdHub(macdFast, macdSlow, macdSignal);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // final results
        IReadOnlyList<MacdResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<MacdResult> seriesList
           = quotesList
            .ToEma(emaPeriods)
            .ToMacd(macdFast, macdSlow, macdSignal);

        // assert
        streamList.Should().HaveCount(length);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider()
    {
        const int smaPeriods = 10;
        const int macdFast = 5;
        const int macdSlow = 10;
        const int macdSignal = 3;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup chain quoteHub
        QuoteHub quoteProvider = new();
        SmaHub quoteHub = quoteProvider.ToSmaHub(smaPeriods);

        // initialize observer
        MacdHub observer = quoteHub
            .ToMacdHub(macdFast, macdSlow, macdSignal);

        // emulate live quotes
        for (int i = 0; i < length; i++)
        {
            quoteProvider.Add(quotesList[i]);
        }

        // final results
        IReadOnlyList<MacdResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<MacdResult> seriesList
           = quotesList
            .ToSma(smaPeriods)
            .ToMacd(macdFast, macdSlow, macdSignal);

        // assert
        streamList.Should().HaveCount(length);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
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
        MacdHub observer = quoteHub
            .ToMacdHub(fastPeriods, slowPeriods, signalPeriods);

        // stream first 100 quotes
        for (int i = 0; i < 100; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // get streaming results
        IReadOnlyList<MacdResult> streamResults = observer.Results;

        // time-series for comparison
        IReadOnlyList<MacdResult> seriesResults = quotesList.Take(100).ToList().ToMacd(fastPeriods, slowPeriods, signalPeriods);

        // validate specific data points
        MacdResult streamResult = streamResults[50];
        MacdResult seriesResult = seriesResults[50];

        streamResult.Macd.Should().Be(seriesResult.Macd);
        streamResult.Signal.Should().Be(seriesResult.Signal);
        streamResult.Histogram.Should().Be(seriesResult.Histogram);
        streamResult.FastEma.Should().Be(seriesResult.FastEma);
        streamResult.SlowEma.Should().Be(seriesResult.SlowEma);

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
        MacdHub observer = quoteHub
            .ToMacdHub(8, 21, 5);

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
        IReadOnlyList<MacdResult> streamResults = observer.Results;
        IReadOnlyList<MacdResult> seriesResults = quotesList.Take(50).ToList().ToMacd(8, 21, 5);

        streamResults.Should().BeEquivalentTo(seriesResults);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }
}
