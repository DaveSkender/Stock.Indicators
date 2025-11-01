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
/// Current implementation recalculates using Series for correctness and simplicity.
/// Future optimization opportunity: Only recalculate from last confirmed pivot forward,
/// as historical values before that pivot never change.
/// </para>
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
    /// ZigZag is a repaint-by-design indicator. Only values from the last confirmed
    /// pivot forward may change with new data; earlier pivots are stable.
    /// </para>
    /// <para>
    /// Current implementation: Recalculates entire series using Series algorithm.
    /// This ensures correctness and Series parity.
    /// </para>
    /// <para>
    /// Future optimization opportunity: Maintain state tracking last confirmed pivot
    /// (lastPoint, lastHighPoint, lastLowPoint) and only recalculate from that pivot
    /// forward, not the entire series. This would improve from O(n) to O(k) where
    /// k = quotes since last pivot.
    /// </para>
    /// </remarks>
    /// <inheritdoc/>
    protected override (ZigZagResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Check if we need to recalculate
        bool needsRecalc = i < 3 || Cache.Count == 0 || Cache.Count != ProviderCache.Count;

        if (needsRecalc)
        {
            // Recalculate using Series implementation
            // Future: Track pivot state and only recalculate from last pivot forward
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
    /// Current implementation: No state maintained. Framework's Rebuild() calls
    /// ToIndicator() which recalculates via Series.
    /// </para>
    /// <para>
    /// Future optimization: Would restore pivot state (lastPoint, lastHighPoint,
    /// lastLowPoint) from cache to enable recalculation from last pivot forward only.
    /// </para>
    /// </remarks>
    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // No-op: Current implementation maintains no pivot state.
        //
        // Future optimization: Restore pivot state from cache to enable
        // partial recalculation from last pivot forward (O(k) vs O(n)).
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
