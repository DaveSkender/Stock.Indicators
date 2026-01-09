namespace Skender.Stock.Indicators;

/// <summary>
/// Represents a Standard Deviation stream hub.
/// </summary>
public class StdDevHub
    : ChainHub<IReusable, StdDevResult>, IStdDev
{
    internal StdDevHub(
        IChainProvider<IReusable> provider,
        int lookbackPeriods) : base(provider)
    {
        StdDev.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        Name = $"STDDEV({lookbackPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }
    /// <inheritdoc/>
    protected override (StdDevResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Calculate StdDev using two-pass algorithm over ProviderCache
        // This is O(lookbackPeriods) complexity (linear in lookback period)
        // Two-pass method is necessary for numerical stability and exact Series precision
        double? stdDev = null;
        double? mean = null;
        double? zScore = null;

        if (i >= LookbackPeriods - 1)
        {
            // Calculate mean using Sma.Increment utility
            double meanValue = Sma.Increment(ProviderCache, LookbackPeriods, i);

            if (!double.IsNaN(meanValue))
            {
                mean = meanValue;

                // Calculate sum of squared deviations (numerically stable method)
                double sumSqDev = 0;
                for (int p = i - LookbackPeriods + 1; p <= i; p++)
                {
                    double value = ProviderCache[p].Value;
                    double deviation = value - mean.Value;
                    sumSqDev += deviation * deviation;
                }

                // Calculate standard deviation
                stdDev = Math.Sqrt(sumSqDev / LookbackPeriods);

                // Calculate z-score
                zScore = stdDev == 0 ? double.NaN : (item.Value - mean.Value) / stdDev.Value;
            }
        }

        // candidate result
        StdDevResult r = new(
            Timestamp: item.Timestamp,
            StdDev: stdDev,
            Mean: mean,
            ZScore: zScore.NaN2Null());

        return (r, i);
    }

}

/// <summary>
/// Provides methods for creating StdDev hubs.
/// </summary>
public static partial class StdDev
{
    /// <summary>
    /// Converts the chain provider to a StdDev hub.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>A StdDev hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static StdDevHub ToStdDevHub(
        this IChainProvider<IReusable> chainProvider,
        int lookbackPeriods = 14)
             => new(chainProvider, lookbackPeriods);
}
