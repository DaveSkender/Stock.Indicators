namespace Skender.Stock.Indicators;

/// <summary>
/// Weighted Moving Average (WMA) from incremental reusable values.
/// </summary>
public class WmaList : BufferList<WmaResult>, IBufferReusable, IWma
{
    private readonly Queue<double> _buffer;
    private const int DefaultMaxListSize = (int)(0.9 * int.MaxValue);
    private readonly double _divisor;
    private double _sum;
    private double _weightedAverage;

    /// <summary>
    /// Initializes a new instance of the <see cref="WmaList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    public WmaList(int lookbackPeriods)
    {
        Wma.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;

        // Pre-calculate divisor for WMA: n * (n + 1) / 2
        _divisor = (double)lookbackPeriods * (lookbackPeriods + 1) / 2d;

        MaxListSize = DefaultMaxListSize;
        _buffer = new Queue<double>(lookbackPeriods);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WmaList"/> class with initial quotes.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <param name="quotes">Initial quotes to populate the list.</param>
    public WmaList(int lookbackPeriods, IReadOnlyList<IQuote> quotes)
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
        double sumBefore = _sum;
        double? dequeuedValue = _buffer.UpdateWithDequeue(LookbackPeriods, value);

        _sum = dequeuedValue.HasValue
            ? sumBefore - dequeuedValue.Value + value
            : sumBefore + value;

        double? wma = Wma.ComputeWeightedMovingAverage(
            _buffer,
            LookbackPeriods,
            _divisor);

        if (wma.HasValue)
        {
            _weightedAverage = wma.Value;
        }

        AddInternal(new WmaResult(timestamp, wma));
        PruneList();
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
        Add(quote.Timestamp, quote.Value);
    }

    /// <inheritdoc />
    public void Add(IReadOnlyList<IQuote> quotes)
    {
        ArgumentNullException.ThrowIfNull(quotes);

        for (int i = 0; i < quotes.Count; i++)
        {
            Add(quotes[i].Timestamp, quotes[i].Value);
        }
    }

    /// <inheritdoc />
    public override void Clear()
    {
        ClearInternal();
        _buffer.Clear();
        _sum = 0d;
        _weightedAverage = 0d;
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

public static partial class Wma
{
    /// <summary>
    /// Creates a buffer list for Weighted Moving Average (VWMA) calculations.
    /// </summary>
    public static WmaList ToWmaList<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote
        => new(lookbackPeriods) { (IReadOnlyList<IQuote>)quotes };
}
