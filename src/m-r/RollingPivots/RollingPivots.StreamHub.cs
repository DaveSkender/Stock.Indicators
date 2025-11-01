namespace Skender.Stock.Indicators;

/// <summary>
/// Represents a stream hub for calculating the Rolling Pivot Points.
/// </summary>
public class RollingPivotsHub
    : StreamHub<IQuote, RollingPivotsResult>, IRollingPivots
{
    private readonly string hubName;
    private readonly RollingWindowMax<decimal> _highWindow;
    private readonly RollingWindowMin<decimal> _lowWindow;

    /// <summary>
    /// Initializes a new instance of the <see cref="RollingPivotsHub"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="windowPeriods">The number of periods in the rolling window.</param>
    /// <param name="offsetPeriods">The number of periods to offset the window.</param>
    /// <param name="pointType">The type of pivot point calculation to use.</param>
    internal RollingPivotsHub(
        IQuoteProvider<IQuote> provider,
        int windowPeriods,
        int offsetPeriods,
        PivotPointType pointType) : base(provider)
    {
        RollingPivots.Validate(windowPeriods, offsetPeriods);

        WindowPeriods = windowPeriods;
        OffsetPeriods = offsetPeriods;
        PointType = pointType;

        hubName = $"ROLLING-PIVOTS({windowPeriods},{offsetPeriods},{pointType.ToString().ToUpperInvariant()})";

        // Initialize rolling windows for O(1) amortized max/min tracking
        _highWindow = new RollingWindowMax<decimal>(windowPeriods);
        _lowWindow = new RollingWindowMin<decimal>(windowPeriods);

        Reinitialize();
    }

    /// <summary>
    /// Gets the number of periods in the rolling window.
    /// </summary>
    public int WindowPeriods { get; init; }

    /// <summary>
    /// Gets the number of periods to offset the window.
    /// </summary>
    public int OffsetPeriods { get; init; }

    /// <summary>
    /// Gets the type of pivot point calculation to use.
    /// </summary>
    public PivotPointType PointType { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (RollingPivotsResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Handle warmup periods
        // Need windowPeriods + offsetPeriods worth of data before we can calculate
        if (i < WindowPeriods + OffsetPeriods)
        {
            // Build up the windows during warmup
            // We need to track quotes up to i - offsetPeriods - 1
            if (i >= OffsetPeriods)
            {
                IQuote warmupQuote = ProviderCache[i - OffsetPeriods];
                _highWindow.Add(warmupQuote.High);
                _lowWindow.Add(warmupQuote.Low);
            }

            return (new RollingPivotsResult { Timestamp = item.Timestamp }, i);
        }

        // Get the window close value (the last quote in the window)
        decimal windowClose = ProviderCache[i - OffsetPeriods - 1].Close;

        // Get highest high and lowest low from rolling windows (O(1))
        decimal windowHigh = _highWindow.Max;
        decimal windowLow = _lowWindow.Min;

        // Calculate pivot points
        WindowPoint wp = PivotPoints.GetPivotPoint(
            PointType, item.Open, windowHigh, windowLow, windowClose);

        // Add current offset quote to windows AFTER calculating result
        // This maintains the invariant that windows contain the prior WindowPeriods items
        IQuote currentOffsetQuote = ProviderCache[i - OffsetPeriods];
        _highWindow.Add(currentOffsetQuote.High);
        _lowWindow.Add(currentOffsetQuote.Low);

        RollingPivotsResult r = new() {
            Timestamp = item.Timestamp,
            PP = wp.PP,
            S1 = wp.S1,
            S2 = wp.S2,
            S3 = wp.S3,
            S4 = wp.S4,
            R1 = wp.R1,
            R2 = wp.R2,
            R3 = wp.R3,
            R4 = wp.R4
        };

        return (r, i);
    }

    /// <summary>
    /// Restores the rolling window state up to the specified timestamp.
    /// Clears and rebuilds rolling windows from ProviderCache for Insert/Remove operations.
    /// </summary>
    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // Clear rolling windows
        _highWindow.Clear();
        _lowWindow.Clear();

        // Find target index in ProviderCache
        int index = ProviderCache.IndexGte(timestamp);
        if (index <= 0)
        {
            return;
        }

        // Rebuild up to the index before the rollback timestamp
        int targetIndex = index - 1;

        // We need to rebuild the window state as it would be at targetIndex
        // The window at targetIndex should contain quotes from
        // [targetIndex - WindowPeriods - OffsetPeriods + 1] to [targetIndex - OffsetPeriods]

        int windowEndIdx = targetIndex - OffsetPeriods;
        if (windowEndIdx < 0)
        {
            return; // Not enough data to rebuild window yet
        }

        int startIdx = Math.Max(0, windowEndIdx - WindowPeriods + 1);

        // Rebuild rolling windows from ProviderCache
        for (int p = startIdx; p <= windowEndIdx; p++)
        {
            IQuote quote = ProviderCache[p];
            _highWindow.Add(quote.High);
            _lowWindow.Add(quote.Low);
        }
    }
}

/// <summary>
/// Provides methods for calculating the Rolling Pivot Points using a stream hub.
/// </summary>
public static partial class RollingPivots
{
    /// <summary>
    /// Creates a Rolling Pivot Points streaming hub from a quotes provider.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="windowPeriods">The number of periods in the rolling window.</param>
    /// <param name="offsetPeriods">The number of periods to offset the window.</param>
    /// <param name="pointType">The type of pivot point calculation to use.</param>
    /// <returns>An instance of <see cref="RollingPivotsHub"/>.</returns>
    public static RollingPivotsHub ToRollingPivotsHub(
        this IQuoteProvider<IQuote> quoteProvider,
        int windowPeriods = 20,
        int offsetPeriods = 0,
        PivotPointType pointType = PivotPointType.Standard)
        => new(quoteProvider, windowPeriods, offsetPeriods, pointType);

    /// <summary>
    /// Creates a Rolling Pivot Points hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="windowPeriods">The number of periods in the rolling window.</param>
    /// <param name="offsetPeriods">The number of periods to offset the window.</param>
    /// <param name="pointType">The type of pivot point calculation to use.</param>
    /// <returns>An instance of <see cref="RollingPivotsHub"/>.</returns>
    public static RollingPivotsHub ToRollingPivotsHub(
        this IReadOnlyList<IQuote> quotes,
        int windowPeriods = 20,
        int offsetPeriods = 0,
        PivotPointType pointType = PivotPointType.Standard)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToRollingPivotsHub(windowPeriods, offsetPeriods, pointType);
    }
}
