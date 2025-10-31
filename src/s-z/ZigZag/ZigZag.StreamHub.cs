namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for generating ZigZag series in a streaming manner.
/// </summary>
/// <remarks>
/// ZigZag is a "repaint-by-design" indicator. Values from the last confirmed pivot
/// forward may change as new price data arrives, but earlier pivots remain stable.
/// Current implementation recalculates from Series; future optimization can
/// recalculate only from last pivot forward.
/// </remarks>
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
    /// ZigZag is a repaint-by-design indicator where values from the last confirmed
    /// pivot forward may change, but earlier pivots are stable.
    /// </para>
    /// <para>
    /// Current implementation: Recalculates using Series for correctness.
    /// Future optimization: Maintain pivot state and only recalculate from last pivot.
    /// Historical values before the last confirmed pivot never change.
    /// </para>
    /// <para>
    /// Pattern: Repaint from last pivot (optimization opportunity)
    /// - Only values from last pivot forward need recalculation
    /// - Earlier pivots are confirmed and stable
    /// - Current impl uses Series; can optimize to partial rebuild
    /// </para>
    /// </remarks>
    /// <inheritdoc/>
    protected override (ZigZagResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Check if we need to rebuild
        bool needsRebuild = i < 3 || Cache.Count == 0 || Cache.Count != ProviderCache.Count;

        if (needsRebuild)
        {
            // Current: Recalculate using Series implementation
            // TODO: Optimize to only recalculate from last confirmed pivot forward
            // by maintaining pivot state (lastPoint, lastHighPoint, lastLowPoint)
            IReadOnlyList<ZigZagResult> results = ProviderCache.ToZigZag(EndType, PercentChange);

            Cache.Clear();
            Cache.AddRange(results);
        }

        return (Cache[i], i);
    }

    /// <summary>
    /// Restores state after provider history mutations.
    /// </summary>
    /// <remarks>
    /// <para>
    /// For optimized implementation: Would restore pivot state from cache
    /// to enable recalculation from last pivot forward only.
    /// </para>
    /// <para>
    /// Current implementation: No state to restore (uses Series each time).
    /// Framework's Rebuild() calls ToIndicator() which handles recalculation.
    /// </para>
    /// </remarks>
    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // Current: No state maintained, so nothing to restore
        // 
        // Future optimization: Restore pivot state from cache:
        // - Find last confirmed pivot before timestamp
        // - Restore lastPoint, lastHighPoint, lastLowPoint
        // - Only recalculate from that pivot forward
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
