namespace StreamHubs;

[TestClass]
public class StarcBandsHubTests : StreamHubTestBase, ITestQuoteObserver
{
    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        quoteHub.Add(Quotes.Take(20));

        // initialize observer
        StarcBandsHub observer = quoteHub.ToStarcBandsHub(5, 2, 10);

        // fetch initial results (early)
        IReadOnlyList<StarcBandsResult> sut = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 20; i < quotesCount; i++)
        {
            // skip one (add later)
            if (i == 80) { continue; }

            Quote q = Quotes[i];
            quoteHub.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105) { quoteHub.Add(q); }
        }

        // late arrival, should equal series
        quoteHub.Insert(Quotes[80]);

        IReadOnlyList<StarcBandsResult> expectedOriginal = Quotes.ToStarcBands(5, 2, 10);
        sut.IsExactly(expectedOriginal);

        // delete, should equal series (revised)
        quoteHub.Remove(Quotes[removeAtIndex]);

        IReadOnlyList<StarcBandsResult> expectedRevised = RevisedQuotes.ToStarcBands(5, 2, 10);
        sut.IsExactly(expectedRevised);
        sut.Should().HaveCount(quotesCount - 1);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(Quotes);
        StarcBandsHub observer = quoteHub.ToStarcBandsHub(5, 2, 10);

        observer.ToString().Should().Be("STARCBANDS(5,2,10)");
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

        StarcBandsHub observer = quoteHub.ToStarcBandsHub(5, 2, 3);

        IReadOnlyList<StarcBandsResult> initialResults = observer.Results;
        IReadOnlyList<StarcBandsResult> expectedInitial = quotes
            .Take(5)
            .ToList()
            .ToStarcBands(5, 2, 3);

        initialResults.IsExactly(expectedInitial);

        for (int i = 5; i < quotes.Count; i++)
        {
            quoteHub.Add(quotes[i]);
        }

        observer.Results.IsExactly(quotes.ToStarcBands(5, 2, 3));

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }
}
