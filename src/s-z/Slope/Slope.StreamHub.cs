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

    // Pre-calculated constant for X variance (sequential integers)
    private readonly double sumSqXConstant;

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

        // Pre-calculate constant sumSqX for sequential X values
        // When X values are [x, x+1, ..., x+n-1], avgX = x + (n-1)/2
        // Sum of (Xi - avgX)^2 = n*(n^2 - 1)/12
        sumSqXConstant = lookbackPeriods * ((lookbackPeriods * lookbackPeriods) - 1) / 12.0;

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

        // Update buffer
        buffer.Update(LookbackPeriods, item.Value);

        // Skip initialization period
        if (i < LookbackPeriods - 1)
        {
            return (new SlopeResult(item.Timestamp), i);
        }

        // Calculate slope, intercept, and statistics
        (double? slope, double? intercept, double? stdDev, double? rSquared)
            = CalculateStatistics(i);

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
    private (double? slope, double? intercept, double? stdDev, double? rSquared)
        CalculateStatistics(int currentIndex)
    {
        // Calculate X values mathematically (sequential integers)
        // X values are: (currentIndex - lookbackPeriods + 2) to (currentIndex + 1)
        // For sequential X = [a, a+1, ..., a+n-1]:
        // - sumX = n*a + n*(n-1)/2
        // - avgX = a + (n-1)/2
        double firstX = currentIndex - LookbackPeriods + 2d;
        double sumX = (LookbackPeriods * firstX) + (LookbackPeriods * (LookbackPeriods - 1) / 2.0);
        double avgX = sumX / LookbackPeriods;

        // Calculate sums for least squares method

        // First pass: calculate avgY
        double avgY = buffer.Average();

        // Second pass: calculate deviations and their products
        double sumSqY = 0;
        double sumSqXy = 0;
        int relativeIndex = 0;
        foreach (double bufferValue in buffer)
        {
            double xValue = firstX + relativeIndex;
            double devX = xValue - avgX;
            double devY = bufferValue - avgY;

            sumSqY += devY * devY;
            sumSqXy += devX * devY;
            relativeIndex++;
        }

        // Use pre-calculated constant for sumSqX
        double? slope = (sumSqXy / sumSqXConstant).NaN2Null();
        double? intercept = (avgY - (slope * avgX)).NaN2Null();

        // Calculate Standard Deviation and R-Squared
        double stdDevX = Math.Sqrt(sumSqXConstant / LookbackPeriods);
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
    /// <param name="currentIndex">The current index in the provider cache.</param>
    /// <param name="slope">The calculated slope value.</param>
    /// <param name="intercept">The calculated intercept value.</param>
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

        // Update Line values for the last lookbackPeriods results
        // Using global indices (p + 1) like the series implementation
        for (int p = startIndex; p <= currentIndex; p++)
        {
            SlopeResult existing = Cache[p];

            // Calculate Line: y = mx + b, using global index (p + 1)
            decimal? line = (decimal?)((slope * (p + 1)) + intercept).NaN2Null();

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
