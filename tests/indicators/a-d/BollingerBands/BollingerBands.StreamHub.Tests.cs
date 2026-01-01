namespace StreamHubs;

[TestClass]
public class BollingerBandsHubTests : StreamHubTestBase, ITestQuoteObserver, ITestChainProvider
{
    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        for (int i = 0; i < 20; i++)
        {
            quoteHub.Add(Quotes[i]);
        }

        // initialize observer
        BollingerBandsHub observer = quoteHub.ToBollingerBandsHub(20, 2);

        // fetch initial results (early)
        IReadOnlyList<BollingerBandsResult> actuals = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 20; i < quotesCount; i++)
        {
            // skip one (add later)
            if (i == 80)
            {
                continue;
            }

            Quote q = Quotes[i];
            quoteHub.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105)
            {
                quoteHub.Add(q);
            }
        }

        // late arrival
        quoteHub.Insert(Quotes[80]);

        // delete
        quoteHub.Remove(Quotes[removeAtIndex]);

        // time-series, for comparison
        IReadOnlyList<BollingerBandsResult> expected = RevisedQuotes.ToBollingerBands(20, 2);

        // assert, should equal series
        actuals.Should().HaveCount(quotesCount - 1);
        actuals.IsExactly(expected);

        // cleanup
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

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer - chain SMA to Bollinger Bands
        SmaHub observer = quoteHub
            .ToBollingerBandsHub(lookbackPeriods, standardDeviations)
            .ToSmaHub(smaPeriods);

        // emulate quote stream
        for (int i = 0; i < quotesCount; i++)
        {
            if (i == 80) { continue; }  // Skip for late arrival

            Quote q = Quotes[i];
            quoteHub.Add(q);

            if (i is > 100 and < 105) { quoteHub.Add(q); }  // Duplicate quotes
        }

        quoteHub.Insert(Quotes[80]);  // Late arrival
        quoteHub.Remove(Quotes[removeAtIndex]);  // Remove

        // final results
        IReadOnlyList<SmaResult> sut = observer.Results;

        // time-series, for comparison (revised)
        IReadOnlyList<SmaResult> expected = RevisedQuotes
            .ToBollingerBands(lookbackPeriods, standardDeviations)
            .ToSma(smaPeriods);

        // assert, should equal series
        sut.Should().HaveCount(quotesCount - 1);
        sut.IsExactly(expected);

        // cleanup
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

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }
}
