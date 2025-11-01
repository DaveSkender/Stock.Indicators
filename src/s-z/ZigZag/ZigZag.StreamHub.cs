namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for generating ZigZag series in a streaming manner.
/// </summary>
/// <remarks>
/// <para>
/// ZigZag is a "repaint-by-design" indicator where values from the last confirmed
/// pivot forward may change as new data arrives, but earlier pivots remain stable.
/// </para>
/// <para>
/// Implementation uses full Series recalculation for correctness. Optimized to avoid
/// recursive rebuild loops.
/// </para>
/// </remarks>
public class ZigZagHub
    : ChainProvider<IQuote, ZigZagResult>
{
    #region fields and constructor

    private readonly string hubName;
    private int _lastProviderCount;

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
        _lastProviderCount = 0;

        Reinitialize();
    }

    #endregion fields and constructor

    /// <inheritdoc/>
    public EndType EndType { get; }

    /// <inheritdoc/>
    public decimal PercentChange { get; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <summary>
    /// Converts provider item into ZigZag result.
    /// </summary>
    /// <remarks>
    /// <para>
    /// ZigZag is a repaint-by-design indicator. Values from the last confirmed
    /// pivot forward may change with new data; earlier pivots are stable.
    /// </para>
    /// <para>
    /// Uses full Series recalculation when provider count changes, but avoids
    /// recursive rebuilds by tracking provider count.
    /// </para>
    /// </remarks>
    /// <inheritdoc/>
    protected override (ZigZagResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);
        int providerCount = ProviderCache.Count;

        // Only recalculate if provider count changed (new data or rebuild)
        // This prevents recursive loops during rebuilds
        if (providerCount != _lastProviderCount || Cache.Count != providerCount)
        {
            // Recalculate using Series implementation
            IReadOnlyList<ZigZagResult> results = ProviderCache.ToZigZag(EndType, PercentChange);

            // Update cache to match
            Cache.Clear();
            Cache.AddRange(results);

            _lastProviderCount = providerCount;
        }

        return (Cache[i], i);
    }

    /// <summary>
    /// Restores state after provider history mutations.
    /// </summary>
    /// <remarks>
    /// Resets tracking state to force recalculation on next ToIndicator call.
    /// </remarks>
    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // Reset tracking to force recalc on next ToIndicator
        _lastProviderCount = 0;
    }
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
