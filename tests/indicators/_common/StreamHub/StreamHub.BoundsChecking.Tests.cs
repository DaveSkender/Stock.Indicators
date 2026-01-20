namespace Observables;

/// <summary>
/// Tests to validate bounds-checking on Cache and ProviderCache access.
/// These tests attempt to reproduce potential IndexOutOfRangeException vulnerabilities
/// as described in Issue #1917.
/// </summary>
[TestClass]
public class BoundsCheckingTests : TestBase
{
    /// <summary>
    /// Test Case 1: Initialize AtrStopHub with exactly LookbackPeriods quotes.
    /// This tests the boundary condition where warmup period just completes.
    /// </summary>
    [TestMethod]
    public void AtrStopHub_WithExactlyLookbackPeriods_ShouldNotThrow()
    {
        // Arrange - use exactly 21 quotes (default LookbackPeriods for AtrStop)
        const int lookbackPeriods = 21;
        QuoteHub quoteHub = new();

        // Act - add exactly lookbackPeriods quotes
        quoteHub.Add(Quotes.Take(lookbackPeriods));

        // Create hub after data is added
        AtrStopHub observer = quoteHub.ToAtrStopHub(lookbackPeriods);

        // Assert - should not throw and should have correct count
        observer.Results.Should().HaveCount(lookbackPeriods);

        // Cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    /// <summary>
    /// Test Case 2: Initialize AtrStopHub with one less than LookbackPeriods quotes,
    /// then add one more to reach exactly the boundary.
    /// </summary>
    [TestMethod]
    public void AtrStopHub_AddQuoteToReachBoundary_ShouldNotThrow()
    {
        // Arrange - use one less than lookbackPeriods
        const int lookbackPeriods = 21;
        QuoteHub quoteHub = new();

        // Initialize with one less than lookback
        quoteHub.Add(Quotes.Take(lookbackPeriods - 1));
        AtrStopHub observer = quoteHub.ToAtrStopHub(lookbackPeriods);

        // Act - add the quote that reaches exactly lookbackPeriods
        quoteHub.Add(Quotes[lookbackPeriods - 1]);

        // Assert - should not throw and should have correct count
        observer.Results.Should().HaveCount(lookbackPeriods);

        // Cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    /// <summary>
    /// Test Case 3: Remove a quote from the middle and trigger rebuild.
    /// This tests RollbackState when i > LookbackPeriods.
    /// </summary>
    [TestMethod]
    public void AtrStopHub_RemoveQuoteAndRebuild_ShouldNotThrow()
    {
        // Arrange
        const int lookbackPeriods = 21;
        QuoteHub quoteHub = new();
        quoteHub.Add(Quotes.Take(50));

        AtrStopHub observer = quoteHub.ToAtrStopHub(lookbackPeriods);

        // Act - remove a quote in the middle (after warmup period)
        // This should trigger RollbackState with i > LookbackPeriods
        quoteHub.RemoveAt(30);

        // Assert - should not throw
        observer.Results.Should().HaveCount(49);

        // Cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    /// <summary>
    /// Test Case 4: Insert a late arrival quote that triggers rebuild.
    /// This tests the scenario where rebuild starts from a middle index.
    /// </summary>
    [TestMethod]
    public void AtrStopHub_InsertLateArrival_ShouldNotThrow()
    {
        // Arrange
        const int lookbackPeriods = 21;
        QuoteHub quoteHub = new();

        // Add quotes but skip one in the middle
        for (int i = 0; i < 50; i++)
        {
            if (i == 30) continue;  // Skip index 30
            quoteHub.Add(Quotes[i]);
        }

        AtrStopHub observer = quoteHub.ToAtrStopHub(lookbackPeriods);
        observer.Results.Should().HaveCount(49);

        // Act - insert the late arrival
        quoteHub.Insert(Quotes[30]);

        // Assert - should not throw and should have all quotes
        observer.Results.Should().HaveCount(50);

        // Cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    /// <summary>
    /// Test Case 5: Test with minimum valid LookbackPeriods for AtrStop (2).
    /// </summary>
    [TestMethod]
    public void AtrStopHub_WithMinimumLookbackPeriods_ShouldNotThrow()
    {
        // Arrange - use minimum valid lookback period of 2 for AtrStop
        const int lookbackPeriods = 2;
        QuoteHub quoteHub = new();
        quoteHub.Add(Quotes.Take(10));

        // Act - create hub with minimum lookback
        AtrStopHub observer = quoteHub.ToAtrStopHub(lookbackPeriods);

        // Assert - should not throw
        observer.Results.Should().HaveCount(10);

        // Cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    /// <summary>
    /// Test Case 6: Rapid add/remove operations around warmup boundary.
    /// </summary>
    [TestMethod]
    public void AtrStopHub_RapidOperationsAroundBoundary_ShouldNotThrow()
    {
        // Arrange
        const int lookbackPeriods = 21;
        QuoteHub quoteHub = new();

        // Pre-fill with warmup data (26 quotes initially)
        quoteHub.Add(Quotes.Take(lookbackPeriods + 5));
        AtrStopHub observer = quoteHub.ToAtrStopHub(lookbackPeriods);
        int initialCount = observer.Results.Count;  // Should be 26

        // Act - add more quotes (15 more)
        for (int i = lookbackPeriods + 5; i < lookbackPeriods + 20; i++)
        {
            quoteHub.Add(Quotes[i]);
        }

        int afterAddCount = observer.Results.Count;  // Should be 41

        // Remove some quotes (2 removals)
        quoteHub.RemoveAt(25);
        quoteHub.RemoveAt(20);

        // Assert - should not throw and count should be afterAddCount - 2
        observer.Results.Should().HaveCount(afterAddCount - 2);

        // Cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    /// <summary>
    /// Test Case 7: Test ATR Hub (simpler case with same pattern).
    /// </summary>
    [TestMethod]
    public void AtrHub_RemoveAndRebuild_ShouldNotThrow()
    {
        // Arrange
        const int lookbackPeriods = 14;
        QuoteHub quoteHub = new();
        quoteHub.Add(Quotes.Take(50));

        AtrHub observer = quoteHub.ToAtrHub(lookbackPeriods);

        // Act - remove a quote after warmup
        quoteHub.RemoveAt(30);

        // Assert - should not throw
        observer.Results.Should().HaveCount(49);

        // Cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    /// <summary>
    /// Test Case 8: Test Keltner Hub (uses both Cache and ProviderCache[i-1]).
    /// </summary>
    [TestMethod]
    public void KeltnerHub_RemoveAndRebuild_ShouldNotThrow()
    {
        // Arrange
        const int emaPeriods = 20;
        const int atrPeriods = 10;
        QuoteHub quoteHub = new();
        quoteHub.Add(Quotes.Take(50));

        KeltnerHub observer = quoteHub.ToKeltnerHub(emaPeriods, 2, atrPeriods);

        // Act - remove a quote after warmup
        quoteHub.RemoveAt(35);

        // Assert - should not throw
        observer.Results.Should().HaveCount(49);

        // Cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    /// <summary>
    /// Test Case 9: Test Stochastic Hub (has complex cache access patterns).
    /// </summary>
    [TestMethod]
    public void StochHub_RemoveAndRebuild_ShouldNotThrow()
    {
        // Arrange
        const int lookbackPeriods = 14;
        QuoteHub quoteHub = new();
        quoteHub.Add(Quotes.Take(50));

        StochHub observer = quoteHub.ToStochHub(lookbackPeriods);

        // Act - remove a quote after warmup
        quoteHub.RemoveAt(30);

        // Assert - should not throw
        observer.Results.Should().HaveCount(49);

        // Cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    /// <summary>
    /// Test Case 10: Test empty provider scenario.
    /// </summary>
    [TestMethod]
    public void AtrStopHub_EmptyProvider_ShouldNotThrow()
    {
        // Arrange
        QuoteHub quoteHub = new();

        // Act - create hub with empty provider
        AtrStopHub observer = quoteHub.ToAtrStopHub();

        // Assert - should not throw and should be empty
        observer.Results.Should().BeEmpty();

        // Cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    /// <summary>
    /// Test Case 11: Test single quote scenario.
    /// </summary>
    [TestMethod]
    public void AtrStopHub_SingleQuote_ShouldNotThrow()
    {
        // Arrange
        QuoteHub quoteHub = new();
        quoteHub.Add(Quotes.Take(1));

        // Act - create hub with single quote
        AtrStopHub observer = quoteHub.ToAtrStopHub();

        // Assert - should not throw
        observer.Results.Should().HaveCount(1);

        // Cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    /// <summary>
    /// Test Case 12: Test rebuild from middle with pruned cache.
    /// Tests the scenario where MaxCacheSize causes pruning before a rebuild.
    /// </summary>
    [TestMethod]
    public void AtrStopHub_PrunedCacheWithRebuild_ShouldNotThrow()
    {
        // Arrange - use small max cache size
        const int maxCacheSize = 40;
        const int lookbackPeriods = 21;
        QuoteHub quoteHub = new(maxCacheSize);
        AtrStopHub observer = quoteHub.ToAtrStopHub(lookbackPeriods);

        // Add enough quotes to trigger pruning
        quoteHub.Add(Quotes.Take(60));

        // At this point, cache should be pruned to maxCacheSize
        observer.Results.Should().HaveCount(maxCacheSize);

        // Act - remove a quote and trigger rebuild
        int removeIndex = 30;
        quoteHub.RemoveAt(removeIndex);

        // Assert - should not throw
        observer.Results.Should().HaveCount(maxCacheSize - 1);

        // Cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    /// <summary>
    /// Test Case 13: Test Reinitialize after data is added.
    /// </summary>
    [TestMethod]
    public void AtrStopHub_ReinitializeWithData_ShouldNotThrow()
    {
        // Arrange
        const int lookbackPeriods = 21;
        QuoteHub quoteHub = new();
        quoteHub.Add(Quotes.Take(50));

        AtrStopHub observer = quoteHub.ToAtrStopHub(lookbackPeriods);
        observer.Results.Should().HaveCount(50);

        // Act - reinitialize the observer
        observer.Reinitialize();

        // Assert - should not throw and should have same count
        observer.Results.Should().HaveCount(50);

        // Cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    /// <summary>
    /// Test Case 14: Test multiple late arrivals in sequence.
    /// </summary>
    [TestMethod]
    public void AtrStopHub_MultipleLateArrivals_ShouldNotThrow()
    {
        // Arrange
        const int lookbackPeriods = 21;
        QuoteHub quoteHub = new();

        // Add quotes but skip several in the middle
        int[] skipIndices = { 25, 30, 35 };
        for (int i = 0; i < 50; i++)
        {
            if (skipIndices.Contains(i)) continue;
            quoteHub.Add(Quotes[i]);
        }

        AtrStopHub observer = quoteHub.ToAtrStopHub(lookbackPeriods);
        observer.Results.Should().HaveCount(47);

        // Act - insert late arrivals in reverse order
        foreach (int idx in skipIndices.Reverse())
        {
            quoteHub.Insert(Quotes[idx]);
        }

        // Assert - should not throw and should have all quotes
        observer.Results.Should().HaveCount(50);

        // Cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    /// <summary>
    /// Test Case 15: Test SuperTrend Hub (complex state-dependent indicator).
    /// </summary>
    [TestMethod]
    public void SuperTrendHub_RemoveAndRebuild_ShouldNotThrow()
    {
        // Arrange
        const int atrPeriods = 10;
        QuoteHub quoteHub = new();
        quoteHub.Add(Quotes.Take(50));

        SuperTrendHub observer = quoteHub.ToSuperTrendHub(atrPeriods);

        // Act - remove a quote after warmup
        quoteHub.RemoveAt(30);

        // Assert - should not throw
        observer.Results.Should().HaveCount(49);

        // Cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    /// <summary>
    /// Test Case 16: Initialize observer BEFORE adding data to provider.
    /// This tests the scenario where provider is empty at observer creation.
    /// </summary>
    [TestMethod]
    public void AtrStopHub_InitializeBeforeData_ShouldNotThrow()
    {
        // Arrange
        const int lookbackPeriods = 21;
        QuoteHub quoteHub = new();

        // Create observer with empty provider
        AtrStopHub observer = quoteHub.ToAtrStopHub(lookbackPeriods);
        observer.Results.Should().BeEmpty();

        // Act - now add data
        quoteHub.Add(Quotes.Take(50));

        // Assert - should not throw and should have all quotes
        observer.Results.Should().HaveCount(50);

        // Cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    /// <summary>
    /// Test Case 17: Add quotes one by one, then remove one at the boundary.
    /// </summary>
    [TestMethod]
    public void AtrStopHub_AddOneByOneThenRemoveAtBoundary_ShouldNotThrow()
    {
        // Arrange
        const int lookbackPeriods = 21;
        QuoteHub quoteHub = new();
        AtrStopHub observer = quoteHub.ToAtrStopHub(lookbackPeriods);

        // Add quotes one by one
        for (int i = 0; i < lookbackPeriods + 5; i++)
        {
            quoteHub.Add(Quotes[i]);
        }

        // Act - remove exactly at the boundary (index = lookbackPeriods)
        quoteHub.RemoveAt(lookbackPeriods);

        // Assert - should not throw
        observer.Results.Should().HaveCount(lookbackPeriods + 4);

        // Cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    /// <summary>
    /// Test Case 18: Test Rebuild() method directly.
    /// </summary>
    [TestMethod]
    public void AtrStopHub_DirectRebuild_ShouldNotThrow()
    {
        // Arrange
        const int lookbackPeriods = 21;
        QuoteHub quoteHub = new();
        quoteHub.Add(Quotes.Take(50));
        AtrStopHub observer = quoteHub.ToAtrStopHub(lookbackPeriods);

        // Act - call Rebuild directly
        observer.Rebuild();

        // Assert - should not throw and should have same count
        observer.Results.Should().HaveCount(50);

        // Cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    /// <summary>
    /// Test Case 19: Test Rebuild from a specific timestamp.
    /// </summary>
    [TestMethod]
    public void AtrStopHub_RebuildFromTimestamp_ShouldNotThrow()
    {
        // Arrange
        const int lookbackPeriods = 21;
        QuoteHub quoteHub = new();
        quoteHub.Add(Quotes.Take(50));
        AtrStopHub observer = quoteHub.ToAtrStopHub(lookbackPeriods);

        // Get a timestamp in the middle (after warmup)
        DateTime rebuildTimestamp = Quotes[30].Timestamp;

        // Act - rebuild from specific timestamp
        observer.Rebuild(rebuildTimestamp);

        // Assert - should not throw and should have same count
        observer.Results.Should().HaveCount(50);

        // Cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    /// <summary>
    /// Test Case 20: Chained StreamHubs - test cascading rebuilds.
    /// </summary>
    [TestMethod]
    public void ChainedHubs_RemoveAndRebuild_ShouldNotThrow()
    {
        // Arrange
        QuoteHub quoteHub = new();
        quoteHub.Add(Quotes.Take(50));

        // Create a chain: QuoteHub -> AtrHub -> SmaHub
        AtrHub atrHub = quoteHub.ToAtrHub(14);
        SmaHub smaHub = atrHub.ToSmaHub(10);

        // Act - remove a quote, which should cascade rebuild through the chain
        quoteHub.RemoveAt(35);

        // Assert - should not throw
        atrHub.Results.Should().HaveCount(49);
        smaHub.Results.Should().HaveCount(49);

        // Cleanup
        smaHub.Unsubscribe();
        atrHub.Unsubscribe();
        quoteHub.EndTransmission();
    }

    /// <summary>
    /// Test Case 21: Test with very small data set (less than warmup period).
    /// </summary>
    [TestMethod]
    public void AtrStopHub_SmallDataSet_ShouldNotThrow()
    {
        // Arrange
        const int lookbackPeriods = 21;
        QuoteHub quoteHub = new();

        // Add only 5 quotes (less than lookback period)
        quoteHub.Add(Quotes.Take(5));
        AtrStopHub observer = quoteHub.ToAtrStopHub(lookbackPeriods);

        // Act - try to remove from small dataset
        quoteHub.RemoveAt(2);

        // Assert - should not throw
        observer.Results.Should().HaveCount(4);

        // Cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    /// <summary>
    /// Test Case 22: Remove all quotes after warmup.
    /// </summary>
    [TestMethod]
    public void AtrStopHub_RemoveAllAfterWarmup_ShouldNotThrow()
    {
        // Arrange
        const int lookbackPeriods = 21;
        QuoteHub quoteHub = new();
        quoteHub.Add(Quotes.Take(30));
        AtrStopHub observer = quoteHub.ToAtrStopHub(lookbackPeriods);

        // Act - remove all quotes after warmup period (indices 21-29)
        for (int i = 29; i >= lookbackPeriods; i--)
        {
            quoteHub.RemoveAt(i);
        }

        // Assert - should not throw and should have only warmup quotes left
        observer.Results.Should().HaveCount(lookbackPeriods);

        // Cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    /// <summary>
    /// Test Case 23: Remove quotes from the beginning.
    /// </summary>
    [TestMethod]
    public void AtrStopHub_RemoveFromBeginning_ShouldNotThrow()
    {
        // Arrange
        const int lookbackPeriods = 21;
        QuoteHub quoteHub = new();
        quoteHub.Add(Quotes.Take(50));
        AtrStopHub observer = quoteHub.ToAtrStopHub(lookbackPeriods);

        // Act - remove first few quotes
        for (int i = 0; i < 5; i++)
        {
            quoteHub.RemoveAt(0);
        }

        // Assert - should not throw
        observer.Results.Should().HaveCount(45);

        // Cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    /// <summary>
    /// Test Case 24: Test VolatilityStop Hub (similar pattern to AtrStop).
    /// </summary>
    [TestMethod]
    public void VolatilityStopHub_RemoveAndRebuild_ShouldNotThrow()
    {
        // Arrange
        const int lookbackPeriods = 7;
        QuoteHub quoteHub = new();
        quoteHub.Add(Quotes.Take(50));

        VolatilityStopHub observer = quoteHub.ToVolatilityStopHub(lookbackPeriods);

        // Act - remove a quote after warmup
        quoteHub.RemoveAt(30);

        // Assert - should not throw
        observer.Results.Should().HaveCount(49);

        // Cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    /// <summary>
    /// Test Case 25: Test Chandelier Hub.
    /// </summary>
    [TestMethod]
    public void ChandelierHub_RemoveAndRebuild_ShouldNotThrow()
    {
        // Arrange
        const int lookbackPeriods = 22;
        QuoteHub quoteHub = new();
        quoteHub.Add(Quotes.Take(50));

        ChandelierHub observer = quoteHub.ToChandelierHub(lookbackPeriods);

        // Act - remove a quote after warmup
        quoteHub.RemoveAt(35);

        // Assert - should not throw
        observer.Results.Should().HaveCount(49);

        // Cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    /// <summary>
    /// Test Case 26: Test Renko Hub (asymmetric - can produce 0 or many bricks per quote).
    /// </summary>
    [TestMethod]
    public void RenkoHub_RemoveAndRebuild_ShouldNotThrow()
    {
        // Arrange
        const decimal brickSize = 2.5m;
        QuoteHub quoteHub = new();
        quoteHub.Add(Quotes.Take(100));

        RenkoHub observer = quoteHub.ToRenkoHub(brickSize);
        int initialBrickCount = observer.Results.Count;

        // Act - remove a quote in the middle
        quoteHub.RemoveAt(50);

        // Assert - should not throw
        // Renko count may vary since it's asymmetric
        observer.Results.Should().NotBeNull();

        // Cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    /// <summary>
    /// Test Case 27: Test Renko Hub with small brick size (more bricks).
    /// </summary>
    [TestMethod]
    public void RenkoHub_SmallBrickSize_RemoveAndRebuild_ShouldNotThrow()
    {
        // Arrange
        const decimal brickSize = 0.5m;
        QuoteHub quoteHub = new();
        quoteHub.Add(Quotes.Take(50));

        RenkoHub observer = quoteHub.ToRenkoHub(brickSize);

        // Act - remove a quote
        quoteHub.RemoveAt(25);

        // Assert - should not throw
        observer.Results.Should().NotBeNull();

        // Cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    /// <summary>
    /// Test Case 28: Test RemoveRange on StreamHub.
    /// </summary>
    [TestMethod]
    public void AtrStopHub_RemoveRange_ShouldNotThrow()
    {
        // Arrange
        const int lookbackPeriods = 21;
        QuoteHub quoteHub = new();
        quoteHub.Add(Quotes.Take(50));
        AtrStopHub observer = quoteHub.ToAtrStopHub(lookbackPeriods);

        // Get timestamp to remove from
        DateTime removeFromTimestamp = Quotes[30].Timestamp;

        // Act - remove range from observer
        observer.RemoveRange(removeFromTimestamp, notify: true);

        // Assert - should not throw and should have fewer items
        observer.Results.Should().HaveCountLessThan(50);

        // Cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    /// <summary>
    /// Test Case 29: Test RemoveRange with index on StreamHub.
    /// </summary>
    [TestMethod]
    public void AtrStopHub_RemoveRangeByIndex_ShouldNotThrow()
    {
        // Arrange
        const int lookbackPeriods = 21;
        QuoteHub quoteHub = new();
        quoteHub.Add(Quotes.Take(50));
        AtrStopHub observer = quoteHub.ToAtrStopHub(lookbackPeriods);

        // Act - remove range from index 30
        observer.RemoveRange(30, notify: true);

        // Assert - should not throw and should have 30 items
        observer.Results.Should().HaveCount(30);

        // Cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    /// <summary>
    /// Test Case 30: Test RemoveAt on StreamHub directly.
    /// </summary>
    [TestMethod]
    public void AtrStopHub_RemoveAtDirectly_ShouldNotThrow()
    {
        // Arrange
        const int lookbackPeriods = 21;
        QuoteHub quoteHub = new();
        quoteHub.Add(Quotes.Take(50));
        AtrStopHub observer = quoteHub.ToAtrStopHub(lookbackPeriods);

        // Act - remove directly from observer cache
        observer.RemoveAt(30);

        // Assert - should not throw and should have 49 items
        observer.Results.Should().HaveCount(49);

        // Cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    /// <summary>
    /// Test Case 31: Test deeply chained StreamHubs (3 levels).
    /// </summary>
    [TestMethod]
    public void DeeplyChainedHubs_RemoveAndRebuild_ShouldNotThrow()
    {
        // Arrange
        QuoteHub quoteHub = new();
        quoteHub.Add(Quotes.Take(100));

        // Create a deep chain: QuoteHub -> AtrHub -> EmaHub -> SmaHub
        AtrHub atrHub = quoteHub.ToAtrHub(14);
        EmaHub emaHub = atrHub.ToEmaHub(10);
        SmaHub smaHub = emaHub.ToSmaHub(5);

        // Act - remove a quote, triggering cascade through all levels
        quoteHub.RemoveAt(60);

        // Assert - should not throw
        atrHub.Results.Should().HaveCount(99);
        emaHub.Results.Should().HaveCount(99);
        smaHub.Results.Should().HaveCount(99);

        // Cleanup
        smaHub.Unsubscribe();
        emaHub.Unsubscribe();
        atrHub.Unsubscribe();
        quoteHub.EndTransmission();
    }

    /// <summary>
    /// Test Case 32: Test concurrent operations (add and remove).
    /// </summary>
    [TestMethod]
    public void AtrStopHub_ConcurrentAddAndRemove_ShouldNotThrow()
    {
        // Arrange
        const int lookbackPeriods = 21;
        QuoteHub quoteHub = new();
        quoteHub.Add(Quotes.Take(40));
        AtrStopHub observer = quoteHub.ToAtrStopHub(lookbackPeriods);

        // Act - interleaved add and remove operations
        for (int i = 40; i < 60; i++)
        {
            quoteHub.Add(Quotes[i]);
            if (i % 5 == 0 && quoteHub.Quotes.Count > lookbackPeriods + 5)
            {
                // Remove from the middle occasionally
                quoteHub.RemoveAt(lookbackPeriods + 2);
            }
        }

        // Assert - should not throw
        observer.Results.Should().NotBeNull();
        observer.Results.Should().NotBeEmpty();

        // Cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    /// <summary>
    /// Test Case 33: Attempt to trigger RollbackState edge case.
    /// Tests scenario where RollbackState is called with i > LookbackPeriods
    /// but Cache might be in an unexpected state.
    /// </summary>
    [TestMethod]
    public void AtrStopHub_RollbackStateEdgeCase_ShouldNotThrow()
    {
        // Arrange
        const int lookbackPeriods = 10;  // Small lookback for easier testing
        QuoteHub quoteHub = new();

        // Add just enough quotes to get past lookback
        quoteHub.Add(Quotes.Take(15));
        AtrStopHub observer = quoteHub.ToAtrStopHub(lookbackPeriods);

        // Verify initial state
        observer.Results.Should().HaveCount(15);

        // Act - remove a quote after warmup, triggering RollbackState
        // This should call RollbackState with i > lookbackPeriods
        quoteHub.RemoveAt(12);

        // Assert - should not throw
        observer.Results.Should().HaveCount(14);

        // Cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    /// <summary>
    /// Test Case 34: Test with BadQuotes data (edge case data).
    /// </summary>
    [TestMethod]
    public void AtrStopHub_WithBadQuotes_ShouldNotThrow()
    {
        // Arrange - BadQuotes may have unusual values
        const int lookbackPeriods = 21;
        QuoteHub quoteHub = new();
        quoteHub.Add(BadQuotes);

        // Act - create hub
        AtrStopHub observer = quoteHub.ToAtrStopHub(lookbackPeriods);

        // Assert - should not throw (count may differ from input due to duplicates)
        observer.Results.Should().NotBeNull();
        observer.Results.Should().NotBeEmpty();

        // Cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    /// <summary>
    /// Test Case 35: Test rebuild at exactly i = LookbackPeriods.
    /// </summary>
    [TestMethod]
    public void AtrStopHub_RebuildAtExactlyLookback_ShouldNotThrow()
    {
        // Arrange
        const int lookbackPeriods = 21;
        QuoteHub quoteHub = new();
        quoteHub.Add(Quotes.Take(50));
        AtrStopHub observer = quoteHub.ToAtrStopHub(lookbackPeriods);

        // Get timestamp at exactly lookback
        DateTime rebuildTimestamp = Quotes[lookbackPeriods].Timestamp;

        // Act - rebuild from exactly lookback
        observer.Rebuild(rebuildTimestamp);

        // Assert - should not throw
        observer.Results.Should().HaveCount(50);

        // Cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }
}
