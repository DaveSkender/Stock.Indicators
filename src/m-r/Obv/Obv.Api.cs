namespace Skender.Stock.Indicators;

// ON-BALANCE VOLUME (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<ObvResult> GetObv<TQuote>(
        this IEnumerable<TQuote> quotes,
        int? smaPeriods = null)
        where TQuote : IQuote => quotes
            .ToQuoteD()
            .CalcObv(smaPeriods);
}
