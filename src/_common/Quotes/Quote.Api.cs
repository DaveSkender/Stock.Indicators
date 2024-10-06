namespace Skender.Stock.Indicators;

// QUOTE (API)

public static partial class Utility
{
    // OBSERVER, from Quote Provider (redistribution)
    public static QuoteHub<TQuote> ToQuote<TQuote>(
        this IQuoteProvider<TQuote> quoteProvider)
        where TQuote : IQuote => new(quoteProvider);
}
