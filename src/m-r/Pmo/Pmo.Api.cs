namespace Skender.Stock.Indicators;

// PRICE MOMENTUM OSCILLATOR (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<PmoResult> GetPmo<TQuote>(
        this IEnumerable<TQuote> quotes,
        int timePeriods = 35,
        int smoothPeriods = 20,
        int signalPeriods = 10)
        where TQuote : IQuote => quotes
            .ToBasicTuple(CandlePart.Close)
            .CalcPmo(timePeriods, smoothPeriods, signalPeriods);

    // SERIES, from CHAIN
    public static IEnumerable<PmoResult> GetPmo(
        this IEnumerable<IReusableResult> results,
        int timePeriods = 35,
        int smoothPeriods = 20,
        int signalPeriods = 10) => results
            .ToResultTuple()
            .CalcPmo(timePeriods, smoothPeriods, signalPeriods)
            .SyncIndex(results, SyncType.Prepend);

    // SERIES, from TUPLE
    public static IEnumerable<PmoResult> GetPmo(
        this IEnumerable<(DateTime, double)> priceTuples,
        int timePeriods = 35,
        int smoothPeriods = 20,
        int signalPeriods = 10) => priceTuples
            .ToSortedList()
            .CalcPmo(timePeriods, smoothPeriods, signalPeriods);
}
