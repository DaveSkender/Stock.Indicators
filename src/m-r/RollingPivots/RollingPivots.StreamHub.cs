namespace Skender.Stock.Indicators;

// ROLLING PIVOT POINTS (STREAM HUB)

/// <summary>
/// Provides methods for calculating Rolling Pivot Points using a stream hub.
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
}

/// <summary>
/// Represents a stream hub for calculating Rolling Pivot Points.
/// </summary>
public class RollingPivotsHub
    : StreamHub<IQuote, RollingPivotsResult>
{
    private readonly string hubName;
    private readonly RollingWindowMax<decimal> _highWindow;
    private readonly RollingWindowMin<decimal> _lowWindow;
    private readonly Queue<IQuote> _offsetBuffer;

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
        hubName = $"ROLLING-PIVOTS({windowPeriods},{offsetPeriods},{pointType})";
        _highWindow = new RollingWindowMax<decimal>(windowPeriods);
        _lowWindow = new RollingWindowMin<decimal>(windowPeriods);
        _offsetBuffer = new Queue<IQuote>(offsetPeriods + 1);

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
    /// Gets the type of pivot point calculation.
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

        // Build offset buffer during early periods
        // We need windowPeriods + offsetPeriods + 1 total quotes before we can calculate
        if (i < WindowPeriods + OffsetPeriods)
        {
            // During warmup, add to windows and buffer
            _highWindow.Add(item.High);
            _lowWindow.Add(item.Low);
            _offsetBuffer.Update(OffsetPeriods + 1, item);
            return (new RollingPivotsResult { Timestamp = item.Timestamp }, i);
        }

        // Get window close value from offset buffer
        // The offset buffer contains the last (OffsetPeriods + 1) quotes
        // We want the close from OffsetPeriods quotes ago (the first item in buffer)
        decimal windowClose = _offsetBuffer.Peek().Close;

        // Get high/low from rolling windows (these track the prior WindowPeriods quotes)
        decimal windowHigh = _highWindow.Max;
        decimal windowLow = _lowWindow.Min;

        // Calculate pivot points
        WindowPoint wp = PivotPoints.GetPivotPoint(
            PointType, item.Open, windowHigh, windowLow, windowClose);

        // Update windows and offset buffer with current item AFTER calculation
        _highWindow.Add(item.High);
        _lowWindow.Add(item.Low);
        _offsetBuffer.Update(OffsetPeriods + 1, item);

        RollingPivotsResult r = new() {
            Timestamp = item.Timestamp,
            PP = wp.PP,
            S1 = wp.S1, S2 = wp.S2, S3 = wp.S3, S4 = wp.S4,
            R1 = wp.R1, R2 = wp.R2, R3 = wp.R3, R4 = wp.R4
        };

        return (r, i);
    }

    /// <summary>
    /// Restores the rolling window and offset buffer state up to the specified timestamp.
    /// Clears and rebuilds state from ProviderCache for Insert/Remove operations.
    /// </summary>
    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // Clear state
        _highWindow.Clear();
        _lowWindow.Clear();
        _offsetBuffer.Clear();

        // Find target index in ProviderCache
        int index = ProviderCache.IndexGte(timestamp);
        if (index <= 0)
        {
            return;
        }

        // Rebuild up to the index before the rollback timestamp
        int targetIndex = index - 1;
        int totalNeeded = WindowPeriods + OffsetPeriods;
        int startIdx = Math.Max(0, targetIndex + 1 - totalNeeded);

        // Rebuild rolling windows and offset buffer from ProviderCache
        for (int p = startIdx; p <= targetIndex; p++)
        {
            IQuote quote = ProviderCache[p];
            _highWindow.Add(quote.High);
            _lowWindow.Add(quote.Low);

            // Only add to offset buffer for the last (OffsetPeriods + 1) quotes
            if (p > targetIndex - OffsetPeriods - 1)
            {
                _offsetBuffer.Enqueue(quote);
            }
        }
    }
}
