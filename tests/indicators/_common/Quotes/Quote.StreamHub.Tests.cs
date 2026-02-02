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
        provider.Add(Quotes[80]);

        sut.IsExactly(Quotes);

        // delete, should equal series (revised)
        provider.RemoveAt(removeAtIndex);

        sut.IsExactly(RevisedQuotes);
        sut.Should().HaveCount(quotesCount - 1);

        // cleanup
        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void WithCachePruning_MatchesSeriesExactly()
    {
        const int maxCacheSize = 50;
        const int totalQuotes = 100;

        IReadOnlyList<Quote> quotes = Quotes.Take(totalQuotes).ToList();
        IReadOnlyList<IQuote> expected = quotes
            .Cast<IQuote>()
            .TakeLast(maxCacheSize)
            .ToList();

        // Setup with cache limit
        QuoteHub quoteHub = new(maxCacheSize);
        QuoteHub observer = quoteHub.ToQuoteHub();

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

        provider.Add(Quotes[80]);  // Late arrival
        provider.RemoveAt(removeAtIndex);  // Delete

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

    [TestMethod]
    public void IgnoreQuotesPrecedingTimeline_Standalone()
    {
        const int maxCacheSize = 50;
        const int totalQuotes = 100;

        IReadOnlyList<Quote> quotes = Quotes.Take(totalQuotes).ToList();

        // Setup standalone QuoteHub with cache limit
        QuoteHub quoteHub = new(maxCacheSize);

        // Stream more quotes than cache can hold
        quoteHub.Add(quotes);

        // Verify cache was pruned to maxCacheSize
        quoteHub.Quotes.Should().HaveCount(maxCacheSize);

        // Cache should now contain quotes [50..99]
        DateTime firstTimestamp = quoteHub.Cache[0].Timestamp;

        // Try to add a quote that precedes the current timeline
        Quote oldQuote = quotes[10]; // This is before quote[50]
        oldQuote.Timestamp.Should().BeBefore(firstTimestamp);

        // This should be silently ignored
        quoteHub.Add(oldQuote);

        // Cache size should remain unchanged
        quoteHub.Quotes.Should().HaveCount(maxCacheSize);

        // First quote in cache should still be the same
        quoteHub.Cache[0].Timestamp.Should().Be(firstTimestamp);

        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void IgnoreQuotesPrecedingTimeline_NonStandalone_NoRebuild()
    {
        const int maxCacheSize = 50;
        const int totalQuotes = 100;

        IReadOnlyList<Quote> quotes = Quotes.Take(totalQuotes).ToList();

        // Setup provider QuoteHub with cache limit
        QuoteHub provider = new(maxCacheSize);

        // Create non-standalone observer
        QuoteHub observer = provider.ToQuoteHub();

        // Create a downstream observer to track rebuilds
        QuoteHub downstream = observer.ToQuoteHub();

        // Subscribe to downstream to track rebuilds
        // We'll use a custom observer that tracks OnRebuild calls
        var mockObserver = new MockRebuildTracker();
        downstream.Subscribe(mockObserver);

        // Stream more quotes than cache can hold
        provider.Add(quotes);

        // Verify caches were pruned
        observer.Results.Should().HaveCount(maxCacheSize);

        // Reset rebuild tracking
        mockObserver.Reset();

        // Cache should now contain quotes [50..99]
        DateTime observerFirstTimestamp = observer.Cache[0].Timestamp;

        // Try to add a quote directly to the observer (non-standalone)
        // that precedes its current timeline
        Quote oldQuote = quotes[10]; // This is before quote[50]
        oldQuote.Timestamp.Should().BeBefore(observerFirstTimestamp);

        // This should be silently ignored - no rebuild should occur
        observer.Add(oldQuote);

        // Verify no rebuild was triggered
        mockObserver.RebuildCount.Should().Be(0, "no rebuild should be triggered for quotes preceding timeline");

        // Cache should remain unchanged
        observer.Results.Should().HaveCount(maxCacheSize);
        observer.Cache[0].Timestamp.Should().Be(observerFirstTimestamp);

        downstream.Unsubscribe();
        observer.Unsubscribe();
        provider.EndTransmission();
    }

    // Helper class to track rebuild calls
    private class MockRebuildTracker : IStreamObserver<IQuote>
    {
        public int RebuildCount { get; private set; }
        public List<DateTime> RebuildTimestamps { get; } = new();

        public bool IsSubscribed => false;

        public void Reset()
        {
            RebuildCount = 0;
            RebuildTimestamps.Clear();
        }

        public void Unsubscribe() { }
        public void OnAdd(IQuote item, bool notify, int? indexHint) { }

        public void OnRebuild(DateTime fromTimestamp)
        {
            RebuildCount++;
            RebuildTimestamps.Add(fromTimestamp);
        }

        public void OnPrune(DateTime toTimestamp) { }
        public void OnError(Exception exception) { }
        public void OnCompleted() { }
    }

    [TestMethod]
    public void IgnoreQuotesPrecedingTimeline_NonStandalone()
    {
        const int maxCacheSize = 50;
        const int totalQuotes = 100;

        IReadOnlyList<Quote> quotes = Quotes.Take(totalQuotes).ToList();

        // Setup provider QuoteHub with cache limit
        QuoteHub provider = new(maxCacheSize);

        // Create non-standalone observer
        QuoteHub observer = provider.ToQuoteHub();

        // Stream more quotes than cache can hold
        provider.Add(quotes);

        // Verify caches were pruned to maxCacheSize
        provider.Quotes.Should().HaveCount(maxCacheSize);
        observer.Results.Should().HaveCount(maxCacheSize);

        // Cache should now contain quotes [50..99]
        DateTime observerFirstTimestamp = observer.Cache[0].Timestamp;

        // Try to add a quote directly to the observer (non-standalone)
        // that precedes its current timeline
        Quote oldQuote = quotes[10]; // This is before quote[50]
        oldQuote.Timestamp.Should().BeBefore(observerFirstTimestamp);

        // Get initial count
        int initialCount = observer.Cache.Count;

        // This should be silently ignored (but currently is NOT for non-standalone)
        observer.Add(oldQuote);

        // Cache size should remain unchanged (or show the bug if it doesn't)
        int finalCount = observer.Cache.Count;
        Console.WriteLine($"Observer cache count before: {initialCount}, after: {finalCount}");
        Console.WriteLine($"Observer first timestamp before: {observerFirstTimestamp}, after: {observer.Cache[0].Timestamp}");

        // Check if old quote was actually added
        bool oldQuoteFound = observer.Cache.Any(q => q.Timestamp == oldQuote.Timestamp);
        Console.WriteLine($"Old quote found in observer cache: {oldQuoteFound}");

        if (oldQuoteFound)
        {
            int oldQuoteIndex = observer.Cache.ToList().FindIndex(q => q.Timestamp == oldQuote.Timestamp);
            Console.WriteLine($"Old quote was inserted at index: {oldQuoteIndex}");
        }

        // These assertions will fail if the bug exists
        observer.Results.Should().HaveCount(maxCacheSize, "cache size should not change when adding old quotes");
        observer.Cache[0].Timestamp.Should().Be(observerFirstTimestamp, "first timestamp should not change");
        oldQuoteFound.Should().BeFalse("old quote should not be in cache");

        observer.Unsubscribe();
        provider.EndTransmission();
    }
}
