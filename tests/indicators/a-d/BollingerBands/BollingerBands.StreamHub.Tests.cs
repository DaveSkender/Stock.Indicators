namespace StreamHubs;

[TestClass]
public class BollingerBandsStreamHub : StreamHubTestBase, ITestQuoteObserver
{
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
        BollingerBandsHub<Quote> observer = quoteHub.ToBollingerBandsHub(20, 2);

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
        streamList.Should().BeEquivalentTo(
            seriesList,
            options => options.WithStrictOrdering());

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        QuoteHub<Quote> quoteHub = new();
        quoteHub.Add(Quotes);
        BollingerBandsHub<Quote> observer = quoteHub.ToBollingerBandsHub(20, 2);

        observer.ToString().Should().Be("BB(20,2)");
    }

    [TestMethod]
    public void ChainProvider()
    {
        // arrange
        const int lookbackPeriods = 20;
        const double standardDeviations = 2;
        const int smaPeriods = 10;

        List<Quote> quotesList = Quotes.ToList();
        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub<Quote> quoteHub = new();

        // initialize observer - chain SMA to Bollinger Bands
        SmaHub<BollingerBandsResult> observer = quoteHub
            .ToBollingerBandsHub(lookbackPeriods, standardDeviations)
            .ToSma(smaPeriods);

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
        streamList.Should().BeEquivalentTo(
            seriesList,
            options => options.WithStrictOrdering());

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void PrefilledProviderRebuilds()
    {
        QuoteHub<Quote> quoteHub = new();
        List<Quote> quotes = Quotes.Take(25).ToList();

        for (int i = 0; i < 5; i++)
        {
            quoteHub.Add(quotes[i]);
        }

        BollingerBandsHub<Quote> observer = quoteHub.ToBollingerBandsHub(5, 2);

        IReadOnlyList<BollingerBandsResult> initialResults = observer.Results;
        IReadOnlyList<BollingerBandsResult> expectedInitial = quotes
            .Take(5)
            .ToList()
            .ToBollingerBands(5, 2);

        initialResults.Should().BeEquivalentTo(
            expectedInitial,
            options => options.WithStrictOrdering());

        for (int i = 5; i < quotes.Count; i++)
        {
            quoteHub.Add(quotes[i]);
        }

        observer.Results.Should().BeEquivalentTo(
            quotes.ToBollingerBands(5, 2),
            options => options.WithStrictOrdering());

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }
}
