namespace Skender.Stock.Indicators;

/// <summary>
/// Double Exponential Moving Average (DEMA) from incremental reusable values.
/// </summary>
public class DemaList : List<DemaResult>, IDema, IBufferList, IBufferReusable
{
    private readonly Queue<double> _buffer;
    private double _bufferSum;
    private double _lastEma1 = double.NaN;
    private double _lastEma2 = double.NaN;

    /// <summary>
    /// Initializes a new instance of the <see cref="DemaList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    public DemaList(
        int lookbackPeriods
    )
    {
        Dema.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        K = 2d / (lookbackPeriods + 1);

        _buffer = new Queue<double>(lookbackPeriods);
        _bufferSum = 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DemaList"/> class with initial quotes.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <param name="quotes">Initial quotes to populate the list.</param>
    public DemaList(
        int lookbackPeriods,
        IReadOnlyList<IQuote> quotes
    )
        : this(lookbackPeriods)
    {
        Add(quotes);
    }

    /// <summary>
    /// Gets the number of periods to look back for the calculation.
    /// </summary>
    public int LookbackPeriods { get; init; }

    /// <summary>
    /// Gets the smoothing factor for the calculation.
    /// </summary>
    public double K { get; private init; }

    /// <inheritdoc />
    public void Add(DateTime timestamp, double value)
    {
        // update buffer
        if (_buffer.Count == LookbackPeriods)
        {
            _bufferSum -= _buffer.Dequeue();
        }

        _buffer.Enqueue(value);
        _bufferSum += value;

        // add nulls for incalculable periods
        if (Count < LookbackPeriods - 1)
        {
            base.Add(new DemaResult(timestamp));
            return;
        }

        double ema1;
        double ema2;

        // when no prior EMA, reset as SMA
        if (double.IsNaN(_lastEma2))
        {
            ema1 = ema2 = _bufferSum / LookbackPeriods;
        }
        else
        {
            // normal DEMA calculation
            ema1 = Ema.Increment(K, _lastEma1, value);
            ema2 = Ema.Increment(K, _lastEma2, ema1);
        }

        // store state for next iteration
        _lastEma1 = ema1;
        _lastEma2 = ema2;

        // calculate and store DEMA result
        base.Add(new DemaResult(
            timestamp,
            Dema.Calculate(ema1, ema2)));
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
    public new void Clear()
    {
        base.Clear();
        _buffer.Clear();
        _bufferSum = 0;
        _lastEma1 = double.NaN;
        _lastEma2 = double.NaN;
    }
}
