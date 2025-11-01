namespace StreamHub;

[TestClass]
public class SmaHubTests : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
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
        SmaHub observer = quoteHub
            .ToSmaHub(5);

        // fetch initial results (early)
        IReadOnlyList<SmaResult> streamList
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
        IReadOnlyList<SmaResult> seriesList
           = quotesList
            .ToSma(5);

        // assert, should equal series
        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver()
    {
        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        for (int i = 0; i < 50; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // initialize observer
        SmaHub observer = quoteHub
            .ToQuotePartHub(CandlePart.OC2)
            .ToSmaHub(11);

        // emulate quote stream
        for (int i = 50; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        IReadOnlyList<SmaResult> streamList =
            observer.Results;

        // time-series, for comparison
        IReadOnlyList<SmaResult> seriesList
           = quotesList
            .Use(CandlePart.OC2)
            .ToSma(11);

        // assert, should equal series
        streamList.Should().HaveCount(length);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider()
    {
        const int emaPeriods = 12;
        const int smaPeriods = 8;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        EmaHub observer
           = quoteHub
            .ToSmaHub(smaPeriods)
            .ToEmaHub(emaPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // delete
        quoteHub.Remove(quotesList[400]);
        quotesList.RemoveAt(400);

        // final results
        IReadOnlyList<EmaResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<EmaResult> seriesList
           = quotesList
            .ToSma(smaPeriods)
            .ToEma(emaPeriods);

        // assert, should equal series
        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        SmaHub hub = new(new QuoteHub(), 5);
        hub.ToString().Should().Be("SMA(5)");
    }
}
