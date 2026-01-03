namespace StreamHubs;

[TestClass]
public class BopHubTests : StreamHubTestBase, ITestQuoteObserver, ITestChainProvider
{
    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        // setup quote provider
        QuoteHub quoteHub = new();

        // prefill quotes to provider
        for (int i = 0; i < 40; i++)
        {
            quoteHub.Add(Quotes[i]);
        }

        // initialize observer
        BopHub bopHub = quoteHub
            .ToBopHub(14);

        // fetch initial results (early)
        IReadOnlyList<BopResult> actuals
            = bopHub.Results;

        // emulate adding quotes to provider
        for (int i = 40; i < quotesCount; i++)
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
        IReadOnlyList<BopResult> expected = RevisedQuotes.ToBop(14);

        // assert
        actuals.Should().HaveCount(quotesCount - 1);
        actuals.IsExactly(expected);

        bopHub.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void Results_AreAlwaysBounded()
    {
        IReadOnlyList<BopResult> sut = Quotes.ToBopHub(14).Results;
        sut.IsBetween(static x => x.Bop, -1, 1);
    }

    [TestMethod]
    public void ChainObserver()
    {
        // BOP requires IQuote input (OHLC data), so we can't chain from EMA
        // Instead, test chaining from a quote converter that produces quotes
        const int smoothPeriods = 14;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub quoteHub = new();

        // initialize observer - BOP directly from quotes
        BopHub bopHub = quoteHub
            .ToBopHub(smoothPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // final results
        IReadOnlyList<BopResult> streamList
            = bopHub.Results;

        // time-series, for comparison
        IReadOnlyList<BopResult> seriesList
           = quotesList
            .ToBop(smoothPeriods);

        // assert
        streamList.Should().HaveCount(length);
        streamList.IsExactly(seriesList);

        bopHub.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider_MatchesSeriesExactly()
    {
        const int smoothPeriods = 14;
        const int emaPeriods = 12;

        // setup quote provider
        QuoteHub quoteHub = new();

        // initialize observer
        EmaHub emaHub = quoteHub
            .ToBopHub(smoothPeriods)
            .ToEmaHub(emaPeriods);

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
        IReadOnlyList<EmaResult> sut = emaHub.Results;

        // time-series, for comparison (revised)
        IReadOnlyList<EmaResult> expected = RevisedQuotes
            .ToBop(smoothPeriods)
            .ToEma(emaPeriods);

        // assert
        sut.Should().HaveCount(quotesCount - 1);
        sut.IsExactly(expected);

        emaHub.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        BopHub hub = new(new QuoteHub(), 14);
        hub.ToString().Should().Be("BOP(14)");
    }
}
