namespace StreamHub;

[TestClass]
public class ChaikinOscHubTests : StreamHubTestBase, ITestQuoteObserver, ITestChainProvider
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
        ChaikinOscHub chaikinOscHub = quoteHub
            .ToChaikinOscHub(3, 10);

        // fetch initial results (early)
        IReadOnlyList<ChaikinOscResult> streamList
            = chaikinOscHub.Results;

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
        IReadOnlyList<ChaikinOscResult> seriesList = quotesList.ToChaikinOsc(3, 10);

        // assert, should equal series (with tolerance for floating point precision)
        streamList.Should().HaveCount(length - 1);
        streamList.AssertEquals(seriesList, Precision.LastDigit);

        chaikinOscHub.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver()
    {
        // ChaikinOsc requires IQuote input, so similar to BOP pattern
        const int fastPeriods = 3;
        const int slowPeriods = 10;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub quoteHub = new();

        // initialize observer
        ChaikinOscHub chaikinOscHub = quoteHub
            .ToChaikinOscHub(fastPeriods, slowPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // final results
        IReadOnlyList<ChaikinOscResult> streamList
            = chaikinOscHub.Results;

        // time-series, for comparison
        IReadOnlyList<ChaikinOscResult> seriesList
           = quotesList
            .ToChaikinOsc(fastPeriods, slowPeriods);

        // assert, should equal series (with tolerance for floating point precision)
        streamList.Should().HaveCount(length);
        streamList.AssertEquals(seriesList, Precision.LastDigit);

        chaikinOscHub.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider_MatchesSeriesExactly()
    {
        const int fastPeriods = 3;
        const int slowPeriods = 10;
        const int emaPeriods = 12;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub quoteHub = new();

        // initialize observer
        EmaHub emaHub = quoteHub
            .ToChaikinOscHub(fastPeriods, slowPeriods)
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
            .ToChaikinOsc(fastPeriods, slowPeriods)
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
        ChaikinOscHub chaikinOscHub = quoteHub.ToChaikinOscHub(3, 10);

        chaikinOscHub.ToString().Should().Be("CHAIKIN_OSC(3,10)");

        chaikinOscHub.Unsubscribe();
        quoteHub.EndTransmission();
    }
}
