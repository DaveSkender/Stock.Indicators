namespace Skender.Stock.Indicators;

/// <summary>
/// Williams Alligator from incremental reusable values.
/// </summary>
public class AlligatorList : BufferList<AlligatorResult>, IBufferReusable, IAlligator
{
    private readonly Queue<double> _inputBuffer;
    private readonly Queue<double> _jawBuffer;
    private readonly Queue<double> _teethBuffer;
    private readonly Queue<double> _lipsBuffer;

    private double _jawBufferSum;
    private double _teethBufferSum;
    private double _lipsBufferSum;

    private double? _lastJaw;
    private double? _lastTeeth;
    private double? _lastLips;

    // Track how many total values we've seen
    private int _count;

    /// <summary>
    /// Initializes a new instance of the <see cref="AlligatorList"/> class.
    /// </summary>
    /// <param name="jawPeriods">Lookback periods for the Jaw line.</param>
    /// <param name="jawOffset">Offset periods for the Jaw line.</param>
    /// <param name="teethPeriods">Lookback periods for the Teeth line.</param>
    /// <param name="teethOffset">Offset periods for the Teeth line.</param>
    /// <param name="lipsPeriods">Lookback periods for the Lips line.</param>
    /// <param name="lipsOffset">Offset periods for the Lips line.</param>
    public AlligatorList(
        int jawPeriods = 13,
        int jawOffset = 8,
        int teethPeriods = 8,
        int teethOffset = 5,
        int lipsPeriods = 5,
        int lipsOffset = 3)
    {
        Alligator.Validate(jawPeriods, jawOffset, teethPeriods, teethOffset, lipsPeriods, lipsOffset);

        JawPeriods = jawPeriods;
        JawOffset = jawOffset;
        TeethPeriods = teethPeriods;
        TeethOffset = teethOffset;
        LipsPeriods = lipsPeriods;
        LipsOffset = lipsOffset;

        // Input buffer needs to store enough history for the largest (periods + offset)
        int maxBuffer = Math.Max(
            jawPeriods + jawOffset,
            Math.Max(teethPeriods + teethOffset, lipsPeriods + lipsOffset));

        _inputBuffer = new Queue<double>(maxBuffer);
        _jawBuffer = new Queue<double>(jawPeriods);
        _teethBuffer = new Queue<double>(teethPeriods);
        _lipsBuffer = new Queue<double>(lipsPeriods);

        _jawBufferSum = 0;
        _teethBufferSum = 0;
        _lipsBufferSum = 0;
        _count = 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AlligatorList"/> class with initial quotes.
    /// </summary>
    /// <param name="jawPeriods">Lookback periods for the Jaw line.</param>
    /// <param name="jawOffset">Offset periods for the Jaw line.</param>
    /// <param name="teethPeriods">Lookback periods for the Teeth line.</param>
    /// <param name="teethOffset">Offset periods for the Teeth line.</param>
    /// <param name="lipsPeriods">Lookback periods for the Lips line.</param>
    /// <param name="lipsOffset">Offset periods for the Lips line.</param>
    /// <param name="quotes">Initial quotes to populate the list.</param>
    public AlligatorList(
        int jawPeriods,
        int jawOffset,
        int teethPeriods,
        int teethOffset,
        int lipsPeriods,
        int lipsOffset,
        IReadOnlyList<IQuote> quotes)
        : this(jawPeriods, jawOffset, teethPeriods, teethOffset, lipsPeriods, lipsOffset)
        => Add(quotes);

    /// <inheritdoc/>
    public int JawPeriods { get; init; }

    /// <inheritdoc/>
    public int JawOffset { get; init; }

    /// <inheritdoc/>
    public int TeethPeriods { get; init; }

    /// <inheritdoc/>
    public int TeethOffset { get; init; }

    /// <inheritdoc/>
    public int LipsPeriods { get; init; }

    /// <inheritdoc/>
    public int LipsOffset { get; init; }

    /// <summary>
    /// Adds a new value to the Alligator list.
    /// </summary>
    /// <param name="timestamp">The timestamp of the value.</param>
    /// <param name="value">The value to add.</param>
    public void Add(DateTime timestamp, double value)
    {
        // Add to input buffer and maintain maximum size
        _inputBuffer.Enqueue(value);
        int maxNeeded = Math.Max(
            JawPeriods + JawOffset,
            Math.Max(TeethPeriods + TeethOffset, LipsPeriods + LipsOffset));

        if (_inputBuffer.Count > maxNeeded)
        {
            _inputBuffer.Dequeue();
        }

        _count++;

        double? jaw = null;
        double? teeth = null;
        double? lips = null;

        // Calculate Jaw (SMMA with offset)
        // Start adding offset values to buffer when we have enough history
        if (_count > JawOffset)
        {
            // Get the value from JawOffset positions ago
            double offsetValue = _inputBuffer.ElementAt(_inputBuffer.Count - 1 - JawOffset);

            // Add to jaw buffer
            _jawBuffer.Enqueue(offsetValue);

            // Maintain buffer size
            if (_jawBuffer.Count > JawPeriods)
            {
                double removed = _jawBuffer.Dequeue();
                _jawBufferSum -= removed;
            }

            // Calculate when we have enough values in the jaw buffer
            if (_jawBuffer.Count == JawPeriods)
            {
                _jawBufferSum += offsetValue;

                jaw = !_lastJaw.HasValue
                    // First value: SMA
                    ? _jawBufferSum / JawPeriods
                    // Subsequent values: SMMA
                    : ((_lastJaw.Value * (JawPeriods - 1)) + offsetValue) / JawPeriods;

                _lastJaw = jaw;
            }
            else
            {
                // Still accumulating for initial SMA
                _jawBufferSum += offsetValue;
            }
        }

        // Calculate Teeth (SMMA with offset)
        if (_count > TeethOffset)
        {
            double offsetValue = _inputBuffer.ElementAt(_inputBuffer.Count - 1 - TeethOffset);

            _teethBuffer.Enqueue(offsetValue);

            if (_teethBuffer.Count > TeethPeriods)
            {
                double removed = _teethBuffer.Dequeue();
                _teethBufferSum -= removed;
            }

            if (_teethBuffer.Count == TeethPeriods)
            {
                _teethBufferSum += offsetValue;

                teeth = !_lastTeeth.HasValue ? _teethBufferSum / TeethPeriods : ((_lastTeeth.Value * (TeethPeriods - 1)) + offsetValue) / TeethPeriods;

                _lastTeeth = teeth;
            }
            else
            {
                _teethBufferSum += offsetValue;
            }
        }

        // Calculate Lips (SMMA with offset)
        if (_count > LipsOffset)
        {
            double offsetValue = _inputBuffer.ElementAt(_inputBuffer.Count - 1 - LipsOffset);

            _lipsBuffer.Enqueue(offsetValue);

            if (_lipsBuffer.Count > LipsPeriods)
            {
                double removed = _lipsBuffer.Dequeue();
                _lipsBufferSum -= removed;
            }

            if (_lipsBuffer.Count == LipsPeriods)
            {
                _lipsBufferSum += offsetValue;

                lips = !_lastLips.HasValue ? _lipsBufferSum / LipsPeriods : ((_lastLips.Value * (LipsPeriods - 1)) + offsetValue) / LipsPeriods;

                _lastLips = lips;
            }
            else
            {
                _lipsBufferSum += offsetValue;
            }
        }

        AddInternal(new AlligatorResult(timestamp, jaw, teeth, lips));
    }

    /// <summary>
    /// Adds a new reusable value to the Alligator list.
    /// </summary>
    /// <param name="value">The reusable value to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when the value is null.</exception>
    public void Add(IReusable value)
    {
        ArgumentNullException.ThrowIfNull(value);
        Add(value.Timestamp, value.Value);
    }

    /// <summary>
    /// Adds a list of reusable values to the Alligator list.
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
    /// Adds a new quote to the Alligator list.
    /// </summary>
    /// <param name="quote">The quote to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when the quote is null.</exception>
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);
        Add(quote.Timestamp, quote.Hl2OrValue());
    }

    /// <summary>
    /// Adds a list of quotes to the Alligator list.
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

        _inputBuffer.Clear();
        _jawBuffer.Clear();
        _teethBuffer.Clear();
        _lipsBuffer.Clear();

        _jawBufferSum = 0;
        _teethBufferSum = 0;
        _lipsBufferSum = 0;

        _lastJaw = null;
        _lastTeeth = null;
        _lastLips = null;

        _count = 0;
    }
}

