namespace Skender.Stock.Indicators;

// ACCUMULATION/DISTRIBUTION LINE (API)

public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/type[@name="standard"]/*' />
    ///
    public static IEnumerable<AdlResult> GetAdl<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote
        => quotes
            .ToQuoteD()
            .CalcAdl();

    // OBSERVER, from Quote Provider
    public static Adl<TQuote> ToAdl<TQuote>(
        this IQuoteProvider<TQuote> quoteProvider)
        where TQuote : struct, IQuote
        => new(quoteProvider);
}
