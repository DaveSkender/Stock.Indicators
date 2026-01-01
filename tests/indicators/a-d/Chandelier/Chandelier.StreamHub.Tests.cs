namespace StreamHubs;

[TestClass]
public class Chandelier : StreamHubTestBase, ITestQuoteObserver
{
    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider (batch)
        quoteHub.Add(Quotes.Take(25));

        // initialize observer
        ChandelierHub observer = quoteHub
            .ToChandelierHub(22, 3);

        observer.Results.Should().HaveCount(25);

        // fetch initial results (early)
        IReadOnlyList<ChandelierResult> actuals
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
        IReadOnlyList<ChandelierResult> expected = RevisedQuotes.ToChandelier(22, 3);

        // assert, should equal series
        actuals.Should().HaveCount(quotesCount - 1);
        actuals.IsExactly(expected);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactlyShort()
    {
        // simple test, just to check Short variant

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        ChandelierHub observer = quoteHub
            .ToChandelierHub(22, 3, Direction.Short);

        // add quotes to quoteHub
        quoteHub.Add(Quotes);

        // stream results
        IReadOnlyList<ChandelierResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<ChandelierResult> seriesList
           = Quotes
            .ToChandelier(22, 3, Direction.Short);

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
        ChandelierHub hub = new(new QuoteHub(), 22, 3, Direction.Long);
        hub.ToString().Should().Be("CHEXIT(22,3,LONG)");
    }
}
