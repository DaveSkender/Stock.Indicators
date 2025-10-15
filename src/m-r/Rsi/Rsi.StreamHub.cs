namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Relative Strength Index (RSI) indicator.
/// </summary>
public class RsiHub
    : ChainProvider<IReusable, RsiResult>, IRsi
 {
    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="RsiHub"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    internal RsiHub(
        IChainProvider<IReusable> provider,
        int lookbackPeriods) : base(provider)
    {
        Rsi.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        hubName = $"RSI({lookbackPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (RsiResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        double? rsi = null;

        if (i >= LookbackPeriods)
        {
            // Build a subset of provider cache for RSI calculation
            List<IReusable> subset = [];
            for (int k = 0; k <= i; k++)
            {
                subset.Add(ProviderCache[k]);
            }

            // Use the existing series calculation
            IReadOnlyList<RsiResult> seriesResults = subset.ToRsi(LookbackPeriods);
            RsiResult? latestResult = seriesResults.Count > 0 ? seriesResults[seriesResults.Count - 1] : null;

            rsi = latestResult?.Rsi;
        }

        // candidate result
        RsiResult r = new(
            Timestamp: item.Timestamp,
            Rsi: rsi);

        return (r, i);
    }
}


public static partial class Rsi
{
    /// <summary>
    /// Creates an RSI streaming hub from a chain provider.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <returns>An RSI hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static RsiHub ToRsiHub(
        this IChainProvider<IReusable> chainProvider,
        int lookbackPeriods = 14)
        => new(chainProvider, lookbackPeriods);

    /// <summary>
    /// Creates a Rsi hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="lookbackPeriods">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="RsiHub"/>.</returns>
    public static RsiHub ToRsiHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToRsiHub(lookbackPeriods);
    }

}
