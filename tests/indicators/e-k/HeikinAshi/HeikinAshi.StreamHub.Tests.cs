namespace StreamHubs;

[TestClass]
public class HeikinAshiHubTests : StreamHubTestBase, ITestQuoteObserver, ITestChainProvider
{
    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        // setup quote provider
        QuoteHub quoteHub = new();

        // prefill quotes to provider
        for (int i = 0; i < 20; i++)
        {
            quoteHub.Add(Quotes[i]);
        }

        // initialize observer
        HeikinAshiHub heikinAshiHub = quoteHub
            .ToHeikinAshiHub();

        // fetch initial results (early)
        IReadOnlyList<HeikinAshiResult> actuals
            = heikinAshiHub.Results;

        // emulate adding quotes to provider
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
        IReadOnlyList<HeikinAshiResult> expected = RevisedQuotes.ToHeikinAshi();

        // assert, should equal series
        actuals.Should().HaveCount(quotesCount - 1);
        actuals.IsExactly(expected);

        heikinAshiHub.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider_MatchesSeriesExactly()
    {
        const int smaPeriods = 50;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        SmaHub observer = quoteHub
            .ToHeikinAshiHub()
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
            .ToHeikinAshi()
            .ToSma(smaPeriods);

        // assert, should equal series
        sut.Should().HaveCount(quotesCount - 1);
        sut.IsExactly(expected);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        HeikinAshiHub hub = new(new QuoteHub());
        hub.ToString().Should().Be("HEIKINASHI");
    }
}
