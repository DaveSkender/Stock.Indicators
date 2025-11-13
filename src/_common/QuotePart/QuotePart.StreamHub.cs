namespace Skender.Stock.Indicators;

// USE / QUOTE CONVERTER (STREAM HUB)

public static partial class QuoteParts
{
    /// <summary>
    /// Converts the quote provider to a QuotePartHub.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="candlePart">The <see cref="CandlePart" /> element.</param>
    /// <returns>A QuotePartHub instance.</returns>
    public static QuotePartHub ToQuotePartHub(
    this IQuoteProvider<IQuote> quoteProvider,
    CandlePart candlePart)
    => new(quoteProvider, candlePart);
}

/// <summary>
/// Streaming hub for managing quote parts.
/// </summary>
public class QuotePartHub
    : ChainProvider<IQuote, QuotePart>, IQuotePart
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QuotePartHub"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="candlePart">The <see cref="CandlePart" /> element.</param>
    internal QuotePartHub(
        IQuoteProvider<IQuote> provider,
        CandlePart candlePart
    ) : base(provider)
    {
        CandlePartSelection = candlePart;

        Reinitialize();
    }

    /// <summary>
    /// Gets the selected candle part.
    /// </summary>
    public CandlePart CandlePartSelection { get; init; }

    // METHODS

    /// <inheritdoc/>
    protected override (QuotePart result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        // candidate result
        QuotePart r
            = item.ToQuotePart(CandlePartSelection);

        return (r, indexHint ?? Cache.Count);
    }

    /// <inheritdoc/>
    public override string ToString()
        => $"QUOTE-PART({CandlePartSelection.ToString().ToUpperInvariant()})";
}
