namespace StreamHub;

[TestClass]
public class RollingPivots : StreamHubTestBase, ITestQuoteObserver
{
    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider (batch)
        quoteHub.Add(quotesList.Take(25));

        // initialize observer
        RollingPivotsHub observer = quoteHub
            .ToRollingPivotsHub(20, 0, PivotPointType.Standard);

        observer.Results.Should().HaveCount(25);

        // fetch initial results (early)
        IReadOnlyList<RollingPivotsResult> streamList
            = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 25; i < length; i++)
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
        IReadOnlyList<RollingPivotsResult> seriesList
           = quotesList
            .ToRollingPivots(20, 0, PivotPointType.Standard);

        // assert, should equal series
        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList, static options => options.WithStrictOrdering());

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        RollingPivotsHub hub = new(new QuoteHub(), 20, 0, PivotPointType.Standard);
        hub.ToString().Should().Be("ROLLING-PIVOTS(20,0,Standard)");
    }
}
