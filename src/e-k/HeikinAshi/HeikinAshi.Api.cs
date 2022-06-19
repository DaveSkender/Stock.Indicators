namespace Skender.Stock.Indicators;

// HEIKIN-ASHI (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<HeikinAshiResult> GetHeikinAshi<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote => quotes
            .ToSortedList()
            .CalcHeikinAshi();
}
