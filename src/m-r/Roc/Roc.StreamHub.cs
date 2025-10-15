namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Rate of Change (ROC) indicator.
/// </summary>
public class RocHub
    : ChainProvider<IReusable, RocResult>, IRoc
 {
    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="RocHub"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    internal RocHub(
        IChainProvider<IReusable> provider,
        int lookbackPeriods) : base(provider)
    {
        Roc.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        hubName = $"ROC({lookbackPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (RocResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        double roc;
        double momentum;

        if (i + 1 > LookbackPeriods)
        {
            IReusable back = ProviderCache[i - LookbackPeriods];

            momentum = item.Value - back.Value;

            roc = back.Value == 0
                ? double.NaN
                : 100d * momentum / back.Value;
        }
        else
        {
            momentum = double.NaN;
            roc = double.NaN;
        }

        // candidate result
        RocResult r = new(
            Timestamp: item.Timestamp,
            Momentum: momentum.NaN2Null(),
            Roc: roc.NaN2Null());

        return (r, i);
    }
}


public static partial class Roc
{
    /// <summary>
    /// Creates an ROC streaming hub from a chain provider.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <returns>An EMA hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static RocHub ToRocHub(
        this IChainProvider<IReusable> chainProvider,
        int lookbackPeriods = 14)
             => new(chainProvider, lookbackPeriods);

    /// <summary>
    /// Creates a Roc hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="lookbackPeriods">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="RocHub"/>.</returns>
    public static RocHub ToRocHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToRocHub(lookbackPeriods);
    }

}
