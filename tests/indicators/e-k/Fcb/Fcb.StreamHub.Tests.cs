namespace StreamHub;

[TestClass]
public class Fcb : StreamHubTestBase, ITestQuoteObserver
{
    [TestMethod]
    public void QuoteObserver()
    {
        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider (batch)
        quoteHub.Add(quotesList.Take(20));

        // initialize observer
        FcbHub observer = quoteHub.ToFcbHub();

        observer.Results.Should().HaveCount(20);

        // fetch initial results (early)
        IReadOnlyList<FcbResult> streamList
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
        quoteHub.Insert(quotesList[30]);
        quoteHub.Insert(quotesList[80]);

        // delete
        quoteHub.Remove(quotesList[400]);
        quotesList.RemoveAt(400);

        // time-series, for comparison
        IReadOnlyList<FcbResult> seriesList
           = quotesList.ToFcb();

        // assert, should equal series
        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList, options => options.WithStrictOrdering());

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void QuoteObserverWithCustomWindowSpan()
    {
        // simple test with custom window span

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer with windowSpan = 3
        FcbHub observer = quoteHub.ToFcbHub(windowSpan: 3);

        // add quotes to quoteHub
        quoteHub.Add(Quotes);

        // stream results
        IReadOnlyList<FcbResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<FcbResult> seriesList
           = Quotes.ToFcb(windowSpan: 3);

        // assert, should equal series
        streamList.Should().HaveCount(Quotes.Count);
        streamList.Should().BeEquivalentTo(seriesList, options => options.WithStrictOrdering());

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        FcbHub hub = new(new QuoteHub(), 2);
        hub.ToString().Should().Be("FCB(2)");
    }
}
