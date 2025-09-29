namespace Skender.Stock.Indicators;

/// <summary>
/// Simple Moving Average (SMA) from incremental reusable values.
/// </summary>
public class SmaList : List<SmaResult>, ISma, IBufferList, IBufferReusable
{
    private readonly Queue<double> buffer;
    private double sum;

    /// <summary>
    /// Initializes a new instance of the <see cref="SmaList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    public SmaList(int lookbackPeriods)
    {
        Sma.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        buffer = new Queue<double>(lookbackPeriods);
        sum = 0d;
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
        // Update the rolling buffer and sum
        if (buffer.Count == LookbackPeriods)
        {
            // Remove the oldest value from the sum
            double oldValue = buffer.Dequeue();
            sum -= oldValue;
        }

        // Add the new value to the buffer and sum
        buffer.Enqueue(value);
        sum += value;

        // Calculate SMA when we have enough values
        double? sma = buffer.Count == LookbackPeriods ? sum / LookbackPeriods : null;

        base.Add(new SmaResult(timestamp, sma));
    }

    /// <summary>
    /// Adds a new reusable value to the SMA list.
    /// </summary>
    /// <param name="value">The reusable value to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when the value is null.</exception>
    public void Add(IReusable value)
    {
        ArgumentNullException.ThrowIfNull(value);
        Add(value.Timestamp, value.Value);
    }

    /// <summary>
    /// Adds a list of reusable values to the SMA list.
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
    /// Adds a new quote to the SMA list.
    /// </summary>
    /// <param name="quote">The quote to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when the quote is null.</exception>
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);
        Add(quote.Timestamp, quote.Value);
    }

    /// <summary>
    /// Adds a list of quotes to the SMA list.
    /// </summary>
    /// <param name="quotes">The list of quotes to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when the quotes list is null.</exception>
    public void Add(IReadOnlyList<IQuote> quotes)
    {
        ArgumentNullException.ThrowIfNull(quotes);

        for (int i = 0; i < quotes.Count; i++)
        {
            Add(quotes[i].Timestamp, quotes[i].Value);
        }
    }

    /// <summary>
    /// Clears the list and resets internal buffers so the instance can be reused.
    /// </summary>
    public new void Clear()
    {
        base.Clear();
        buffer.Clear();
        sum = 0d;
    }
}
