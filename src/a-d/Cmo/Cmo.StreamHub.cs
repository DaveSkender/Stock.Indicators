namespace Skender.Stock.Indicators;


/// <summary>
/// Streaming hub for Chande Momentum Oscillator (CMO) calculations.
/// </summary>
public class CmoHub
    : ChainProvider<IReusable, CmoResult>, ICmo
 {
    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="CmoHub"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    internal CmoHub(
        IChainProvider<IReusable> provider,
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
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        double? cmo = null;

        if (i >= LookbackPeriods)
        {
            // Build a subset of provider cache for CMO calculation
            List<IReusable> subset = [];
            for (int k = 0; k <= i; k++)
            {
                subset.Add(ProviderCache[k]);
            }

            // Use the existing series calculation
            IReadOnlyList<CmoResult> seriesResults = subset.ToCmo(LookbackPeriods);
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


public static partial class Cmo
{
    /// <summary>
    /// Creates a CMO streaming hub from a chain provider.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <returns>A CMO hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static CmoHub ToCmo(
        this IChainProvider<IReusable> chainProvider,
        int lookbackPeriods = 14)
        => new(chainProvider, lookbackPeriods);

    /// <summary>
    /// Creates a Cmo hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="lookbackPeriods"></param>
    /// <returns>An instance of <see cref="CmoHub"/>.</returns>
    public static CmoHub ToCmoHub(
        this IReadOnlyList<IQuote> quotes, int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToCmo(14);
    }
}

