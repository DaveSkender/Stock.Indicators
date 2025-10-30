namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for generating ZigZag series in a streaming manner.
/// </summary>
public class ZigZagHub
    : ChainProvider<IQuote, ZigZagResult>
{
    #region fields and constructor

    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="ZigZagHub"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="endType">The type of price to use for the end of the pivot.</param>
    /// <param name="percentChange">The percentage change threshold for ZigZag points.</param>
    internal ZigZagHub(
        IChainProvider<IQuote> provider,
        EndType endType,
        decimal percentChange) : base(provider)
    {
        ZigZag.Validate(percentChange);
        EndType = endType;
        PercentChange = percentChange;
        hubName = $"ZIGZAG({endType.ToString().ToUpperInvariant()},{percentChange})";

        Reinitialize();
    }

    #endregion fields and constructor

    /// <summary>
    /// ZigZag hub settings. Since it can repaint historical values,
    /// we need to handle rebuilds on provider history mutations.
    /// </summary>
    public override BinarySettings Properties { get; init; } = new(0b00000000);  // default

    /// <inheritdoc/>
    public EndType EndType { get; }

    /// <inheritdoc/>
    public decimal PercentChange { get; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (ZigZagResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // For the first few items or when cache is empty, just rebuild
        if (i < 3 || Cache.Count == 0)
        {
            RebuildCache();
            return (Cache[i], i);
        }

        // For ZigZag, we need to check if adding this quote could affect pivots
        // If the cache and provider are in sync, just use cached result
        if (Cache.Count == ProviderCache.Count)
        {
            // Already have this result cached
            return (Cache[i], i);
        }

        // New quote - need to rebuild to check for pivot changes
        // This is the repaint-by-design behavior
        RebuildCache();

        return (Cache[i], i);
    }

    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // ZigZag is a repaint-by-design indicator
        // Simply rebuild from the provider cache
        RebuildCache();
    }

    #region private helpers

    private void RebuildCache()
    {
        // Use Series implementation to rebuild entire cache
        IReadOnlyList<ZigZagResult> seriesResults = ProviderCache.ToZigZag(EndType, PercentChange);

        // Clear and repopulate cache
        Cache.Clear();
        Cache.AddRange(seriesResults);

        // Note: This is a repaint-by-design indicator
        // Historical values change as new pivots are confirmed
    }

    #endregion private helpers
}

public static partial class ZigZag
{
    /// <summary>
    /// Converts a chain provider to a ZigZag hub.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="endType">The type of price to use for the end of the pivot.</param>
    /// <param name="percentChange">The percentage change threshold for ZigZag points.</param>
    /// <returns>A ZigZag hub.</returns>
    public static ZigZagHub ToZigZagHub(
        this IChainProvider<IQuote> chainProvider,
        EndType endType = EndType.Close,
        decimal percentChange = 5)
        => new(chainProvider, endType, percentChange);

    /// <summary>
    /// Creates a ZigZag hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="endType">The type of price to use for the end of the pivot.</param>
    /// <param name="percentChange">The percentage change threshold for ZigZag points.</param>
    /// <returns>An instance of <see cref="ZigZagHub"/>.</returns>
    public static ZigZagHub ToZigZagHub(
        this IReadOnlyList<IQuote> quotes,
        EndType endType = EndType.Close,
        decimal percentChange = 5)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToZigZagHub(endType, percentChange);
    }
}
