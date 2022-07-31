namespace Skender.Stock.Indicators;

// ACCUMULATION/DISTRIBUTION LINE (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<AdlResult> GetAdl<TQuote>(
        this IEnumerable<TQuote> quotes,
        int? smaPeriods = null)
        where TQuote : IQuote => quotes
            .ToQuoteD()
            .CalcAdl(smaPeriods);
}
