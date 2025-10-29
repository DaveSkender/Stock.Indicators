namespace StreamHubs;

[TestClass]
public class StarcBandsStreamHub : StreamHubTestBase, ITestQuoteObserver
{
    [TestMethod]
    public void QuoteObserver()
    {
        List<Quote> quotesList = Quotes.ToList();
        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        for (int i = 0; i < 20; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // initialize observer
        StarcBandsHub observer = quoteHub.ToStarcBandsHub(5, 2, 10);

        // fetch initial results (early)
        IReadOnlyList<StarcBandsResult> streamList = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 20; i < length; i++)
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
        IReadOnlyList<StarcBandsResult> seriesList = quotesList.ToStarcBands(5, 2, 10);

        // assert, should equal series
        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(
            seriesList,
            static options => options.WithStrictOrdering());

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(Quotes);
        StarcBandsHub observer = quoteHub.ToStarcBandsHub(5, 2, 10);

        observer.ToString().Should().Be("STARC(5,2,10)");
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

        initialResults.Should().BeEquivalentTo(
            expectedInitial,
            static options => options.WithStrictOrdering());

        for (int i = 5; i < quotes.Count; i++)
        {
            quoteHub.Add(quotes[i]);
        }

        observer.Results.Should().BeEquivalentTo(
            quotes.ToStarcBands(5, 2, 3),
            static options => options.WithStrictOrdering());

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }
}
