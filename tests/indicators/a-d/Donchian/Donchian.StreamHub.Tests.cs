namespace StreamHubs;

[TestClass]
public class Donchian : StreamHubTestBase, ITestQuoteObserver
{
    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider (batch)
        quoteHub.Add(Quotes.Take(25));

        // initialize observer
        DonchianHub observer = quoteHub
            .ToDonchianHub(20);

        observer.Results.Should().HaveCount(25);

        // fetch initial results (early)
        IReadOnlyList<DonchianResult> actuals
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
        IReadOnlyList<DonchianResult> expected = RevisedQuotes.ToDonchian(20);

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
        DonchianHub hub = new(new QuoteHub(), 20);
        hub.ToString().Should().Be("DONCHIAN(20)");
    }
}
