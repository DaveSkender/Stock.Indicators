namespace Skender.Stock.Indicators;

/// <summary>
/// Kaufman's Adaptive Moving Average (KAMA) from incremental reusable values.
/// </summary>
public class KamaList : BufferList<KamaResult>, IIncrementFromChain, IKama
{
    private readonly Queue<double> _buffer;
    private readonly int _erPeriods;
    private readonly double _scFast;
    private readonly double _scSlow;
    private double _prevKama = double.NaN;

    /// <summary>
    /// Initializes a new instance of the <see cref="KamaList"/> class.
    /// </summary>
    /// <param name="erPeriods">The number of periods for the Efficiency Ratio (ER).</param>
    /// <param name="fastPeriods">The number of periods for the fast EMA.</param>
    /// <param name="slowPeriods">The number of periods for the slow EMA.</param>
    public KamaList(
        int erPeriods = 10,
        int fastPeriods = 2,
        int slowPeriods = 30
    )
    {
        Kama.Validate(erPeriods, fastPeriods, slowPeriods);

        _erPeriods = erPeriods;
        ErPeriods = erPeriods;
        FastPeriods = fastPeriods;
        SlowPeriods = slowPeriods;

        _scFast = 2d / (fastPeriods + 1);
        _scSlow = 2d / (slowPeriods + 1);

        _buffer = new Queue<double>(erPeriods + 1);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KamaList"/> class with initial quotes.
    /// </summary>
    /// <param name="erPeriods">The number of periods for the Efficiency Ratio (ER).</param>
    /// <param name="fastPeriods">The number of periods for the fast EMA.</param>
    /// <param name="slowPeriods">The number of periods for the slow EMA.</param>
    /// <param name="quotes">Initial quotes to populate the list.</param>
    public KamaList(
        int erPeriods,
        int fastPeriods,
        int slowPeriods,
        IReadOnlyList<IQuote> quotes
    )
        : this(erPeriods, fastPeriods, slowPeriods)
        => Add(quotes);

    /// <summary>
    /// Gets the number of periods for the Efficiency Ratio (ER).
    /// </summary>
    public int ErPeriods { get; init; }

    /// <summary>
    /// Gets the number of periods for the fast EMA.
    /// </summary>
    public int FastPeriods { get; init; }

    /// <summary>
    /// Gets the number of periods for the slow EMA.
    /// </summary>
    public int SlowPeriods { get; init; }




    /// <inheritdoc />
    public void Add(DateTime timestamp, double value)
    {
        // Use universal buffer extension method for consistent buffer management
        _buffer.Update(_erPeriods + 1, value);

        // add nulls for incalculable periods
        if (_buffer.Count < _erPeriods)
        {
            AddInternal(new KamaResult(timestamp));
            return;
        }

        double er;
        double kama;

        // Calculate if we have enough data
        if (_buffer.Count == _erPeriods + 1)
        {
            double[] bufferArray = _buffer.ToArray();
            double newVal = bufferArray[^1]; // Current value

            // Check if we have a previous KAMA value
            if (!double.IsNaN(_prevKama))
            {
                // ER period change
                double change = Math.Abs(newVal - bufferArray[0]); // First value in buffer

                // volatility - sum of absolute differences
                double sumPv = 0;
                for (int i = 1; i < bufferArray.Length; i++)
                {
                    sumPv += Math.Abs(bufferArray[i] - bufferArray[i - 1]);
                }

                if (sumPv != 0)
                {
                    // efficiency ratio
                    er = change / sumPv;

                    // smoothing constant
                    double sc = (er * (_scFast - _scSlow)) + _scSlow;  // squared later

                    // kama calculation
                    kama = _prevKama + (sc * sc * (newVal - _prevKama));
                }
                else
                {
                    // handle flatline case
                    er = 0;
                    kama = newVal;
                }
            }
            else
            {
                // re/initialize
                er = double.NaN;
                kama = newVal;
            }
        }
        else
        {
            // Not enough data yet
            er = double.NaN;
            kama = value;
        }

        AddInternal(new KamaResult(
            Timestamp: timestamp,
            Er: er.NaN2Null(),
            Kama: kama.NaN2Null()));

        _prevKama = kama;
    }

    /// <inheritdoc />
    public void Add(IReusable value)
    {
        ArgumentNullException.ThrowIfNull(value);
        Add(value.Timestamp, value.Value);
    }

    /// <inheritdoc />
    public void Add(IReadOnlyList<IReusable> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        for (int i = 0; i < values.Count; i++)
        {
            Add(values[i].Timestamp, values[i].Value);
        }
    }

    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);
        Add(quote.Timestamp, quote.Value);
    }

    /// <inheritdoc />
    public void Add(IReadOnlyList<IQuote> quotes)
    {
        ArgumentNullException.ThrowIfNull(quotes);

        for (int i = 0; i < quotes.Count; i++)
        {
            Add(quotes[i].Timestamp, quotes[i].Value);
        }
    }

    /// <inheritdoc />
    public override void Clear()
    {
        base.Clear();
        _buffer.Clear();
        _prevKama = double.NaN;
    }
}

public static partial class Kama
{
    /// <summary>
    /// Creates a buffer list for Kaufman's Adaptive Moving Average (KAMA) calculations.
    /// </summary>
    public static KamaList ToKamaList<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int erPeriods = 10,
        int fastPeriods = 2,
        int slowPeriods = 30)
        where TQuote : IQuote
        => new(erPeriods, fastPeriods, slowPeriods) { (IReadOnlyList<IQuote>)quotes };
}
