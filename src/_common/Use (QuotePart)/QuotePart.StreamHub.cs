// USE / QUOTE CONVERTER (STREAM HUB)

#region hub interface and initializer

/// <summary>
/// Interface for Quote Part Hub.
/// </summary>
public interface IQuotePartHub
{
    /// <summary>
    /// Gets the selected candle part.
    /// </summary>
    CandlePart CandlePartSelection { get; }

    // TODO: consider renaming to IBarPartHub, with IQuote to IBar
}

public static partial class QuoteParts
{
    /// <summary>
    /// Converts the quote provider to a QuotePartHub.
    /// </summary>
    /// <typeparam name="TIn">The type of the quote.</typeparam>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="candlePart">The candle part to select.</param>
    /// <returns>A QuotePartHub instance.</returns>
    public static QuotePartHub<TIn> ToQuotePart<TIn>(
    this IQuoteProvider<TIn> quoteProvider,
    CandlePart candlePart)
    where TIn : IQuote
    => new(quoteProvider, candlePart);
}
#endregion

/// <summary>
/// Represents a hub for managing quote parts.
/// </summary>
/// <typeparam name="TQuote">The type of the quote.</typeparam>
public class QuotePartHub<TQuote>
    : ChainProvider<TQuote, QuotePart>, IQuotePartHub
    where TQuote : IQuote
{
    #region constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="QuotePartHub{TQuote}"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="candlePart">The candle part to select.</param>
    internal QuotePartHub(
        IQuoteProvider<TQuote> provider,
        CandlePart candlePart)
        : base(provider)
    {
        CandlePartSelection = candlePart;

        Reinitialize();
    }
    #endregion

    /// <summary>
    /// Gets the selected candle part.
    /// </summary>
    public CandlePart CandlePartSelection { get; init; }

    // METHODS

    /// <inheritdoc/>
    protected override (QuotePart result, int index)
        ToIndicator(TQuote item, int? indexHint)
    {
        // candidate result
        QuotePart r
            = item.ToQuotePart(CandlePartSelection);

        return (r, indexHint ?? Cache.Count);
    }

    /// <inheritdoc/>
    public override string ToString()
        => $"QUOTE-PART({CandlePartSelection.ToString().ToUpperInvariant()})";
}
