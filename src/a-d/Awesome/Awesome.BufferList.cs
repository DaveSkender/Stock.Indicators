namespace Skender.Stock.Indicators;

/// <summary>
/// Awesome Oscillator from incremental reusable values.
/// </summary>
public class AwesomeList : BufferList<AwesomeResult>, IBufferReusable, IAwesome
{
    private readonly Queue<double> _buffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="AwesomeList"/> class.
    /// </summary>
    /// <param name="fastPeriods">The number of periods for the fast moving average.</param>
    /// <param name="slowPeriods">The number of periods for the slow moving average.</param>
    public AwesomeList(int fastPeriods = 5, int slowPeriods = 34)
    {
        Awesome.Validate(fastPeriods, slowPeriods);
        FastPeriods = fastPeriods;
        SlowPeriods = slowPeriods;

        _buffer = new Queue<double>(slowPeriods);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AwesomeList"/> class with initial quotes.
    /// </summary>
    /// <param name="fastPeriods">The number of periods for the fast moving average.</param>
    /// <param name="slowPeriods">The number of periods for the slow moving average.</param>
    /// <param name="quotes">Initial quotes to populate the list.</param>
    public AwesomeList(int fastPeriods, int slowPeriods, IReadOnlyList<IQuote> quotes)
        : this(fastPeriods, slowPeriods)
        => Add(quotes);

    /// <summary>
    /// Gets the number of periods for the fast moving average.
    /// </summary>
    public int FastPeriods { get; init; }

    /// <summary>
    /// Gets the number of periods for the slow moving average.
    /// </summary>
    public int SlowPeriods { get; init; }

    /// <summary>
    /// Adds a new value to the Awesome list.
    /// </summary>
    /// <param name="timestamp">The timestamp of the value.</param>
    /// <param name="value">The value to add.</param>
    public void Add(DateTime timestamp, double value)
    {
        // Add to buffer
        _buffer.Update(SlowPeriods, value);

        double? oscillator = null;
        double? normalized = null;

        // Calculate when we have enough data
        if (_buffer.Count == SlowPeriods)
        {
            // Calculate both SMAs from the same buffer
            double sumSlow = 0;
            double sumFast = 0;
            int index = 0;

            foreach (double val in _buffer)
            {
                sumSlow += val;

                // Fast SMA uses only the last FastPeriods values
                if (index >= SlowPeriods - FastPeriods)
                {
                    sumFast += val;
                }

                index++;
            }

            double fastSma = sumFast / FastPeriods;
            double slowSma = sumSlow / SlowPeriods;
            oscillator = fastSma - slowSma;
            normalized = value != 0 ? 100 * oscillator / value : null;
        }

        AddInternal(new AwesomeResult(
            Timestamp: timestamp,
            Oscillator: oscillator,
            Normalized: normalized
        ));
    }

    /// <summary>
    /// Adds a new reusable value to the Awesome list.
    /// </summary>
    /// <param name="value">The reusable value to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when the value is null.</exception>
    public void Add(IReusable value)
    {
        ArgumentNullException.ThrowIfNull(value);
        Add(value.Timestamp, value.Value);
    }

    /// <summary>
    /// Adds a list of reusable values to the Awesome list.
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
    /// Adds a new quote to the Awesome list.
    /// </summary>
    /// <param name="quote">The quote to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when the quote is null.</exception>
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);
        Add(quote.Timestamp, quote.Hl2OrValue());
    }

    /// <summary>
    /// Adds a list of quotes to the Awesome list.
    /// </summary>
    /// <param name="quotes">The list of quotes to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when the quotes list is null.</exception>
    public void Add(IReadOnlyList<IQuote> quotes)
    {
        ArgumentNullException.ThrowIfNull(quotes);

        for (int i = 0; i < quotes.Count; i++)
        {
            Add(quotes[i].Timestamp, quotes[i].Hl2OrValue());
        }
    }

    /// <summary>
    /// Clears the list and resets internal buffers so the instance can be reused.
    /// </summary>
    public override void Clear()
    {
        ClearInternal();
        _buffer.Clear();
    }
}

// EXTENSION METHODS
public static partial class Awesome
{
    /// <summary>
    /// Creates a buffer list for Awesome Oscillator calculations.
    /// </summary>
    /// <typeparam name="T">The type that implements IReusable.</typeparam>
    /// <param name="source">Time-series values to transform.</param>
    /// <param name="fastPeriods">The number of periods for the fast moving average. Default is 5.</param>
    /// <param name="slowPeriods">The number of periods for the slow moving average. Default is 34.</param>
    /// <returns>An AwesomeList instance pre-populated with historical data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when source is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when parameters are invalid.</exception>
    public static AwesomeList ToAwesomeList<T>(
        this IReadOnlyList<T> source,
        int fastPeriods = 5,
        int slowPeriods = 34)
        where T : IReusable
    {
        ArgumentNullException.ThrowIfNull(source);
        Validate(fastPeriods, slowPeriods);

        return source is IReadOnlyList<IQuote> quotes
            ? new(fastPeriods, slowPeriods) { quotes }
            : new(fastPeriods, slowPeriods) { (IReadOnlyList<IReusable>)source };
    }
}
