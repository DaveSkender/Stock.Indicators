namespace StreamHub;

[TestClass]
public class VolatilityStop : StreamHubTestBase, ITestQuoteObserver
{
    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider (batch)
        quoteHub.Add(quotesList.Take(20));

        // initialize observer
        VolatilityStopHub observer = quoteHub
            .ToVolatilityStopHub();

        observer.Results.Should().HaveCount(20);

        // fetch initial results (early)
        IReadOnlyList<VolatilityStopResult> streamList
            = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 20; i < length; i++)
        {
            // skip one (add later)
            if (i is 30 or 80)
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

        // late arrivals (test Insert functionality)
        quoteHub.Insert(quotesList[30]);  // rebuilds complete series
        quoteHub.Insert(quotesList[80]);  // rebuilds from insertion point

        // delete (test Remove functionality)
        quoteHub.Remove(quotesList[removeAtIndex]);
        quotesList.RemoveAt(removeAtIndex);

        // time-series, for comparison
        IReadOnlyList<VolatilityStopResult> seriesList
           = quotesList
            .ToVolatilityStop();

        // assert, should equal series
        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList, options => options.WithStrictOrdering());

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
        streamList.Should().BeEquivalentTo(seriesList, options => options.WithStrictOrdering());

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        VolatilityStopHub hub = new(new QuoteHub(), 7, 3);
        hub.ToString().Should().Be("VOLATILITY-STOP(7,3)");
    }
}
