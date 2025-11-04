namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Detrended Price Oscillator (DPO) indicator.
/// DPO uses a centered moving average, causing repaint behavior as new data arrives.
/// </summary>
/// <remarks>
/// DPO calculates value[i] = price[i] - SMA[i+offset].
/// In streaming mode, results initially appear as null until sufficient future
/// data arrives. This "repaint-by-design" pattern is similar to indicators
/// like Parabolic SAR that adjust historical values based on new information.
/// </remarks>
public class DpoHub
    : ChainProvider<IReusable, DpoResult>, IDpo
{
    private readonly string hubName;
    private readonly int offset;
    private readonly int warmupIndex;

    /// <summary>
    /// Initializes a new instance of the <see cref="DpoHub"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    internal DpoHub(
        IChainProvider<IReusable> provider,
        int lookbackPeriods) : base(provider)
    {
        Dpo.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        offset = (lookbackPeriods / 2) + 1;
        warmupIndex = LookbackPeriods - offset - 1;
        hubName = $"DPO({lookbackPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <summary>
    /// Override OnAdd to handle DPO's repaint behavior.
    /// When new data arrives, we calculate and update earlier positions
    /// that can now be computed with this new data.
    /// </summary>
    public override void OnAdd(IReusable item, bool notify, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        // Add the current item to cache (may have null values initially)
        (DpoResult result, int currentIndex) = ToIndicator(item, indexHint);
        AppendCache(result, notify);

        // DPO[i] needs SMA[i+offset], so when we're at position N,
        // we can calculate DPO[N-offset] (if it exists)
        int targetIndex = currentIndex - offset;

        // In streaming mode, update just one earlier position
        if (notify && targetIndex >= warmupIndex && targetIndex < Cache.Count)
        {
            IReusable targetItem = ProviderCache[targetIndex];
            (DpoResult updatedResult, int _) = ToIndicator(targetItem, targetIndex);
            Cache[targetIndex] = updatedResult;
            return;
        }

        // In rebuild mode (notify=false), update ALL affected earlier positions
        // that can now be calculated with the new data
        if (!notify)
        {
            // Start from the target position and work backwards to fill gaps
            while (targetIndex >= warmupIndex && targetIndex < Cache.Count)
            {
                // Check if this position needs recalculation
                DpoResult currentCacheValue = Cache[targetIndex];
                IReusable targetItem = ProviderCache[targetIndex];
                (DpoResult updatedResult, int _) = ToIndicator(targetItem, targetIndex);

                // Update if value changed or was null
                if (currentCacheValue.Dpo != updatedResult.Dpo)
                {
                    Cache[targetIndex] = updatedResult;
                }

                // Move to next earlier position
                targetIndex--;

                // Stop if we've gone too far back
                // (positions that can't be calculated anyway)
                if (targetIndex < warmupIndex || targetIndex >= ProviderCache.Count - offset)
                {
                    break;
                }
            }
        }
    }

    /// <inheritdoc/>
    protected override (DpoResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        double? dpoSma = null;
        double? dpoVal = null;

        // DPO calculation: at position i, we need SMA at i+offset
        // Only calculate when:
        // 1. We have warmed up (i >= warmupIndex)
        // 2. Future data exists (i + offset < ProviderCache.Count)
        // 3. We're not in the final offset positions (i < ProviderCache.Count - offset)
        //    because those positions will never have enough future data
        if (i >= warmupIndex
            && i + offset < ProviderCache.Count
            && i < ProviderCache.Count - offset)
        {
            double smaValue = Sma.Increment(
                ProviderCache,
                LookbackPeriods,
                i + offset);

            if (!double.IsNaN(smaValue))
            {
                dpoSma = smaValue;
                double currentValue = item.Value;

                if (!double.IsNaN(currentValue))
                {
                    dpoVal = currentValue - smaValue;
                }
            }
        }

        DpoResult r = new(
            Timestamp: item.Timestamp,
            Dpo: dpoVal,
            Sma: dpoSma);

        return (r, i);
    }

    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // DPO depends on future data: DPO[i] needs SMA[i+offset]
        // Use known parameters to calculate affected range and fill gaps

        int provIndex = ProviderCache.IndexGte(timestamp);

        if (provIndex < 0)
        {
            base.RollbackState(timestamp);
            return;
        }

        // Calculate earliest affected position using known dependency formula
        int earliestAffected = provIndex - offset - LookbackPeriods + 1;
        earliestAffected = Math.Max(0, earliestAffected);
        earliestAffected = Math.Max(earliestAffected, warmupIndex);

        // Recalculate all positions from earliest affected to provIndex
        // This handles both affected positions AND fills gaps from Insert operations
        for (int i = earliestAffected; i < provIndex && i < Cache.Count; i++)
        {
            if (i < ProviderCache.Count)
            {
                IReusable item = ProviderCache[i];
                (DpoResult updatedResult, int _) = ToIndicator(item, i);

                // Always update to ensure positions are recalculated correctly
                // (handles both mutations and gap-filling after inserts)
                Cache[i] = updatedResult;
            }
        }

        base.RollbackState(timestamp);
    }
}/// <summary>
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
