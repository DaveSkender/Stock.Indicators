namespace Skender.Stock.Indicators;

// AVERAGE TRUE RANGE (API)

public static partial class Atr
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IReadOnlyList<AtrResult> ToAtr<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 14)
        where TQuote : IQuote => quotes
            .ToQuoteDList()
            .CalcAtr(lookbackPeriods);

    // OBSERVER, from Quote Provider
    public static AtrHub<TIn> ToAtr<TIn>(
        this IQuoteProvider<TIn> quoteProvider,
        int lookbackPeriods = 14)
        where TIn : IQuote
        => new(quoteProvider, lookbackPeriods);
}
