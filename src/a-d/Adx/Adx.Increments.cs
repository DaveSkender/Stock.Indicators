namespace Skender.Stock.Indicators;

/// <summary>
/// Interface for Average Directional Index (ADX) calculations.
/// </summary>
public interface IAdx
{
    /// <summary>
    /// Gets the number of periods to look back for the calculation.
    /// </summary>
    int LookbackPeriods { get; }
}

/// <summary>
/// Average Directional Index (ADX) from incremental reusable values.
/// </summary>
public class AdxList : List<AdxResult>, IAdx, IAddQuote, IAddReusable
{
    private readonly Queue<double> _buffer;
    private double _bufferSum;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdxList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    public AdxList(int lookbackPeriods)
    {
        Adx.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;

        _buffer = new(lookbackPeriods);
        _bufferSum = 0;
    }

    /// <summary>
    /// Gets the number of periods to look back for the calculation.
    /// </summary>
    public int LookbackPeriods { get; init; }

    /// <summary>
    /// Adds a new value to the ADX list.
    /// </summary>
    /// <param name="timestamp">The timestamp of the value.</param>
    /// <param name="value">The value to add.</param>
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
            base.Add(new AdxResult(timestamp));
            return;
        }

        // re/initialize as SMA
        if (this[^1].Adx is null)
        {
            base.Add(new AdxResult(
                timestamp,
                _bufferSum / LookbackPeriods));
            return;
        }

        // calculate ADX normally
        base.Add(new AdxResult(
            timestamp,
            Adx.Increment(this[^1].Adx, value)));
    }

    /// <summary>
    /// Adds a new reusable value to the ADX list.
    /// </summary>
    /// <param name="value">The reusable value to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when the value is null.</exception>
    public void Add(IReusable value)
    {
        ArgumentNullException.ThrowIfNull(value);
        Add(value.Timestamp, value.Value);
    }

    /// <summary>
    /// Adds a list of reusable values to the ADX list.
    /// </summary>
    /// <param name="values">The list of reusable values to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when the values list is null.</exception>
    public void Add(IReadOnlyList<IReusable> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        for (int i = 0; i < values.Count; i++)
        {
            Add(values[i].Timestamp, values[i].Value);
        }
    }

    /// <summary>
    /// Adds a new quote to the ADX list.
    /// </summary>
    /// <param name="quote">The quote to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when the quote is null.</exception>
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);
        Add(quote.Timestamp, quote.Value);
    }

    /// <summary>
    /// Adds a list of quotes to the ADX list.
    /// </summary>
    /// <param name="quotes">The list of quotes to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when the quotes list is null.</exception>
    public void Add(IReadOnlyList<IQuote> quotes)
    {
        ArgumentNullException.ThrowIfNull(quotes);

        for (int i = 0; i < quotes.Count; i++)
        {
            Add(quotes[i]);
        }
    }
}
