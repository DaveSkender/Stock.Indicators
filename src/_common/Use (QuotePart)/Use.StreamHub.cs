namespace Skender.Stock.Indicators;

// USE / QUOTE CONVERTER (STREAM HUB)

#region hub interface
public interface IQuotePartHub
{
    CandlePart CandlePartSelection { get; }

    // TODO: consider renaming to IBarPartHub, with IQuote to IBar
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

    protected override (QuotePart result, int index)
        ToCandidate(TQuote item, int? indexHint)
    {
        // candidate result
        QuotePart r
            = item.ToQuotePart(CandlePartSelection);

        return (r, indexHint ?? Cache.Count);
    }

    public override string ToString()
        => $"QUOTE-PART({CandlePartSelection.ToString().ToUpperInvariant()})";
}
