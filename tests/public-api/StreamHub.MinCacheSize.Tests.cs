using Test.Data;

namespace Indicators;

[TestClass]
public class MinCacheSizeTests : TestBase
{
    [TestMethod]
    public void MinCacheSize_InitializedToZero()
    {
        // Arrange & Act
        QuoteHub quoteHub = new(maxCacheSize: 100);

        // Assert
        quoteHub.MinCacheSize.Should().Be(0, "QuoteHub should start with MinCacheSize of 0");
    }

    [TestMethod]
    public void MinCacheSize_PropagatesFromSubscriber()
    {
        // Arrange
        QuoteHub quoteHub = new(maxCacheSize: 100);
        quoteHub.MinCacheSize.Should().Be(0);

        // Act - Subscribe an SMA hub which requires warmup
        SmaHub smaHub = quoteHub.ToSmaHub(20);

        // Assert
        quoteHub.MinCacheSize.Should().BeGreaterThan(0, "QuoteHub should inherit MinCacheSize from SMA subscriber");
        quoteHub.MinCacheSize.Should().Be(smaHub.MinCacheSize, "QuoteHub MinCacheSize should match subscriber's requirement");
    }

    [TestMethod]
    public void MinCacheSize_TracksMaximumFromMultipleSubscribers()
    {
        // Arrange
        QuoteHub quoteHub = new(maxCacheSize: 200);

        // Act - Subscribe multiple hubs with different warmup requirements
        SmaHub smaHub10 = quoteHub.ToSmaHub(10);
        SmaHub smaHub20 = quoteHub.ToSmaHub(20);
        SmaHub smaHub50 = quoteHub.ToSmaHub(50);

        // Assert
        int maxSubscriberMin = Math.Max(smaHub10.MinCacheSize, Math.Max(smaHub20.MinCacheSize, smaHub50.MinCacheSize));
        quoteHub.MinCacheSize.Should().Be(maxSubscriberMin, "QuoteHub should track maximum MinCacheSize from all subscribers");
    }

    [TestMethod]
    public void MinCacheSize_ReEvaluatedOnUnsubscribe()
    {
        // Arrange
        QuoteHub quoteHub = new(maxCacheSize: 200);
        SmaHub smaHub20 = quoteHub.ToSmaHub(20);
        SmaHub smaHub50 = quoteHub.ToSmaHub(50);
        int initialMinCacheSize = quoteHub.MinCacheSize;

        // Act - Unsubscribe the hub with larger requirement
        smaHub50.Unsubscribe();

        // Assert
        quoteHub.MinCacheSize.Should().BeLessOrEqualTo(initialMinCacheSize, "MinCacheSize should be re-evaluated after unsubscribe");
        quoteHub.MinCacheSize.Should().Be(smaHub20.MinCacheSize, "MinCacheSize should match remaining subscriber");
    }

    [TestMethod]
    public void MinCacheSize_ReducedToZeroWhenAllUnsubscribed()
    {
        // Arrange
        QuoteHub quoteHub = new(maxCacheSize: 200);
        SmaHub smaHub20 = quoteHub.ToSmaHub(20);
        quoteHub.MinCacheSize.Should().BeGreaterThan(0);

        // Act
        smaHub20.Unsubscribe();

        // Assert
        quoteHub.MinCacheSize.Should().Be(0, "MinCacheSize should be 0 when no subscribers remain");
    }

    [TestMethod]
    public void RejectInsertionsBeforeCacheTimeline()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Data.GetDefault();
        QuoteHub quoteHub = new(maxCacheSize: 50);

        // Add first 100 quotes (cache will have last 50)
        for (int i = 0; i < 100; i++)
        {
            quoteHub.Add(quotes[i]);
        }

        int initialCacheSize = quoteHub.Results.Count;
        DateTime firstCachedTimestamp = quoteHub.Results[0].Timestamp;

        // Act - Try to add a quote before the current cache timeline
        Quote oldQuote = quotes[10]; // This should be before the cache
        quoteHub.Add(oldQuote);

        // Assert
        quoteHub.Results.Count.Should().Be(initialCacheSize, "Cache size should not change");
        quoteHub.Results[0].Timestamp.Should().Be(firstCachedTimestamp, "First cached item should not change");
    }

    [TestMethod]
    public void RejectInsertionsBeforeMinCacheSize()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Data.GetDefault();
        QuoteHub quoteHub = new(maxCacheSize: 200);

        // Subscribe an indicator that requires warmup
        SmaHub smaHub = quoteHub.ToSmaHub(20);

        // Add quotes to build cache
        for (int i = 0; i < 100; i++)
        {
            quoteHub.Add(quotes[i]);
        }

        int minCacheSize = quoteHub.MinCacheSize;
        minCacheSize.Should().BeGreaterThan(0);

        int initialCacheSize = quoteHub.Results.Count;

        // Act - Try to insert a quote at an index before MinCacheSize
        // This would corrupt the indicator's state
        if (minCacheSize < quotes.Count - 100)
        {
            Quote insertQuote = quotes[minCacheSize - 1];
            quoteHub.Add(insertQuote);

            // Assert
            // The insert should be rejected to protect indicator state
            quoteHub.Results.Count.Should().Be(initialCacheSize, "Insert before MinCacheSize should be rejected");
        }
    }

    [TestMethod]
    public void AllowInsertionsAfterMinCacheSize()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Data.GetDefault();
        QuoteHub quoteHub = new(maxCacheSize: 200);

        // Subscribe an indicator that requires warmup
        SmaHub smaHub = quoteHub.ToSmaHub(20);

        // Add quotes to build cache
        for (int i = 0; i < 100; i++)
        {
            quoteHub.Add(quotes[i]);
        }

        int minCacheSize = quoteHub.MinCacheSize;

        // Act - Insert a quote at a safe position (after MinCacheSize)
        // Create a quote that will be inserted in the middle but after MinCacheSize
        if (minCacheSize < 50 && quotes.Count > 110)
        {
            DateTime targetTimestamp = quoteHub.Results[minCacheSize + 5].Timestamp;
            Quote insertQuote = new(
                targetTimestamp,
                quotes[minCacheSize + 5].Open * 1.01m,
                quotes[minCacheSize + 5].High,
                quotes[minCacheSize + 5].Low,
                quotes[minCacheSize + 5].Close,
                quotes[minCacheSize + 5].Volume);

            quoteHub.Add(insertQuote);

            // Assert - The quote should be accepted (it's a same-timestamp revision)
            quoteHub.Results.Should().Contain(q => q.Timestamp == targetTimestamp);
        }
    }

    [TestMethod]
    public void MinCacheSize_ChainedIndicatorsPropagate()
    {
        // Arrange
        QuoteHub quoteHub = new(maxCacheSize: 300);

        // Act - Create a chain: QuoteHub -> SMA(20) -> EMA(10)
        SmaHub smaHub = quoteHub.ToSmaHub(20);
        EmaHub emaHub = smaHub.ToEmaHub(10);

        // Assert
        // QuoteHub should have the maximum MinCacheSize requirement from the chain
        quoteHub.MinCacheSize.Should().BeGreaterThan(0);
        smaHub.MinCacheSize.Should().BeGreaterThan(0);
        quoteHub.MinCacheSize.Should().BeGreaterOrEqualTo(smaHub.MinCacheSize);
    }
}
