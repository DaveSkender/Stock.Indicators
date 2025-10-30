namespace Skender.Stock.Indicators;

/// <summary>
/// Provides streaming hub for calculating the Ulcer Index indicator.
/// </summary>
public class UlcerIndexHub
    : ChainProvider<IReusable, UlcerIndexResult>, IUlcerIndex
{
    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="UlcerIndexHub"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    internal UlcerIndexHub(
        IChainProvider<IReusable> provider,
        int lookbackPeriods) : base(provider)
    {
        UlcerIndex.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        hubName = $"ULCER({lookbackPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (UlcerIndexResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        double? ui = null;

        // Only calculate if we have enough data
        if (i >= LookbackPeriods - 1)
        {
            double sumSquared = 0;
            int startIdx = i + 1 - LookbackPeriods;

            // For each period in the lookback window
            for (int p = startIdx; p <= i; p++)
            {
                IReusable ps = ProviderCache[p];
                double pValue = ps.Value;

                // Find maximum from start of window up to current period p
                double maxClose = 0;
                for (int z = startIdx; z <= p; z++)
                {
                    IReusable zs = ProviderCache[z];
                    double zValue = zs.Value;
                    if (zValue > maxClose)
                    {
                        maxClose = zValue;
                    }
                }

                // Calculate percent drawdown
                double percentDrawdown = maxClose == 0 ? double.NaN
                    : 100 * ((pValue - maxClose) / maxClose);

                sumSquared += percentDrawdown * percentDrawdown;
            }

            ui = Math.Sqrt(sumSquared / LookbackPeriods).NaN2Null();
        }

        // Candidate result
        UlcerIndexResult r = new(
            Timestamp: item.Timestamp,
            UlcerIndex: ui);

        return (r, i);
    }
}


public static partial class UlcerIndex
{
    /// <summary>
    /// Creates an Ulcer Index streaming hub from a chain provider.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>An Ulcer Index hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static UlcerIndexHub ToUlcerIndexHub(
        this IChainProvider<IReusable> chainProvider,
        int lookbackPeriods = 14)
             => new(chainProvider, lookbackPeriods);

    /// <summary>
    /// Creates an Ulcer Index hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>An instance of <see cref="UlcerIndexHub"/>.</returns>
    public static UlcerIndexHub ToUlcerIndexHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToUlcerIndexHub(lookbackPeriods);
    }
}
