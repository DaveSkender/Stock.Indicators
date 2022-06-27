namespace Skender.Stock.Indicators;

// KAUFMAN's ADAPTIVE MOVING AVERAGE (API)
public static partial class Indicator
{
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<KamaResult> GetKama<TQuote>(
        this IEnumerable<TQuote> quotes,
        int erPeriods = 10,
        int fastPeriods = 2,
        int slowPeriods = 30)
        where TQuote : IQuote => quotes
            .ToBasicTuple(CandlePart.Close)
            .CalcKama(erPeriods, fastPeriods, slowPeriods);

    // SERIES, from CHAIN
    public static IEnumerable<KamaResult> GetKama(
        this IEnumerable<IReusableResult> results,
        int erPeriods = 10,
        int fastPeriods = 2,
        int slowPeriods = 30) => results
            .ToResultTuple()
            .CalcKama(erPeriods, fastPeriods, slowPeriods)
            .SyncIndex(results, SyncType.Prepend);

    // SERIES, from TUPLE
    public static IEnumerable<KamaResult> GetKama(
        this IEnumerable<(DateTime, double)> priceTuples,
        int erPeriods = 10,
        int fastPeriods = 2,
        int slowPeriods = 30) => priceTuples
            .ToSortedList()
            .CalcKama(erPeriods, fastPeriods, slowPeriods);
}
