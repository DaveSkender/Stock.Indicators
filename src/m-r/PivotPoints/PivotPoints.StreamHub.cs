namespace Skender.Stock.Indicators;

/// <summary>
/// Streaming hub for Pivot Points using a stream hub.
/// </summary>
public class PivotPointsHub
    : StreamHub<IQuote, PivotPointsResult>
{

    private int windowId;
    private bool firstWindow;
    private decimal windowHigh;
    private decimal windowLow;
    private decimal windowOpen;
    private decimal windowClose;
    private WindowPoint windowPoint;

    internal PivotPointsHub(
        IQuoteProvider<IQuote> provider,
        PeriodSize windowSize,
        PivotPointType pointType) : base(provider)
    {
        WindowSize = windowSize;
        PointType = pointType;
        Name = $"PIVOT-POINTS({windowSize},{pointType})";

        // Initialize state
        windowId = 0;
        firstWindow = true;
        windowHigh = 0;
        windowLow = 0;
        windowOpen = 0;
        windowClose = 0;
        windowPoint = new();

        Reinitialize();
    }

    /// <inheritdoc/>
    public PeriodSize WindowSize { get; init; }

    /// <inheritdoc/>
    public PivotPointType PointType { get; init; }
    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // Reset all state
        windowId = 0;
        firstWindow = true;
        windowHigh = 0;
        windowLow = 0;
        windowOpen = 0;
        windowClose = 0;
        windowPoint = new();

        // Find the index for the rollback timestamp
        int index = ProviderCache.IndexGte(timestamp);
        if (index <= 0)
        {
            return;
        }

        // Rebuild state up to the rollback point
        int targetIndex = index - 1;

        for (int p = 0; p <= targetIndex; p++)
        {
            IQuote q = ProviderCache[p];

            if (p == 0)
            {
                windowId = PivotPoints.GetWindowNumber(q.Timestamp, WindowSize);
                windowHigh = q.High;
                windowLow = q.Low;
                windowOpen = q.Open;
                windowClose = q.Close;
                continue;
            }

            UpdateWindowState(q);
        }
    }

    /// <inheritdoc/>
    protected override (PivotPointsResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Initialize on first quote
        if (i == 0)
        {
            windowId = PivotPoints.GetWindowNumber(item.Timestamp, WindowSize);
            firstWindow = true;
            windowHigh = item.High;
            windowLow = item.Low;
            windowOpen = item.Open;
            windowClose = item.Close;
            windowPoint = new();
        }

        UpdateWindowState(item);

        // Create result
        PivotPointsResult result = !firstWindow
            ? new() {
                Timestamp = item.Timestamp,
                PP = windowPoint.PP,
                S1 = windowPoint.S1,
                S2 = windowPoint.S2,
                S3 = windowPoint.S3,
                S4 = windowPoint.S4,
                R1 = windowPoint.R1,
                R2 = windowPoint.R2,
                R3 = windowPoint.R3,
                R4 = windowPoint.R4
            }
            : new PivotPointsResult {
                Timestamp = item.Timestamp
            };

        return (result, i);
    }

    private void UpdateWindowState(IQuote q)
    {
        // Check for new window
        int windowEval = PivotPoints.GetWindowNumber(q.Timestamp, WindowSize);

        if (windowEval != windowId)
        {
            windowId = windowEval;
            firstWindow = false;

            // Set new levels based on previous window
            decimal pivotOpen = windowOpen;
            if (PointType == PivotPointType.Woodie)
            {
                pivotOpen = q.Open;
            }

            windowPoint = PivotPoints.GetPivotPoint(
                PointType, pivotOpen, windowHigh, windowLow, windowClose);

            // Reset window min/max thresholds
            windowOpen = q.Open;
            windowHigh = q.High;
            windowLow = q.Low;
        }

        // Update window thresholds (for next iteration)
        windowHigh = q.High > windowHigh ? q.High : windowHigh;
        windowLow = q.Low < windowLow ? q.Low : windowLow;
        windowClose = q.Close;
    }

}

public static partial class PivotPoints
{
    /// <summary>
    /// Creates a PivotPoints streaming hub from a quote provider.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="windowSize">The size of the window for pivot calculation. Default is <see cref="PeriodSize.Month"/>.</param>
    /// <param name="pointType">The type of pivot points to calculate. Default is <see cref="PivotPointType.Standard"/>.</param>
    /// <returns>An instance of <see cref="PivotPointsHub"/>.</returns>
    public static PivotPointsHub ToPivotPointsHub(
        this IQuoteProvider<IQuote> quoteProvider,
        PeriodSize windowSize = PeriodSize.Month,
        PivotPointType pointType = PivotPointType.Standard)
    {
        ArgumentNullException.ThrowIfNull(quoteProvider);
        return new(quoteProvider, windowSize, pointType);
    }
}
