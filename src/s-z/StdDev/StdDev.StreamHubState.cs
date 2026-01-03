namespace Skender.Stock.Indicators;

/// <summary>
/// State object for StdDev streaming hub.
/// </summary>
/// <param name="RollingSum">Rolling sum of values in the window.</param>
/// <param name="RollingSumSqDev">Rolling sum of squared deviations.</param>
/// <param name="Mean">Current mean value.</param>
/// <param name="Count">Count of values in the window.</param>
public record StdDevState(double RollingSum, double RollingSumSqDev, double Mean, int Count) : IHubState;

/// <summary>
/// Streaming hub for Standard Deviation using state management.
/// </summary>
/// <remarks>
/// This implementation caches the rolling sum and squared deviations for efficient updates.
/// State restoration after rollback uses StateCache instead of recalculating.
/// Note: For numerical stability, we still use two-pass calculation (mean first, then deviations).
/// </remarks>
public class StdDevHubState
    : ChainHubState<IReusable, StdDevState, StdDevResult>, IStdDev
{
    private double _rollingSum;
    private double _rollingSumSqDev;
    private double _mean;
    private int _count;

    internal StdDevHubState(
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
    /// <remarks>
    /// O(k) complexity per quote where k = lookbackPeriods due to two-pass calculation.
    /// Two-pass method is necessary for numerical stability and exact Series precision.
    /// </remarks>
    protected override (StdDevResult result, StdDevState state, int index)
        ToIndicatorState(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        double? stdDev = null;
        double? mean = null;
        double? zScore = null;

        if (i >= LookbackPeriods - 1)
        {
            // Calculate mean (using SMA utility for consistency)
            double meanValue = Sma.Increment(ProviderCache, LookbackPeriods, i);

            if (!double.IsNaN(meanValue))
            {
                mean = meanValue;
                _mean = meanValue;

                // Calculate sum of squared deviations (numerically stable method)
                double sumSqDev = 0;
                for (int p = i - LookbackPeriods + 1; p <= i; p++)
                {
                    double value = ProviderCache[p].Value;
                    double deviation = value - mean.Value;
                    sumSqDev += deviation * deviation;
                }

                _rollingSumSqDev = sumSqDev;

                // Calculate standard deviation
                stdDev = Math.Sqrt(sumSqDev / LookbackPeriods);

                // Calculate z-score
                zScore = stdDev == 0 ? double.NaN : (item.Value - mean.Value) / stdDev.Value;

                // Update rolling sum
                _rollingSum = 0;
                for (int p = i - LookbackPeriods + 1; p <= i; p++)
                {
                    _rollingSum += ProviderCache[p].Value;
                }
                _count = LookbackPeriods;
            }
        }
        else
        {
            _rollingSum = 0;
            _rollingSumSqDev = 0;
            _mean = 0;
            _count = 0;
        }

        // Create state object
        StdDevState stateObj = new(_rollingSum, _rollingSumSqDev, _mean, _count);

        // Candidate result
        StdDevResult r = new(
            Timestamp: item.Timestamp,
            StdDev: stdDev,
            Mean: mean,
            ZScore: zScore.NaN2Null());

        return (r, stateObj, i);
    }

    /// <summary>
    /// Restores the StdDev state from previous cached state.
    /// </summary>
    /// <param name="previousState">The cached state from one bar ago, or null to reset.</param>
    protected override void RestorePreviousState(StdDevState? previousState)
    {
        if (previousState is null)
        {
            // Reset to initial state
            _rollingSum = 0;
            _rollingSumSqDev = 0;
            _mean = 0;
            _count = 0;
        }
        else
        {
            _rollingSum = previousState.RollingSum;
            _rollingSumSqDev = previousState.RollingSumSqDev;
            _mean = previousState.Mean;
            _count = previousState.Count;
        }
    }
}

/// <summary>
/// Provides methods for creating StdDev hubs.
/// </summary>
public static partial class StdDev
{
    /// <summary>
    /// Creates a StdDev streaming hub with state management from a chain provider.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>A StdDev hub with state management.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static StdDevHubState ToStdDevHubState(
        this IChainProvider<IReusable> chainProvider,
        int lookbackPeriods = 14)
        => new(chainProvider, lookbackPeriods);
}
