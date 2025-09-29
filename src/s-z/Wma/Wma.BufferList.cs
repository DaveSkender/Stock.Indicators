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
        // Update the rolling buffer using the utility method
        Wma.UpdateBuffer(_buffer, LookbackPeriods, value);

        // Calculate WMA using the utility method
        double? wma = Wma.ComputeWeightedMovingAverage(_buffer, LookbackPeriods, _divisor);

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
