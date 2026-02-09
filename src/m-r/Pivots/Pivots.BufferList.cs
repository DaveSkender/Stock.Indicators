namespace Skender.Stock.Indicators;

/// <summary>
/// Pivot Points from incremental quotes.
/// </summary>
public class PivotsList : BufferList<PivotsResult>, IIncrementFromQuote
{
    private readonly int _leftSpan;
    private readonly int _rightSpan;
    private readonly int _maxTrendPeriods;
    private readonly EndType _endType;
    private readonly List<QuoteBuffer> _quoteBuffer;

    private int? _lastHighIndex;
    private decimal? _lastHighValue;
    private int? _lastLowIndex;
    private decimal? _lastLowValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="PivotsList"/> class.
    /// </summary>
    /// <param name="leftSpan">The number of periods to the left of the pivot point.</param>
    /// <param name="rightSpan">The number of periods to the right of the pivot point.</param>
    /// <param name="maxTrendPeriods">The maximum number of periods for trend calculation.</param>
    /// <param name="endType">The type of end point for the pivot calculation.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="endType"/> is invalid.</exception>
    public PivotsList(
        int leftSpan = 2,
        int rightSpan = 2,
        int maxTrendPeriods = 20,
        EndType endType = EndType.HighLow)
    {
        Pivots.Validate(leftSpan, rightSpan, maxTrendPeriods);
        LeftSpan = leftSpan;
        RightSpan = rightSpan;
        MaxTrendPeriods = maxTrendPeriods;
        EndType = endType;

        _leftSpan = leftSpan;
        _rightSpan = rightSpan;
        _maxTrendPeriods = maxTrendPeriods;
        _endType = endType;

        // Buffer needs to hold enough quotes to compute fractals and track trends
        _quoteBuffer = [];

        Name = $"PIVOTS({2}, {2}, {20}, {EndType.HighLow})";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PivotsList"/> class with initial quotes.
    /// </summary>
    /// <param name="leftSpan">The number of periods to the left of the pivot point.</param>
    /// <param name="rightSpan">The number of periods to the right of the pivot point.</param>
    /// <param name="maxTrendPeriods">The maximum number of periods for trend calculation.</param>
    /// <param name="endType">The type of end point for the pivot calculation.</param>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    public PivotsList(
        int leftSpan,
        int rightSpan,
        int maxTrendPeriods,
        EndType endType,
        IReadOnlyList<IQuote> quotes)
        : this(leftSpan, rightSpan, maxTrendPeriods, endType) => Add(quotes);

    /// <inheritdoc />
    public int LeftSpan { get; init; }

    /// <inheritdoc />
    public int RightSpan { get; init; }

    /// <inheritdoc />
    public int MaxTrendPeriods { get; init; }

