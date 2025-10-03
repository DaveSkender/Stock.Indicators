namespace Skender.Stock.Indicators;

/// <summary>
/// Triple Exponential Moving Average Oscillator (TRIX) from incremental reusable values.
/// </summary>
public class TrixList : BufferList<TrixResult>, IBufferReusable, ITrix
{
    private readonly Queue<double> _buffer;
    private double _bufferSum;

    // State for triple EMA calculations
    private double _lastEma1 = double.NaN;
    private double _lastEma2 = double.NaN;
    private double _lastEma3 = double.NaN;

    /// <summary>
    /// Initializes a new instance of the <see cref="TrixList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    public TrixList(
        int lookbackPeriods
    )
    {
        Trix.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        K = 2d / (lookbackPeriods + 1);

        _buffer = new Queue<double>(lookbackPeriods);
        _bufferSum = 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TrixList"/> class with initial quotes.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <param name="quotes">Initial quotes to populate the list.</param>
    public TrixList(
        int lookbackPeriods,
        IReadOnlyList<IQuote> quotes
    )
        : this(lookbackPeriods)
        => Add(quotes);

    /// <inheritdoc />
    public int LookbackPeriods { get; init; }

    /// <inheritdoc />
    public double K { get; private init; }

    /// <inheritdoc />
    public void Add(DateTime timestamp, double value)
    {
        // Use BufferUtilities extension method for efficient buffer management
        double? dequeuedValue = _buffer.UpdateWithDequeue(LookbackPeriods, value);

        // Update running sum efficiently
        if (dequeuedValue.HasValue)
        {
            _bufferSum = _bufferSum - dequeuedValue.Value + value;
        }
        else
        {
            _bufferSum += value;
        }

        // add nulls for incalculable periods
        if (Count < LookbackPeriods - 1)
        {
            AddInternal(new TrixResult(timestamp));
            return;
        }

        double ema1;
        double ema2;
        double ema3;

        // when no prior EMA, reset as SMA
        if (double.IsNaN(_lastEma3))
        {
            ema1 = ema2 = ema3 = _bufferSum / LookbackPeriods;

            AddInternal(new TrixResult(timestamp));
        }
        // normal TRIX calculation
        else
        {
            ema1 = _lastEma1 + (K * (value - _lastEma1));
            ema2 = _lastEma2 + (K * (ema1 - _lastEma2));
            ema3 = _lastEma3 + (K * (ema2 - _lastEma3));

            double trix = 100 * (ema3 - _lastEma3) / _lastEma3;

            AddInternal(new TrixResult(
                Timestamp: timestamp,
                Ema3: ema3.NaN2Null(),
                Trix: trix.NaN2Null()));
        }

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
    public override void Clear()
    {
        ClearInternal();
        _buffer.Clear();
        _bufferSum = 0;
        _lastEma1 = double.NaN;
        _lastEma2 = double.NaN;
        _lastEma3 = double.NaN;
    }
}

public static partial class Trix
{
    /// <summary>
    /// Creates a buffer list for TRIX calculations.
    /// </summary>
    public static TrixList ToTrixList<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods = 14)
        where TQuote : IQuote
        => new(lookbackPeriods) { (IReadOnlyList<IQuote>)quotes };
}
