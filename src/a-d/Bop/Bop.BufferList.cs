namespace Skender.Stock.Indicators;

/// <summary>
/// Balance of Power (BOP) from incremental quotes.
/// </summary>
public class BopList : BufferList<BopResult>, IIncrementFromQuote, IBop
{
    private readonly Queue<double> _buffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="BopList"/> class.
    /// </summary>
    /// <param name="smoothPeriods">The number of periods to use for smoothing.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="smoothPeriods"/> is invalid.</exception>
    public BopList(int smoothPeriods)
    {
        Bop.Validate(smoothPeriods);
        SmoothPeriods = smoothPeriods;
        _buffer = new Queue<double>(smoothPeriods);

        Name = $"BOP({smoothPeriods})";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BopList"/> class with initial quotes.
    /// </summary>
    /// <param name="smoothPeriods">The number of periods to use for smoothing.</param>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    public BopList(int smoothPeriods, IReadOnlyList<IQuote> quotes)
        : this(smoothPeriods) => Add(quotes);

    /// <inheritdoc />
    public int SmoothPeriods { get; init; }

    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);

        DateTime timestamp = quote.Timestamp;

        // Calculate raw BOP for this quote
        double range = (double)quote.High - (double)quote.Low;
        double rawBop = range != 0
            ? ((double)quote.Close - (double)quote.Open) / range
            : double.NaN;

        // Update buffer with raw BOP value
        _buffer.Update(SmoothPeriods, rawBop);

        // Calculate smoothed BOP
        double? bop = null;
        if (_buffer.Count == SmoothPeriods)
        {
            bop = _buffer.Average().NaN2Null();
        }

        AddInternal(new BopResult(timestamp, bop));
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

public static partial class Bop
{
    /// <summary>
    /// Creates a buffer list for Balance of Power (BOP) calculations.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="smoothPeriods">Number of periods for smoothing</param>
    public static BopList ToBopList(
        this IReadOnlyList<IQuote> quotes,
        int smoothPeriods = 14)
        => new(smoothPeriods) { quotes };
}
