namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Commodity Channel Index (CCI) indicator.
/// </summary>
public static partial class Cci
{
    /// <summary>
    /// Creates a CCI hub from a quote provider.
    /// </summary>
    /// <typeparam name="T">The type of the quote data.</typeparam>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <returns>A CCI hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quote provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static CciHub<T> ToCci<T>(
        this IQuoteProvider<T> quoteProvider,
        int lookbackPeriods = 20)
        where T : IQuote
        => new(quoteProvider, lookbackPeriods);
}

/// <summary>
/// Represents a hub for Commodity Channel Index (CCI) calculations.
/// </summary>
/// <typeparam name="TIn">The type of the input data.</typeparam>
public class CciHub<TIn>
    : ChainProvider<TIn, CciResult>, ICci
    where TIn : IQuote
{
    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="CciHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    internal CciHub(
        IQuoteProvider<TIn> provider,
        int lookbackPeriods) : base(provider)
    {
        Cci.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        hubName = $"CCI({lookbackPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (CciResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        double? cci = null;

        if (i >= LookbackPeriods - 1)
        {
            // Build a subset of provider cache for CCI calculation
            List<TIn> subset = [];
            for (int k = 0; k <= i; k++)
            {
                subset.Add(ProviderCache[k]);
            }

            // Use the existing series calculation
            var seriesResults = subset.ToCci(LookbackPeriods);
            var latestResult = seriesResults.Count > 0 ? seriesResults[seriesResults.Count - 1] : null;

            cci = latestResult?.Cci;
        }

        // candidate result
        CciResult r = new(
            Timestamp: item.Timestamp,
            Cci: cci);

        return (r, i);
    }
}
