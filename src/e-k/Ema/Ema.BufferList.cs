namespace Skender.Stock.Indicators;

/// <summary>
/// Exponential Moving Average (EMA) from incremental reusable values.
/// </summary>
public class EmaList : List<EmaResult>, IEma, IBufferList, IBufferReusable
{
    private readonly Queue<double> _buffer;
    private double _bufferSum;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmaList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    public EmaList(
        int lookbackPeriods
    )
    {
        Ema.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        K = 2d / (lookbackPeriods + 1);

        _buffer = new Queue<double>(lookbackPeriods);
        _bufferSum = 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmaList"/> class with initial quotes.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <param name="quotes">Initial quotes to populate the list.</param>
    public EmaList(
        int lookbackPeriods,
        IReadOnlyList<IQuote> quotes
    )
    {
        Ema.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        K = 2d / (lookbackPeriods + 1);

        _buffer = new Queue<double>(lookbackPeriods);
        _bufferSum = 0;
        Add(quotes);
    }

    /// <inheritdoc />
    public int LookbackPeriods { get; init; }

    /// <inheritdoc />
    public double K { get; private init; }

    /// <inheritdoc />
    public void Add(DateTime timestamp, double value)
    {
        // update buffer and track dequeued value for sum maintenance using extension method
        double? dequeuedValue = _buffer.UpdateWithDequeue(LookbackPeriods, value);

        // update running sum
        if (_buffer.Count == LookbackPeriods && dequeuedValue.HasValue)
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
            base.Add(new EmaResult(timestamp));
            return;
        }

        double? lastEma = this[^1].Ema;

        // re/initialize as SMA
        if (lastEma is null)
        {
            base.Add(new EmaResult(
                timestamp,
                _bufferSum / LookbackPeriods));
            return;
        }

        // calculate EMA normally
        base.Add(new EmaResult(
            timestamp,
            Ema.Increment(K, lastEma.Value, value)));
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
    }
}

public static partial class Ema
{
    /// <summary>
    /// Creates a buffer list for Exponential Moving Average (EMA) calculations.
    /// </summary>
    public static EmaList ToEmaList<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote
        => new(lookbackPeriods) { (IReadOnlyList<IQuote>)quotes };
}
