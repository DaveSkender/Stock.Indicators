namespace Skender.Stock.Indicators;

// STOCHASTIC RSI (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<StochRsiResult> GetStochRsi<TQuote>(
        this IEnumerable<TQuote> quotes,
        int rsiPeriods,
        int stochPeriods,
        int signalPeriods,
        int smoothPeriods = 1)
        where TQuote : IQuote => quotes
            .ToTuple(CandlePart.Close)
            .CalcStochRsi(
                rsiPeriods,
                stochPeriods,
                signalPeriods,
                smoothPeriods);

    // SERIES, from CHAIN
    public static IEnumerable<StochRsiResult> GetStochRsi(
        this IEnumerable<IReusableResult> results,
        int rsiPeriods,
        int stochPeriods,
        int signalPeriods,
        int smoothPeriods) => results
            .ToTuple()
            .CalcStochRsi(
                rsiPeriods,
                stochPeriods,
                signalPeriods,
                smoothPeriods)
            .SyncIndex(results, SyncType.Prepend);

    // SERIES, from TUPLE
    public static IEnumerable<StochRsiResult> GetStochRsi(
        this IEnumerable<(DateTime, double)> priceTuples,
        int rsiPeriods,
        int stochPeriods,
        int signalPeriods,
        int smoothPeriods) => priceTuples
            .ToSortedList()
            .CalcStochRsi(
                rsiPeriods,
                stochPeriods,
                signalPeriods,
                smoothPeriods);
}
