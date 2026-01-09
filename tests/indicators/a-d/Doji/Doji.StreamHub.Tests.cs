namespace StreamHubs;

[TestClass]
public class DojiHubTests : StreamHubTestBase, ITestQuoteObserver
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
        DojiHub dojiHub = quoteHub
            .ToDojiHub(0.1);

        // fetch initial results (early)
        IReadOnlyList<CandleResult> actuals
            = dojiHub.Results;

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
        IReadOnlyList<CandleResult> expected = RevisedQuotes.ToDoji(0.1);

        // assert, should equal series
        actuals.Should().HaveCount(quotesCount - 1);
        actuals.IsExactly(expected);

        dojiHub.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        DojiHub hub = new(new QuoteHub(), 0.1);
        hub.ToString().Should().Be("DOJI(0.1)");
    }
}
