namespace Skender.Stock.Indicators;

/// <summary>
/// Williams Fractal from incremental quote values.
/// </summary>
public class FractalList : BufferList<FractalResult>, IIncrementFromQuote, IFractal
{
    private readonly List<FractalBuffer> _quotes;

    /// <summary>
    /// Initializes a new instance of the <see cref="FractalList"/> class.
    /// </summary>
    /// <param name="windowSpan">The number of periods to look back and forward for the calculation.</param>
    /// <param name="endType">The type of price to use for the calculation.</param>
    public FractalList(int windowSpan = 2, EndType endType = EndType.HighLow)
        : this(windowSpan, windowSpan, endType)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FractalList"/> class with different left and right spans.
    /// </summary>
    /// <param name="leftSpan">The number of periods to look back for the calculation.</param>
    /// <param name="rightSpan">The number of periods to look forward for the calculation.</param>
    /// <param name="endType">The type of price to use for the calculation.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="endType"/> is invalid.</exception>
    public FractalList(int leftSpan, int rightSpan, EndType endType = EndType.HighLow)
    {
        Fractal.Validate(Math.Min(leftSpan, rightSpan));
        LeftSpan = leftSpan;
        RightSpan = rightSpan;
        EndType = endType;

        _quotes = [];

        Name = $"FRACTAL({2}, {EndType.HighLow})";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FractalList"/> class with initial quotes.
    /// </summary>
    /// <param name="windowSpan">The number of periods to look back and forward for the calculation.</param>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="endType">The type of price to use for the calculation.</param>
    public FractalList(int windowSpan, IReadOnlyList<IQuote> quotes, EndType endType = EndType.HighLow)
        : this(windowSpan, endType) => Add(quotes);

    /// <summary>
    /// Initializes a new instance of the <see cref="FractalList"/> class with different spans and initial quotes.
    /// </summary>
    /// <param name="leftSpan">The number of periods to look back for the calculation.</param>
    /// <param name="rightSpan">The number of periods to look forward for the calculation.</param>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="endType">The type of price to use for the calculation.</param>
    public FractalList(int leftSpan, int rightSpan, IReadOnlyList<IQuote> quotes, EndType endType = EndType.HighLow)
        : this(leftSpan, rightSpan, endType) => Add(quotes);

    /// <inheritdoc />
    public int LeftSpan { get; init; }

    /// <inheritdoc />
    public int RightSpan { get; init; }

    /// <inheritdoc />
    public EndType EndType { get; init; }

    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);

        // store the quote
        _quotes.Add(new FractalBuffer(
            quote.High,
            quote.Low,
            quote.Close,
            quote.Timestamp));

        // always add a result (initially null)
        AddInternal(new FractalResult(quote.Timestamp, null, null));

        int length = _quotes.Count;

        // Calculate fractal for the quote that just became calculable (if any)
        // We only need to calculate the newest calculable index, not recalculate a range
        // Previous fractals are already correct from earlier calls
        int lastCalculableIndex = length - RightSpan - 1;

        // Only calculate if this index is newly calculable and has sufficient context
        if (lastCalculableIndex >= 0 && lastCalculableIndex + 1 > LeftSpan)
        {
            int i = lastCalculableIndex;
            FractalBuffer center = _quotes[i];
            bool isHigh = true;
            bool isLow = true;

            decimal evalHigh = EndType == EndType.Close ? center.Close : center.High;
            decimal evalLow = EndType == EndType.Close ? center.Close : center.Low;

            // compare with wings
            for (int p = i - LeftSpan; p <= i + RightSpan; p++)
            {
                // skip center
                if (p == i)
                {
                    continue;
                }

                FractalBuffer wing = _quotes[p];
                decimal wingHigh = EndType == EndType.Close ? wing.Close : wing.High;
                decimal wingLow = EndType == EndType.Close ? wing.Close : wing.Low;

                if (evalHigh <= wingHigh)
                {
                    isHigh = false;
                }

                if (evalLow >= wingLow)
                {
                    isLow = false;
                }
            }

            decimal? fractalBear = isHigh ? evalHigh : null;
            decimal? fractalBull = isLow ? evalLow : null;

            // update the result at index i
            UpdateInternal(i, new FractalResult(center.Timestamp, fractalBear, fractalBull));
        }
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
        _quotes.Clear();
    }
    /// <inheritdoc />
    protected override void PruneList()
    {
        int overflow = Count - MaxListSize;

        if (overflow <= 0)
        {
            return;
        }

        // remove from results
        base.PruneList();

        // also remove from quotes
        _quotes.RemoveRange(0, overflow);
    }

    internal class FractalBuffer(
        decimal high,
        decimal low,
        decimal close,
        DateTime timestamp)
    {
        internal decimal High { get; init; } = high;
        internal decimal Low { get; init; } = low;
        internal decimal Close { get; init; } = close;
        internal DateTime Timestamp { get; init; } = timestamp;
    }
}

public static partial class Fractal
{
    /// <summary>
    /// Creates a buffer list for Williams Fractal calculations.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="windowSpan">The number of periods to look back and forward for the calculation.</param>
    /// <param name="endType">The type of price to use for the calculation.</param>
    /// <returns>An initialized <see cref="FractalList" />.</returns>
    public static FractalList ToFractalList(
        this IReadOnlyList<IQuote> quotes,
        int windowSpan = 2,
        EndType endType = EndType.HighLow)
        => new(windowSpan, quotes, endType);

    /// <summary>
    /// Creates a buffer list for Williams Fractal calculations with different left and right spans.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="leftSpan">The number of periods to look back for the calculation.</param>
    /// <param name="rightSpan">The number of periods to look forward for the calculation.</param>
    /// <param name="endType">The type of price to use for the calculation.</param>
    /// <returns>An initialized <see cref="FractalList" />.</returns>
    public static FractalList ToFractalList(
        this IReadOnlyList<IQuote> quotes,
        int leftSpan,
        int rightSpan,
        EndType endType = EndType.HighLow)
        => new(leftSpan, rightSpan, quotes, endType);
}
