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
    private readonly Queue<(double Value, DateTime Timestamp)> valueBuffer;
    private int inputCount;

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

        // Initialize buffer for lookahead offset
        valueBuffer = new Queue<(double, DateTime)>(Offset);
        inputCount = 0;

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
    /// Overridden to handle DPO's lookahead offset pattern.
    /// When new data arrives, we emit all DPO results that can now be calculated
    /// with the available lookahead data.
    /// </remarks>
    public override void OnAdd(IReusable item, bool notify, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Track total inputs
        inputCount++;

        // Add current value to buffer (for potential future use)
        valueBuffer.Update(Offset, (item.Value, item.Timestamp));

        // Calculate how many results we can emit now
        // With input at position i, we can calculate DPO up to position (i - offset)
        int maxDpoIndex = i - Offset;

        // We need to emit results starting from the last emitted position
        int startEmitIndex = Cache.Count;

        // Emit all results from startEmitIndex to maxDpoIndex (inclusive)
        for (int dpoIndex = startEmitIndex; dpoIndex <= maxDpoIndex; dpoIndex++)
        {
            if (dpoIndex < 0 || dpoIndex >= ProviderCache.Count)
            {
                continue; // Skip invalid indices
            }

            IReusable dpoTargetItem = ProviderCache[dpoIndex];
            int smaIndex = dpoIndex + Offset;

            // Check if we can calculate DPO for this position
            int firstValidDpoIndex = LookbackPeriods - Offset - 1;

            DpoResult result;

            if (dpoIndex >= firstValidDpoIndex && smaIndex >= LookbackPeriods - 1 && smaIndex < ProviderCache.Count)
            {
                // Calculate SMA at smaIndex
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

                result = new DpoResult(dpoTargetItem.Timestamp, dpoVal, sma);
            }
            else
            {
                // Empty result for positions outside valid range
                result = new DpoResult(dpoTargetItem.Timestamp);
            }

            AppendCache(result, notify);
        }

        // Check if provider is completed and we need to emit trailing empty results
        if (i == ProviderCache.Count - 1)
        {
            // Emit trailing empty results for positions that can never have DPO values
            int startTrailingIndex = Math.Max(Cache.Count, ProviderCache.Count - Offset);
            for (int j = startTrailingIndex; j < ProviderCache.Count; j++)
            {
                if (j >= Cache.Count)
                {
                    DpoResult emptyResult = new(ProviderCache[j].Timestamp);
                    AppendCache(emptyResult, notify);
                }
            }
        }
    }

    /// <inheritdoc/>
    /// <remarks>
    /// This method is only used during Rebuild() operations, not during normal streaming.
    /// It maintains compatibility with the base StreamHub rebuild mechanism.
    /// </remarks>
    protected override (DpoResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // During rebuild, we need to check if we can calculate DPO for this position
        // DPO at position i needs data at position (i + offset)
        int futureIndex = i + Offset;

        // If we don't have future data yet, return empty result
        if (futureIndex >= ProviderCache.Count)
        {
            return (new DpoResult(item.Timestamp), i);
        }

        // Calculate SMA at the future position
        double? sma = null;
        if (futureIndex >= LookbackPeriods - 1)
        {
            double sum = 0;
            bool hasNaN = false;

            for (int p = futureIndex - LookbackPeriods + 1; p <= futureIndex; p++)
            {
                if (p >= ProviderCache.Count)
                {
                    hasNaN = true;
                    break;
                }

                double value = ProviderCache[p].Value;
                if (double.IsNaN(value))
                {
                    hasNaN = true;
                    break;
                }

                sum += value;
            }

            sma = hasNaN ? null : sum / LookbackPeriods;
        }

        double? dpoVal = sma.HasValue
            ? item.Value - sma.Value
            : null;

        DpoResult result = new(item.Timestamp, dpoVal, sma);
        return (result, i);
    }

    /// <summary>
    /// Restores the DPO state up to the specified timestamp.
    /// </summary>
    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // Clear state
        valueBuffer.Clear();
        inputCount = 0;

        // Find the target index in provider cache
        int targetIndex = ProviderCache.IndexGte(timestamp);

        if (targetIndex < 0)
        {
            return;
        }

        int restoreIndex = targetIndex > 0 ? targetIndex - 1 : 0;

        // Track inputs up to restoreIndex
        inputCount = restoreIndex + 1;

        // Rebuild buffer up to restoreIndex
        // We need to populate the buffer with the last 'Offset' values before restoreIndex
        int startIdx = Math.Max(0, restoreIndex - Offset + 1);
        for (int p = startIdx; p <= restoreIndex; p++)
        {
            IReusable providerItem = ProviderCache[p];
            valueBuffer.Update(Offset, (providerItem.Value, providerItem.Timestamp));
        }
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
