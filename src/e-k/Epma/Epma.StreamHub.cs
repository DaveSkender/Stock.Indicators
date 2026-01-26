namespace Skender.Stock.Indicators;

/// <summary>
/// Streaming hub for Endpoint Moving Average (EPMA)
/// </summary>
public class EpmaHub
    : ChainHub<SlopeResult, EpmaResult>, IEpma
{
    // Track global position counter
    private int globalPositionCounter;
    private DateTime? lastSeenTimestamp;

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

        // Initialize global position tracking
        globalPositionCounter = 0;
        lastSeenTimestamp = null;

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    protected override (EpmaResult result, int index)
        ToIndicator(SlopeResult item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Track global position for new unique items only
        bool isNewItem = item.Timestamp != lastSeenTimestamp;
        if (isNewItem)
        {
            lastSeenTimestamp = item.Timestamp;
            globalPositionCounter++;
        }

        // Calculate EPMA
        double? epma = null;

        if (item.Slope != null && item.Intercept != null)
        {
            // Calculate offset: globalPosition - cacheIndex - 1
            int cacheOffset = globalPositionCounter - i - 1;
            int x = cacheOffset + i + 1;

            epma = (item.Slope * x) + item.Intercept;
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

        // Reset global position tracking
        globalPositionCounter = targetIndex;
        lastSeenTimestamp = targetIndex > 0 ? ProviderCache[targetIndex - 1].Timestamp : null;
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
