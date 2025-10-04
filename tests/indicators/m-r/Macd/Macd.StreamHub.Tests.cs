namespace StreamHub;

[TestClass]
public class MacdHub : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    public override void CustomToString()
    {
        List<Quote> quotesList = Quotes.ToList();

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // initialize observer
        MacdHub<Quote> observer = provider
            .ToMacd(12, 26, 9);

        // emulate quote stream
        for (int i = 0; i < 20; i++)
        {
            provider.Add(quotesList[i]);
        }

        // test string output
        observer.ToString().Should().Be("MACD(12,26,9)");

        observer.Unsubscribe();
        provider.EndTransmission();
    }
    [TestMethod]
    public override void QuoteObserver()
    {
        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // prefill quotes to provider
        for (int i = 0; i < 20; i++)
        {
            provider.Add(quotesList[i]);
        }

        // initialize observer
        MacdHub<Quote> observer = provider
            .ToMacd(12, 26, 9);

        // fetch initial results (early)
        IReadOnlyList<MacdResult> streamList
            = observer.Results;

        // emulate adding quotes to provider
        for (int i = 20; i < length; i++)
        {
            // skip one (add later)
            if (i == 80)
            {
                continue;
            }

            Quote q = quotesList[i];
            provider.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105)
            {
                provider.Add(q);
            }
        }

        // late arrival
        provider.Insert(quotesList[80]);

        // delete
        provider.Remove(quotesList[400]);
        quotesList.RemoveAt(400);

        // time-series, for comparison
        IReadOnlyList<MacdResult> seriesList = quotesList.ToMacd(12, 26, 9);

        // assert, should equal series
        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver()
    {
        int emaPeriods = 14;
        int macdFast = 12;
        int macdSlow = 26;
        int macdSignal = 9;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // initialize observer
        MacdHub<EmaResult> observer = provider
            .ToEma(emaPeriods)
            .ToMacd(macdFast, macdSlow, macdSignal);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
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
        provider.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider()
    {
        int smaPeriods = 10;
        int macdFast = 5;
        int macdSlow = 10;
        int macdSignal = 3;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup chain provider
        QuoteHub<Quote> quoteProvider = new();
        SmaHub<Quote> provider = quoteProvider.ToSma(smaPeriods);

        // initialize observer
        MacdHub<SmaResult> observer = provider
            .ToMacd(macdFast, macdSlow, macdSignal);

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
        provider.EndTransmission();
        quoteProvider.EndTransmission();
    }

    [TestMethod]
    public void StreamingAccuracy()
    {
        int fastPeriods = 12;
        int slowPeriods = 26;
        int signalPeriods = 9;

        List<Quote> quotesList = Quotes.ToList();

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // initialize observer
        MacdHub<Quote> observer = provider
            .ToMacd(fastPeriods, slowPeriods, signalPeriods);

        // stream first 100 quotes
        for (int i = 0; i < 100; i++)
        {
            provider.Add(quotesList[i]);
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
        provider.EndTransmission();
    }

    [TestMethod]
    public void Parameters()
    {
        List<Quote> quotesList = Quotes.ToList();

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // initialize observer with custom parameters
        MacdHub<Quote> observer = provider
            .ToMacd(8, 21, 5);

        // verify parameters
        observer.FastPeriods.Should().Be(8);
        observer.SlowPeriods.Should().Be(21);
        observer.SignalPeriods.Should().Be(5);

        // process some quotes
        for (int i = 0; i < 50; i++)
        {
            provider.Add(quotesList[i]);
        }

        // verify results consistency
        IReadOnlyList<MacdResult> streamResults = observer.Results;
        IReadOnlyList<MacdResult> seriesResults = quotesList.Take(50).ToList().ToMacd(8, 21, 5);

        streamResults.Should().BeEquivalentTo(seriesResults);

        observer.Unsubscribe();
        provider.EndTransmission();
    }
}
