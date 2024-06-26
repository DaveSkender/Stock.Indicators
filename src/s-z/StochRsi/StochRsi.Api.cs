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
        where T : IReusableResult
        => results
            .ToSortedList()
            .CalcStochRsi(
                rsiPeriods,
                stochPeriods,
                signalPeriods,
                smoothPeriods);
}
