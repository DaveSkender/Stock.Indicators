namespace Skender.Stock.Indicators;

// TRUE RANGE (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<TrResult> GetTr<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote => quotes
            .ToQuoteD()
            .CalcTr();
}
