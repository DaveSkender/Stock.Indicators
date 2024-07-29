namespace StreamHub;

// QUOTEHUB

[TestClass]
public class QuoteHub : StreamHubTestBase, ITestChainProvider
{
    [TestMethod]
    public override void QuoteObserver()
    {
        // tests quote redistribution

        List<Quote> quotesList = Quotes
            .ToSortedList();

        int length = Quotes.Count;

        // add base quotes (batch)
        QuoteHub<Quote> provider = new();

        provider.Add(quotesList.Take(200));

        // add incremental quotes
        for (int i = 200; i < length; i++)
        {
            Quote q = quotesList[i];
            provider.Add(q);
        }

        QuoteHub<Quote> observer
            = provider.ToQuote();

        // close observations
        provider.EndTransmission();

        // assert same as original
        observer.Cache.Should().HaveCount(length);
        observer.Cache.Should().BeEquivalentTo(provider.Cache);
    }

    [TestMethod]
    public void ChainProvider()
    {
        int smaPeriods = 8;

        List<Quote> quotesList = Quotes
            .ToSortedList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // initialize observer
        SmaHub<Quote> observer
           = provider
            .ToSma(smaPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        // delete
        provider.RemoveAt(400);
        quotesList.RemoveAt(400);

        // final results
        IReadOnlyList<SmaResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<SmaResult> seriesList
           = quotesList
            .GetSma(smaPeriods);

        // assert, should equal series
        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList);

        // cleanup
        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        QuoteHub<Quote> hub = new();

        hub.ToString().Should().Be("QUOTES<Quote>: 0 items");

        hub.Add(Quotes[0]);
        hub.Add(Quotes[1]);

        hub.ToString().Should().Be("QUOTES<Quote>: 2 items");
    }

    [TestMethod]
    public void AddQuote()
    {
        // covers both single and batch add

        List<Quote> quotesList = Quotes
            .ToSortedList();

        int length = Quotes.Count;

        // add base quotes (batch)
        QuoteHub<Quote> provider = new();

        provider.Add(quotesList.Take(200));

        // add incremental quotes
        for (int i = 200; i < length; i++)
        {
            Quote q = quotesList[i];
            provider.Add(q);
        }

        // assert same as original
        for (int i = 0; i < length; i++)
        {
            Quote o = quotesList[i];
            Quote q = provider.Cache[i];

            Assert.AreEqual(o, q);  // same ref
        }

        // confirm public interfaces
        Assert.AreEqual(provider.Cache.Count, provider.Quotes.Count);

        // close observations
        provider.EndTransmission();
    }
}