    /// <inheritdoc />
    public EndType EndType { get; init; }

    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);

        DateTime timestamp = quote.Timestamp;

        // Add quote to buffer
        _quoteBuffer.Add(new QuoteBuffer(
            quote.Timestamp,
            quote.High,
            quote.Low,
            quote.Close));

        // Trim buffer if it exceeds maximum needed size
        int maxBufferSize = _leftSpan + _rightSpan + _maxTrendPeriods + 1;
        while (_quoteBuffer.Count > maxBufferSize && Count > maxBufferSize)
        {
            _quoteBuffer.RemoveAt(0);
        }

        // Initialize result with no values
        PivotsResult result = new(
            Timestamp: timestamp,
            HighPoint: null,
            LowPoint: null,
            HighLine: null,
            LowLine: null,
            HighTrend: null,
            LowTrend: null);

        // Add result to list
        AddInternal(result);

        // Need at least leftSpan + rightSpan + 1 quotes to identify fractals
        if (_quoteBuffer.Count < _leftSpan + _rightSpan + 1)
        {
            return;
        }

        // Compute fractal for quote at position (Count - rightSpan - 1)
        // This is the quote that now has rightSpan quotes after it
        int fractalCheckIndex = Count - _rightSpan - 1;
        if (fractalCheckIndex < _leftSpan)
        {
            return;
        }

        int bufferCheckIndex = _quoteBuffer.Count - _rightSpan - 1;
        if (bufferCheckIndex < _leftSpan)
        {
            return;
        }

        ComputeFractalAndUpdatePivots(fractalCheckIndex, bufferCheckIndex);
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
        _quoteBuffer.Clear();
        _lastHighIndex = null;
        _lastHighValue = null;
        _lastLowIndex = null;
        _lastLowValue = null;
    }
    /// <summary>
    /// Overrides list pruning to reset pivot tracking state when pruning occurs.
    /// </summary>
    protected override void PruneList()
    {
        int originalCount = Count;

        // Call base implementation to prune the outer result list
        base.PruneList();

        int pruneCount = originalCount - Count;

        // Adjust pivot tracking indices after pruning
        if (pruneCount > 0)
        {
            // Adjust last high index
            if (_lastHighIndex != null)
            {
                int? adjustedHighIndex = (int)_lastHighIndex - pruneCount;

                // If the last high was pruned away, reset it
                if (adjustedHighIndex < 0)
                {
                    _lastHighIndex = null;
                    _lastHighValue = null;
                }
                else
                {
                    _lastHighIndex = adjustedHighIndex;
                }
            }

            // Adjust last low index
            if (_lastLowIndex != null)
            {
                int? adjustedLowIndex = (int)_lastLowIndex - pruneCount;

                // If the last low was pruned away, reset it
                if (adjustedLowIndex < 0)
                {
                    _lastLowIndex = null;
                    _lastLowValue = null;
                }
                else
                {
                    _lastLowIndex = adjustedLowIndex;
                }
            }
        }
    }

    private void ComputeFractalAndUpdatePivots(int resultIndex, int bufferIndex)
    {
        QuoteBuffer evalQuote = _quoteBuffer[bufferIndex];
        bool isHighFractal = true;
        bool isLowFractal = true;

        decimal evalHigh = _endType == EndType.Close ? evalQuote.Close : evalQuote.High;
        decimal evalLow = _endType == EndType.Close ? evalQuote.Close : evalQuote.Low;

        // Check left and right wings
        for (int p = bufferIndex - _leftSpan; p <= bufferIndex + _rightSpan; p++)
        {
            if (p == bufferIndex)
            {
                continue;
            }

            QuoteBuffer wing = _quoteBuffer[p];
            decimal wingHigh = _endType == EndType.Close ? wing.Close : wing.High;
            decimal wingLow = _endType == EndType.Close ? wing.Close : wing.Low;

            if (evalHigh <= wingHigh)
            {
                isHighFractal = false;
            }

            if (evalLow >= wingLow)
            {
                isLowFractal = false;
            }
        }

        decimal? highPoint = isHighFractal ? evalHigh : null;
        decimal? lowPoint = isLowFractal ? evalLow : null;

        // Update the result at resultIndex with fractal points
        PivotsResult current = this[resultIndex];
        PivotsResult updated = current with {
            HighPoint = highPoint,
            LowPoint = lowPoint
        };
        UpdateInternal(resultIndex, updated);

        // Reset expired indexes
        if (_lastHighIndex < resultIndex - _maxTrendPeriods)
        {
            _lastHighIndex = null;
            _lastHighValue = null;
        }

        if (_lastLowIndex < resultIndex - _maxTrendPeriods)
        {
            _lastLowIndex = null;
            _lastLowValue = null;
        }

        // Update high trend if new high fractal found
        if (highPoint != null)
        {
            if (_lastHighIndex != null && highPoint != _lastHighValue && resultIndex != _lastHighIndex)
            {
                PivotTrend trend = highPoint > _lastHighValue
                    ? PivotTrend.Hh
                    : PivotTrend.Lh;

                // Set the line value at the last high index
                PivotsResult lastHighResult = this[(int)_lastHighIndex];
                PivotsResult lastHighUpdated = lastHighResult with {
                    HighLine = _lastHighValue
                };
                UpdateInternal((int)_lastHighIndex, lastHighUpdated);

                decimal? incr = (highPoint - _lastHighValue) / (resultIndex - _lastHighIndex);

                // Repaint trend line from last high + 1 to current high
                for (int t = (int)_lastHighIndex + 1; t <= resultIndex; t++)
                {
                    if (t < Count)
                    {
                        PivotsResult existingResult = this[t];
                        decimal? highLineValue = highPoint + (incr * (t - resultIndex));
                        PivotsResult repainted = existingResult with {
                            HighLine = highLineValue,
                            HighTrend = trend
                        };
                        UpdateInternal(t, repainted);
                    }
                }
            }

            _lastHighIndex = resultIndex;
            _lastHighValue = highPoint;
        }

        // Update low trend if new low fractal found
        if (lowPoint != null)
        {
            if (_lastLowIndex != null && lowPoint != _lastLowValue && resultIndex != _lastLowIndex)
            {
                PivotTrend trend = lowPoint > _lastLowValue
                    ? PivotTrend.Hl
                    : PivotTrend.Ll;

                // Set the line value at the last low index
                PivotsResult lastLowResult = this[(int)_lastLowIndex];
                PivotsResult lastLowUpdated = lastLowResult with {
                    LowLine = _lastLowValue
                };
                UpdateInternal((int)_lastLowIndex, lastLowUpdated);

                decimal? incr = (lowPoint - _lastLowValue) / (resultIndex - _lastLowIndex);

                // Repaint trend line from last low + 1 to current low
                for (int t = (int)_lastLowIndex + 1; t <= resultIndex; t++)
                {
                    if (t < Count)
                    {
                        PivotsResult existingResult = this[t];
                        decimal? lowLineValue = lowPoint + (incr * (t - resultIndex));
                        PivotsResult repainted = existingResult with {
                            LowLine = lowLineValue,
                            LowTrend = trend
                        };
                        UpdateInternal(t, repainted);
                    }
                }
            }

            _lastLowIndex = resultIndex;
            _lastLowValue = lowPoint;
        }
    }

    private sealed record QuoteBuffer(DateTime Timestamp, decimal High, decimal Low, decimal Close);
}

public static partial class Pivots
{
    /// <summary>
    /// Creates a buffer list for Pivot Points calculations.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="leftSpan">The number of periods to the left of the pivot point.</param>
    /// <param name="rightSpan">The number of periods to the right of the pivot point.</param>
    /// <param name="maxTrendPeriods">The maximum number of periods for trend calculation.</param>
    /// <param name="endType">The type of end point for the pivot calculation.</param>
    /// <returns>A new <see cref="PivotsList"/> instance.</returns>
    public static PivotsList ToPivotsList(
        this IReadOnlyList<IQuote> quotes,
        int leftSpan = 2,
        int rightSpan = 2,
        int maxTrendPeriods = 20,
        EndType endType = EndType.HighLow)
        => new(leftSpan, rightSpan, maxTrendPeriods, endType) { quotes };
}
