namespace Skender.Stock.Indicators;

/// <summary>
/// Streaming hub for Endpoint Moving Average (EPMA)
/// </summary>
public class EpmaHub
    : ChainHub<SlopeResult, EpmaResult>, IEpma
{
    // Track how many unique items have been processed (should match Slope's count)
    private int itemsProcessed;
    private DateTime? lastSeenTimestamp;
    private int lastCacheSize;

    internal EpmaHub(
        IChainProvider<IReusable> provider,
        int lookbackPeriods)
        : this(provider.ToSlopeHub(lookbackPeriods), lookbackPeriods)
    { }

    internal EpmaHub(
        SlopeHub slopeHub,
        int lookbackPeriods) : base(slopeHub)
    {
        Epma.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        Name = $"EPMA({lookbackPeriods})";

        // Initialize items processed counter
        itemsProcessed = 0;
        lastSeenTimestamp = null;
        lastCacheSize = 0;

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public override void OnAdd(SlopeResult item, bool notify, int? indexHint)
    {
        // Call base to add the result to cache
        base.OnAdd(item, notify, indexHint);

        // Track cache size for pruning detection
        lastCacheSize = Cache.Count;
    }

    /// <inheritdoc/>
    protected override (EpmaResult result, int index)
        ToIndicator(SlopeResult item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Increment items processed for new unique items only
        bool isNewItem = item.Timestamp != lastSeenTimestamp;
        if (isNewItem)
        {
            lastSeenTimestamp = item.Timestamp;
            itemsProcessed++;
        }

        // Calculate EPMA using the endpoint formula
        // EPMA = slope × endpointX + intercept
        // where endpointX = itemsProcessed (the global position of this quote)
        double? epma = null;

        if (item.Slope != null && item.Intercept != null)
        {
            // Use itemsProcessed as the endpoint X value
            epma = (item.Slope * itemsProcessed) + item.Intercept;
        }

        EpmaResult r = new(
            Timestamp: item.Timestamp,
            Epma: epma.NaN2Null());

        return (r, i);
    }

    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        int targetIndex = ProviderCache.IndexGte(timestamp);

        // Reset items processed counter to match the target index
        itemsProcessed = targetIndex;
        lastSeenTimestamp = targetIndex > 0 ? ProviderCache[targetIndex - 1].Timestamp : null;
    }

    /// <inheritdoc/>
    protected override void PruneState(DateTime toTimestamp)
    {
        // Calculate how many items were removed by comparing cache sizes
        int currentCacheSize = Cache.Count;
        int removedCount = lastCacheSize - currentCacheSize;

        if (removedCount > 0)
        {
            // Increment itemsProcessed by the number of removed items
            itemsProcessed += removedCount;
        }

        // Update lastCacheSize for next pruning event
        lastCacheSize = currentCacheSize;
    }
}

public static partial class Epma
{
    /// <summary>
    /// Converts the chain provider to an EPMA hub.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>An EPMA hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static EpmaHub ToEpmaHub(
        this IChainProvider<IReusable> chainProvider,
        int lookbackPeriods)
             => new(chainProvider, lookbackPeriods);
}
