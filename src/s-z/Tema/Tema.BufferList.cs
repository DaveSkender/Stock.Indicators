namespace Skender.Stock.Indicators;

/// <summary>
/// Triple Exponential Moving Average (TEMA) from incremental reusable values.
/// </summary>
public class TemaList : List<TemaResult>, ITema, IBufferList, IBufferReusable
{
    private readonly Queue<double> _buffer;
    private double _bufferSum;

    // State for triple EMA calculations
    private double _lastEma1 = double.NaN;
    private double _lastEma2 = double.NaN;
    private double _lastEma3 = double.NaN;

    /// <summary>
    /// Initializes a new instance of the <see cref="TemaList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    public TemaList(
        int lookbackPeriods
    )
    {
        Tema.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        K = 2d / (lookbackPeriods + 1);

        _buffer = new Queue<double>(lookbackPeriods);
        _bufferSum = 0;
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
            base.Add(new TemaResult(timestamp));
            return;
        }

        double ema1;
        double ema2;
        double ema3;

        // when no prior EMA, reset as SMA
        if (double.IsNaN(_lastEma3))
        {
            ema1 = ema2 = ema3 = _bufferSum / LookbackPeriods;
        }
        // normal TEMA calculation
        else
        {
            ema1 = _lastEma1 + (K * (value - _lastEma1));
            ema2 = _lastEma2 + (K * (ema1 - _lastEma2));
            ema3 = _lastEma3 + (K * (ema2 - _lastEma3));
        }

        // calculate TEMA
        double tema = (3 * ema1) - (3 * ema2) + ema3;

        base.Add(new TemaResult(
            timestamp,
            tema.NaN2Null()));

        // store state for next calculation
        _lastEma1 = ema1;
        _lastEma2 = ema2;
        _lastEma3 = ema3;
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
        _lastEma3 = double.NaN;
    }
}