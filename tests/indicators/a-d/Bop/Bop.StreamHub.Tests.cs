namespace StreamHub;

[TestClass]
public class BopHubTests : StreamHubTestBase, ITestQuoteObserver, ITestChainProvider
{
    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub quoteHub = new();

        // prefill quotes to provider
        for (int i = 0; i < 40; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // initialize observer
        BopHub bopHub = quoteHub
            .ToBopHub(14);

        // fetch initial results (early)
        IReadOnlyList<BopResult> streamList
            = bopHub.Results;

        // emulate adding quotes to provider
        for (int i = 40; i < length; i++)
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
        IReadOnlyList<BopResult> seriesList = quotesList.ToBop(14);

        // assert, should equal series (with tolerance for floating point precision)
        streamList.Should().HaveCount(length - 1);
        streamList.AssertEquals(seriesList, Precision.LastDigit);

        bopHub.Unsubscribe();
        quoteHub.EndTransmission();
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

        // assert, should equal series (with tolerance for floating point precision)
        streamList.Should().HaveCount(length);
        streamList.AssertEquals(seriesList, Precision.LastDigit);

        bopHub.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider_MatchesSeriesExactly()
    {
        const int smoothPeriods = 14;
        const int emaPeriods = 12;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub quoteHub = new();

        // initialize observer
        EmaHub emaHub = quoteHub
            .ToBopHub(smoothPeriods)
            .ToEmaHub(emaPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // final results
        IReadOnlyList<EmaResult> streamList
            = emaHub.Results;

        // time-series, for comparison
        IReadOnlyList<EmaResult> seriesList
           = quotesList
            .ToBop(smoothPeriods)
            .ToEma(emaPeriods);

        // assert, should equal series (with tolerance for floating point precision)
        streamList.Should().HaveCount(length);
        streamList.AssertEquals(seriesList, Precision.LastDigit);

        emaHub.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        QuoteHub quoteHub = new();
        BopHub bopHub = quoteHub.ToBopHub(14);

        bopHub.ToString().Should().Be("BOP(14)");

        bopHub.Unsubscribe();
        quoteHub.EndTransmission();
    }
}
