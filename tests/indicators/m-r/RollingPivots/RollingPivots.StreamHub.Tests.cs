namespace StreamHubs;

[TestClass]
public class RollingPivots : StreamHubTestBase, ITestQuoteObserver
{
    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider (batch)
        quoteHub.Add(Quotes.Take(25));

        // initialize observer
        RollingPivotsHub observer = quoteHub
            .ToRollingPivotsHub(20, 0, PivotPointType.Standard);

        observer.Results.Should().HaveCount(25);

        // fetch initial results (early)
        IReadOnlyList<RollingPivotsResult> actuals
            = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 25; i < quotesCount; i++)
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
        IReadOnlyList<RollingPivotsResult> expected = RevisedQuotes.ToRollingPivots(20, 0, PivotPointType.Standard);

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
        RollingPivotsHub hub = new(new QuoteHub(), 20, 0, PivotPointType.Standard);
        hub.ToString().Should().Be("ROLLING-PIVOTS(20,0,Standard)");
    }
}
