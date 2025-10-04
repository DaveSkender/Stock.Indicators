namespace Skender.Stock.Indicators;

/// <summary>
/// Endpoint Moving Average (EPMA) from incremental reusable values.
/// </summary>
/// <remarks>
/// <para>
/// <b>Memory Management:</b> This implementation maintains a cache of all added values
/// to support the EPMA calculation which requires global position indices. The cache
/// is automatically pruned when it exceeds <see cref="MaxCacheSize"/> items, keeping
/// only the minimum required for the lookback period plus a buffer.
/// </para>
/// </remarks>
public class EpmaList : BufferList<EpmaResult>, IBufferReusable, IEpma
{
    private readonly Queue<double> _buffer;
    private readonly List<IReusable> _cache;
    private int _cacheOffset; // Tracks how many items have been pruned from cache

    private const int MaxCacheSize = 1000; // Trigger point to prune cache

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
        _cacheOffset = 0;
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
        // The actual global index is cache index + offset (to account for pruned items)
        int cacheIndex = _cache.Count - 1;
        int globalIndex = _cacheOffset + cacheIndex;
        double epma = Epma.Increment(_cache, LookbackPeriods, cacheIndex, globalIndex);

        AddInternal(new EpmaResult(timestamp, epma.NaN2Null()));

        // Prune cache if it exceeds MaxCacheSize
        PruneCache();
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
        _cacheOffset = 0;
        ClearInternal();
    }

    /// <summary>
    /// Prunes the internal cache to prevent unbounded memory growth.
    /// Removes older data while preserving the minimum required periods for calculations.
    /// </summary>
    private void PruneCache()
    {
        if (_cache.Count <= MaxCacheSize)
        {
            return;
        }

        // Calculate how many items to remove
        // Keep at least LookbackPeriods elements for calculations
        int removeCount = _cache.Count - LookbackPeriods;

        if (removeCount > 0)
        {
            _cache.RemoveRange(0, removeCount);
            _cacheOffset += removeCount; // Track total number of items removed
        }
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