// EXTENSION METHODS
public static partial class Alligator
{
    /// <summary>
    /// Creates a buffer list for Alligator calculations.
    /// </summary>
    /// <typeparam name="T">The type that implements IReusable.</typeparam>
    /// <param name="source">Time-series values to transform.</param>
    /// <param name="jawPeriods">Lookback periods for the Jaw line. Default is 13.</param>
    /// <param name="jawOffset">Offset periods for the Jaw line. Default is 8.</param>
    /// <param name="teethPeriods">Lookback periods for the Teeth line. Default is 8.</param>
    /// <param name="teethOffset">Offset periods for the Teeth line. Default is 5.</param>
    /// <param name="lipsPeriods">Lookback periods for the Lips line. Default is 5.</param>
    /// <param name="lipsOffset">Offset periods for the Lips line. Default is 3.</param>
    /// <returns>An AlligatorList instance pre-populated with historical data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when source is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when parameters are invalid.</exception>
    public static AlligatorList ToAlligatorList<T>(
        this IReadOnlyList<T> source,
        int jawPeriods = 13,
        int jawOffset = 8,
        int teethPeriods = 8,
        int teethOffset = 5,
        int lipsPeriods = 5,
        int lipsOffset = 3)
        where T : IReusable
    {
        ArgumentNullException.ThrowIfNull(source);
        Validate(jawPeriods, jawOffset, teethPeriods, teethOffset, lipsPeriods, lipsOffset);

        return source is IReadOnlyList<IQuote> quotes
            ? new(jawPeriods, jawOffset, teethPeriods, teethOffset, lipsPeriods, lipsOffset) { quotes }
            : new(jawPeriods, jawOffset, teethPeriods, teethOffset, lipsPeriods, lipsOffset) { (IReadOnlyList<IReusable>)source };
    }
}
