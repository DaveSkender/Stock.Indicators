namespace Skender.Stock.Indicators;

/// <summary>
/// Detrended Price Oscillator (DPO) from incremental reusable values.
/// </summary>
public class DpoList : BufferList<DpoResult>, IBufferReusable
{
    private readonly SmaList smaList;
    private readonly int offset;
    private readonly List<double> valueBuffer;
    private readonly List<DateTime> timestampBuffer;

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
        valueBuffer = [];
        timestampBuffer = [];
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
        // Add value and timestamp to buffers
        valueBuffer.Add(value);
        timestampBuffer.Add(timestamp);

        // Add to SMA calculation
        smaList.Add(timestamp, value);

        // Calculate DPO
        int currentIndex = valueBuffer.Count - 1;
        double? dpoSma = null;
        double? dpoVal = null;

        // Check if we can calculate DPO for current position
        // We need: currentIndex >= LookbackPeriods - offset - 1
        // AND we need future SMA available at (currentIndex + offset)
        if (currentIndex >= LookbackPeriods - offset - 1 
            && currentIndex + offset < valueBuffer.Count)
        {
            // Get the SMA for the future period
            int smaIndex = currentIndex + offset;
            if (smaIndex < smaList.Count)
            {
                SmaResult smaResult = smaList[smaIndex];
                if (smaResult.Sma.HasValue)
                {
                    dpoSma = smaResult.Sma;
                    dpoVal = valueBuffer[currentIndex] - smaResult.Sma;
                }
            }
        }

        AddInternal(new DpoResult(timestamp, dpoVal, dpoSma));
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
