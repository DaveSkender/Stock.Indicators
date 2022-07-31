namespace Skender.Stock.Indicators;

// AWESOME OSCILLATOR (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<AwesomeResult> GetAwesome<TQuote>(
        this IEnumerable<TQuote> quotes,
        int fastPeriods = 5,
        int slowPeriods = 34)
        where TQuote : IQuote => quotes
            .ToBasicTuple(CandlePart.HL2)
            .CalcAwesome(fastPeriods, slowPeriods);

    // SERIES, from CHAIN
    public static IEnumerable<AwesomeResult> GetAwesome(
        this IEnumerable<IReusableResult> results,
        int fastPeriods = 5,
        int slowPeriods = 34) => results
            .ToResultTuple()
            .CalcAwesome(fastPeriods, slowPeriods)
            .SyncIndex(results, SyncType.Prepend);

    // SERIES, from TUPLE
    public static IEnumerable<AwesomeResult> GetAwesome(
        this IEnumerable<(DateTime, double)> priceTuples,
        int fastPeriods = 5,
        int slowPeriods = 34) => priceTuples
            .ToSortedList()
            .CalcAwesome(fastPeriods, slowPeriods);
}
