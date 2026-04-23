namespace Skender.Stock.Indicators;

/// <summary>
/// Streaming hub for Rolling Pivot Points using a stream hub.
/// </summary>
public static partial class RollingPivots
{
    /// <summary>
    /// Creates a Rolling Pivot Points streaming hub from a quotes provider.
    /// </summary>
    /// <param name="quoteProvider">Quote provider.</param>
    /// <param name="windowPeriods">Number of periods in the rolling window.</param>
    /// <param name="offsetPeriods">Number of periods to offset the window.</param>
    /// <param name="pointType">Type of pivot point calculation to use.</param>
    /// <returns>An instance of <see cref="RollingPivotsHub"/>.</returns>
    public static RollingPivotsHub ToRollingPivotsHub(
        this IQuoteProvider<IQuote> quoteProvider,
        int windowPeriods = 20,
        int offsetPeriods = 0,
        PivotPointType pointType = PivotPointType.Standard)
        => new(quoteProvider, windowPeriods, offsetPeriods, pointType);
}

/// <summary>
/// Streaming hub for Rolling Pivot Points.
/// </summary>
public class RollingPivotsHub
    : StreamHub<IQuote, RollingPivotsResult>
{
    private CircularDoubleBuffer _highWindow;
    private CircularDoubleBuffer _lowWindow;
    private readonly Queue<IQuote> _offsetBuffer;

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
        Name = $"ROLLING-PIVOTS({windowPeriods},{offsetPeriods},{pointType})";
        _highWindow = new CircularDoubleBuffer(windowPeriods);
        _lowWindow = new CircularDoubleBuffer(windowPeriods);
        _offsetBuffer = new Queue<IQuote>(offsetPeriods + 1);

        // Validate cache size for warmup requirements
        // RollingPivots needs windowPeriods + offsetPeriods + 1 items before first valid result.
        ValidateCacheSize(windowPeriods + offsetPeriods + 1, Name);

        Reinitialize();
    }

    /// <inheritdoc/>
    public int WindowPeriods { get; init; }

    /// <inheritdoc/>
    public int OffsetPeriods { get; init; }

    /// <summary>
    /// Gets the deterministic warmup period (WindowPeriods + OffsetPeriods), i.e., the number
    /// of initial items that produce null results before the first valid pivot point.
    /// </summary>
    public int LookbackPeriods => WindowPeriods + OffsetPeriods;

    /// <inheritdoc/>
    public PivotPointType PointType { get; init; }
    /// <inheritdoc/>
    protected override (RollingPivotsResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        if (i < WindowPeriods + OffsetPeriods)
        {
            // During warmup, add to windows and buffer
            _highWindow.Add((double)item.High);
            _lowWindow.Add((double)item.Low);
            _offsetBuffer.Update(OffsetPeriods + 1, item);
            return (new RollingPivotsResult { Timestamp = item.Timestamp }, i);
        }

        // Get window close value from offset buffer
        // The offset buffer contains the last (OffsetPeriods + 1) quotes
        // We want the close from OffsetPeriods quotes ago (the first item in buffer)
        double windowClose = (double)_offsetBuffer.Peek().Close;

        // Get high/low from rolling windows (these track the prior WindowPeriods quotes)
        double windowHigh = _highWindow.GetMax();
        double windowLow = _lowWindow.GetMin();

        // Calculate pivot points
        WindowPoint wp = PivotPoints.GetPivotPoint(
            PointType, (double)item.Open, windowHigh, windowLow, windowClose);

        // Update windows and offset buffer with current item AFTER calculation
        _highWindow.Add((double)item.High);
        _lowWindow.Add((double)item.Low);
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
    /// Clears and rebuilds state from ProviderCache for Add/Remove operations.
    /// </summary>
    /// <inheritdoc/>
    protected override void RollbackState(int restoreIndex)
    {
        // Clear state
        _highWindow.Clear();
        _lowWindow.Clear();
        _offsetBuffer.Clear();

        if (restoreIndex < 0)
        {
            return;
        }

        // Rebuild rolling windows and offset buffer from ProviderCache
        int totalNeeded = WindowPeriods + OffsetPeriods;
        int startIdx = Math.Max(0, restoreIndex + 1 - totalNeeded);

        for (int p = startIdx; p <= restoreIndex; p++)
        {
            IQuote quote = ProviderCache[p];
            _highWindow.Add((double)quote.High);
            _lowWindow.Add((double)quote.Low);

            // Only add to offset buffer for the last (OffsetPeriods + 1) quotes
            if (p > restoreIndex - OffsetPeriods - 1)
            {
                _offsetBuffer.Enqueue(quote);
            }
        }
    }
}
