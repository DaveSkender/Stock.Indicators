namespace Skender.Stock.Indicators;

/// <summary>
/// State object for RSI streaming hub.
/// </summary>
/// <param name="AvgGain">Average gain over lookback period.</param>
/// <param name="AvgLoss">Average loss over lookback period.</param>
public record RsiState(double AvgGain, double AvgLoss) : IHubState;

/// <summary>
/// Streaming hub for Relative Strength Index (RSI) using state management.
/// </summary>
/// <remarks>
/// This implementation uses O(1) incremental updates per quote via Wilder's smoothing.
/// State restoration after rollback uses StateCache instead of recalculating, eliminating
/// the need for complex RollbackState logic.
/// </remarks>
public class RsiHubState
    : ChainHubState<IReusable, RsiState, RsiResult>, IRsi
{
    private double _avgGain = double.NaN;
    private double _avgLoss = double.NaN;

    internal RsiHubState(
        IChainProvider<IReusable> provider,
        int lookbackPeriods) : base(provider)
    {
        Rsi.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        Name = $"RSI({lookbackPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    /// <remarks>
    /// O(1) complexity per quote using Wilder's smoothing for incremental updates.
    /// The initial SMA calculation at lookback period is O(k) where k = lookbackPeriods.
    /// </remarks>
    protected override (RsiResult result, RsiState state, int index)
        ToIndicatorState(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        double? rsi = null;
        double currentValue = item.Value;

        // Get previous value for gain/loss calculation
        double prevValue = i > 0 ? ProviderCache[i - 1].Value : double.NaN;

        // Calculate current gain/loss - O(1)
        double gain;
        double loss;
        if (!double.IsNaN(currentValue) && !double.IsNaN(prevValue))
        {
            gain = currentValue > prevValue ? currentValue - prevValue : 0;
            loss = currentValue < prevValue ? prevValue - currentValue : 0;
        }
        else
        {
            gain = loss = double.NaN;
        }

        // Re/initialize average gain/loss when needed - O(k)
        // This happens at first calculable position OR when state is NaN
        if (i >= LookbackPeriods && (double.IsNaN(_avgGain) || double.IsNaN(_avgLoss)))
        {
            (double sumGain, double sumLoss) = CalculateInitialSums(i);

            _avgGain = sumGain / LookbackPeriods;
            _avgLoss = sumLoss / LookbackPeriods;

            rsi = CalculateRsi(_avgGain, _avgLoss);
        }
        // Incremental update using Wilder's smoothing - O(1)
        else if (i > LookbackPeriods)
        {
            if (!double.IsNaN(gain) && !double.IsNaN(_avgGain) && !double.IsNaN(_avgLoss))
            {
                // Wilder's smoothing: avgGain = ((prevAvgGain * (n-1)) + currentGain) / n
                _avgGain = ((_avgGain * (LookbackPeriods - 1)) + gain) / LookbackPeriods;
                _avgLoss = ((_avgLoss * (LookbackPeriods - 1)) + loss) / LookbackPeriods;

                rsi = CalculateRsi(_avgGain, _avgLoss);
            }
            else if (double.IsNaN(gain))
            {
                // NaN input contaminates state
                _avgGain = double.NaN;
                _avgLoss = double.NaN;
            }
            // else: state is NaN, will be re-initialized when valid data appears
        }

        // Create state object with current values
        RsiState stateObj = new(_avgGain, _avgLoss);

        // Candidate result
        RsiResult r = new(
            Timestamp: item.Timestamp,
            Rsi: rsi);

        return (r, stateObj, i);
    }

    /// <summary>
    /// Restores the RSI state from previous cached state.
    /// </summary>
    /// <param name="previousState">The cached state from one bar ago, or null to reset.</param>
    protected override void RestorePreviousState(RsiState? previousState)
    {
        if (previousState is null)
        {
            // Reset to initial state
            _avgGain = double.NaN;
            _avgLoss = double.NaN;
        }
        else
        {
            _avgGain = previousState.AvgGain;
            _avgLoss = previousState.AvgLoss;
        }
    }

    /// <summary>
    /// Rollback state with proper reconstruction for complex operations.
    /// Overrides base to provide full state reconstruction when needed.
    /// </summary>
    protected override void RollbackState(DateTime timestamp)
    {
        // Let base handle cache management and fast-path detection
        base.RollbackState(timestamp);

        // If base called RestorePreviousState with non-null, we're done (fast path)
        // Otherwise, we need to do full reconstruction (slow path after reset to NaN)
        if (double.IsNaN(_avgGain) || double.IsNaN(_avgLoss))
        {
            // Full reconstruction needed - find target index in ProviderCache
            int index = ProviderCache.IndexGte(timestamp);
            if (index == -1)
            {
                index = ProviderCache.Count;
            }

            // Target is the position just before where rebuild will start
            int targetIndex = index - 1;

            // Not enough data to initialize state
            if (targetIndex < LookbackPeriods)
            {
                return;
            }

            // Calculate initial averages at first calculable position
            (double sumGain, double sumLoss) = CalculateInitialSums(LookbackPeriods);
            _avgGain = sumGain / LookbackPeriods;
            _avgLoss = sumLoss / LookbackPeriods;

            // Apply Wilder's smoothing for subsequent positions up to targetIndex
            for (int p = LookbackPeriods + 1; p <= targetIndex; p++)
            {
                double pPrevVal = ProviderCache[p - 1].Value;
                double pCurrVal = ProviderCache[p].Value;

                if (!double.IsNaN(pCurrVal) && !double.IsNaN(pPrevVal))
                {
                    double pGain = pCurrVal > pPrevVal ? pCurrVal - pPrevVal : 0;
                    double pLoss = pCurrVal < pPrevVal ? pPrevVal - pCurrVal : 0;

                    _avgGain = ((_avgGain * (LookbackPeriods - 1)) + pGain) / LookbackPeriods;
                    _avgLoss = ((_avgLoss * (LookbackPeriods - 1)) + pLoss) / LookbackPeriods;
                }
                else
                {
                    // NaN contaminates state - will be re-initialized in ToIndicator
                    _avgGain = double.NaN;
                    _avgLoss = double.NaN;
                }
            }
        }
    }

    /// <summary>
    /// Calculates the initial sum of gains and losses over the lookback period.
    /// </summary>
    private (double sumGain, double sumLoss) CalculateInitialSums(int endIndex)
    {
        double sumGain = 0;
        double sumLoss = 0;

        for (int p = endIndex - LookbackPeriods + 1; p <= endIndex; p++)
        {
            double pPrevVal = ProviderCache[p - 1].Value;
            double pCurrVal = ProviderCache[p].Value;

            if (!double.IsNaN(pCurrVal) && !double.IsNaN(pPrevVal))
            {
                sumGain += pCurrVal > pPrevVal ? pCurrVal - pPrevVal : 0;
                sumLoss += pCurrVal < pPrevVal ? pPrevVal - pCurrVal : 0;
            }
            else
            {
                // NaN contaminates the sum - exit early
                return (double.NaN, double.NaN);
            }
        }

        return (sumGain, sumLoss);
    }

    /// <summary>
    /// Calculates RSI from average gain and loss.
    /// Matches Series parity by checking division result for NaN.
    /// </summary>
    private static double? CalculateRsi(double avgGain, double avgLoss)
    {
        // Check if division would produce NaN (e.g., 0/0 case)
        // This matches Series parity which checks !double.IsNaN(avgGain / avgLoss)
        if (double.IsNaN(avgGain / avgLoss))
        {
            return null;
        }

        if (avgLoss > 0)
        {
            double rs = avgGain / avgLoss;
            return 100 - (100 / (1 + rs));
        }

        return 100;
    }
}

public static partial class Rsi
{
    /// <summary>
    /// Creates an RSI streaming hub with state management from a chain provider.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>An RSI hub with state management.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static RsiHubState ToRsiHubState(
        this IChainProvider<IReusable> chainProvider,
        int lookbackPeriods = 14)
        => new(chainProvider, lookbackPeriods);
}
