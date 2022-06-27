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
            .ToBasicTuple(CandlePart.HL2)
            .CalcHtTrendline();

    // SERIES, from CHAIN
    public static IEnumerable<HtlResult> GetHtTrendline(
        this IEnumerable<IReusableResult> results) => results
            .ToResultTuple()
            .CalcHtTrendline()
            .SyncIndex(results, SyncType.Prepend);

    // SERIES, from TUPLE
    public static IEnumerable<HtlResult> GetHtTrendline(
        this IEnumerable<(DateTime, double)> priceTuples) => priceTuples
            .ToSortedList()
            .CalcHtTrendline();
}
