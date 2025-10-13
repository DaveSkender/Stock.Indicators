namespace StreamHub;

[TestClass]
public class MamaHub : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    private const double fastLimit = 0.5;
    private const double slowLimit = 0.05;

    [TestMethod]
    public void QuoteObserver()
    {
        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub<Quote> quoteHub = new();

        // prefill quotes at provider
        for (int i = 0; i < 20; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // initialize observer
        MamaHub<Quote> observer = quoteHub
            .ToMamaHub(fastLimit, slowLimit);

        // fetch initial results (early)
        IReadOnlyList<MamaResult> streamList
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
        IReadOnlyList<MamaResult> seriesList = quotesList.ToMama(fastLimit, slowLimit);

        // assert, should equal series
        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver()
    {
        const int smaPeriods = 8;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub<Quote> quoteHub = new();

        // initialize observer
        MamaHub<SmaResult> observer = quoteHub
            .ToSma(smaPeriods)
            .ToMamaHub(fastLimit, slowLimit);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // final results
        IReadOnlyList<MamaResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<MamaResult> seriesList
           = quotesList
            .ToSma(smaPeriods)
            .ToMama(fastLimit, slowLimit);

        // assert, should equal series
        streamList.Should().HaveCount(length);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider()
    {
        const int smaPeriods = 10;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub<Quote> quoteHub = new();

        // initialize observer
        SmaHub<MamaResult> observer = quoteHub
            .ToMamaHub(fastLimit, slowLimit)
            .ToSma(smaPeriods);

        // emulate adding quotes to provider hub
        for (int i = 0; i < length; i++)
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

        // final results
        IReadOnlyList<SmaResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<SmaResult> seriesList
           = quotesList.ToMama(fastLimit, slowLimit)
            .ToSma(smaPeriods);

        // assert, should equal series
        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        MamaHub<Quote> hub = new(new QuoteHub<Quote>(), fastLimit, slowLimit);
        hub.ToString().Should().Be("MAMA(0.5,0.05)");
    }
}
