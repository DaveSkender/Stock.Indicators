namespace Skender.Stock.Indicators;

// EXPONENTIAL MOVING AVERAGE (INCREMENTING LIST)

/// <summary>
/// Exponential Moving Average (EMA)
/// from incremental reusable values.
/// </summary>
public class EmaInc : List<EmaResult>, IEma, IIncrementQuote, IIncrementReusable
{
    private readonly Queue<double> _buffer;
    private double _bufferSum;

    public EmaInc(int lookbackPeriods)
    {
        Ema.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        K = 2d / (lookbackPeriods + 1);

        _buffer = new(lookbackPeriods);
        _bufferSum = 0;
    }

    public int LookbackPeriods { get; init; }
    public double K { get; init; }

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
            base.Add(new EmaResult(timestamp));
            return;
        }

        // re/initialize as SMA
        if (this[^1].Ema is null)
        {
            base.Add(new EmaResult(
                timestamp,
                _bufferSum / LookbackPeriods));
            return;
        }

        // calculate EMA normally
        base.Add(new EmaResult(
            timestamp,
            Ema.Increment(K, this[^1].Ema, value)));
    }

    public void Add(IReusable value)
    {
        ArgumentNullException.ThrowIfNull(value);
        Add(value.Timestamp, value.Value);
    }

    public void Add(IReadOnlyList<IReusable> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        for (int i = 0; i < values.Count; i++)
        {
            Add(values[i].Timestamp, values[i].Value);
        }
    }

    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);
        Add(quote.Timestamp, quote.Value);
    }

    public void Add(IReadOnlyList<IQuote> quotes)
    {
        ArgumentNullException.ThrowIfNull(quotes);

        for (int i = 0; i < quotes.Count; i++)
        {
            Add(quotes[i]);
        }
    }
}

/// <summary>
/// Exponential Moving Average (EMA)
/// from incremental primitives, without date context.
/// </summary>
/// <inheritdoc cref="IIncrementPrimitive"/>
public class EmaIncPrimitive : List<double?>, IEma, IIncrementPrimitive
{
    private readonly Queue<double> _buffer;
    private double _bufferSum;

    public EmaIncPrimitive(int lookbackPeriods)
    {
        Ema.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        K = 2d / (lookbackPeriods + 1);

        _buffer = new(lookbackPeriods);
        _bufferSum = 0;
    }

    public int LookbackPeriods { get; init; }
    public double K { get; init; }

    public void Add(double value)
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
            base.Add(null);
            return;
        }

        // re/initialize as SMA
        if (this[^1] is null)
        {
            base.Add(_bufferSum / LookbackPeriods);
            return;
        }

        // calculate EMA normally
        base.Add(Ema.Increment(K, this[^1], value));
    }

    public void Add(double[] values)
    {
        ArgumentNullException.ThrowIfNull(values);

        for (int i = 0; i < values.Length; i++)
        {
            Add(values[i]);
        }
    }
}
