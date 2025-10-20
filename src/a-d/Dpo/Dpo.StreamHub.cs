namespace Skender.Stock.Indicators;

/// <summary>
/// Represents a Detrended Price Oscillator (DPO) stream hub.
/// Note: DPO results use lookahead, calculated as value[i] - SMA[i+offset].
/// </summary>
public class DpoHub
    : ChainProvider<IReusable, DpoResult>, IDpo
{
    private readonly string hubName;
    private readonly int offset;

    /// <summary>
    /// Initializes a new instance of the <see cref="DpoHub"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    internal DpoHub(
        IChainProvider<IReusable> provider,
        int lookbackPeriods) : base(provider)
    {
        Dpo.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        offset = (lookbackPeriods / 2) + 1;
        hubName = $"DPO({lookbackPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (DpoResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // DPO calculation matches StaticSeries logic:
        // Result at position i uses value[i] - SMA[i+offset]
        // DPO values are only calculable when i >= lookbackPeriods - offset - 1 && i+offset exists

        double? dpoSma = null;
        double? dpoVal = null;

        int futureIndex = i + offset;
        int cacheCount = ProviderCache.Count;

        // Check if we have the future SMA data needed for this position
        if (i >= LookbackPeriods - offset - 1 && futureIndex < cacheCount && futureIndex >= LookbackPeriods - 1)
        {
            // Calculate SMA at the future position
            double sma = Sma.Increment(ProviderCache, LookbackPeriods, futureIndex);
            dpoSma = sma;
            dpoVal = item.Value - sma;
        }

        // Create result with timestamp from current position
        DpoResult r = new(
            Timestamp: item.Timestamp,
            Dpo: dpoVal,
            Sma: dpoSma);

        return (r, i);
    }
}

/// <summary>
/// Provides methods for creating DPO hubs.
/// </summary>
public static partial class Dpo
{
    /// <summary>
    /// Creates a DPO streaming hub from a chain provider.
    /// Note: DPO results are delayed by offset periods due to lookahead requirements.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <returns>A DPO hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static DpoHub ToDpoHub(
        this IChainProvider<IReusable> chainProvider,
        int lookbackPeriods)
             => new(chainProvider, lookbackPeriods);

    /// <summary>
    /// Creates a DPO hub from a collection of quotes.
    /// Note: DPO results are delayed by offset periods due to lookahead requirements.
    /// </summary>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <returns>An instance of <see cref="DpoHub"/>.</returns>
    public static DpoHub ToDpoHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToDpoHub(lookbackPeriods);
    }
}
