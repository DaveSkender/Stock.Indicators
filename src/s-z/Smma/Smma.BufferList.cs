namespace Skender.Stock.Indicators;

/// <summary>
/// Smoothed Moving Average (SMMA) from incremental reusable values.
/// </summary>
public class SmmaList : List<SmmaResult>, ISmma, IBufferList, IBufferReusable
{
    private double? _previousSmma;
    private readonly Queue<double> _buffer;
    private double _bufferSum;

    /// <summary>
    /// Initializes a new instance of the <see cref="SmmaList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    public SmmaList(
        int lookbackPeriods
    )
    {
        Smma.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;

        _buffer = new Queue<double>(lookbackPeriods);
        _bufferSum = 0;
        _previousSmma = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SmmaList"/> class with initial quotes.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <param name="quotes">Initial quotes to populate the list.</param>
    public SmmaList(
        int lookbackPeriods,
        IReadOnlyList<IQuote> quotes
    )
    {
        Smma.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;

        _buffer = new Queue<double>(lookbackPeriods);
        _bufferSum = 0;
        _previousSmma = null;
        Add(quotes);
    }

    /// <summary>
    /// Gets the number of periods to look back for the calculation.
    /// </summary>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc />
    public void Add(DateTime timestamp, double value)
    {
        // update buffer for SMA initialization using buffer utilities
        double? dequeuedValue = _buffer.UpdateWithDequeue(LookbackPeriods, value);

        // update running sum efficiently
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
            base.Add(new SmmaResult(timestamp));
            return;
        }

        double smma = _previousSmma is null
            ? _bufferSum / LookbackPeriods
            // normal SMMA calculation
            : ((_previousSmma.Value * (LookbackPeriods - 1)) + value) / LookbackPeriods;

        // when no prior SMMA, reset as SMA

        SmmaResult result = new(timestamp, smma.NaN2Null());
        base.Add(result);
        _previousSmma = smma;
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
        _previousSmma = null;
    }
}
