namespace Skender.Stock.Indicators;

/// <summary>
/// Simple Moving Average (SMA) from incremental reusable values.
/// </summary>
public class SmaList : List<SmaResult>, ISma, IBufferQuote, IBufferReusable
{
    private readonly CircularBufferManager<double> _buffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="SmaList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    public SmaList(int lookbackPeriods)
    {
        Sma.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        _buffer = new CircularBufferManager<double>(lookbackPeriods);
    }

    /// <summary>
    /// Gets the number of periods to look back for the calculation.
    /// </summary>
    public int LookbackPeriods { get; init; }

    /// <summary>
    /// Adds a new value to the SMA list.
    /// </summary>
    /// <param name="timestamp">The timestamp of the value.</param>
    /// <param name="value">The value to add.</param>
    public void Add(DateTime timestamp, double value)
    {
        // Add value to buffer
        _buffer.Add(value);

        // Calculate SMA if we have enough data
        SmaResult? result = null;
        
        if (_buffer.Count >= LookbackPeriods)
        {
            // Calculate average from buffer
            var sum = 0.0;
            for (int i = 0; i < _buffer.Count; i++)
            {
                sum += _buffer[i];
            }
            
            var sma = sum / _buffer.Count;
            
            result = new SmaResult(timestamp, sma.NaN2Null());
        }
        else
        {
            // Not enough data yet
            result = new SmaResult(timestamp, null);
        }

        Add(result);
    }

    /// <summary>
    /// Adds a reusable value to the SMA list.
    /// </summary>
    /// <param name="value">The reusable value to add.</param>
    public void Add(IReusable value)
    {
        ArgumentNullException.ThrowIfNull(value);
        Add(value.Timestamp, value.Value);
    }

    /// <summary>
    /// Adds a quote to the SMA list using the close price.
    /// </summary>
    /// <param name="quote">The quote to add.</param>
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);
        Add(quote.Timestamp, (double)quote.Close);
    }

    /// <summary>
    /// Adds a batch of reusable values to the SMA list.
    /// </summary>
    /// <param name="values">The batch of reusable values to add.</param>
    public void Add(IReadOnlyList<IReusable> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        
        foreach (var value in values)
        {
            Add(value);
        }
    }

    /// <summary>
    /// Adds a batch of quotes to the SMA list.
    /// </summary>
    /// <param name="quotes">The batch of quotes to add.</param>
    public void Add(IReadOnlyList<IQuote> quotes)
    {
        ArgumentNullException.ThrowIfNull(quotes);
        
        foreach (var quote in quotes)
        {
            Add(quote);
        }
    }
}