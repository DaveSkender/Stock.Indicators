namespace Skender.Stock.Indicators;

/// <summary>
/// Provides streaming hub for Slope and Linear Regression calculations.
/// </summary>
/// <remarks>
/// The Slope indicator exhibits legitimate historical repaint behavior where Line values
/// for the last lookbackPeriods results are recalculated using the most recent slope/intercept.
/// This matches the Series implementation's behavior.
/// </remarks>
public class SlopeHub
    : ChainHub<IReusable, SlopeResult>, ISlope
{
    private readonly Queue<double> buffer;

    // Track how many unique items have been processed (including pruned items)
    private int itemsProcessed;
    private DateTime? lastSeenTimestamp;
    
    // Track cache size to detect pruning
    private int lastCacheSize;

    // Cache latest slope/intercept to avoid cache lookups in OnAdd
    private double? currentSlope;
    private double? currentIntercept;

    /// <summary>
    /// Initializes a new instance of the <see cref="SlopeHub"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    internal SlopeHub(
        IChainProvider<IReusable> provider,
        int lookbackPeriods) : base(provider)
    {
        Slope.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        Name = $"SLOPE({lookbackPeriods})";
        buffer = new Queue<double>(lookbackPeriods);

        // Validate cache size for warmup requirements
        ValidateCacheSize(lookbackPeriods, Name);

        // Initialize items processed counter
        itemsProcessed = 0;
        lastSeenTimestamp = null;
        lastCacheSize = 0;

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }
    /// <inheritdoc/>
    public override void OnAdd(IReusable item, bool notify, int? indexHint)
    {
        // Call base to add the result to cache
        base.OnAdd(item, notify, indexHint);
        
        // Track cache size for pruning detection
        lastCacheSize = Cache.Count;

        // Update Line values for the last lookbackPeriods results
        // This is done after the result is in the cache
        int currentIndex = Cache.Count - 1;
        if (currentIndex >= LookbackPeriods - 1)
        {
            UpdateLineValues(currentIndex, currentSlope, currentIntercept);
        }
    }

    /// <inheritdoc/>
    protected override (SlopeResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Increment items processed for new unique items only
        // Duplicates (same timestamp) don't increment the counter
        bool isNewItem = item.Timestamp != lastSeenTimestamp;
        if (isNewItem)
        {
            lastSeenTimestamp = item.Timestamp;
            itemsProcessed++;
        }

        // Update buffer
        buffer.Update(LookbackPeriods, item.Value);

        // Skip initialization period
        if (i < LookbackPeriods - 1)
        {
            return (new SlopeResult(item.Timestamp), i);
        }

        // Calculate slope, intercept, and statistics using local sequential X values
        (double? slope, double? intercept, double? stdDev, double? rSquared)
            = CalculateStatistics();

        // Cache slope and intercept for use in OnAdd
        currentSlope = slope;
        currentIntercept = intercept;

        // Create result (Line will be updated in OnAdd after this is added to Cache)
        SlopeResult r = new(
            Timestamp: item.Timestamp,
            Slope: slope,
            Intercept: intercept,
            StdDev: stdDev,
            RSquared: rSquared,
            Line: null);

        return (r, i);
    }

    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        int targetIndex = ProviderCache.IndexGte(timestamp);

        // Clear buffer
        buffer.Clear();

        // Clear cached slope/intercept
        currentSlope = null;
        currentIntercept = null;

        // Reset items processed counter to match the target index
        // (assumes no pruning happened before this point, or that we're rolling back past pruning)
        itemsProcessed = targetIndex;
        lastSeenTimestamp = targetIndex > 0 ? ProviderCache[targetIndex - 1].Timestamp : null;

        if (targetIndex <= LookbackPeriods - 1)
        {
            return;
        }

        // Rebuild buffer from cache up to targetIndex - 1
        int restoreIndex = targetIndex - 1;
        int startIdx = Math.Max(0, restoreIndex - LookbackPeriods + 1);

        for (int p = startIdx; p <= restoreIndex; p++)
        {
            buffer.Enqueue(ProviderCache[p].Value);
        }
    }

    /// <summary>
    /// Called when items are pruned from the cache.
    /// Increment itemsProcessed to account for removed items.
    /// </summary>
    /// <param name="toTimestamp">Timestamp of last item being removed.</param>
    protected override void PruneState(DateTime toTimestamp)
    {
        // Calculate how many items were removed by comparing cache sizes
        // This is called AFTER items are removed from Cache
        int currentCacheSize = Cache.Count;
        int removedCount = lastCacheSize - currentCacheSize;
        
        if (removedCount > 0)
        {
            // Increment itemsProcessed by the number of removed items
            // This maintains the invariant: itemsProcessed = (items removed) + (items in cache)
            itemsProcessed += removedCount;
        }
        
        // Update lastCacheSize for next pruning event
        lastCacheSize = currentCacheSize;
    }

    /// <summary>
    /// Calculates slope, intercept, standard deviation, and R-squared using local sequential X values.
    /// </summary>
    private (double? slope, double? intercept, double? stdDev, double? rSquared)
        CalculateStatistics()
    {
        // Use local sequential X values: 1, 2, 3, ..., lookbackPeriods
        // Slope is invariant to X-axis translation, so this produces the same slope
        // as using global positions, while avoiding complex state tracking
        
        // Calculate averages using local X values (1 to n)
        double sumX = 0;
        double sumY = 0;
        int relativeIndex = 0;

        foreach (double bufferValue in buffer)
        {
            sumX += relativeIndex + 1; // X values: 1, 2, 3, ..., n
            sumY += bufferValue;
            relativeIndex++;
        }

        double avgX = sumX / LookbackPeriods;
        double avgY = sumY / LookbackPeriods;

        // Least squares method with local X values
        double sumSqX = 0;
        double sumSqY = 0;
        double sumSqXy = 0;
        relativeIndex = 0;

        foreach (double bufferValue in buffer)
        {
            double xValue = relativeIndex + 1;
            double devX = xValue - avgX;
            double devY = bufferValue - avgY;

            sumSqX += devX * devX;
            sumSqY += devY * devY;
            sumSqXy += devX * devY;
            relativeIndex++;
        }

        // Calculate slope (same as Series because slope is translation-invariant)
        double? slope = (sumSqXy / sumSqX).NaN2Null();
        
        // Calculate local intercept
        double? interceptLocal = (avgY - (slope * avgX)).NaN2Null();

        // Convert to global intercept for compatibility with Series
        // Global intercept = local intercept + slope × (1 - firstX)
        // where firstX = itemsProcessed - lookbackPeriods + 1
        int firstX = itemsProcessed - LookbackPeriods + 1;
        double? intercept = (interceptLocal + (slope * (1 - firstX))).NaN2Null();

        // Calculate Standard Deviation and R-Squared
        double stdDevX = Math.Sqrt(sumSqX / LookbackPeriods);
        double stdDevY = Math.Sqrt(sumSqY / LookbackPeriods);

        double? rSquared = null;

        if (stdDevX * stdDevY != 0)
        {
            double arrr = sumSqXy / (stdDevX * stdDevY) / LookbackPeriods;
            rSquared = (arrr * arrr).NaN2Null();
        }

        return (slope, intercept, stdDevY.NaN2Null(), rSquared);
    }

    /// <summary>
    /// Updates Line values for the last lookbackPeriods results using the current slope/intercept.
    /// This is legitimate historical repaint behavior matching the Series implementation.
    /// </summary>
    /// <param name="currentIndex">The current index in the cache.</param>
    /// <param name="slope">The calculated slope value.</param>
    /// <param name="intercept">The calculated global intercept value.</param>
    private void UpdateLineValues(int currentIndex, double? slope, double? intercept)
    {
        // Calculate the range of indices that should have Line values
        int startIndex = currentIndex - LookbackPeriods + 1;

        // Nullify the SINGLE Line value that just exited the window (if any)
        int exitedIndex = startIndex - 1;
        if (exitedIndex >= 0)
        {
            SlopeResult exited = Cache[exitedIndex];
            // Only update if Line was previously set (optimization)
            if (exited.Line is not null)
            {
                Cache[exitedIndex] = exited with { Line = null };
            }
        }

        // Calculate firstX for this window
        int firstX = itemsProcessed - LookbackPeriods + 1;
        
        // Convert global intercept back to local for Line calculation
        // Local intercept = global intercept - slope × (1 - firstX)
        double? interceptLocal = intercept - (slope * (1 - firstX));

        // Update Line values using local form: line = slope × (p + 1) + interceptLocal
        // where p is the relative position within the window (0 to lookbackPeriods-1)
        for (int p = startIndex; p <= currentIndex; p++)
        {
            SlopeResult existing = Cache[p];

            // Calculate Line using local X values
            int relativePos = p - startIndex; // 0-based position in window
            decimal? line = (decimal?)((slope * (relativePos + 1)) + interceptLocal).NaN2Null();

            Cache[p] = existing with { Line = line };
        }
    }
}

public static partial class Slope
{
    /// <summary>
    /// Creates a Slope streaming hub from a chain provider.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>A Slope hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static SlopeHub ToSlopeHub(
        this IChainProvider<IReusable> chainProvider,
        int lookbackPeriods = 14)
        => new(chainProvider, lookbackPeriods);
}
