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
    public FractalList(int leftSpan, int rightSpan, EndType endType = EndType.HighLow)
    {
        Fractal.Validate(Math.Min(leftSpan, rightSpan));
        LeftSpan = leftSpan;
        RightSpan = rightSpan;
        EndType = endType;

        _quotes = [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FractalList"/> class with initial quotes.
    /// </summary>
    /// <param name="windowSpan">The number of periods to look back and forward for the calculation.</param>
    /// <param name="quotes">Initial quotes to populate the list.</param>
    /// <param name="endType">The type of price to use for the calculation.</param>
    public FractalList(int windowSpan, IReadOnlyList<IQuote> quotes, EndType endType = EndType.HighLow)
        : this(windowSpan, endType) => Add(quotes);

    /// <summary>
    /// Initializes a new instance of the <see cref="FractalList"/> class with different spans and initial quotes.
    /// </summary>
    /// <param name="leftSpan">The number of periods to look back for the calculation.</param>
    /// <param name="rightSpan">The number of periods to look forward for the calculation.</param>
    /// <param name="quotes">Initial quotes to populate the list.</param>
    /// <param name="endType">The type of price to use for the calculation.</param>
    public FractalList(int leftSpan, int rightSpan, IReadOnlyList<IQuote> quotes, EndType endType = EndType.HighLow)
        : this(leftSpan, rightSpan, endType) => Add(quotes);

    /// <summary>
    /// Gets the number of periods to look back for the calculation.
    /// </summary>
    public int LeftSpan { get; init; }

    /// <summary>
    /// Gets the number of periods to look forward for the calculation.
    /// </summary>
    public int RightSpan { get; init; }

    /// <summary>
    /// Gets the type of price to use for the calculation.
    /// </summary>
    public EndType EndType { get; init; }

    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);

        // store the quote
        _quotes.Add(new FractalBuffer(
            (decimal)quote.High,
            (decimal)quote.Low,
            (decimal)quote.Close,
            quote.Timestamp));

        // calculate fractal for the current quote
        // Series logic: i + 1 > leftSpan && i + 1 <= length - rightSpan
        int currentIndex = _quotes.Count - 1;
        int length = _quotes.Count;

        decimal? fractalBear = null;
        decimal? fractalBull = null;

        if (currentIndex + 1 > LeftSpan && currentIndex + 1 <= length - RightSpan)
        {
            FractalBuffer center = _quotes[currentIndex];
            bool isHigh = true;
            bool isLow = true;

            decimal evalHigh = EndType == EndType.Close ? center.Close : center.High;
            decimal evalLow = EndType == EndType.Close ? center.Close : center.Low;

            // compare with wings
            for (int p = currentIndex - LeftSpan; p <= currentIndex + RightSpan; p++)
            {
                // skip center
                if (p == currentIndex)
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

            fractalBear = isHigh ? evalHigh : null;
            fractalBull = isLow ? evalLow : null;
        }

        AddInternal(new FractalResult(quote.Timestamp, fractalBear, fractalBull));
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
    /// <param name="quotes">Historical price quotes.</param>
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
    /// <param name="quotes">Historical price quotes.</param>
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
