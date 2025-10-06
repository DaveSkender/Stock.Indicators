namespace Skender.Stock.Indicators;

/// <summary>
/// Rate of Change (ROC) from incremental reusable values.
/// </summary>
public class RocList : BufferList<RocResult>, IBufferReusable, IRoc
{
    private readonly Queue<double> buffer;
    private const int DefaultMaxListSize = (int)(0.9 * int.MaxValue);

    /// <summary>
    /// Initializes a new instance of the <see cref="RocList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    public RocList(int lookbackPeriods)
    {
        Roc.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;

        MaxListSize = DefaultMaxListSize;        buffer = new Queue<double>(lookbackPeriods + 1);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RocList"/> class with initial quotes.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <param name="quotes">Initial quotes to populate the list.</param>
    public RocList(int lookbackPeriods, IReadOnlyList<IQuote> quotes)
        : this(lookbackPeriods)
        => Add(quotes);

    /// <summary>
    /// Gets the number of periods to look back for the calculation.
    /// </summary>
    public int LookbackPeriods { get; init; }


    /// <summary>
    /// Gets or sets the maximum size of the result list before pruning occurs.
    /// When the list exceeds this size, older results are removed. Default is 90% of int.MaxValue.
    /// </summary>
    public int MaxListSize { get; init; }


    /// <inheritdoc />
    public void Add(DateTime timestamp, double value)
    {
        // Add new value to buffer
        buffer.Enqueue(value);

        double roc;
        double momentum;

        // Calculate ROC when we have enough values
        if (buffer.Count > LookbackPeriods)
        {
            double backValue = buffer.Peek();
            momentum = value - backValue;

            roc = backValue == 0
                ? double.NaN
                : 100d * momentum / backValue;

            // Remove oldest value to maintain buffer size
            buffer.Dequeue();
        }
        else
        {
            momentum = double.NaN;
            roc = double.NaN;
        }

        AddInternal(new RocResult(
            Timestamp: timestamp,
            Momentum: momentum.NaN2Null(),
            Roc: roc.NaN2Null()));
        PruneList();
    }

    /// <summary>
    /// Adds a new reusable value to the ROC list.
    /// </summary>
    /// <param name="value">The reusable value to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when the value is null.</exception>
    public void Add(IReusable value)
    {
        ArgumentNullException.ThrowIfNull(value);
        Add(value.Timestamp, value.Value);
    }

    /// <summary>
    /// Adds a list of reusable values to the ROC list.
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
    /// Adds a new quote to the ROC list.
    /// </summary>
    /// <param name="quote">The quote to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when the quote is null.</exception>
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);
        Add(quote.Timestamp, quote.Value);
    }

    /// <summary>
    /// Adds a list of quotes to the ROC list.
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
        buffer.Clear();
    }

    /// <summary>
    /// Prunes the result list to prevent unbounded memory growth.
    /// </summary>
    private void PruneList()
    {
        if (Count < MaxListSize)
        {
            return;
        }

        // Remove oldest results while keeping the list under MaxListSize
        while (Count >= MaxListSize)
        {
            RemoveAtInternal(0);
        }
    }
}

public static partial class Roc
{
    /// <summary>
    /// Creates a buffer list for Rate of Change (ROC) calculations.
    /// </summary>
    public static RocList ToRocList<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote
        => new(lookbackPeriods) { (IReadOnlyList<IQuote>)quotes };
}
