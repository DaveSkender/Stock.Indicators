namespace Skender.Stock.Indicators;

// PRS (STREAM HUB)

/// <summary>
/// Provides methods for calculating the Price Relative Strength (PRS) series.
/// </summary>
public class PrsHub
    : PairsProvider<IReusable, PrsResult>
{
    private readonly string hubName;
    private readonly int lookbackPeriods;

    /// <summary>
    /// Initializes a new instance of the <see cref="PrsHub"/> class.
    /// </summary>
    /// <param name="providerEval">The evaluation asset chain provider.</param>
    /// <param name="providerBase">The base/benchmark chain provider.</param>
    /// <param name="lookbackPeriods">
    /// The number of periods for the PRS% lookback calculation.
    /// Use int.MinValue to disable PrsPercent calculation.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when either provider is null.</exception>
    internal PrsHub(
        IChainProvider<IReusable> providerEval,
        IChainProvider<IReusable> providerBase,
        int lookbackPeriods) : base(providerEval, providerBase)
    {
        ArgumentNullException.ThrowIfNull(providerBase);

        // Validate lookback periods
        if (lookbackPeriods is <= 0 and not int.MinValue)
        {
            throw new ArgumentOutOfRangeException(
                nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Price Relative Strength.");
        }

        this.lookbackPeriods = lookbackPeriods;
        hubName = lookbackPeriods == int.MinValue
            ? "PRS"
            : $"PRS({lookbackPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (PrsResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // PRS only requires 1 period; PrsPercent requires lookbackPeriods + 1
        if (!HasSufficientData(i, 1))
        {
            // Not enough data in one or both streams yet
            return (new PrsResult(Timestamp: item.Timestamp, Prs: null, PrsPercent: null), i);
        }

        // Validate timestamps match
        ValidateTimestampSync(i, item);

        // Get current values - safe to access now after HasSufficientData check
        double evalValue = item.Value;
        double baseValue = ProviderCacheB[i].Value;

        // Calculate PRS (relative strength ratio)
        double? prs = Math.Abs(baseValue) < double.Epsilon
            ? null
            : (evalValue / baseValue).NaN2Null();

        // Calculate PRS% if lookback is specified
        double? prsPercent = null;

        if (lookbackPeriods > 0 && i >= lookbackPeriods)
        {
            // Get values from lookback periods ago - safe because HasSufficientData already verified
            IReusable evalOld = ProviderCache[i - lookbackPeriods];
            IReusable baseOld = ProviderCacheB[i - lookbackPeriods];

            if (Math.Abs(baseOld.Value) >= double.Epsilon && Math.Abs(evalOld.Value) >= double.Epsilon)
            {
                double? pctBase = (baseValue - baseOld.Value) / baseOld.Value;
                double? pctEval = (evalValue - evalOld.Value) / evalOld.Value;

                prsPercent = (pctEval - pctBase).NaN2Null();
            }
        }

        PrsResult r = new(
            Timestamp: item.Timestamp,
            Prs: prs,
            PrsPercent: prsPercent);

        return (r, i);
    }
}


public static partial class Prs
{
    /// <summary>
    /// Creates a PRS hub from two synchronized chain providers.
    /// Note: Both providers must be synchronized (same timestamps).
    /// </summary>

    /// <param name="providerEval">The evaluation asset chain provider.</param>
    /// <param name="providerBase">The base/benchmark chain provider.</param>
    /// <param name="lookbackPeriods">
    /// The number of periods for the PRS% lookback calculation. Optional.
    /// Use int.MinValue to disable PrsPercent calculation.
    /// </param>
    /// <returns>A PRS hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when either provider is null.</exception>
    public static PrsHub ToPrsHub(
        this IChainProvider<IReusable> providerEval,
        IChainProvider<IReusable> providerBase,
        int lookbackPeriods = int.MinValue)
    {
        ArgumentNullException.ThrowIfNull(providerEval);
        ArgumentNullException.ThrowIfNull(providerBase);
        return new PrsHub(providerEval, providerBase, lookbackPeriods);
    }

}
