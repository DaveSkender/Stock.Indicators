namespace Skender.Stock.Indicators;

/// <summary>
/// Streaming hub for calculating Simple Moving Average (SMA).
/// </summary>
public class SmaHub
    : ChainProvider<IReusable, SmaResult>, ISma
{

    internal SmaHub(
        IChainProvider<IReusable> provider,
        int lookbackPeriods) : base(provider)
    {
        Sma.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        Name = $"SMA({lookbackPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }
    /// <inheritdoc/>
    protected override (SmaResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Calculate SMA efficiently using a rolling window over ProviderCache
        // This is O(lookbackPeriods) which is constant for a given configuration
        // and maintains exact precision with Series implementation
        double sma = Sma.Increment(ProviderCache, LookbackPeriods, i);

        // candidate result
        SmaResult r = new(
            Timestamp: item.Timestamp,
            Sma: sma.NaN2Null());

        return (r, i);
    }

}

public static partial class Sma
{
    /// <summary>
    /// Creates an SMA streaming hub with a chain provider source.
    /// </summary>
    /// <remarks>If providers contain historical data, this hub will fast-forward its cache.</remarks>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>A chain-sourced instance of <see cref="SmaHub"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="chainProvider"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="lookbackPeriods"/> is invalid.</exception>
    public static SmaHub ToSmaHub(
        this IChainProvider<IReusable> chainProvider,
        int lookbackPeriods)
             => new(chainProvider, lookbackPeriods);

    /// <summary>
    /// Creates a standalone SMA hub with an internal <see cref="IQuote"/> cache
    /// based on an initial set of provided quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>A standalone instance of <see cref="SmaHub"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="quotes"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="lookbackPeriods"/> is invalid.</exception>
    public static SmaHub ToSmaHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToSmaHub(lookbackPeriods);
    }
}
