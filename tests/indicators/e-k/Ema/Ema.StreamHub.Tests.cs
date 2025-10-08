namespace StreamHub;

[TestClass]
public class EmaHub : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    [TestMethod]
    public override void QuoteObserver()
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
        EmaHub<Quote> observer = quoteHub
            .ToEmaHub(5);

        // fetch initial results (early)
        IReadOnlyList<EmaResult> streamList
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
        IReadOnlyList<EmaResult> seriesList = quotesList.ToEma(5);

        // assert, should equal series
        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver()
    {
        int emaPeriods = 12;
        int smaPeriods = 8;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub<Quote> quoteHub = new();

        // initialize observer
        EmaHub<SmaResult> observer = quoteHub
            .ToSma(smaPeriods)
            .ToEmaHub(emaPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // final results
        IReadOnlyList<EmaResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<EmaResult> seriesList
           = quotesList
            .ToSma(smaPeriods)
            .ToEma(emaPeriods);

        // assert, should equal series
        streamList.Should().HaveCount(length);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider()
    {
        int emaPeriods = 20;
        int smaPeriods = 10;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub<Quote> quoteHub = new();

        // initialize observer
        SmaHub<EmaResult> observer = quoteHub
            .ToEmaHub(emaPeriods)
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
           = quotesList.ToEma(emaPeriods)
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
        EmaHub<Quote> hub = new(new QuoteHub<Quote>(), 14);
        hub.ToString().Should().Be("EMA(14)");
    }
}
