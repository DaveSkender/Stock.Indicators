namespace StreamHubs;

[TestClass]
public class VolatilityStopHubTests : StreamHubTestBase, ITestQuoteObserver
{
    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider (batch)
        quoteHub.Add(Quotes.Take(20));

        // initialize observer
        VolatilityStopHub observer = quoteHub.ToVolatilityStopHub();

        observer.Results.Should().HaveCount(20);

        // fetch initial results (early)
        IReadOnlyList<VolatilityStopResult> sut = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 20; i < quotesCount; i++)
        {
            // skip one (add later)
            if (i is 30 or 80) { continue; }

            Quote q = Quotes[i];
            quoteHub.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105) { quoteHub.Add(q); }
        }

        // late arrivals (test Insert functionality)
        quoteHub.Insert(Quotes[30]);  // rebuilds complete series
        quoteHub.Insert(Quotes[80]);  // rebuilds from insertion point

        IReadOnlyList<VolatilityStopResult> expectedOriginal = Quotes.ToVolatilityStop();
        sut.IsExactly(expectedOriginal);

        // delete (test Remove functionality), should equal series (revised)
        quoteHub.Remove(Quotes[removeAtIndex]);

        IReadOnlyList<VolatilityStopResult> expectedRevised = RevisedQuotes.ToVolatilityStop();
        sut.IsExactly(expectedRevised);
        sut.Should().HaveCount(quotesCount - 1);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactlyCustomParameters()
    {
        // simple test with custom parameters

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer with custom parameters
        VolatilityStopHub observer = quoteHub
            .ToVolatilityStopHub(lookbackPeriods: 14, multiplier: 2.5);

        // add quotes to quoteHub
        quoteHub.Add(Quotes);

        // stream results
        IReadOnlyList<VolatilityStopResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<VolatilityStopResult> seriesList
           = Quotes
            .ToVolatilityStop(lookbackPeriods: 14, multiplier: 2.5);

        // assert, should equal series
        streamList.Should().HaveCount(Quotes.Count);
        streamList.IsExactly(seriesList);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        VolatilityStopHub hub = new(new QuoteHub(), 7, 3);
        hub.ToString().Should().Be("VOLATILITY-STOP(7,3)");
    }
}
