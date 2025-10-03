namespace StreamHub;

[TestClass]
public class StochRsiHub : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
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
        StochRsiHub<Quote> observer = provider
            .ToStochRsi(14, 14, 3, 1);

        // fetch initial results (early)
        IReadOnlyList<StochRsiResult> streamList
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
        IReadOnlyList<StochRsiResult> seriesList = quotesList.ToStochRsi(14, 14, 3, 1);

        // assert, should equal series
        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver()
    {
        int emaPeriods = 12;
        int rsiPeriods = 14;
        int stochPeriods = 14;
        int signalPeriods = 3;
        int smoothPeriods = 1;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // initialize observer
        StochRsiHub<EmaResult> observer = provider
            .ToEma(emaPeriods)
            .ToStochRsi(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
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
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider()
    {
        int rsiPeriods = 14;
        int stochPeriods = 14;
        int signalPeriods = 3;
        int smoothPeriods = 1;
        int emaPeriods = 12;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // initialize observer
        EmaHub<StochRsiResult> observer = provider
            .ToStochRsi(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods)
            .ToEma(emaPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        // final results
        IReadOnlyList<EmaResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<EmaResult> seriesList
           = quotesList
            .ToStochRsi(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods)
            .ToEma(emaPeriods);

        // assert, should equal series
        streamList.Should().HaveCount(length);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        QuoteHub<Quote> provider = new();
        StochRsiHub<Quote> observer = provider.ToStochRsi(14, 14, 3, 1);

        observer.ToString().Should().Be("STOCH-RSI(14,14,3,1)");

        observer.Unsubscribe();
        provider.EndTransmission();
    }
}
