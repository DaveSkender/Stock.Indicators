namespace Skender.Stock.Indicators;

// HILBERT TRANSFORM - INSTANTANEOUS TRENDLINE (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<HtlResult> GetHtTrendline<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote => quotes
            .ToTuple(CandlePart.HL2)
            .CalcHtTrendline();

    // SERIES, from CHAIN
    public static IEnumerable<HtlResult> GetHtTrendline(
        this IEnumerable<IReusableResult> results) => results
            .ToTuple()
            .CalcHtTrendline()
            .SyncIndex(results, SyncType.Prepend);

    // SERIES, from TUPLE
    public static IEnumerable<HtlResult> GetHtTrendline(
        this IEnumerable<(DateTime, double)> priceTuples) => priceTuples
            .ToSortedList()
            .CalcHtTrendline();
}
