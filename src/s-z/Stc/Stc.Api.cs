namespace Skender.Stock.Indicators;

// SCHAFF TREND CYCLE (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<StcResult> GetStc<TQuote>(
        this IEnumerable<TQuote> quotes,
        int cyclePeriods = 10,
        int fastPeriods = 23,
        int slowPeriods = 50)
        where TQuote : IQuote => quotes
            .ToBasicTuple(CandlePart.Close)
            .CalcStc(cyclePeriods, fastPeriods, slowPeriods);

    // SERIES, from CHAIN
    public static IEnumerable<StcResult> GetStc(
        this IEnumerable<IReusableResult> results,
        int cyclePeriods = 10,
        int fastPeriods = 23,
        int slowPeriods = 50) => results
            .ToResultTuple()
            .CalcStc(cyclePeriods, fastPeriods, slowPeriods)
            .SyncIndex(results, SyncType.Prepend);

    // SERIES, from TUPLE
    public static IEnumerable<StcResult> GetStc(
        this IEnumerable<(DateTime, double)> priceTuples,
        int cyclePeriods = 10,
        int fastPeriods = 23,
        int slowPeriods = 50) => priceTuples
            .ToSortedList()
            .CalcStc(cyclePeriods, fastPeriods, slowPeriods);
}
