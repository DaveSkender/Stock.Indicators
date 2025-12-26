namespace Skender.Stock.Indicators;

// DETRENDED PRICE OSCILLATOR (STREAM HUB)

/// <summary>
/// Provides methods for calculating the Detrended Price Oscillator (DPO) using a stream hub.
/// </summary>
/// <remarks>
/// DPO requires lookahead data, so results are emitted with a delay of offset periods.
/// This hub overrides OnAdd to handle the delayed emission pattern.
/// </remarks>
public class DpoHub
    : ChainProvider<IReusable, DpoResult>, IDpo
{
    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="DpoHub"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    internal DpoHub(
        IChainProvider<IReusable> provider,
        int lookbackPeriods) : base(provider)
    {
        Dpo.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        Offset = (lookbackPeriods / 2) + 1;
        hubName = $"DPO({lookbackPeriods})";

        Reinitialize();
    }

    /// <summary>
    /// Gets the number of lookback periods.
    /// </summary>
    public int LookbackPeriods { get; init; }

    /// <summary>
    /// Gets the offset for lookahead calculation.
    /// </summary>
    public int Offset { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    /// <remarks>
    /// Overrides OnAdd to implement batch emission pattern for DPO's lookahead requirement.
    /// Emits results only when sufficient lookahead data is available.
    /// During Rebuild, ensures all calculable positions are emitted.
    /// </remarks>
    public override void OnAdd(IReusable item, bool notify, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // DPO at position dpoIndex requires SMA at position (dpoIndex + offset)
        // So with input at position i, we can calculate DPO up to position (i - offset)
        int maxCalculableIndex = i - Offset;

        // Emit all results from current cache size up to current provider index
        // to maintain 1:1 correspondence
        for (int dpoIndex = Cache.Count; dpoIndex <= i; dpoIndex++)
        {
            if (dpoIndex < 0 || dpoIndex >= ProviderCache.Count)
            {
                continue;
            }

            DpoResult result = CalculateDpoAtIndex(dpoIndex);
            AppendCache(result, notify);
        }
    }

    /// <summary>
    /// Calculates DPO result for a specific index position.
    /// </summary>
    private DpoResult CalculateDpoAtIndex(int dpoIndex)
    {
        IReusable dpoTargetItem = ProviderCache[dpoIndex];
        int smaIndex = dpoIndex + Offset;

        // Check if we can calculate DPO for this position
        int firstValidDpoIndex = LookbackPeriods - Offset - 1;

        // Need sufficient historical data AND lookahead data
        if (dpoIndex >= firstValidDpoIndex
            && smaIndex < ProviderCache.Count
            && smaIndex >= LookbackPeriods - 1)
        {
            // Calculate SMA at the lookahead position
            double sum = 0;
            bool hasNaN = false;

            for (int p = smaIndex - LookbackPeriods + 1; p <= smaIndex; p++)
            {
                double value = ProviderCache[p].Value;
                if (double.IsNaN(value))
                {
                    hasNaN = true;
                    break;
                }

                sum += value;
            }

            double? sma = hasNaN ? null : sum / LookbackPeriods;
            double? dpoVal = sma.HasValue ? dpoTargetItem.Value - sma.Value : null;

            return new DpoResult(dpoTargetItem.Timestamp, dpoVal, sma);
        }

        // Return empty result if we can't calculate
        return new DpoResult(dpoTargetItem.Timestamp);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// ToIndicator is used during explicit calls and must handle lookahead correctly.
    /// This implementation delegates to CalculateDpoAtIndex for consistency.
    /// </remarks>
    protected override (DpoResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);
        DpoResult result = CalculateDpoAtIndex(i);
        return (result, i);
    }

    /// <summary>
    /// Rebuilds the cache from a specific timestamp.
    /// For DPO, always rebuilds from the beginning to ensure lookahead data is considered.
    /// </summary>
    public new void Rebuild(DateTime fromTimestamp)
    {
        // For DPO, always rebuild from the beginning
        base.Rebuild(DateTime.MinValue);
    }

    /// <summary>
    /// Restores the DPO state up to the specified timestamp.
    /// For DPO, no persistent state needs restoration.
    /// </summary>
    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // No state to restore for DPO
    }
}

/// <summary>
/// Provides methods for creating DPO hubs.
/// </summary>
public static partial class Dpo
{
    /// <summary>
    /// Converts the chain provider to a DPO hub.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>A DPO hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static DpoHub ToDpoHub(
        this IChainProvider<IReusable> chainProvider,
        int lookbackPeriods = 14)
             => new(chainProvider, lookbackPeriods);

    /// <summary>
    /// Creates a DPO hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>An instance of <see cref="DpoHub"/>.</returns>
    public static DpoHub ToDpoHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToDpoHub(lookbackPeriods);
    }
}
