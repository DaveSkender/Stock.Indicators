namespace Skender.Stock.Indicators;

// MOVING AVERAGE CONVERGENCE/DIVERGENCE (MACD) OSCILLATOR (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<MacdResult> GetMacd<TQuote>(
        this IEnumerable<TQuote> quotes,
        int fastPeriods = 12,
        int slowPeriods = 26,
        int signalPeriods = 9)
        where TQuote : IQuote => quotes
            .ToBasicTuple(CandlePart.Close)
            .CalcMacd(fastPeriods, slowPeriods, signalPeriods);

    // SERIES, from CHAIN
    public static IEnumerable<MacdResult> GetMacd(
        this IEnumerable<IReusableResult> results,
        int fastPeriods = 12,
        int slowPeriods = 26,
        int signalPeriods = 9) => results
            .ToResultTuple()
            .CalcMacd(fastPeriods, slowPeriods, signalPeriods)
            .SyncIndex(results, SyncType.Prepend);

    // SERIES, from TUPLE
    public static IEnumerable<MacdResult> GetMacd(
        this IEnumerable<(DateTime, double)> priceTuples,
        int fastPeriods = 12,
        int slowPeriods = 26,
        int signalPeriods = 9) => priceTuples
            .ToSortedList()
            .CalcMacd(fastPeriods, slowPeriods, signalPeriods);
}
