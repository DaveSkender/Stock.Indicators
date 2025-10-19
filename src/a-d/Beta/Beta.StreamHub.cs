namespace Skender.Stock.Indicators;

// BETA (STREAM HUB)

/// <summary>
/// Provides methods for calculating the Beta coefficient.
/// </summary>
public class BetaHub
    : PairsProvider<IReusable, BetaResult>, IBeta
 {
    private readonly string hubName;
    private readonly bool calcSd;
    private readonly bool calcUp;
    private readonly bool calcDn;

    // State tracking for returns calculation
    private double _prevEval;
    private double _prevMrkt;
    private bool _isFirst = true;

    // Rolling window state for Standard beta
    private RollingWindowState _stateSd;

    // Rolling window state for Up beta
    private RollingWindowState _stateUp;

    // Rolling window state for Down beta
    private RollingWindowState _stateDn;

    /// <summary>
    /// Initializes a new instance of the <see cref="BetaHub"/> class.
    /// </summary>
    /// <param name="providerEval">The evaluation asset chain provider.</param>
    /// <param name="providerMrkt">The market chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <param name="type">The type of Beta calculation.</param>
    /// <exception cref="ArgumentNullException">Thrown when either provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    internal BetaHub(
        IChainProvider<IReusable> providerEval,
        IChainProvider<IReusable> providerMrkt,
        int lookbackPeriods,
        BetaType type) : base(providerEval, providerMrkt)
    {
        ArgumentNullException.ThrowIfNull(providerMrkt);
        Beta.Validate<IReusable>([], [], lookbackPeriods);

        LookbackPeriods = lookbackPeriods;
        Type = type;
        hubName = $"BETA({lookbackPeriods},{type})";

        calcSd = type is BetaType.All or BetaType.Standard;
        calcUp = type is BetaType.All or BetaType.Up;
        calcDn = type is BetaType.All or BetaType.Down;

        _stateSd = new(lookbackPeriods);
        _stateUp = new(lookbackPeriods);
        _stateDn = new(lookbackPeriods);

        ResetState();
        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public BetaType Type { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (BetaResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Check if provider B is lagging - return early before updating state
        if (ProviderCacheB.Count <= i)
        {
            return (new BetaResult(Timestamp: item.Timestamp), i);
        }

        // Validate timestamps match
        ValidateTimestampSync(i, item);

        // Get current values
        double evalValue = item.Value;
        double mrktValue = ProviderCacheB[i].Value;

        // Calculate returns
        double evalReturn = _isFirst ? 0 : (_prevEval != 0 ? (evalValue / _prevEval) - 1d : 0);
        double mrktReturn = _isFirst ? 0 : (_prevMrkt != 0 ? (mrktValue / _prevMrkt) - 1d : 0);

        // Update state only after we have valid data from both providers
        _prevEval = evalValue;
        _prevMrkt = mrktValue;
        bool wasFirst = _isFirst;
        _isFirst = false;

        // Don't update rolling windows on first item (returns are 0)
        if (!wasFirst)
        {
            // Update Standard beta rolling window
            if (calcSd)
            {
                UpdateRollingWindow(evalReturn, mrktReturn, _stateSd);
            }

            // Update Up beta rolling window (only when market return is positive)
            if (calcUp && mrktReturn > 0)
            {
                UpdateRollingWindow(evalReturn, mrktReturn, _stateUp);
            }

            // Update Down beta rolling window (only when market return is negative)
            if (calcDn && mrktReturn < 0)
            {
                UpdateRollingWindow(evalReturn, mrktReturn, _stateDn);
            }
        }

        // Check if we have enough data in both caches for calculation
        if (!HasSufficientData(i, LookbackPeriods))
        {
            // Not enough data yet, but return the returns
            BetaResult r = new(
                Timestamp: item.Timestamp,
                ReturnsEval: evalReturn,
                ReturnsMrkt: mrktReturn);

            return (r, i);
        }

        double? beta = null;
        double? betaUp = null;
        double? betaDown = null;
        double? ratio = null;
        double? convexity = null;

        // Calculate beta variants from rolling window stats
        if (calcSd && _stateSd.WindowCount > 0)
        {
            beta = CalculateBetaFromStats(_stateSd);
        }

        if (calcUp && _stateUp.WindowCount > 0)
        {
            betaUp = CalculateBetaFromStats(_stateUp);
        }

        if (calcDn && _stateDn.WindowCount > 0)
        {
            betaDown = CalculateBetaFromStats(_stateDn);
        }

        // Ratio and convexity
        if (Type == BetaType.All && betaUp != null && betaDown != null)
        {
            ratio = betaDown != 0 ? betaUp / betaDown : null;
            convexity = (betaUp - betaDown) * (betaUp - betaDown);
        }

        BetaResult result = new(
            Timestamp: item.Timestamp,
            Beta: beta,
            BetaUp: betaUp,
            BetaDown: betaDown,
            Ratio: ratio,
            Convexity: convexity,
            ReturnsEval: evalReturn,
            ReturnsMrkt: mrktReturn);

        return (result, i);
    }

    /// <summary>
    /// Updates a rolling window with new values and maintains incremental statistics.
    /// </summary>
    /// <param name="evalReturn">The evaluation return to add.</param>
    /// <param name="mrktReturn">The market return to add.</param>
    /// <param name="state">The rolling window state to update.</param>
    private static void UpdateRollingWindow(
        double evalReturn,
        double mrktReturn,
        RollingWindowState state)
    {
        int capacity = state.WindowEval.Length;

        // If window is full, subtract the oldest values from the sums
        if (state.WindowCount == capacity)
        {
            double oldEval = state.WindowEval[state.WindowIndex];
            double oldMrkt = state.WindowMrkt[state.WindowIndex];

            state.SumEval -= oldEval;
            state.SumMrkt -= oldMrkt;
            state.SumEval2 -= oldEval * oldEval;
            state.SumMrkt2 -= oldMrkt * oldMrkt;
            state.SumCross -= oldEval * oldMrkt;
        }
        else
        {
            state.WindowCount++;
        }

        // Add new values to the window and sums
        state.WindowEval[state.WindowIndex] = evalReturn;
        state.WindowMrkt[state.WindowIndex] = mrktReturn;

        state.SumEval += evalReturn;
        state.SumMrkt += mrktReturn;
        state.SumEval2 += evalReturn * evalReturn;
        state.SumMrkt2 += mrktReturn * mrktReturn;
        state.SumCross += evalReturn * mrktReturn;

        // Advance circular buffer index
        state.WindowIndex = (state.WindowIndex + 1) % capacity;
    }

    /// <summary>
    /// Calculates Beta from incremental statistics (covariance / variance).
    /// </summary>
    /// <param name="state">The rolling window state containing statistics.</param>
    /// <returns>The calculated Beta value or null if variance is zero.</returns>
    private static double? CalculateBetaFromStats(RollingWindowState state)
    {
        if (state.WindowCount == 0)
        {
            return null;
        }

        // Calculate averages
        double avgEval = state.SumEval / state.WindowCount;
        double avgMrkt = state.SumMrkt / state.WindowCount;
        double avgMrkt2 = state.SumMrkt2 / state.WindowCount;
        double avgCross = state.SumCross / state.WindowCount;

        // Calculate variance and covariance
        double varMrkt = avgMrkt2 - (avgMrkt * avgMrkt);
        double cov = avgCross - (avgMrkt * avgEval);

        // Calculate beta
        return varMrkt != 0
            ? (cov / varMrkt).NaN2Null()
            : null;
    }

    /// <summary>
    /// Resets rolling state when rebuilding or reinitializing.
    /// </summary>
    /// <param name="timestamp">The timestamp to roll back to.</param>
    protected override void RollbackState(DateTime timestamp)
    {
        if (timestamp <= DateTime.MinValue)
        {
            ResetState();
            return;
        }

        // Find the rollback index
        int index = -1;
        for (int i = ProviderCache.Count - 1; i >= 0; i--)
        {
            if (ProviderCache[i].Timestamp < timestamp)
            {
                index = i;
                break;
            }
        }

        if (index < 0 || index >= ProviderCacheB.Count)
        {
            ResetState();
            return;
        }

        // Reset all state
        ResetState();

        // Rebuild rolling windows from cache up to the rollback point
        // Start from index 0 to rebuild the state correctly
        double prevEval = 0;
        double prevMrkt = 0;
        bool isFirst = true;

        for (int i = 0; i <= index; i++)
        {
            if (i >= ProviderCache.Count || i >= ProviderCacheB.Count)
            {
                break;
            }

            double evalValue = ProviderCache[i].Value;
            double mrktValue = ProviderCacheB[i].Value;

            // Calculate returns
            double evalReturn = isFirst ? 0 : (prevEval != 0 ? (evalValue / prevEval) - 1d : 0);
            double mrktReturn = isFirst ? 0 : (prevMrkt != 0 ? (mrktValue / prevMrkt) - 1d : 0);

            // Update previous values
            prevEval = evalValue;
            prevMrkt = mrktValue;

            // Don't update rolling windows on first item
            if (!isFirst)
            {
                // Update Standard beta rolling window
                if (calcSd && _stateSd != null)
                {
                    UpdateRollingWindow(evalReturn, mrktReturn, _stateSd);
                }

                // Update Up beta rolling window (only when market return is positive)
                if (calcUp && _stateUp != null && mrktReturn > 0)
                {
                    UpdateRollingWindow(evalReturn, mrktReturn, _stateUp);
                }

                // Update Down beta rolling window (only when market return is negative)
                if (calcDn && _stateDn != null && mrktReturn < 0)
                {
                    UpdateRollingWindow(evalReturn, mrktReturn, _stateDn);
                }
            }

            isFirst = false;
        }

        // Update the final state tracking variables
        _prevEval = prevEval;
        _prevMrkt = prevMrkt;
        _isFirst = false;
    }

    private void ResetState()
    {
        _prevEval = 0;
        _prevMrkt = 0;
        _isFirst = true;

        // Initialize Standard beta rolling window
        if (calcSd)
        {
            _stateSd = new RollingWindowState(LookbackPeriods);
        }

        // Initialize Up beta rolling window
        if (calcUp)
        {
            _stateUp = new RollingWindowState(LookbackPeriods);
        }

        // Initialize Down beta rolling window
        if (calcDn)
        {
            _stateDn = new RollingWindowState(LookbackPeriods);
        }
    }

    /// <summary>
    /// Encapsulates rolling window state for incremental Beta calculations.
    /// </summary>
    private sealed class RollingWindowState(int capacity)
    {
        public double[] WindowEval = new double[capacity];
        public double[] WindowMrkt = new double[capacity];
        public int WindowIndex;
        public int WindowCount;
        public double SumEval;
        public double SumMrkt;
        public double SumEval2;
        public double SumMrkt2;
        public double SumCross;

        public void Reset()
        {
            WindowIndex = 0;
            WindowCount = 0;
            SumEval = 0;
            SumMrkt = 0;
            SumEval2 = 0;
            SumMrkt2 = 0;
            SumCross = 0;
        }
    }
}


public static partial class Beta
{
    /// <summary>
    /// Creates a Beta hub from two synchronized chain providers.
    /// Note: Both providers must be synchronized (same timestamps).
    /// </summary>
    /// <param name="providerEval">The evaluation asset chain provider.</param>
    /// <param name="providerMrkt">The market chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <param name="type">The type of Beta calculation. Default is <see cref="BetaType.Standard"/>.</param>
    /// <returns>A Beta hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when either provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static BetaHub ToBetaHub(
        this IChainProvider<IReusable> providerEval,
        IChainProvider<IReusable> providerMrkt,
        int lookbackPeriods,
        BetaType type = BetaType.Standard)
        => new(providerEval, providerMrkt, lookbackPeriods, type);

}
