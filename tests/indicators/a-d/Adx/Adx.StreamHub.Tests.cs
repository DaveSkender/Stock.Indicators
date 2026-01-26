namespace StreamHubs;

[TestClass]
public class AdxHubTests : StreamHubTestBase, ITestQuoteObserver, ITestChainProvider
{
    private const int lookbackPeriods = 14;
    private static readonly IReadOnlyList<AdxResult> expectedOriginal = Quotes.ToAdx(lookbackPeriods);

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider (warmup coverage)
        quoteHub.Add(Quotes.Take(20));

        // initialize observer
        AdxHub observer = quoteHub.ToAdxHub(lookbackPeriods);

        // fetch initial results (early)
        IReadOnlyList<AdxResult> actuals = observer.Results;

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

        actuals.Should().HaveCount(length);
        actuals.IsExactly(expectedOriginal);

        // delete, should equal series (revised)
        quoteHub.RemoveAt(removeAtIndex);

        IReadOnlyList<AdxResult> expectedRevised = RevisedQuotes.ToAdx(lookbackPeriods);

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
        IReadOnlyList<AdxResult> expected = quotes
            .ToAdx(lookbackPeriods)
            .TakeLast(maxCacheSize)
            .ToList();

        // Setup with cache limit
        QuoteHub quoteHub = new(maxCacheSize);
        AdxHub observer = quoteHub.ToAdxHub(lookbackPeriods);

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
        // ADX emits IReusable results (AdxResult implements IReusable with Value = Adx),
        // so it can act as a chain provider for downstream indicators.

        const int adxPeriods = 14;
        const int emaPeriods = 10;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize chain: ADX then EMA over its Value
        EmaHub observer = quoteHub
            .ToAdxHub(adxPeriods)
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
            .ToAdx(adxPeriods)
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

        AdxHub hub = new(quoteHub, 14);
        hub.ToString().Should().Be("ADX(14)");

        quoteHub.Add(Quotes[0]);
        quoteHub.Add(Quotes[1]);

        string s = $"ADX(14)({Quotes[0].Timestamp:d})";
        hub.ToString().Should().Be(s);
    }

    [TestMethod]
    public void RollbackValidation()
    {
        QuoteHub quoteHub = new();

        // Precondition: Normal quote stream with 502 expected entries
        AdxHub observer = quoteHub.ToAdxHub(lookbackPeriods);
        quoteHub.Add(Quotes);

        observer.Results.Should().HaveCount(502);
        observer.Results.IsExactly(expectedOriginal);

        // Act: Remove a single historical value
        quoteHub.RemoveAt(removeAtIndex);

        // Assert: Observer should have 501 results and match revised series
        IReadOnlyList<AdxResult> expectedRevised = RevisedQuotes.ToAdx(lookbackPeriods);

        observer.Results.Should().HaveCount(501);
        observer.Results.IsExactly(expectedRevised);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }
}
