namespace Skender.Stock.Indicators;

/// <summary>
/// Detrended Price Oscillator (DPO) from incremental reusable values.
/// Note: DPO requires lookahead, so results are delayed by offset periods.
/// </summary>
public class DpoList : BufferList<DpoResult>, IBufferReusable
{
    private readonly SmaList smaList;
    private readonly int offset;
    private readonly Queue<double> valueBuffer;
    private readonly Queue<DateTime> timestampBuffer;
    private int inputCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="DpoList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    public DpoList(int lookbackPeriods)
    {
        Dpo.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;

        offset = (lookbackPeriods / 2) + 1;
        smaList = new SmaList(lookbackPeriods);
        valueBuffer = new Queue<double>(offset);
        timestampBuffer = new Queue<DateTime>(offset);
        inputCount = 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DpoList"/> class with initial quotes.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <param name="quotes">Initial quotes to populate the list.</param>
    public DpoList(int lookbackPeriods, IReadOnlyList<IQuote> quotes)
        : this(lookbackPeriods)
        => Add(quotes);

    /// <summary>
    /// Gets the number of periods to look back for the calculation.
    /// </summary>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc />
    public void Add(DateTime timestamp, double value)
    {
        // Add to SMA calculation first
        smaList.Add(timestamp, value);

        // Track total inputs to determine when we can emit
        inputCount++;

        // Calculate the index of the value we can now emit a DPO result for
        int dpoIndex = inputCount - offset - 1;

        // If we can emit a result, do so BEFORE updating buffers
        // This ensures we use the correct oldest value before it gets dequeued
        if (dpoIndex >= 0 && valueBuffer.Count == offset)
        {
            // Get the oldest buffered value and timestamp (this is what we're calculating DPO for)
            double oldestValue = valueBuffer.Peek();
            DateTime oldestTimestamp = timestampBuffer.Peek();

            // Get the current SMA result (this is the "future" SMA for the oldest value)
            SmaResult currentSma = smaList[^1];

            double? dpoSma = currentSma.Sma;
            double? dpoVal = currentSma.Sma.HasValue
                ? oldestValue - currentSma.Sma.Value
                : null;

            AddInternal(new DpoResult(oldestTimestamp, dpoVal, dpoSma));
        }

        // Now update the buffers for the next iteration
        valueBuffer.Update(offset, value);
        timestampBuffer.Update(offset, timestamp);
    }

    /// <summary>
    /// Adds a new reusable value to the DPO list.
    /// </summary>
    /// <param name="value">The reusable value to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when the value is null.</exception>
    public void Add(IReusable value)
    {
        ArgumentNullException.ThrowIfNull(value);
        Add(value.Timestamp, value.Value);
    }

    /// <summary>
    /// Adds a list of reusable values to the DPO list.
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
    /// Adds a new quote to the DPO list.
    /// </summary>
    /// <param name="quote">The quote to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when the quote is null.</exception>
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);
        Add(quote.Timestamp, quote.Value);
    }

    /// <summary>
    /// Adds a list of quotes to the DPO list.
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
    public override void Clear()
    {
        ClearInternal();
        smaList.Clear();
        valueBuffer.Clear();
        timestampBuffer.Clear();
        inputCount = 0;
    }
}

public static partial class Dpo
{
    /// <summary>
    /// Creates a buffer list for Detrended Price Oscillator (DPO) calculations.
    /// </summary>
    public static DpoList ToDpoList<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote
        => new(lookbackPeriods) { (IReadOnlyList<IQuote>)quotes };
}
