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

    // Track total number of items processed to calculate global positions after pruning
    private int totalProcessed;

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

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }
    /// <inheritdoc/>
    public override void OnAdd(IReusable item, bool notify, int? indexHint)
    {
        // Call base to add the result to cache
        base.OnAdd(item, notify, indexHint);

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

        // Increment total processed count
        totalProcessed++;

        // Update buffer
        buffer.Update(LookbackPeriods, item.Value);

        // Skip initialization period
        if (i < LookbackPeriods - 1)
        {
            return (new SlopeResult(item.Timestamp), i);
        }

        // Calculate cache offset: totalProcessed - current cache size
        // This allows reconstruction of global positions after pruning
        int cacheOffset = totalProcessed - ProviderCache.Count;

        // Calculate slope, intercept, and statistics
        (double? slope, double? intercept, double? stdDev, double? rSquared)
            = CalculateStatistics(i, cacheOffset);

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

        // Reset totalProcessed to match the target state
        // Count items in provider cache up to (but not including) targetIndex
        totalProcessed = targetIndex;

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
    /// Calculates slope, intercept, standard deviation, and R-squared.
    /// </summary>
    /// <param name="currentIndex">The current index in the provider cache.</param>
    /// <param name="cacheOffset">The offset to calculate global positions (totalCount - cacheSize).</param>
    private (double? slope, double? intercept, double? stdDev, double? rSquared)
        CalculateStatistics(int currentIndex, int cacheOffset)
    {
        // Calculate X values using GLOBAL positions (p + 1) to match Series implementation
        // For cache index i, global position = cacheOffset + i
        // This allows mathematical equivalence even after cache pruning
        int startIndex = currentIndex - LookbackPeriods + 1;

        // Calculate averages
        double sumX = 0;
        double sumY = 0;

        for (int p = startIndex; p <= currentIndex; p++)
        {
            int globalPos = cacheOffset + p;
            sumX += globalPos + 1; // X values: (p + 1) where p is global position
            sumY += ProviderCache[p].Value;
        }

        double avgX = sumX / LookbackPeriods;
        double avgY = sumY / LookbackPeriods;

        // Least squares method
        double sumSqX = 0;
        double sumSqY = 0;
        double sumSqXy = 0;

        for (int p = startIndex; p <= currentIndex; p++)
        {
            int globalPos = cacheOffset + p;
            double devX = (globalPos + 1) - avgX;
            double devY = ProviderCache[p].Value - avgY;

            sumSqX += devX * devX;
            sumSqY += devY * devY;
            sumSqXy += devX * devY;
        }

        // Calculate slope and intercept
        double? slope = (sumSqXy / sumSqX).NaN2Null();
        double? intercept = (avgY - (slope * avgX)).NaN2Null();

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
    /// <param name="intercept">The calculated intercept value.</param>
    private void UpdateLineValues(int currentIndex, double? slope, double? intercept)
    {
        // Calculate cache offset: totalProcessed - current cache size
        int cacheOffset = totalProcessed - Cache.Count;

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

        // Update Line values for the last lookbackPeriods results
        // Using global position indices (p + 1) to match Series implementation
        for (int p = startIndex; p <= currentIndex; p++)
        {
            SlopeResult existing = Cache[p];

            // Calculate Line: y = mx + b, where x = (globalPos + 1)
            int globalPos = cacheOffset + p;
            decimal? line = (decimal?)((slope * (globalPos + 1)) + intercept).NaN2Null();

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
