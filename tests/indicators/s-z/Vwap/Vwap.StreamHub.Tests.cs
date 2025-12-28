namespace StreamHubs;

[TestClass]
public class VwapHubTests : StreamHubTestBase, ITestQuoteObserver, ITestChainProvider
{
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
        VwapHub observer = quoteHub.ToVwapHub();

        // fetch initial results (early)
        IReadOnlyList<VwapResult> streamList = observer.Results;

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

        // removal
        quoteHub.Remove(quotesList[400]);
        quotesList.RemoveAt(400);

        // final results
        IReadOnlyList<VwapResult> seriesList = quotesList.ToVwap();

        streamList.Should().HaveCount(length - 1);
        streamList.IsExactly(seriesList);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactlyWithStartDate()
    {
        List<Quote> quotesList = Quotes.ToList();
        int length = quotesList.Count;

        // Use a start date somewhere in the middle
        DateTime startDate = quotesList[100].Timestamp;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        for (int i = 0; i < 20; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // initialize observer with start date
        VwapHub observer = quoteHub.ToVwapHub(startDate);

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

        // removal
        quoteHub.Remove(quotesList[400]);
        quotesList.RemoveAt(400);

        // final results
        IReadOnlyList<VwapResult> seriesList = quotesList.ToVwap(startDate);

        observer.Results.Should().HaveCount(length - 1);
        observer.Results.IsExactly(seriesList);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider_MatchesSeriesExactly()
    {
        const int smaPeriods = 8;

        List<Quote> quotesList = Quotes.ToList();
        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        VwapHub vwapHub = quoteHub.ToVwapHub();

        SmaHub observer = vwapHub
            .ToSmaHub(smaPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // delete
        quoteHub.Remove(quotesList[400]);
        quotesList.RemoveAt(400);

        // final results
        IReadOnlyList<SmaResult> streamList = observer.Results;

        // time-series, for comparison
        IReadOnlyList<SmaResult> seriesList = quotesList
            .ToVwap()
            .ToSma(smaPeriods);

        // assert, should equal series
        streamList.Should().HaveCount(length - 1);
        streamList.IsExactly(seriesList);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        QuoteHub quoteHub = new();

        VwapHub hub = new(quoteHub);
        hub.ToString().Should().Be("VWAP");

        quoteHub.Add(Quotes[0]);
        quoteHub.Add(Quotes[1]);

        string s = $"VWAP({Quotes[0].Timestamp:d})";
        hub.ToString().Should().Be(s);
    }

    [TestMethod]
    public void CustomToStringWithStartDate()
    {
        QuoteHub quoteHub = new();
        DateTime startDate = new(2018, 1, 15);

        VwapHub hub = new(quoteHub, startDate);
        string expected = $"VWAP({startDate:d})";
        hub.ToString().Should().Be(expected);
    }
}
