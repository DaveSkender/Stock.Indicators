namespace Skender.Stock.Indicators;

/// <summary>
/// Pivot Points from incremental quotes.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PivotPointsList"/> class.
/// </remarks>
/// <param name="windowSize">The size of the window for pivot point calculation.</param>
/// <param name="pointType">The type of pivot point calculation to use.</param>
public class PivotPointsList(
    PeriodSize windowSize = PeriodSize.Month,
    PivotPointType pointType = PivotPointType.Standard) : BufferList<PivotPointsResult>, IIncrementFromQuote, IPivotPoints
{
    private int _windowId = int.MinValue;
    private bool _firstWindow = true;
    private decimal _windowHigh;
    private decimal _windowLow;
    private decimal _windowOpen;
    private decimal _windowClose;
    private WindowPoint _windowPoint = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="PivotPointsList"/> class with initial quotes.
    /// </summary>
    /// <param name="windowSize">The size of the window for pivot point calculation.</param>
    /// <param name="pointType">The type of pivot point calculation to use.</param>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    public PivotPointsList(
        PeriodSize windowSize,
        PivotPointType pointType,
        IReadOnlyList<IQuote> quotes)
        : this(windowSize, pointType) => Add(quotes);

    /// <inheritdoc />
    public PeriodSize WindowSize { get; init; } = windowSize;

    /// <inheritdoc />
    public PivotPointType PointType { get; init; } = pointType;

    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);

        DateTime timestamp = quote.Timestamp;

        // Initialize window tracking on first quote
        if (_windowId == int.MinValue)
        {
            _windowId = GetWindowNumber(timestamp, WindowSize);
            _windowHigh = quote.High;
            _windowLow = quote.Low;
            _windowOpen = quote.Open;
            _windowClose = quote.Close;
        }

        // Check for new window
        int windowEval = GetWindowNumber(timestamp, WindowSize);

        if (windowEval != _windowId)
        {
            _windowId = windowEval;
            _firstWindow = false;

            // Set new levels
            if (PointType == PivotPointType.Woodie)
            {
                _windowOpen = quote.Open;
            }

            _windowPoint = GetPivotPoint(
                PointType, _windowOpen, _windowHigh, _windowLow, _windowClose);

            // Reset window min/max thresholds
            _windowOpen = quote.Open;
            _windowHigh = quote.High;
            _windowLow = quote.Low;
        }

        // Add levels
        PivotPointsResult result = !_firstWindow
            ? new() {
                Timestamp = timestamp,
                PP = _windowPoint.PP,
                S1 = _windowPoint.S1,
                S2 = _windowPoint.S2,
                S3 = _windowPoint.S3,
                S4 = _windowPoint.S4,
                R1 = _windowPoint.R1,
                R2 = _windowPoint.R2,
                R3 = _windowPoint.R3,
                R4 = _windowPoint.R4
            }
            : new PivotPointsResult {
                Timestamp = timestamp
            };

        AddInternal(result);

        // Capture window thresholds (for next iteration)
        _windowHigh = quote.High > _windowHigh ? quote.High : _windowHigh;
        _windowLow = quote.Low < _windowLow ? quote.Low : _windowLow;
        _windowClose = quote.Close;
    }

    /// <inheritdoc />
    public void Add(IReadOnlyList<IQuote> quotes)
    {
        ArgumentNullException.ThrowIfNull(quotes);

        for (int i = 0; i < quotes.Count; i++)
        {
            Add(quotes[i]);
        }
    }

    /// <inheritdoc />
    public override void Clear()
    {
        base.Clear();
        _windowId = int.MinValue;
        _firstWindow = true;
        _windowHigh = 0;
        _windowLow = 0;
        _windowOpen = 0;
        _windowClose = 0;
        _windowPoint = new();
    }

    private static int GetWindowNumber(DateTime d, PeriodSize windowSize)
        => PivotPoints.GetWindowNumber(d, windowSize);

    private static WindowPoint GetPivotPoint(
        PivotPointType pointType, decimal open, decimal high, decimal low, decimal close)
        => PivotPoints.GetPivotPoint(pointType, open, high, low, close);
}

public static partial class PivotPoints
{
    /// <summary>
    /// Creates a buffer list for Pivot Points calculations.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="windowSize">The size of the window for pivot point calculation.</param>
    /// <param name="pointType">The type of pivot point calculation to use.</param>
    /// <returns>A new <see cref="PivotPointsList"/> instance.</returns>
    public static PivotPointsList ToPivotPointsList(
        this IReadOnlyList<IQuote> quotes,
        PeriodSize windowSize = PeriodSize.Month,
        PivotPointType pointType = PivotPointType.Standard)
        => new(windowSize, pointType) { quotes };
}
