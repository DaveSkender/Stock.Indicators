namespace Skender.Stock.Indicators;

// PIVOT POINTS (STREAM HUB)

/// <summary>
/// Provides methods for calculating Pivot Points using a stream hub.
/// </summary>
public class PivotPointsHub
    : StreamHub<IQuote, PivotPointsResult>
{
    #region fields

    private readonly string hubName;
    private int windowId;
    private bool firstWindow;
    private decimal windowHigh;
    private decimal windowLow;
    private decimal windowOpen;
    private decimal windowClose;
    private WindowPoint windowPoint;

    #endregion fields

    #region constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="PivotPointsHub"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="windowSize">The size of the window for pivot calculation.</param>
    /// <param name="pointType">The type of pivot points to calculate.</param>
    internal PivotPointsHub(
        IQuoteProvider<IQuote> provider,
        PeriodSize windowSize,
        PivotPointType pointType) : base(provider)
    {
        WindowSize = windowSize;
        PointType = pointType;
        hubName = $"PIVOT-POINTS({windowSize},{pointType})";

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

    #endregion constructors

    #region properties

    /// <summary>
    /// Gets the window size for pivot calculation.
    /// </summary>
    public PeriodSize WindowSize { get; init; }

    /// <summary>
    /// Gets the type of pivot points to calculate.
    /// </summary>
    public PivotPointType PointType { get; init; }

    #endregion properties

    #region methods

    /// <inheritdoc/>
    public override string ToString() => hubName;

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
            if (PointType == PivotPointType.Woodie)
            {
                windowOpen = q.Open;
            }

            windowPoint = PivotPoints.GetPivotPoint(
                PointType, windowOpen, windowHigh, windowLow, windowClose);

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

    #endregion methods
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

    /// <summary>
    /// Creates a PivotPoints hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="windowSize">The size of the window for pivot calculation. Default is <see cref="PeriodSize.Month"/>.</param>
    /// <param name="pointType">The type of pivot points to calculate. Default is <see cref="PivotPointType.Standard"/>.</param>
    /// <returns>An instance of <see cref="PivotPointsHub"/>.</returns>
    public static PivotPointsHub ToPivotPointsHub(
        this IReadOnlyList<IQuote> quotes,
        PeriodSize windowSize = PeriodSize.Month,
        PivotPointType pointType = PivotPointType.Standard)
    {
        ArgumentNullException.ThrowIfNull(quotes);
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToPivotPointsHub(windowSize, pointType);
    }
}
