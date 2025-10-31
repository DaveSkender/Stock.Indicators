namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for generating ZigZag series in a streaming manner.
/// </summary>
/// <remarks>
/// ZigZag is a "repaint-by-design" indicator. Historical pivot values change as new
/// price data confirms or invalidates previous pivots. This implementation uses full
/// cache rebuilds to ensure correctness, as ZigZag's pivot logic requires complete
/// historical context for accurate calculation.
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
    /// Converts provider item into ZigZag result with full historical context.
    /// </summary>
    /// <remarks>
    /// <para>
    /// ZigZag uses a non-standard StreamHub pattern due to its repaint-by-design nature.
    /// Unlike incremental indicators (EMA, RSI) that can calculate new values from
    /// previous state, ZigZag pivot detection requires analyzing the complete price series.
    /// </para>
    /// <para>
    /// This implementation rebuilds the entire cache on each new quote to ensure pivot
    /// accuracy. While less efficient than incremental patterns, this approach guarantees
    /// mathematical correctness for repaint-by-design indicators.
    /// </para>
    /// <para>
    /// Pattern classification: Full Rebuild (non-incremental)
    /// - When to use: Indicators where new data invalidates historical calculations
    /// - Examples: ZigZag, Renko (when brick size changes), PivotPoints (session-based)
    /// - Trade-off: Correctness over incremental performance
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
            // Rebuild entire cache using Series implementation
            // This is required because ZigZag pivot logic needs full historical context
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
    /// ZigZag has no internal state variables (no running averages, no windows, no buffers).
    /// It's a stateless algorithm that recalculates from scratch using the Series implementation.
    /// </para>
    /// <para>
    /// However, we still override RollbackState to document this design decision and ensure
    /// future maintainers understand that ZigZag intentionally has no state to roll back.
    /// The framework's Rebuild() mechanism will call ToIndicator() which handles the
    /// cache reconstruction through Series calculation.
    /// </para>
    /// <para>
    /// Pattern: Stateless repaint-by-design
    /// - No state variables to restore
    /// - Relies on framework's Rebuild() calling ToIndicator()
    /// - ToIndicator() performs full recalculation via Series
    /// </para>
    /// </remarks>
    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // No-op: ZigZag maintains no internal state.
        // 
        // Unlike indicators with state (EMA's _prevValue, Stoch's _rawKBuffer,
        // Chandelier's RollingWindowMax), ZigZag is stateless. Each calculation
        // uses the Series algorithm which analyzes the complete ProviderCache.
        //
        // The framework will call ToIndicator() during Rebuild(), which
        // recalculates everything via ProviderCache.ToZigZag().
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
