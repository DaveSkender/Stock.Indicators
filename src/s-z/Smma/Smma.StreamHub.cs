namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Smoothed Moving Average (SMMA) indicator.
/// </summary>
public class SmmaHub<TIn>
    : ChainProvider<TIn, SmmaResult>, ISmma
    where TIn : IReusable
{
    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="SmmaHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    internal SmmaHub(
        IChainProvider<TIn> provider,
        int lookbackPeriods) : base(provider)
    {
        Smma.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        hubName = $"SMMA({lookbackPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (SmmaResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        double smma = i >= LookbackPeriods - 1

            // normal SMMA calculation when we have previous value
            ? Cache[i - 1].Smma is not null
                ? ((Cache[i - 1].Value * (LookbackPeriods - 1)) + item.Value) / LookbackPeriods

                // re/initialize as SMA when no previous SMMA
                : Sma.Increment(ProviderCache, LookbackPeriods, i)

            // warmup periods are never calculable
            : double.NaN;

        // candidate result
        SmmaResult r = new(
            Timestamp: item.Timestamp,
            Smma: smma.NaN2Null());

        return (r, i);
    }
}


public static partial class Smma
{
    /// <summary>
    /// Creates an SMMA streaming hub from a chain provider.
    /// </summary>
    /// <typeparam name="T">The type of the reusable data.</typeparam>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <returns>An SMMA hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static SmmaHub<T> ToSmmaHub<T>(
        this IChainProvider<T> chainProvider,
        int lookbackPeriods)
        where T : IReusable
        => new(chainProvider, lookbackPeriods);

    /// <summary>
    /// Creates a Smma hub from a collection of quotes.
    /// </summary>
    /// <typeparam name="TQuote">The type of the quote.</typeparam>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="lookbackPeriods">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="SmmaHub{TQuote}"/>.</returns>
    public static SmmaHub<TQuote> ToSmmaHub<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote
    {
        QuoteHub<TQuote> quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToSmmaHub(lookbackPeriods);
    }

}
