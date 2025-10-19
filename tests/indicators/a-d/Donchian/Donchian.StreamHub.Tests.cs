namespace StreamHub;

[TestClass]
public class Donchian : StreamHubTestBase, ITestQuoteObserver
{
    [TestMethod]
    public void QuoteObserver()
    {
        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider (batch)
        quoteHub.Add(quotesList.Take(25));

        // initialize observer
        DonchianHub observer = quoteHub
            .ToDonchianHub(20);

        observer.Results.Should().HaveCount(25);

        // fetch initial results (early)
        IReadOnlyList<DonchianResult> streamList
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
        IReadOnlyList<DonchianResult> seriesList
           = quotesList
            .ToDonchian(20);

        // assert, should equal series
        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList, options => options.WithStrictOrdering());

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        DonchianHub hub = new(new QuoteHub(), 20);
        hub.ToString().Should().Be("DONCHIAN(20)");
    }
}
