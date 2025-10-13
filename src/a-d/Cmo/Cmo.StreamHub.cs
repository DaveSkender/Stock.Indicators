namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Chande Momentum Oscillator (CMO) indicator.
/// </summary>
public static partial class Cmo
{
    /// <summary>
    /// Creates a CMO streaming hub from a chain provider.
    /// </summary>
    /// <typeparam name="T">The type of the reusable data.</typeparam>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <returns>A CMO hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static CmoHub<T> ToCmo<T>(
        this IChainProvider<T> chainProvider,
        int lookbackPeriods = 14)
        where T : IReusable
        => new(chainProvider, lookbackPeriods);
}

/// <summary>
/// Streaming hub for Chande Momentum Oscillator (CMO) calculations.
/// </summary>
/// <typeparam name="TIn">The type of the input data.</typeparam>
public class CmoHub<TIn>
    : ChainProvider<TIn, CmoResult>, ICmo
    where TIn : IReusable
{
    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="CmoHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    internal CmoHub(
        IChainProvider<TIn> provider,
        int lookbackPeriods) : base(provider)
    {
        Cmo.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        hubName = $"CMO({lookbackPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (CmoResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        double? cmo = null;

        if (i >= LookbackPeriods)
        {
            // Build a subset of provider cache for CMO calculation
            List<TIn> subset = [];
            for (int k = 0; k <= i; k++)
            {
                subset.Add(ProviderCache[k]);
            }

            // Use the existing series calculation
            IReadOnlyList<CmoResult> seriesResults = ((IReadOnlyList<IReusable>)subset).ToCmo(LookbackPeriods);
            CmoResult? latestResult = seriesResults.Count > 0 ? seriesResults[seriesResults.Count - 1] : null;

            cmo = latestResult?.Cmo;
        }

        // candidate result
        CmoResult r = new(
            Timestamp: item.Timestamp,
            Cmo: cmo);

        return (r, i);
    }
}
