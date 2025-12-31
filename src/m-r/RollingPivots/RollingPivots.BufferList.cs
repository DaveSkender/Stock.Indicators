namespace Skender.Stock.Indicators;

/// <summary>
/// Rolling Pivot Points from incremental quotes.
/// </summary>
public class RollingPivotsList : BufferList<RollingPivotsResult>, IIncrementFromQuote
{
    private readonly Queue<IQuote> _buffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="RollingPivotsList"/> class.
    /// </summary>
    /// <param name="windowPeriods">The number of periods in the rolling window.</param>
    /// <param name="offsetPeriods">The number of periods to offset the window.</param>
    /// <param name="pointType">The type of pivot point calculation to use.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="pointType"/> is invalid.</exception>
    public RollingPivotsList(
        int windowPeriods = 20,
        int offsetPeriods = 0,
        PivotPointType pointType = PivotPointType.Standard)
    {
        RollingPivots.Validate(windowPeriods, offsetPeriods);
        WindowPeriods = windowPeriods;
        OffsetPeriods = offsetPeriods;
        PointType = pointType;
        _buffer = new Queue<IQuote>(windowPeriods + offsetPeriods + 1);

        Name = $"ROLLINGPIVOTS({20}, {0}, {PivotPointType.Standard})";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RollingPivotsList"/> class with initial quotes.
    /// </summary>
    /// <param name="windowPeriods">The number of periods in the rolling window.</param>
    /// <param name="offsetPeriods">The number of periods to offset the window.</param>
    /// <param name="pointType">The type of pivot point calculation to use.</param>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    public RollingPivotsList(
        int windowPeriods,
        int offsetPeriods,
        PivotPointType pointType,
        IReadOnlyList<IQuote> quotes)
        : this(windowPeriods, offsetPeriods, pointType) => Add(quotes);

    /// <inheritdoc />
    public int WindowPeriods { get; }

    /// <inheritdoc />
    public int OffsetPeriods { get; }

    /// <inheritdoc />
    public PivotPointType PointType { get; }

    /// <inheritdoc />
    public int LookbackPeriods => WindowPeriods + OffsetPeriods;

    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);

        // Update buffer with new quote
        _buffer.Update(WindowPeriods + OffsetPeriods + 1, quote);

        RollingPivotsResult result;

        // Check if we have enough data to calculate pivots
        // Need buffer to contain windowPeriods + offsetPeriods + 1 quotes
        if (_buffer.Count > WindowPeriods + OffsetPeriods)
        {
            // Get the window data from buffer
            // The buffer contains the last (windowPeriods + offsetPeriods + 1) quotes
            // Current quote is the last element (bufferCount - 1)
            // Window ends at (bufferCount - 1 - 1 - offsetPeriods) = (bufferCount - 2 - offsetPeriods)
            // because the current quote itself is not part of the window when offsetPeriods = 0

            IQuote[] bufferArray = _buffer.ToArray();
            int bufferCount = bufferArray.Length;

            // Window ends offsetPeriods + 1 positions before the end (the "+1" accounts for the current quote)
            int windowEndIndex = bufferCount - 2 - OffsetPeriods;
            int windowStartIndex = windowEndIndex - WindowPeriods + 1;

            decimal windowHigh = bufferArray[windowStartIndex].High;
            decimal windowLow = bufferArray[windowStartIndex].Low;
            decimal windowClose = bufferArray[windowEndIndex].Close;

            for (int p = windowStartIndex; p <= windowEndIndex; p++)
            {
                IQuote d = bufferArray[p];
                windowHigh = d.High > windowHigh ? d.High : windowHigh;
                windowLow = d.Low < windowLow ? d.Low : windowLow;
            }

            // Calculate pivot points
            WindowPoint wp = PivotPoints.GetPivotPoint(
                PointType, quote.Open, windowHigh, windowLow, windowClose);

            result = new RollingPivotsResult {
                Timestamp = quote.Timestamp,
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
        }
        else
        {
            result = new RollingPivotsResult { Timestamp = quote.Timestamp };
        }

        AddInternal(result);
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
        _buffer.Clear();
    }
}

public static partial class RollingPivots
{
    /// <summary>
    /// Creates a buffer list for Rolling Pivot Points calculations.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="windowPeriods">Number of periods in the rolling window</param>
    /// <param name="offsetPeriods">Number of periods to offset</param>
    /// <param name="pointType">Type of pivot point calculation</param>
    public static RollingPivotsList ToRollingPivotsList(
        this IReadOnlyList<IQuote> quotes,
        int windowPeriods = 20,
        int offsetPeriods = 0,
        PivotPointType pointType = PivotPointType.Standard)
        => new(windowPeriods, offsetPeriods, pointType) { quotes };
}
