namespace StreamHub;

[TestClass]
public class AtrStop : StreamHubTestBase, ITestQuoteObserver
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
        AtrStopHub observer = quoteHub
            .ToAtrStopHub();

        observer.Results.Should().HaveCount(20);

        // fetch initial results (early)
        IReadOnlyList<AtrStopResult> streamList
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

        // late arrivals
        quoteHub.Insert(quotesList[30]);  // rebuilds complete series
        quoteHub.Insert(quotesList[80]);  // rebuilds from last reversal

        // delete
        quoteHub.Remove(quotesList[400]);
        quotesList.RemoveAt(400);

        // time-series, for comparison
        IReadOnlyList<AtrStopResult> seriesList
           = quotesList
            .ToAtrStop();

        // assert, should equal series
        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList);

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
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        AtrStopHub hub = new(new QuoteHub(), 14, 3, EndType.Close);
        hub.ToString().Should().Be("ATR-STOP(14,3,CLOSE)");
    }
}
