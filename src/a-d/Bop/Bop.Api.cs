namespace Skender.Stock.Indicators;

// BALANCE OF POWER (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IReadOnlyList<BopResult> ToBop<TQuote>(
        this IEnumerable<TQuote> quotes,
        int smoothPeriods = 14)
        where TQuote : IQuote => quotes
            .ToQuoteDList()
            .CalcBop(smoothPeriods);
}
