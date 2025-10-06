namespace Skender.Stock.Indicators;

/// <summary>
/// Accumulation/Distribution Line (ADL) from incremental quotes.
/// </summary>
public class AdlList : BufferList<AdlResult>, IBufferList
{
    private double _previousAdl;
    private const int DefaultMaxListSize = (int)(0.9 * int.MaxValue);

    /// <summary>
    /// Initializes a new instance of the <see cref="AdlList"/> class.
    /// </summary>
    public AdlList()
    {
        MaxListSize = DefaultMaxListSize;
        _previousAdl = 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AdlList"/> class with initial quotes.
    /// </summary>
    /// <param name="quotes">Initial quotes to populate the list.</param>
    public AdlList(IReadOnlyList<IQuote> quotes)
        : this()
        => Add(quotes);


    /// <summary>
    /// Gets or sets the maximum size of the result list before pruning occurs.
    /// When the list exceeds this size, older results are removed. Default is 90% of int.MaxValue.
    /// </summary>
    public int MaxListSize { get; init; }


    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);

        DateTime timestamp = quote.Timestamp;

        // Calculate ADL using the Increment method
        AdlResult result = Adl.Increment(
            timestamp,
            (double)quote.High,
            (double)quote.Low,
            (double)quote.Close,
            (double)quote.Volume,
            _previousAdl);

        // Update previous ADL for next calculation
        _previousAdl = result.Adl;

        AddInternal(result);
        PruneList();
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
        ClearInternal();
        _previousAdl = 0;
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

public static partial class Adl
{
    /// <summary>
    /// Creates a buffer list for Accumulation/Distribution Line calculations.
    /// </summary>
    public static AdlList ToAdlList<TQuote>(
        this IReadOnlyList<TQuote> quotes)
        where TQuote : IQuote
        => new() { (IReadOnlyList<IQuote>)quotes };
}
