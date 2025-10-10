namespace Skender.Stock.Indicators;

// BETA (STREAM HUB)

/// <summary>
/// Provides methods for calculating the Beta coefficient.
/// </summary>
public static partial class Beta
{
    /// <summary>
    /// Creates a Beta hub from two synchronized chain providers.
    /// Note: Both providers must be synchronized (same timestamps).
    /// </summary>
    /// <typeparam name="T">The type of the reusable data (must be the same for both providers).</typeparam>
    /// <param name="providerEval">The evaluation asset chain provider.</param>
    /// <param name="providerMrkt">The market chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <param name="type">The type of Beta calculation. Default is <see cref="BetaType.Standard"/>.</param>
    /// <returns>A Beta hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when either provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static BetaHub<T> ToBetaHub<T>(
        this IChainProvider<T> providerEval,
        IChainProvider<T> providerMrkt,
        int lookbackPeriods,
        BetaType type = BetaType.Standard)
        where T : IReusable
        => new(providerEval, providerMrkt, lookbackPeriods, type);
}

/// <summary>
/// Represents a hub for Beta calculations between two synchronized series.
/// </summary>
/// <typeparam name="TIn">The type of the input data.</typeparam>
public class BetaHub<TIn>
    : PairsProvider<TIn, BetaResult>, IBeta
    where TIn : IReusable
{
    private readonly string hubName;
    private readonly bool calcSd;
    private readonly bool calcUp;
    private readonly bool calcDn;

    // State tracking for returns calculation
    private double _prevEval;
    private double _prevMrkt;
    private bool _isFirst = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="BetaHub{TIn}"/> class.
    /// </summary>
    /// <param name="providerEval">The evaluation asset chain provider.</param>
    /// <param name="providerMrkt">The market chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <param name="type">The type of Beta calculation.</param>
    /// <exception cref="ArgumentNullException">Thrown when either provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    internal BetaHub(
        IChainProvider<TIn> providerEval,
        IChainProvider<TIn> providerMrkt,
        int lookbackPeriods,
        BetaType type) : base(providerEval, providerMrkt)
    {
        ArgumentNullException.ThrowIfNull(providerMrkt);
        Beta.Validate<TIn>([], [], lookbackPeriods);

        LookbackPeriods = lookbackPeriods;
        Type = type;
        hubName = $"BETA({lookbackPeriods},{type})";

        calcSd = type is BetaType.All or BetaType.Standard;
        calcUp = type is BetaType.All or BetaType.Up;
        calcDn = type is BetaType.All or BetaType.Down;

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
        ToIndicator(TIn item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Validate timestamps match
        if (i < ProviderCacheB.Count)
        {
            ValidateTimestampSync(i, item);
        }

        // Get current values
        double evalValue = item.Value;
        double mrktValue = i < ProviderCacheB.Count ? ProviderCacheB[i].Value : 0;

        // Calculate returns
        double evalReturn = _isFirst ? 0 : (_prevEval != 0 ? (evalValue / _prevEval) - 1d : 0);
        double mrktReturn = _isFirst ? 0 : (_prevMrkt != 0 ? (mrktValue / _prevMrkt) - 1d : 0);

        _prevEval = evalValue;
        _prevMrkt = mrktValue;
        _isFirst = false;

        // Check if we have enough data in both caches
        if (HasSufficientData(i, LookbackPeriods))
        {
            double? beta = null;
            double? betaUp = null;
            double? betaDown = null;
            double? ratio = null;
            double? convexity = null;

            // Calculate beta variants
            if (calcSd)
            {
                beta = CalcBetaWindow(i, LookbackPeriods, BetaType.Standard);
            }

            if (calcDn)
            {
                betaDown = CalcBetaWindow(i, LookbackPeriods, BetaType.Down);
            }

            if (calcUp)
            {
                betaUp = CalcBetaWindow(i, LookbackPeriods, BetaType.Up);
            }

            // Ratio and convexity
            if (Type == BetaType.All && betaUp != null && betaDown != null)
            {
                ratio = betaDown != 0 ? betaUp / betaDown : null;
                convexity = (betaUp - betaDown) * (betaUp - betaDown);
            }

            BetaResult r = new(
                Timestamp: item.Timestamp,
                Beta: beta,
                BetaUp: betaUp,
                BetaDown: betaDown,
                Ratio: ratio,
                Convexity: convexity,
                ReturnsEval: evalReturn,
                ReturnsMrkt: mrktReturn);

            return (r, i);
        }
        else
        {
            // Not enough data yet
            BetaResult r = new(
                Timestamp: item.Timestamp,
                ReturnsEval: evalReturn,
                ReturnsMrkt: mrktReturn);

            return (r, i);
        }
    }

    /// <summary>
    /// Calculates the Beta value for a specific window of data from the caches.
    /// </summary>
    /// <param name="currentIndex">The current index in the data.</param>
    /// <param name="lookbackPeriods">The number of periods to look back.</param>
    /// <param name="type">The type of Beta calculation.</param>
    /// <returns>The calculated Beta value.</returns>
    private double? CalcBetaWindow(
        int currentIndex,
        int lookbackPeriods,
        BetaType type)
    {
        // Note: BetaType.All is ineligible for this method

        // Initialize
        List<double> dataA = new(lookbackPeriods);
        List<double> dataB = new(lookbackPeriods);

        // Extract returns from cache for the window
        for (int p = currentIndex - lookbackPeriods + 1; p <= currentIndex; p++)
        {
            if (p < 0 || p >= Cache.Count)
            {
                continue;
            }

            BetaResult cached = Cache[p];
            double mrktReturn = cached.ReturnsMrkt ?? 0;
            double evalReturn = cached.ReturnsEval ?? 0;

            if (type is BetaType.Standard
            || (type is BetaType.Down && mrktReturn < 0)
            || (type is BetaType.Up && mrktReturn > 0))
            {
                dataA.Add(mrktReturn);
                dataB.Add(evalReturn);
            }
        }

        if (dataA.Count == 0)
        {
            return null;
        }

        // Calculate correlation, covariance, and variance
        CorrResult c = Correlation.PeriodCorrelation(
            default,
            [.. dataA],
            [.. dataB]);

        // Calculate beta
        if (c.VarianceA != 0)
        {
            return (c.Covariance / c.VarianceA).NaN2Null();
        }

        return null;
    }
}
