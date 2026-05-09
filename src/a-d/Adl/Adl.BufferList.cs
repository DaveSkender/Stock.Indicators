namespace Skender.Stock.Indicators;

/// <summary>
/// Accumulation/Distribution Line (ADL) from incremental quotes.
/// </summary>
public class AdlList : BufferList<AdlResult>, IIncrementFromQuote
{
    private double _previousAdl;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdlList"/> class.
    /// </summary>
    public AdlList() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AdlList"/> class with initial quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    public AdlList(IReadOnlyList<IQuote> quotes)
        : this() => Add(quotes);

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
    public override void Clear()
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
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    public static AdlList ToAdlList(
        this IReadOnlyList<IQuote> quotes)
        => new() { quotes };
}
