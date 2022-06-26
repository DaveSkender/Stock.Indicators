namespace Skender.Stock.Indicators;

// CONNORS RSI (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<ConnorsRsiResult> GetConnorsRsi<TQuote>(
        this IEnumerable<TQuote> quotes,
        int rsiPeriods = 3,
        int streakPeriods = 2,
        int rankPeriods = 100)
        where TQuote : IQuote => quotes
            .ToBasicTuple(CandlePart.Close)
            .CalcConnorsRsi(rsiPeriods, streakPeriods, rankPeriods);

    // SERIES, from CHAIN
    public static IEnumerable<ConnorsRsiResult> GetConnorsRsi(
        this IEnumerable<IReusableResult> results,
        int rsiPeriods = 3,
        int streakPeriods = 2,
        int rankPeriods = 100) => results
            .ToResultTuple()
            .CalcConnorsRsi(rsiPeriods, streakPeriods, rankPeriods)
            .SyncIndex(results, SyncType.Prepend);

    // SERIES, from TUPLE
    public static IEnumerable<ConnorsRsiResult> GetConnorsRsi(
        this IEnumerable<(DateTime, double)> priceTuples,
        int rsiPeriods = 3,
        int streakPeriods = 2,
        int rankPeriods = 100) => priceTuples
            .ToSortedList()
            .CalcConnorsRsi(rsiPeriods, streakPeriods, rankPeriods);
}
