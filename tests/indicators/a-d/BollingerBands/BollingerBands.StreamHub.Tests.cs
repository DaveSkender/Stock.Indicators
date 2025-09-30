namespace StreamHubs;

[TestClass]
public class BollingerBandsStreamHub : StreamHubTestBase
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
        BollingerBandsHub<Quote> observer = provider.ToBollingerBands(20, 2);

        // fetch initial results (early)
        IReadOnlyList<BollingerBandsResult> streamList = observer.Results;

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
        IReadOnlyList<BollingerBandsResult> seriesList = quotesList.ToBollingerBands(20, 2);

        // assert, should equal series
        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(
            seriesList,
            options => options.WithStrictOrdering());

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        QuoteHub<Quote> provider = new();
        provider.Add(Quotes);
        BollingerBandsHub<Quote> observer = provider.ToBollingerBands(20, 2);

        observer.ToString().Should().Be("BB(20,2)");
    }

    [TestMethod]
    public void ChainProvider()
    {
        // arrange
        int lookbackPeriods = 20;
        double standardDeviations = 2;
        int smaPeriods = 10;

        List<Quote> quotesList = Quotes.ToList();
        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // initialize observer - chain SMA to Bollinger Bands
        SmaHub<BollingerBandsResult> observer = provider
            .ToBollingerBands(lookbackPeriods, standardDeviations)
            .ToSma(smaPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
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
        provider.EndTransmission();
    }

    [TestMethod]
    public void PrefilledProviderRebuilds()
    {
        QuoteHub<Quote> provider = new();
        List<Quote> quotes = Quotes.Take(25).ToList();

        for (int i = 0; i < 5; i++)
        {
            provider.Add(quotes[i]);
        }

        BollingerBandsHub<Quote> observer = provider.ToBollingerBands(5, 2);

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
            provider.Add(quotes[i]);
        }

        observer.Results.Should().BeEquivalentTo(
            quotes.ToBollingerBands(5, 2),
            options => options.WithStrictOrdering());

        observer.Unsubscribe();
        provider.EndTransmission();
    }
}
