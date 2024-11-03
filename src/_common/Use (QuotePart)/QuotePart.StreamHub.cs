namespace Skender.Stock.Indicators;

// USE / QUOTE CONVERTER (STREAM HUB)

#region hub interface and initializer
public interface IQuotePartHub
{
    CandlePart CandlePartSelection { get; }

    // TODO: consider renaming to IBarPartHub, with IQuote to IBar
}

public static partial class QuoteParts
{
    public static QuotePartHub<TIn> ToQuotePart<TIn>(
    this IQuoteProvider<TIn> quoteProvider,
    CandlePart candlePart)
    where TIn : IQuote
    => new(quoteProvider, candlePart);
}
#endregion

public class QuotePartHub<TQuote>
    : ChainProvider<TQuote, QuotePart>, IQuotePartHub
    where TQuote : IQuote
{
    #region constructors

    internal QuotePartHub(
        IQuoteProvider<TQuote> provider,
        CandlePart candlePart)
        : base(provider)
    {
        CandlePartSelection = candlePart;

        Reinitialize();
    }
    #endregion

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
