namespace Skender.Stock.Indicators;

// MOVING AVERAGE CONVERGENCE/DIVERGENCE (MACD) OSCILLATOR (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IEnumerable<MacdResult> GetMacd<T>(
        this IEnumerable<T> results,
        int fastPeriods = 12,
        int slowPeriods = 26,
        int signalPeriods = 9)
        where T : IReusableResult
        => results
            .ToTupleResult()
            .CalcMacd(fastPeriods, slowPeriods, signalPeriods);

    // SERIES, from TUPLE
    public static IEnumerable<MacdResult> GetMacd(
        this IEnumerable<(DateTime, double)> priceTuples,
        int fastPeriods = 12,
        int slowPeriods = 26,
        int signalPeriods = 9) => priceTuples
            .ToSortedList()
            .CalcMacd(fastPeriods, slowPeriods, signalPeriods);
}
