namespace Skender.Stock.Indicators;

// ACCUMULATION/DISTRIBUTION LINE (API)

public static partial class Adl
{
    // OBSERVER, from Quote Provider
    public static AdlHub<TIn> ToAdl<TIn>(
        this IQuoteProvider<TIn> quoteProvider)
        where TIn : IQuote
        => new(quoteProvider);
}
