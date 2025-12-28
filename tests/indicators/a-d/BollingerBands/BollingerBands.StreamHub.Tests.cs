namespace StreamHubs;

[TestClass]
public class BollingerBandsHubTests : StreamHubTestBase, ITestQuoteObserver, ITestChainProvider
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
        BollingerBandsHub observer = quoteHub.ToBollingerBandsHub(20, 2);

        // fetch initial results (early)
        IReadOnlyList<BollingerBandsResult> streamList = observer.Results;

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
        IReadOnlyList<BollingerBandsResult> seriesList = quotesList.ToBollingerBands(20, 2);

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
        quoteHub.Add(Quotes);
        BollingerBandsHub observer = quoteHub.ToBollingerBandsHub(20, 2);

        observer.ToString().Should().Be("BB(20,2)");
    }

    [TestMethod]
    public void ChainProvider_MatchesSeriesExactly()
    {
        // arrange
        const int lookbackPeriods = 20;
        const double standardDeviations = 2;
        const int smaPeriods = 10;

        List<Quote> quotesList = Quotes.ToList();
        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer - chain SMA to Bollinger Bands
        SmaHub observer = quoteHub
            .ToBollingerBandsHub(lookbackPeriods, standardDeviations)
            .ToSmaHub(smaPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // final results
        IReadOnlyList<SmaResult> streamList = observer.Results;

        // time-series, for comparison
        IReadOnlyList<SmaResult> seriesList = quotesList
            .ToBollingerBands(lookbackPeriods, standardDeviations)
            .ToSma(smaPeriods);

        // assert, should equal series
        streamList.Should().HaveCount(length);
        streamList.IsExactly(seriesList);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void PrefilledProviderRebuilds()
    {
        QuoteHub quoteHub = new();
        List<Quote> quotes = Quotes.Take(25).ToList();

        for (int i = 0; i < 5; i++)
        {
            quoteHub.Add(quotes[i]);
        }

        BollingerBandsHub observer = quoteHub.ToBollingerBandsHub(5, 2);

        IReadOnlyList<BollingerBandsResult> initialResults = observer.Results;
        IReadOnlyList<BollingerBandsResult> expectedInitial = quotes
            .Take(5)
            .ToList()
            .ToBollingerBands(5, 2);

        initialResults.IsExactly(expectedInitial);

        for (int i = 5; i < quotes.Count; i++)
        {
            quoteHub.Add(quotes[i]);
        }

        observer.Results.IsExactly(quotes.ToBollingerBands(5, 2));

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }
}
