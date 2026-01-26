namespace StreamHubs;

[TestClass]
public class MamaHubTests : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    private const double fastLimit = 0.5;
    private const double slowLimit = 0.05;

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
        MamaHub observer = quoteHub
            .ToMamaHub(fastLimit, slowLimit);

        // fetch initial results (early)
        IReadOnlyList<MamaResult> actuals
            = observer.Results;

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
        quoteHub.RemoveAt(removeAtIndex);

        // time-series, for comparison
        IReadOnlyList<MamaResult> expected = RevisedQuotes.ToMama(fastLimit, slowLimit);

        // assert, should equal series
        actuals.Should().HaveCount(quotesCount - 1);
        actuals.IsExactly(expected);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void WithCachePruning_MatchesSeriesExactly()
    {
        const int maxCacheSize = 50;
        const int totalQuotes = 100;

        IReadOnlyList<Quote> quotes = Quotes.Take(totalQuotes).ToList();
        IReadOnlyList<MamaResult> expected = quotes
            .ToMama(fastLimit, slowLimit)
            .TakeLast(maxCacheSize)
            .ToList();

        // Setup with cache limit
        QuoteHub quoteHub = new() { MaxCacheSize = maxCacheSize };
        MamaHub observer = quoteHub.ToMamaHub(fastLimit, slowLimit);

        // Stream more quotes than cache can hold
        quoteHub.Add(quotes);

        // Verify cache was pruned
        quoteHub.Quotes.Should().HaveCount(maxCacheSize);
        observer.Results.Should().HaveCount(maxCacheSize);

        // Streaming results should match last N from full series (original series with front chopped off)
        // NOT recomputation on just the cached quotes (which would have different warmup)
        observer.Results.IsExactly(expected);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver_ChainedProvider_MatchesSeriesExactly()
    {
        const int smaPeriods = 8;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        MamaHub observer = quoteHub
            .ToSmaHub(smaPeriods)
            .ToMamaHub(fastLimit, slowLimit);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // final results
        IReadOnlyList<MamaResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<MamaResult> seriesList
           = quotesList
            .ToSma(smaPeriods)
            .ToMama(fastLimit, slowLimit);

        // assert, should equal series
        streamList.Should().HaveCount(length);
        streamList.IsExactly(seriesList);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider_MatchesSeriesExactly()
    {
        const int smaPeriods = 10;

        List<Quote> quotes = Quotes.ToList();

        int length = quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        SmaHub observer = quoteHub
            .ToMamaHub(fastLimit, slowLimit)
            .ToSmaHub(smaPeriods);

        // emulate adding quotes to provider hub
        for (int i = 0; i < length; i++)
        {
            // skip one (add later)
            if (i == 80)
            {
                continue;
            }

            Quote q = quotes[i];
            quoteHub.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105)
            {
                quoteHub.Add(q);
            }
        }

        // late arrival
        quoteHub.Insert(quotes[80]);

        // delete
        quoteHub.RemoveAt(removeAtIndex);
        quotes.RemoveAt(removeAtIndex);

        // final results
        IReadOnlyList<SmaResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<SmaResult> seriesList
           = quotes.ToMama(fastLimit, slowLimit)
            .ToSma(smaPeriods);

        // assert, should equal series
        streamList.Should().HaveCount(length - 1);
        streamList.IsExactly(seriesList);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        MamaHub hub = new(new QuoteHub(), fastLimit, slowLimit);
        hub.ToString().Should().Be("MAMA(0.5,0.05)");
    }
}
