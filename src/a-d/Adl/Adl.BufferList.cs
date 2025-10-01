namespace Skender.Stock.Indicators;

/// <summary>
/// Accumulation/Distribution Line (ADL) from incremental quotes.
/// </summary>
public class AdlList : BufferListBase<AdlResult>, IBufferList
{
    private double _previousAdl;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdlList"/> class.
    /// </summary>
    public AdlList()
    {
        _previousAdl = 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AdlList"/> class with initial quotes.
    /// </summary>
    /// <param name="quotes">Initial quotes to populate the list.</param>
    public AdlList(IReadOnlyList<IQuote> quotes)
        : this()
        => Add(quotes);

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
    public new void Clear()
    {
        ClearInternal();
        _previousAdl = 0;
        RollbackState(-1);
    }

    /// <inheritdoc/>
    protected override void RollbackState(int index)
    {
        // Reset to initial state
        // For ADL, we could potentially extract the previous ADL value
        // from the result at index, but since manual modifications are
        // not a recommended pattern, we'll just reset to zero
        _previousAdl = index >= 0 && index < Count
            ? this[index].Adl
            : 0;
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
