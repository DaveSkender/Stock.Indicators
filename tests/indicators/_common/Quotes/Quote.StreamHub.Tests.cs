namespace StreamHub;

// QUOTEHUB

[TestClass]
public class QuoteHubTests : StreamHubTestBase, ITestQuoteObserver, ITestChainProvider
{
    [TestMethod]
    public void QuoteObserver()
    {
        // tests quote redistribution

        List<Quote> quotesList = Quotes.ToList();

        int length = Quotes.Count;

        // add base quotes (batch)
        QuoteHub quoteHub = new();

        quoteHub.Add(quotesList.Take(200));

        // add incremental quotes
        for (int i = 200; i < length; i++)
        {
            Quote q = quotesList[i];
            quoteHub.Add(q);
        }

        QuoteHub observer
            = quoteHub.ToQuoteHub();

        // close observations
        quoteHub.EndTransmission();

        // assert same as original
        observer.Cache.Should().HaveCount(length);
        observer.Cache.Should().BeEquivalentTo(quoteHub.Cache);
    }

    [TestMethod]
    public void ChainProvider()
    {
        const int smaPeriods = 8;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        SmaHub observer
           = quoteHub
            .ToSmaHub(smaPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // delete
        quoteHub.RemoveAt(400);
        quotesList.RemoveAt(400);

        // final results
        IReadOnlyList<SmaResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<SmaResult> seriesList
           = quotesList
            .ToSma(smaPeriods);

        // assert, should equal series
        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        QuoteHub hub = new();

        hub.ToString().Should().Be("QUOTES<Quote>: 0 items");

        hub.Add(Quotes[0]);
        hub.Add(Quotes[1]);

        hub.ToString().Should().Be("QUOTES<Quote>: 2 items");
    }

    [TestMethod]
    public void AddQuote()
    {
        // covers both single and batch add

        List<Quote> quotesList = Quotes.ToList();

        int length = Quotes.Count;

        // add base quotes (batch)
        QuoteHub quoteHub = new();

        quoteHub.Add(quotesList.Take(200));

        // add incremental quotes
        for (int i = 200; i < length; i++)
        {
            Quote q = quotesList[i];
            quoteHub.Add(q);
        }

        // assert same as original
        for (int i = 0; i < length; i++)
        {
            Quote o = quotesList[i];
            IQuote q = quoteHub.Cache[i];

            Assert.AreEqual(o, q);  // same ref
        }

        // confirm public interfaces
        Assert.HasCount(quoteHub.Cache.Count, quoteHub.Quotes);

        // close observations
        quoteHub.EndTransmission();
    }
}
