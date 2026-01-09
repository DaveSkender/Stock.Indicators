namespace StreamHubs;

[TestClass]
public class QuoteHubTests : StreamHubTestBase, ITestQuoteObserver, ITestChainProvider
{
    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        // setup quote provider hub
        QuoteHub provider = new();

        // prefill quotes at provider
        provider.Add(Quotes.Take(20));

        // initialize observer
        QuoteHub observer = provider.ToQuoteHub();

        // fetch initial results (early)
        IReadOnlyList<IQuote> sut = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 20; i < quotesCount; i++)
        {
            // skip one (add later)
            if (i == 80) { continue; }

            Quote q = Quotes[i];
            provider.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105) { provider.Add(q); }
        }

        // late arrival, should equal series
        provider.Insert(Quotes[80]);

        sut.IsExactly(Quotes);

        // delete, should equal series (revised)
        provider.Remove(Quotes[removeAtIndex]);

        sut.IsExactly(RevisedQuotes);
        sut.Should().HaveCount(quotesCount - 1);

        // cleanup
        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider_MatchesSeriesExactly()
    {
        const int emaPeriods = 14;

        // setup quote provider hub
        QuoteHub provider = new();

        // initialize observer
        EmaHub observer = provider
            .ToQuoteHub()
            .ToEmaHub(emaPeriods);

        // emulate quote stream with comprehensive provider history testing
        for (int i = 0; i < quotesCount; i++)
        {
            if (i == 80) { continue; }  // Skip one

            Quote q = Quotes[i];
            provider.Add(q);

            if (i is > 100 and < 105) { provider.Add(q); }  // Duplicates
        }

        provider.Insert(Quotes[80]);  // Late arrival
        provider.Remove(Quotes[removeAtIndex]);  // Delete

        // final results
        IReadOnlyList<EmaResult> sut = observer.Results;

        // time-series, for comparison
        IReadOnlyList<EmaResult> expected = RevisedQuotes
            .ToEma(emaPeriods);

        // assert, should equal series
        sut.Should().HaveCount(quotesCount - 1);
        sut.IsExactly(expected);

        // cleanup
        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        QuoteHub hub = new();

        hub.ToString().Should().Be("QUOTES<IQuote>: 0 items");

        hub.Add(Quotes[0]);
        hub.Add(Quotes[1]);

        hub.ToString().Should().Be("QUOTES<IQuote>: 2 items");
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

            q.Should().Be(o);  // same ref
        }

        // confirm public interfaces
        quoteHub.Quotes.Should().HaveCount(quoteHub.Cache.Count);

        // close observations
        quoteHub.EndTransmission();
    }
}
