namespace Skender.Stock.Indicators;

public static partial class QuoteUtility
{
    // OBSERVER, from Quote Provider (redistribution)
    public static QuoteHub<TQuote> ToQuote<TQuote>(
        this IQuoteProvider<TQuote> quoteProvider)
        where TQuote : IQuote => new(quoteProvider);
}
