namespace StreamHubs;

[TestClass]
public class ChaikinOscHubTests : StreamHubTestBase, ITestQuoteObserver, ITestChainProvider
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
        ChaikinOscHub chaikinOscHub = quoteHub
            .ToChaikinOscHub(3, 10);

        // fetch initial results (early)
        IReadOnlyList<ChaikinOscResult> actuals
            = chaikinOscHub.Results;

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
        IReadOnlyList<ChaikinOscResult> expected = RevisedQuotes.ToChaikinOsc(3, 10);

        // assert
        actuals.Should().HaveCount(quotesCount - 1);
        actuals.IsExactly(expected);

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

        // assert
        streamList.Should().HaveCount(length);
        streamList.IsExactly(seriesList);

        chaikinOscHub.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider_MatchesSeriesExactly()
    {
        const int fastPeriods = 3;
        const int slowPeriods = 10;
        const int emaPeriods = 12;

        // setup quote provider
        QuoteHub quoteHub = new();

        // initialize observer
        EmaHub emaHub = quoteHub
            .ToChaikinOscHub(fastPeriods, slowPeriods)
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
            .ToChaikinOsc(fastPeriods, slowPeriods)
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
        ChaikinOscHub hub = new(new QuoteHub(), 3, 10);
        hub.ToString().Should().Be("CHAIKIN_OSC(3,10)");
    }
}
