namespace Skender.Stock.Indicators;

/// <summary>
/// ZigZag indicator from incremental quotes.
/// Supports repainting: historical values are recalculated as new data arrives
/// to properly identify confirmed peaks and troughs.
/// </summary>
public class ZigZagList : BufferList<ZigZagResult>, IIncrementFromQuote
{
    private readonly List<IQuote> _quoteCache;

    /// <summary>
    /// Initializes a new instance of the <see cref="ZigZagList"/> class.
    /// </summary>
    /// <param name="endType">The type of end to use (Close or HighLow).</param>
    /// <param name="percentChange">The percentage change threshold for ZigZag points.</param>
    public ZigZagList(EndType endType = EndType.Close, decimal percentChange = 5)
    {
        ZigZag.Validate(percentChange);
        EndType = endType;
        PercentChange = percentChange;
        _quoteCache = [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ZigZagList"/> class with initial quotes.
    /// </summary>
    /// <param name="endType">The type of end to use (Close or HighLow).</param>
    /// <param name="percentChange">The percentage change threshold for ZigZag points.</param>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    public ZigZagList(EndType endType, decimal percentChange, IReadOnlyList<IQuote> quotes)
        : this(endType, percentChange)
    {
        Add(quotes);
    }

    /// <summary>
    /// Gets the type of end used (Close or HighLow).
    /// </summary>
    public EndType EndType { get; init; }

    /// <summary>
    /// Gets the percentage change threshold.
    /// </summary>
    public decimal PercentChange { get; init; }

    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);

        // Add quote to cache
        _quoteCache.Add(quote);

        // Recalculate entire ZigZag from cached quotes
        Recalculate();
    }

    /// <inheritdoc />
    public void Add(IReadOnlyList<IQuote> quotes)
    {
        ArgumentNullException.ThrowIfNull(quotes);

        _quoteCache.AddRange(quotes);

        // Recalculate once after adding all quotes
        Recalculate();
    }

    /// <inheritdoc />
    public override void Clear()
    {
        base.Clear();
        _quoteCache.Clear();
    }

    private void Recalculate()
    {
        // Recalculate using Series method
        IReadOnlyList<ZigZagResult> results = _quoteCache.ToZigZag(EndType, PercentChange);

        // Update existing results or add new ones
        for (int i = 0; i < results.Count; i++)
        {
            if (i < Count)
            {
                // Update existing result
                UpdateInternal(i, results[i]);
            }
            else
            {
                // Add new result
                AddInternal(results[i]);
            }
        }

        // Remove any excess results (if quote cache was pruned)
        while (Count > results.Count)
        {
            RemoveAt(Count - 1);
        }
    }

    /// <inheritdoc />
    protected override void PruneList()
    {
        // Override pruning to also prune quote cache
        int overflow = Count - MaxListSize;

        if (overflow <= 0)
        {
            return;
        }

        // Remove overflow quotes from cache
        _quoteCache.RemoveRange(0, overflow);

        // Call base pruning
        base.PruneList();
    }
}

/// <summary>
/// Provides extension method to create ZigZag BufferList.
/// </summary>
public static partial class ZigZag
{
    /// <summary>
    /// Creates a buffer list for ZigZag calculations.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="endType">End calculation type</param>
    /// <param name="percentChange">Percent change threshold</param>
    public static ZigZagList ToZigZagList(
        this IReadOnlyList<IQuote> quotes,
        EndType endType = EndType.Close,
        decimal percentChange = 5)
        => new(endType, percentChange) { quotes };
}
