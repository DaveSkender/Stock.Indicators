namespace Skender.Stock.Indicators;

/// <summary>
/// Accumulation/Distribution Line (ADL) from incremental quotes.
/// </summary>
public class AdlList : List<AdlResult>, IBufferList
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
    {
        Add(quotes);
    }

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

        base.Add(result);
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
        base.Clear();
        _previousAdl = 0;
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
