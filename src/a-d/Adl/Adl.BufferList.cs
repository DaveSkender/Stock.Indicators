namespace Skender.Stock.Indicators;

/// <summary>
/// Accumulation/Distribution Line (ADL) from incremental quotes.
/// </summary>
public static partial class Adl
{
    /// <summary>
    /// Creates a buffer list for Accumulation/Distribution Line calculations.
    /// </summary>
    public static AdlList ToAdlBufferList<TQuote>(
        this IReadOnlyList<TQuote> quotes)
        where TQuote : IQuote
    {
        // Input validation
        ArgumentNullException.ThrowIfNull(quotes);

        // Initialize buffer and populate
        AdlList bufferList = new();

        foreach (TQuote quote in quotes)
        {
            bufferList.Add(quote);
        }

        return bufferList;
    }
}

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
