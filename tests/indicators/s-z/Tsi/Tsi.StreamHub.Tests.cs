namespace StreamHub;

[TestClass]
public class TsiHub : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    public override void CustomToString()
    {
        List<Quote> quotesList = Quotes.ToList();

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // initialize observer
        TsiHub<Quote> observer = provider
            .ToTsi(25, 13, 7);

        // emulate quote stream
        for (int i = 0; i < 20; i++)
        {
            provider.Add(quotesList[i]);
        }

        // test string output
        observer.ToString().Should().Be("TSI(25,13,7)");

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
        TsiHub<Quote> observer = provider
            .ToTsi(25, 13, 7);

        // fetch initial results (early)
        IReadOnlyList<TsiResult> streamList
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
        IReadOnlyList<TsiResult> seriesList = quotesList.ToTsi(25, 13, 7);

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
        int lookbackPeriods = 25;
        int smoothPeriods = 13;
        int signalPeriods = 7;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // initialize observer
        TsiHub<EmaResult> observer = provider
            .ToEma(emaPeriods)
            .ToTsi(lookbackPeriods, smoothPeriods, signalPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        // final results
        IReadOnlyList<TsiResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<TsiResult> seriesList
           = quotesList
            .ToEma(emaPeriods)
            .ToTsi(lookbackPeriods, smoothPeriods, signalPeriods);

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
        int lookbackPeriods = 15;
        int smoothPeriods = 8;
        int signalPeriods = 5;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup chain provider
        QuoteHub<Quote> quoteProvider = new();
        SmaHub<Quote> provider = quoteProvider.ToSma(smaPeriods);

        // initialize observer
        TsiHub<SmaResult> observer = provider
            .ToTsi(lookbackPeriods, smoothPeriods, signalPeriods);

        // emulate live quotes
        for (int i = 0; i < length; i++)
        {
            quoteProvider.Add(quotesList[i]);
        }

        // final results
        IReadOnlyList<TsiResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<TsiResult> seriesList
           = quotesList
            .ToSma(smaPeriods)
            .ToTsi(lookbackPeriods, smoothPeriods, signalPeriods);

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
        int lookbackPeriods = 25;
        int smoothPeriods = 13;
        int signalPeriods = 7;

        List<Quote> quotesList = Quotes.ToList();

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // initialize observer
        TsiHub<Quote> observer = provider
            .ToTsi(lookbackPeriods, smoothPeriods, signalPeriods);

        // stream first 100 quotes
        for (int i = 0; i < 100; i++)
        {
            provider.Add(quotesList[i]);
        }

        // get streaming results
        IReadOnlyList<TsiResult> streamResults = observer.Results;

        // time-series for comparison
        IReadOnlyList<TsiResult> seriesResults = quotesList.Take(100).ToList().ToTsi(lookbackPeriods, smoothPeriods, signalPeriods);

        // validate specific data points
        TsiResult streamResult = streamResults[50];
        TsiResult seriesResult = seriesResults[50];

        streamResult.Tsi.Should().BeApproximately(seriesResult.Tsi, 0.0001);
        streamResult.Signal.Should().BeApproximately(seriesResult.Signal, 0.0001);

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
        TsiHub<Quote> observer = provider
            .ToTsi(20, 10, 5);

        // verify parameters
        observer.LookbackPeriods.Should().Be(20);
        observer.SmoothPeriods.Should().Be(10);
        observer.SignalPeriods.Should().Be(5);

        // process some quotes
        for (int i = 0; i < 50; i++)
        {
            provider.Add(quotesList[i]);
        }

        // verify results consistency
        IReadOnlyList<TsiResult> streamResults = observer.Results;
        IReadOnlyList<TsiResult> seriesResults = quotesList.Take(50).ToList().ToTsi(20, 10, 5);

        streamResults.Should().BeEquivalentTo(seriesResults);

        observer.Unsubscribe();
        provider.EndTransmission();
    }
}
