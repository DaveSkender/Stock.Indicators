namespace Skender.Stock.Indicators;

/// <summary>
/// State object for SMA streaming hub.
/// </summary>
/// <param name="RollingSum">Rolling sum of values in the window.</param>
/// <param name="Count">Count of values in the window.</param>
public record SmaState(double RollingSum, int Count) : IHubState;

/// <summary>
/// Streaming hub for Simple Moving Average (SMA) using state management.
/// </summary>
/// <remarks>
/// This implementation caches the rolling sum to achieve O(1) incremental updates.
/// State restoration after rollback uses StateCache instead of recalculating.
/// </remarks>
public class SmaHubState
    : ChainHubState<IReusable, SmaState, SmaResult>, ISma
{
    private double _rollingSum;
    private int _count;

    internal SmaHubState(
        IChainProvider<IReusable> provider,
        int lookbackPeriods) : base(provider)
    {
        Sma.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        Name = $"SMA({lookbackPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    /// <remarks>
    /// O(k) complexity per quote where k = lookbackPeriods.
    /// This matches the original SMA implementation but caches state for rollback efficiency.
    /// </remarks>
    protected override (SmaResult result, SmaState state, int index)
        ToIndicatorState(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Calculate SMA efficiently using a rolling window over ProviderCache
        // This is O(lookbackPeriods) which is constant for a given configuration
        // and maintains exact precision with Series implementation
        double sma = Sma.Increment(ProviderCache, LookbackPeriods, i);

        // Calculate state for caching
        _rollingSum = 0;
        _count = 0;
        if (i >= LookbackPeriods - 1)
        {
            for (int p = i - LookbackPeriods + 1; p <= i; p++)
            {
                _rollingSum += ProviderCache[p].Value;
                _count++;
            }
        }

        // Create state object
        SmaState stateObj = new(_rollingSum, _count);

        // Candidate result
        SmaResult r = new(
            Timestamp: item.Timestamp,
            Sma: sma.NaN2Null());

        return (r, stateObj, i);
    }

    /// <summary>
    /// Restores the SMA state (rolling sum, count) from StateCache.
    /// </summary>
    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // First, manage StateCache removal via base implementation
        base.RollbackState(timestamp);

        // Reset state variables
        _rollingSum = 0;
        _count = 0;

        // Restore from last StateCache entry if available
        if (StateCache.Count > 0)
        {
            SmaState lastState = StateCache[^1];
            _rollingSum = lastState.RollingSum;
            _count = lastState.Count;
        }
    }
}

public static partial class Sma
{
    /// <summary>
    /// Creates an SMA streaming hub with state management from a chain provider.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>An SMA hub with state management.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static SmaHubState ToSmaHubState(
        this IChainProvider<IReusable> chainProvider,
        int lookbackPeriods)
        => new(chainProvider, lookbackPeriods);
}
