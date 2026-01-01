namespace StreamHubs;

[TestClass]
public class PivotsHubTests : StreamHubTestBase, ITestQuoteObserver
{
    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider (batch)
        quoteHub.Add(Quotes.Take(20));

        // initialize observer
        PivotsHub observer = quoteHub
            .ToPivotsHub();

        observer.Results.Should().HaveCount(20);

        // fetch initial results (early)
        IReadOnlyList<PivotsResult> actuals
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
        quoteHub.Insert(Quotes[80]);  // rebuilds from insertion point

        // delete
        quoteHub.Remove(Quotes[removeAtIndex]);

        // Ensure all pivots are calculated with full context
        observer.Rebuild(0);

        // time-series, for comparison
        IReadOnlyList<PivotsResult> expected = RevisedQuotes.ToPivots();

        // assert, should equal series
        actuals.Should().HaveCount(quotesCount - 1);
        actuals.IsExactly(expected);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactlyClose()
    {
        // simple test, just to check Close variant

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        PivotsHub observer = quoteHub
            .ToPivotsHub(endType: EndType.Close);

        // add quotes to quoteHub
        quoteHub.Add(Quotes);

        // Trigger rebuild to calculate all pivots with complete future context
        observer.Rebuild(0);

        // stream results
        IReadOnlyList<PivotsResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<PivotsResult> seriesList
           = Quotes
            .ToPivots(endType: EndType.Close);

        // assert, should equal series
        streamList.Should().HaveCount(Quotes.Count);
        streamList.IsExactly(seriesList);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactlyDifferentSpans()
    {
        // test with different left and right spans and maxTrendPeriods

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer with different parameters
        PivotsHub observer = quoteHub
            .ToPivotsHub(leftSpan: 3, rightSpan: 4, maxTrendPeriods: 15);

        // add quotes to quoteHub
        quoteHub.Add(Quotes);

        // Trigger rebuild to calculate all pivots with complete future context
        observer.Rebuild(0);

        // stream results
        IReadOnlyList<PivotsResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<PivotsResult> seriesList
           = Quotes
            .ToPivots(leftSpan: 3, rightSpan: 4, maxTrendPeriods: 15);

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
        PivotsHub hub1 = new(new QuoteHub(), 2, 2, 20, EndType.HighLow);
        hub1.ToString().Should().Be("PIVOTS(2,2,20,HIGHLOW)");

        PivotsHub hub2 = new(new QuoteHub(), 3, 4, 15, EndType.Close);
        hub2.ToString().Should().Be("PIVOTS(3,4,15,CLOSE)");
    }
}
