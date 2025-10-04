namespace Skender.Stock.Indicators;

/// <summary>
/// Endpoint Moving Average (EPMA) from incremental reusable values.
/// </summary>
public class EpmaList : BufferList<EpmaResult>, IBufferReusable, IEpma
{
    private readonly Queue<double> _buffer;
    private readonly List<IReusable> _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="EpmaList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    public EpmaList(int lookbackPeriods)
    {
        Epma.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        _buffer = new Queue<double>(lookbackPeriods);
        _cache = [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EpmaList"/> class with initial quotes.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <param name="quotes">Initial quotes to populate the list.</param>
    public EpmaList(int lookbackPeriods, IReadOnlyList<IQuote> quotes)
        : this(lookbackPeriods)
        => Add(quotes);

    /// <summary>
    /// Gets the number of periods to look back for the calculation.
    /// </summary>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc />
    public void Add(DateTime timestamp, double value)
    {
        // Use universal buffer extension method for consistent buffer management
        _buffer.Update(LookbackPeriods, value);

        // Add to cache for Increment calculation
        _cache.Add(new Quote { Timestamp = timestamp, Close = (decimal)value });

        // Calculate EPMA when we have enough values using shared Increment method
        int currentIndex = _cache.Count - 1;
        double epma = Epma.Increment(_cache, LookbackPeriods, currentIndex);

        AddInternal(new EpmaResult(timestamp, epma.NaN2Null()));
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
        Add(quote.Timestamp, (double)quote.Close);
    }

    /// <inheritdoc />
    public void Add(IReadOnlyList<IQuote> quotes)
    {
        ArgumentNullException.ThrowIfNull(quotes);

        for (int i = 0; i < quotes.Count; i++)
        {
            Add(quotes[i]);
        }
    }

    /// <inheritdoc />
    public override void Clear()
    {
        _buffer.Clear();
        _cache.Clear();
        ClearInternal();
    }
}

public static partial class Epma
{
    /// <summary>
    /// Creates a buffer list for Endpoint Moving Average (EPMA) calculations.
    /// </summary>
    public static EpmaList ToEpmaList<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote
        => new(lookbackPeriods) { (IReadOnlyList<IQuote>)quotes };
}
