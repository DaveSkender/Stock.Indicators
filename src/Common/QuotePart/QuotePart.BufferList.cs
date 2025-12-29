namespace Skender.Stock.Indicators;

/// <summary>
/// Quote part selection from incremental quotes.
/// </summary>
/// <param name="candlePart">The <see cref="CandlePart" /> element.</param>
public class QuotePartList(CandlePart candlePart) : BufferList<QuotePart>, IIncrementFromQuote, IQuotePart
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QuotePartList"/> class with initial quotes.
    /// </summary>
    /// <param name="candlePart">The <see cref="CandlePart" /> element.</param>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    public QuotePartList(CandlePart candlePart, IReadOnlyList<IQuote> quotes)
        : this(candlePart) => Add(quotes);

    /// <inheritdoc />
    public CandlePart CandlePartSelection { get; init; } = candlePart;

    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);

        QuotePart result = quote.ToQuotePart(CandlePartSelection);
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

    /// <summary>
    /// Clears the list and resets internal state so the instance can be reused.
    /// </summary>
    public override void Clear() => base.Clear();
}

public static partial class QuoteParts
{
    /// <summary>
    /// Creates a buffer list for quote part selection.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="candlePart">The <see cref="CandlePart" /> element.</param>
    public static QuotePartList ToQuotePartList(
        this IReadOnlyList<IQuote> quotes,
        CandlePart candlePart)
        => new(candlePart) { quotes };
}
