namespace Skender.Stock.Indicators;

/// <summary>
/// Streaming hub for managing quote parts.
/// </summary>
public class QuotePartHub
    : ChainHub<IQuote, TimeValue>, IQuotePart
{
    internal QuotePartHub(
        IQuoteProvider<IQuote> provider,
        CandlePart candlePart
    ) : base(provider)
    {
        CandlePartSelection = candlePart;

        Reinitialize();
    }


    /// <inheritdoc/>
    public override IReadOnlyList<TimeValue> AsStaticSeries(IReadOnlyList<IQuote> input)
        => input.ToQuotePart(CandlePartSelection);
    /// <inheritdoc/>
    public CandlePart CandlePartSelection { get; init; }


    /// <inheritdoc/>
    protected override (TimeValue result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        // candidate result
        TimeValue r
            = item.ToQuotePart(CandlePartSelection);

        return (r, indexHint ?? Cache.Count);
    }

    /// <inheritdoc/>
    public override string ToString()
        => $"QUOTE-PART({CandlePartSelection.ToString().ToUpperInvariant()})";
}

public static partial class QuoteParts
{
    /// <summary>
    /// Creates an QuotePart streaming hub from a chain provider.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="candlePart">The <see cref="CandlePart" /> element.</param>
    /// <returns>An new <see cref="QuotePartHub"/> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quote provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="candlePart"/> invalid.</exception>
    public static QuotePartHub ToQuotePartHub(
        this IQuoteProvider<IQuote> quoteProvider,
        CandlePart candlePart)
        => new(quoteProvider, candlePart);
}
