namespace StreamHubs;

[TestClass]
public class AtrStopHubTests : StreamHubTestBase, ITestQuoteObserver
{
    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider (batch)
        quoteHub.Add(Quotes.Take(20));

        // initialize observer
        AtrStopHub observer = quoteHub
            .ToAtrStopHub();

        observer.Results.Should().HaveCount(20);

        // fetch initial results (early)
        IReadOnlyList<AtrStopResult> actuals
            = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 20; i < quotesCount; i++)
        {
            // skip one (add later)
            if (i is 30 or 80)
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

        // late arrivals
        quoteHub.Insert(Quotes[30]);  // rebuilds complete series
        quoteHub.Insert(Quotes[80]);  // rebuilds from last reversal

        // delete
        quoteHub.Remove(Quotes[removeAtIndex]);

        // time-series, for comparison
        IReadOnlyList<AtrStopResult> expected = RevisedQuotes.ToAtrStop();

        // assert, should equal series
        actuals.Should().HaveCount(quotesCount - 1);
        actuals.IsExactly(expected);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactlyHighLow()
    {
        // simple test, just to check High/Low variant

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        AtrStopHub observer = quoteHub
            .ToAtrStopHub(endType: EndType.HighLow);

        // add quotes to quoteHub
        quoteHub.Add(Quotes);

        // stream results
        IReadOnlyList<AtrStopResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<AtrStopResult> seriesList
           = Quotes
            .ToAtrStop(endType: EndType.HighLow);

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
        AtrStopHub hub = new(new QuoteHub(), 14, 3, EndType.Close);
        hub.ToString().Should().Be("ATR-STOP(14,3,CLOSE)");
    }
}
