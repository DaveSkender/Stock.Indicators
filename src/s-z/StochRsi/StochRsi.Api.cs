namespace Skender.Stock.Indicators;

// STOCHASTIC RSI (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IEnumerable<StochRsiResult> GetStochRsi<T>(
        this IEnumerable<T> results,
        int rsiPeriods,
        int stochPeriods,
        int signalPeriods,
        int smoothPeriods = 1)
        where T:IReusableResult
        => results
            .ToTupleResult()
            .CalcStochRsi(
                rsiPeriods,
                stochPeriods,
                signalPeriods,
                smoothPeriods);

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
