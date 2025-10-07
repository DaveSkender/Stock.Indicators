namespace Skender.Stock.Indicators;

/// <summary>
/// Balance of Power (BOP) from incremental quotes.
/// </summary>
public class BopList : BufferList<BopResult>, IBufferList
{
    private readonly Queue<double> _buffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="BopList"/> class.
    /// </summary>
    /// <param name="smoothPeriods">The number of periods to use for smoothing.</param>
    public BopList(int smoothPeriods)
    {
        Bop.Validate(smoothPeriods);
        SmoothPeriods = smoothPeriods;
        _buffer = new Queue<double>(smoothPeriods);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BopList"/> class with initial quotes.
    /// </summary>
    /// <param name="smoothPeriods">The number of periods to use for smoothing.</param>
    /// <param name="quotes">Initial quotes to populate the list.</param>
    public BopList(int smoothPeriods, IReadOnlyList<IQuote> quotes)
        : this(smoothPeriods)
        => Add(quotes);

    /// <summary>
    /// Gets the number of periods to use for smoothing.
    /// </summary>
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
            double sum = 0;
            foreach (double val in _buffer)
            {
                sum += val;
            }

            bop = (sum / SmoothPeriods).NaN2Null();
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
        ClearInternal();
        _buffer.Clear();
    }
}

public static partial class Bop
{
    /// <summary>
    /// Creates a buffer list for Balance of Power (BOP) calculations.
    /// </summary>
    public static BopList ToBopList<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int smoothPeriods = 14)
        where TQuote : IQuote
        => new(smoothPeriods) { (IReadOnlyList<IQuote>)quotes };
}
