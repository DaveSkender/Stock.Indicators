namespace Skender.Stock.Indicators;

// VOLUME WEIGHTED AVERAGE PRICE (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IReadOnlyList<VwapResult> ToVwap<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        DateTime? startDate = null)
        where TQuote : IQuote => quotes
            .ToQuoteDList()
            .CalcVwap(startDate);
}
