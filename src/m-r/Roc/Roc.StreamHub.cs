namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Rate of Change (ROC) indicator.
/// </summary>
public class RocHub<TIn>
    : ChainProvider<TIn, RocResult>, IRoc
    where TIn : IReusable
{
    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="RocHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    internal RocHub(
        IChainProvider<TIn> provider,
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
        ToIndicator(TIn item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        double roc;
        double momentum;

        if (i + 1 > LookbackPeriods)
        {
            TIn back = ProviderCache[i - LookbackPeriods];

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
    /// <typeparam name="TIn">The type of the reusable data.</typeparam>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <returns>An EMA hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static RocHub<TIn> ToRocHub<TIn>(
        this IChainProvider<TIn> chainProvider,
        int lookbackPeriods = 14)
        where TIn : IReusable
        => new(chainProvider, lookbackPeriods);

    /// <summary>
    /// Creates a Roc hub from a collection of quotes.
    /// </summary>
    /// <typeparam name="TQuote">The type of the quote.</typeparam>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="lookbackPeriods">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="RocHub{TQuote}"/>.</returns>
    public static RocHub<TQuote> ToRocHub<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods = 14)
        where TQuote : IQuote
    {
        QuoteHub<TQuote> quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToRocHub(lookbackPeriods);
    }

}
