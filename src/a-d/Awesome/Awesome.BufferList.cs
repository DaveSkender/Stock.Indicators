namespace Skender.Stock.Indicators;

/// <summary>
/// Awesome Oscillator from incremental reusable values.
/// </summary>
public class AwesomeList : BufferList<AwesomeResult>, IIncrementFromChain, IAwesome
{
    private readonly Queue<double> _buffer;
    private double _slowSum;
    private double _fastSum;

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
        _slowSum = 0;
        _fastSum = 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AwesomeList"/> class with initial reusable values.
    /// </summary>
    /// <param name="fastPeriods">The number of periods for the fast moving average.</param>
    /// <param name="slowPeriods">The number of periods for the slow moving average.</param>
    /// <param name="values">Initial reusable values to populate the list.</param>
    public AwesomeList(int fastPeriods, int slowPeriods, IReadOnlyList<IReusable> values)
        : this(fastPeriods, slowPeriods) => Add(values);

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
        // Track dequeued value for sum maintenance
        double? dequeuedValue = _buffer.UpdateWithDequeue(SlowPeriods, value);

        double? oscillator = null;
        double? normalized = null;

        // Calculate when we have enough data
        if (_buffer.Count == SlowPeriods)
        {
            // Update running sums efficiently - O(1) operation
            if (dequeuedValue.HasValue)
            {
                _slowSum = _slowSum - dequeuedValue.Value + value;

                // For fast sum, we need to know what's dropping out of the fast window
                // Get the value that's FastPeriods back from the end
                double[] bufferArray = _buffer.ToArray();
                double fastOutValue = bufferArray[SlowPeriods - FastPeriods - 1];
                _fastSum = _fastSum - fastOutValue + value;
            }
            else
            {
                // First time we reach SlowPeriods, calculate sums from scratch
                _slowSum = 0;
                _fastSum = 0;
                int index = 0;

                foreach (double val in _buffer)
                {
                    _slowSum += val;

                    // Fast SMA uses only the last FastPeriods values
                    if (index >= SlowPeriods - FastPeriods)
                    {
                        _fastSum += val;
                    }

                    index++;
                }
            }

            double fastSma = _fastSum / FastPeriods;
            double slowSma = _slowSum / SlowPeriods;
            oscillator = fastSma - slowSma;
            normalized = value != 0 ? 100 * oscillator / value : null;
        }

        AddInternal(new AwesomeResult(
            Timestamp: timestamp,
            Oscillator: oscillator,
            Normalized: normalized
        ));
    }

    /// <inheritdoc />
    public void Add(IReusable value)
    {
        ArgumentNullException.ThrowIfNull(value);
        // Prefer HL2 when source is IQuote (Awesome Oscillator specification)
        Add(value.Timestamp, value.Hl2OrValue());
    }

    /// <inheritdoc />
    public void Add(IReadOnlyList<IReusable> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        for (int i = 0; i < values.Count; i++)
        {
            IReusable v = values[i];
            // Prefer HL2 when source is IQuote (Awesome Oscillator specification)
            Add(v.Timestamp, v.Hl2OrValue());
        }
    }

    /// <inheritdoc />
    public override void Clear()
    {
        base.Clear();
        _buffer.Clear();
        _slowSum = 0;
        _fastSum = 0;
    }
}

/// <summary>
/// EXTENSION METHODS
/// </summary>
public static partial class Awesome
{
    /// <summary>
    /// Creates a buffer list for Awesome Oscillator calculations.
    /// </summary>
    /// <param name="source">Time-series values to transform.</param>
    /// <param name="fastPeriods">The number of periods for the fast moving average. Default is 5.</param>
    /// <param name="slowPeriods">The number of periods for the slow moving average. Default is 34.</param>
    /// <returns>An AwesomeList instance pre-populated with historical data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when source is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when parameters are invalid.</exception>
    public static AwesomeList ToAwesomeList(
        this IReadOnlyList<IReusable> source,
        int fastPeriods = 5,
        int slowPeriods = 34)
        => new(fastPeriods, slowPeriods) { source };
}
