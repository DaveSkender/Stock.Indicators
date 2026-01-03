namespace Skender.Stock.Indicators;

/// <summary>
/// State for PMO calculations using StreamHubState pattern.
/// </summary>
/// <param name="PrevRocEma">Previous ROC EMA value</param>
/// <param name="PrevPmo">Previous PMO value</param>
public record PmoState(double PrevRocEma, double PrevPmo) : IHubState;

/// <summary>
/// Streaming hub for Price Momentum Oscillator (PMO) with state caching.
/// </summary>
public class PmoHubState
    : ChainHubState<IReusable, PmoState, PmoResult>, IPmo
{
    private readonly double smoothingConstant1;
    private readonly double smoothingConstant2;
    private readonly double smoothingConstant3;

    private double prevRocEma;
    private double prevPmo;

    internal PmoHubState(
        IChainProvider<IReusable> provider,
        int timePeriods,
        int smoothPeriods,
        int signalPeriods) : base(provider)
    {
        Pmo.Validate(timePeriods, smoothPeriods, signalPeriods);
        TimePeriods = timePeriods;
        SmoothPeriods = smoothPeriods;
        SignalPeriods = signalPeriods;

        smoothingConstant1 = 2d / smoothPeriods;
        smoothingConstant2 = 2d / timePeriods;
        smoothingConstant3 = 2d / (signalPeriods + 1);

        Name = $"PMO({timePeriods},{smoothPeriods},{signalPeriods})";

        prevRocEma = double.NaN;
        prevPmo = double.NaN;

        Reinitialize();
    }

    /// <inheritdoc/>
    public int TimePeriods { get; init; }

    /// <inheritdoc/>
    public int SmoothPeriods { get; init; }

    /// <inheritdoc/>
    public int SignalPeriods { get; init; }

    /// <inheritdoc/>
    protected override (PmoResult result, PmoState state, int index)
        ToIndicatorState(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        double currentValue = item.Value;
        double prevValue = i > 0 ? ProviderCache[i - 1].Value : double.NaN;

        // Calculate rate of change (ROC)
        double roc = CalculateRoc(currentValue, prevValue);

        // Restore state if needed (after rollback)
        RestoreStateIfNeeded(i);

        // Calculate ROC EMA (first smoothing with timePeriods)
        double rocEma = CalculateRocEma(i, roc);
        double rocEmaScaled = rocEma * 10;
        prevRocEma = rocEma;

        // Calculate PMO (second smoothing with smoothPeriods)
        double pmoValue = CalculatePmoValue(i, rocEmaScaled);
        prevPmo = pmoValue;

        // Calculate Signal (third smoothing with signalPeriods)
        double signalValue = CalculateSignalValue(i, pmoValue);

        // Candidate result
        PmoResult r = new(
            Timestamp: item.Timestamp,
            Pmo: pmoValue.NaN2Null(),
            Signal: signalValue.NaN2Null());

        // Save state for rapid same-candle updates
        PmoState state = new(prevRocEma, prevPmo);

        return (r, state, i);
    }

    /// <inheritdoc/>
    protected override void RestorePreviousState(PmoState? previousState)
    {
        if (previousState is null)
        {
            // Reset state
            prevRocEma = double.NaN;
            prevPmo = double.NaN;
        }
        else
        {
            // Fast restore from cached state
            prevRocEma = previousState.PrevRocEma;
            prevPmo = previousState.PrevPmo;
        }
    }

    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // Let base handle cache management and fast-path detection
        base.RollbackState(timestamp);

        // If base called RestorePreviousState with non-null, we're done (fast path)
        // Otherwise, we need to do full reconstruction (slow path after reset to NaN)
        if (double.IsNaN(prevRocEma) || double.IsNaN(prevPmo))
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
            if (targetIndex < TimePeriods)
            {
                return;
            }

            // Replay history to reconstruct state
            RestoreStateIfNeeded(targetIndex + 1);
        }
    }

    private static double CalculateRoc(double currentValue, double prevValue)
        => prevValue == 0 ? double.NaN : 100 * ((currentValue / prevValue) - 1);

    private void RestoreStateIfNeeded(int index)
    {
        if (index <= TimePeriods || !double.IsNaN(prevRocEma))
        {
            return;
        }

        // Recalculate state by replaying from the first calculable position
        double tempPrevRocEma = double.NaN;
        double tempPrevPmo = double.NaN;
        double tempPrevSignal = double.NaN;

        // Need to store PMO values for Signal calculation
        double[] pmoValues = new double[index];

        for (int p = 0; p < index; p++)
        {
            double pCurrVal = ProviderCache[p].Value;
            double pPrevVal = p > 0 ? ProviderCache[p - 1].Value : double.NaN;
            double pRoc = CalculateRoc(pCurrVal, pPrevVal);

            // Calculate ROC EMA
            double pRocEma = CalculateRocEmaForRestore(p, pRoc, ref tempPrevRocEma);
            double pRocEmaScaled = pRocEma * 10;
            tempPrevRocEma = pRocEma;

            // Calculate PMO
            double pPmo = CalculatePmoForRestore(p, pRocEmaScaled, ref tempPrevPmo);
            tempPrevPmo = pPmo;
            pmoValues[p] = pPmo;

            // Calculate Signal
            tempPrevSignal = CalculateSignalForRestore(p, pPmo, pmoValues, ref tempPrevSignal);
        }

        prevRocEma = tempPrevRocEma;
        prevPmo = tempPrevPmo;
    }

    private double CalculateRocEmaForRestore(int position, double roc, ref double tempPrevRocEma)
    {
        if (double.IsNaN(tempPrevRocEma) && position >= TimePeriods)
        {
            return InitRocEma(position);
        }

        if (!double.IsNaN(tempPrevRocEma))
        {
            return tempPrevRocEma + (smoothingConstant2 * (roc - tempPrevRocEma));
        }

        return double.NaN;
    }

    private double CalculatePmoForRestore(int position, double rocEmaScaled, ref double tempPrevPmo)
    {
        if (double.IsNaN(tempPrevPmo) && position >= SmoothPeriods + TimePeriods - 1)
        {
            return InitPmo(position);
        }

        if (!double.IsNaN(tempPrevPmo))
        {
            return tempPrevPmo + (smoothingConstant1 * (rocEmaScaled - tempPrevPmo));
        }

        return double.NaN;
    }

    private double CalculateSignalForRestore(int position, double pmo, double[] pmoValues, ref double tempPrevSignal)
    {
        if (double.IsNaN(tempPrevSignal) && position >= SignalPeriods + SmoothPeriods + TimePeriods - 2)
        {
            // Initialize from pmoValues array
            double sum = 0;
            for (int j = position - SignalPeriods + 1; j <= position; j++)
            {
                sum += pmoValues[j];
            }

            return sum / SignalPeriods;
        }

        if (!double.IsNaN(tempPrevSignal))
        {
            return Ema.Increment(smoothingConstant3, tempPrevSignal, pmo);
        }

        return double.NaN;
    }

    private double CalculateRocEma(int index, double roc)
    {
        if (index < TimePeriods)
        {
            return double.NaN;
        }

        if (index > 0 && !double.IsNaN(prevRocEma))
        {
            return prevRocEma + (smoothingConstant2 * (roc - prevRocEma));
        }

        return InitRocEma(index);
    }

    private double CalculatePmoValue(int index, double rocEmaScaled)
    {
        if (index < SmoothPeriods + TimePeriods - 1)
        {
            return double.NaN;
        }

        if (index > 0 && !double.IsNaN(prevPmo))
        {
            return prevPmo + (smoothingConstant1 * (rocEmaScaled - prevPmo));
        }

        return InitPmo(index);
    }

    private double CalculateSignalValue(int index, double pmoValue)
    {
        if (index >= SignalPeriods + SmoothPeriods + TimePeriods - 2
            && (index == 0 || Cache[index - 1].Signal is null))
        {
            return InitSignal(index, pmoValue);
        }

        return Ema.Increment(
            smoothingConstant3,
            index > 0 ? Cache[index - 1].Signal ?? double.NaN : double.NaN,
            pmoValue);
    }

    private double InitSignal(int endIndex, double currentPmo)
    {
        // Initialize signal as SMA of PMO values - match series code pattern exactly
        double sum = 0;  // Start from 0 like series code
        for (int j = endIndex - SignalPeriods + 1; j < endIndex; j++)  // Previous PMO values from Cache
        {
            sum += Cache[j].Pmo ?? double.NaN;
        }

        sum += currentPmo;  // Add current PMO value last
        return sum / SignalPeriods;
    }

    private double InitRocEma(int endIndex)
    {
        double sum = 0;
        for (int p = endIndex - TimePeriods + 1; p <= endIndex; p++)
        {
            double pCurrVal = ProviderCache[p].Value;
            double pPrevVal = p > 0 ? ProviderCache[p - 1].Value : double.NaN;
            double pRoc = pPrevVal == 0 ? double.NaN : 100 * ((pCurrVal / pPrevVal) - 1);
            sum += pRoc;
        }

        return sum / TimePeriods;
    }

    private double InitPmo(int endIndex)
    {
        // Need to rebuild ROC EMA values for this window
        double sum = 0;
        double tempPrevRocEma = double.NaN;

        for (int p = endIndex - SmoothPeriods + 1; p <= endIndex; p++)
        {
            double pCurrVal = ProviderCache[p].Value;
            double pPrevVal = p > 0 ? ProviderCache[p - 1].Value : double.NaN;
            double pRoc = pPrevVal == 0 ? double.NaN : 100 * ((pCurrVal / pPrevVal) - 1);

            double pRocEma;
            if (double.IsNaN(tempPrevRocEma) && p >= TimePeriods)
            {
                pRocEma = InitRocEma(p);
            }
            else if (!double.IsNaN(tempPrevRocEma))
            {
                pRocEma = tempPrevRocEma + (smoothingConstant2 * (pRoc - tempPrevRocEma));
            }
            else
            {
                pRocEma = double.NaN;
            }

            tempPrevRocEma = pRocEma;
            sum += pRocEma * 10;
        }

        return sum / SmoothPeriods;
    }
}

public static partial class Pmo
{
    /// <summary>
    /// Creates a PMO streaming hub with state caching from a chain provider.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="timePeriods">The number of periods for the time span.</param>
    /// <param name="smoothPeriods">The number of periods for smoothing.</param>
    /// <param name="signalPeriods">The number of periods for the signal line.</param>
    /// <returns>A PMO hub with state caching.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the parameters are invalid.</exception>
    public static PmoHubState ToPmoHubState(
        this IChainProvider<IReusable> chainProvider,
        int timePeriods = 35,
        int smoothPeriods = 20,
        int signalPeriods = 10)
        => new(chainProvider, timePeriods, smoothPeriods, signalPeriods);
}
