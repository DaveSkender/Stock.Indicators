namespace Skender.Stock.Indicators;

/// <summary>
/// Weighted Moving Average (WMA) from incremental reusable values.
/// </summary>
public class WmaList : List<WmaResult>, IWma, IBufferList, IBufferReusable
{
    private readonly Queue<double> _buffer;
    private readonly double _divisor;

    /// <summary>
    /// Initializes a new instance of the <see cref="WmaList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    public WmaList(int lookbackPeriods)
    {
        Wma.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;

        // Pre-calculate divisor for WMA: n * (n + 1) / 2
        _divisor = (double)lookbackPeriods * (lookbackPeriods + 1) / 2d;

        _buffer = new Queue<double>(lookbackPeriods);
    }

    /// <summary>
    /// Gets the number of periods to look back for the calculation.
    /// </summary>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc />
    public void Add(DateTime timestamp, double value)
    {
        // Update the rolling buffer
        if (_buffer.Count == LookbackPeriods)
        {
            _buffer.Dequeue();
        }

        _buffer.Enqueue(value);

        // Calculate WMA when we have enough values exactly like static series
        // This matches the precision of the static series implementation exactly
        double? wma = null;
        if (_buffer.Count == LookbackPeriods)
        {
            wma = 0;
            int weight = 1;

            // Calculate exactly like static series: divide inside the loop like the original
            foreach (double bufferValue in _buffer)
            {
                wma += bufferValue * weight / _divisor;
                weight++;
            }
        }

        base.Add(new WmaResult(timestamp, wma));
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
    }
}
