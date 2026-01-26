namespace StreamHubs;

[TestClass]
public class MfiHubTests : StreamHubTestBase, ITestQuoteObserver, ITestChainProvider
{
    private const int lookbackPeriods = 14;
    private static readonly IReadOnlyList<MfiResult> expectedOriginal = Quotes.ToMfi(lookbackPeriods);

    [TestMethod]
    public void Results_AreAlwaysBounded()
    {
        IReadOnlyList<MfiResult> sut = Quotes.ToMfiHub(14).Results;
        sut.IsBetween(static x => x.Mfi, 0, 100);
    }

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        quoteHub.Add(Quotes.Take(20));

        // initialize observer
        MfiHub observer = quoteHub.ToMfiHub(lookbackPeriods);

        // fetch initial results (early)
        IReadOnlyList<MfiResult> actuals = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 20; i < length; i++)
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
        actuals.IsExactly(expectedOriginal);

        // delete, should equal series (revised)
        quoteHub.RemoveAt(removeAtIndex);

        IReadOnlyList<MfiResult> expectedRevised = RevisedQuotes.ToMfi(lookbackPeriods);

        actuals.Should().HaveCount(501);
        actuals.IsExactly(expectedRevised);

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
        IReadOnlyList<MfiResult> expected = quotes
            .ToMfi(lookbackPeriods)
            .TakeLast(maxCacheSize)
            .ToList();

        // Setup with cache limit
        QuoteHub quoteHub = new(maxCacheSize);
        MfiHub observer = quoteHub.ToMfiHub(14);

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
    public void ChainProvider_MatchesSeriesExactly()
    {
        // MFI emits IReusable results (MfiResult implements IReusable with Value = Mfi),
        // so it can act as a chain provider for downstream indicators.

        const int mfiPeriods = 14;
        const int emaPeriods = 10;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize chain: MFI then EMA over its Value
        EmaHub observer = quoteHub
            .ToMfiHub(mfiPeriods)
            .ToEmaHub(emaPeriods);

        // emulate quote stream
        for (int i = 0; i < quotesCount; i++)
        {
            if (i == 80) { continue; }  // Skip for late arrival

            Quote q = Quotes[i];
            quoteHub.Add(q);

            if (i is > 100 and < 105) { quoteHub.Add(q); }  // Duplicate quotes
        }

        quoteHub.Insert(Quotes[80]);  // Late arrival
        quoteHub.RemoveAt(removeAtIndex);  // Remove

        // results from stream
        IReadOnlyList<EmaResult> sut = observer.Results;

        // time-series parity (revised)
        IReadOnlyList<EmaResult> expected = RevisedQuotes
            .ToMfi(mfiPeriods)
            .ToEma(emaPeriods);

        sut.Should().HaveCount(quotesCount - 1);
        sut.IsExactly(expected);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        QuoteHub quoteHub = new();

        MfiHub hub = new(quoteHub, lookbackPeriods);
        hub.ToString().Should().Be($"MFI({lookbackPeriods})");
    }
}
