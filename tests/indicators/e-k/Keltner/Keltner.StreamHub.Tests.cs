namespace StreamHubs;

[TestClass]
public class KeltnerHubTests : StreamHubTestBase, ITestQuoteObserver
{
    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        for (int i = 0; i < 20; i++)
        {
            quoteHub.Add(Quotes[i]);
        }

        // initialize observer
        KeltnerHub observer = quoteHub.ToKeltnerHub(20, 2, 10);

        // fetch initial results (early)
        IReadOnlyList<KeltnerResult> actuals = observer.Results;

        // emulate adding quotes to provider hub
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
        IReadOnlyList<KeltnerResult> expected = RevisedQuotes.ToKeltner(20, 2, 10);

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
        QuoteHub quoteHub = new();
        quoteHub.Add(Quotes);
        KeltnerHub observer = quoteHub.ToKeltnerHub(20, 2, 10);

        observer.ToString().Should().Be("KELTNER(20,2,10)");
    }

    [TestMethod]
    public void PrefilledProviderRebuilds()
    {
        QuoteHub quoteHub = new();
        List<Quote> quotes = Quotes.Take(25).ToList();

        for (int i = 0; i < 5; i++)
        {
            quoteHub.Add(quotes[i]);
        }

        KeltnerHub observer = quoteHub.ToKeltnerHub(5, 1, 3);

        IReadOnlyList<KeltnerResult> initialResults = observer.Results;
        IReadOnlyList<KeltnerResult> expectedInitial = quotes
            .Take(5)
            .ToList()
            .ToKeltner(5, 1, 3);

        initialResults.IsExactly(expectedInitial);

        for (int i = 5; i < quotes.Count; i++)
        {
            quoteHub.Add(quotes[i]);
        }

        observer.Results.IsExactly(quotes.ToKeltner(5, 1, 3));

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }
}
