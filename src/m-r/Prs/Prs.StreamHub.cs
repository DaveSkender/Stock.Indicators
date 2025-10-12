namespace Skender.Stock.Indicators;

// PRS (STREAM HUB)

/// <summary>
/// Provides methods for calculating the Price Relative Strength (PRS) series.
/// </summary>
public static partial class Prs
{
    /// <summary>
    /// Creates a PRS hub from two synchronized chain providers.
    /// Note: Both providers must be synchronized (same timestamps).
    /// </summary>
    /// <typeparam name="T">The type of the reusable data (must be the same for both providers).</typeparam>
    /// <param name="providerEval">The evaluation asset chain provider.</param>
    /// <param name="providerBase">The base/benchmark chain provider.</param>
    /// <param name="lookbackPeriods">
    /// The number of periods for the PRS% lookback calculation. Optional.
    /// Use int.MinValue to disable PrsPercent calculation.
    /// </param>
    /// <returns>A PRS hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when either provider is null.</exception>
    public static PrsHub<T> ToPrsHub<T>(
        this IChainProvider<T> providerEval,
        IChainProvider<T> providerBase,
        int lookbackPeriods = int.MinValue)
        where T : IReusable
        => new(providerEval, providerBase, lookbackPeriods);
}

/// <summary>
/// Represents a hub for Price Relative Strength (PRS) calculations between two synchronized series.
/// </summary>
/// <typeparam name="TIn">The type of the input data.</typeparam>
public class PrsHub<TIn>
    : PairsProvider<TIn, PrsResult>
    where TIn : IReusable
{
    private readonly string hubName;
    private readonly int lookbackPeriods;

    /// <summary>
    /// Initializes a new instance of the <see cref="PrsHub{TIn}"/> class.
    /// </summary>
    /// <param name="providerEval">The evaluation asset chain provider.</param>
    /// <param name="providerBase">The base/benchmark chain provider.</param>
    /// <param name="lookbackPeriods">
    /// The number of periods for the PRS% lookback calculation.
    /// Use int.MinValue to disable PrsPercent calculation.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when either provider is null.</exception>
    internal PrsHub(
        IChainProvider<TIn> providerEval,
        IChainProvider<TIn> providerBase,
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
        ToIndicator(TIn item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Determine minimum required periods (at least 1 for current, more for lookback)
        int minimumPeriods = lookbackPeriods > 0 ? lookbackPeriods + 1 : 1;

        // Check if both caches have sufficient data
        if (!HasSufficientData(i, minimumPeriods))
        {
            // Not enough data in one or both streams yet
            return (new PrsResult(Timestamp: item.Timestamp, Prs: null, PrsPercent: null), i);
        }

        // Validate timestamps match
        ValidateTimestampSync(i, item);

        // Get current values
        double evalValue = item.Value;
        double baseValue = ProviderCacheB[i].Value;

        // Calculate PRS (relative strength ratio)
        double? prs = baseValue == 0
            ? null
            : (evalValue / baseValue).NaN2Null();

        // Calculate PRS% if lookback is specified
        double? prsPercent = null;

        if (lookbackPeriods > 0 && i >= lookbackPeriods)
        {
            // Get values from lookback periods ago
            TIn evalOld = ProviderCache[i - lookbackPeriods];
            TIn baseOld = ProviderCacheB[i - lookbackPeriods];

            if (baseOld.Value != 0 && evalOld.Value != 0)
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
