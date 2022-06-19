namespace Skender.Stock.Indicators;

// VOLUME WEIGHTED AVERAGE PRICE (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<VwapResult> GetVwap<TQuote>(
        this IEnumerable<TQuote> quotes,
        DateTime? startDate = null)
        where TQuote : IQuote => quotes
            .ToQuoteD()
            .CalcVwap(startDate);
}
