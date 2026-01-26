namespace Skender.Stock.Indicators;

/// <summary>
/// Streaming hub for Endpoint Moving Average (EPMA)
/// </summary>
public class EpmaHub
    : ChainHub<IReusable, EpmaResult>, IEpma
{
    // Track total number of items processed to calculate global positions after pruning
    private int totalProcessed;

    internal EpmaHub(
        IChainProvider<IReusable> provider,
        int lookbackPeriods) : base(provider)
    {
        Epma.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        Name = $"EPMA({lookbackPeriods})";

        // Validate cache size for warmup requirements
        ValidateCacheSize(lookbackPeriods * 2, Name);

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    protected override (EpmaResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Increment total processed count
        totalProcessed++;

        // Calculate cache offset: totalProcessed - current cache size
        // This allows reconstruction of global positions after pruning
        int cacheOffset = totalProcessed - ProviderCache.Count;

        // candidate result
        EpmaResult r = new(
            Timestamp: item.Timestamp,
            Epma: Epma.Increment(ProviderCache, LookbackPeriods, i, cacheOffset).NaN2Null());

        return (r, i);
    }

    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        int targetIndex = ProviderCache.IndexGte(timestamp);

        // Reset totalProcessed to match the target state
        // Count items in provider cache up to (but not including) targetIndex
        totalProcessed = targetIndex;
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
