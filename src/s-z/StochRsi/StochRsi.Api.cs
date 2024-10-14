namespace Skender.Stock.Indicators;

// STOCHASTIC RSI (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IReadOnlyList<StochRsiResult> ToStochRsi<T>(
        this IReadOnlyList<T> results,
        int rsiPeriods,
        int stochPeriods,
        int signalPeriods,
        int smoothPeriods = 1)
        where T : IReusable
        => results
            .ToSortedList()
            .CalcStochRsi(
                rsiPeriods,
                stochPeriods,
                signalPeriods,
                smoothPeriods);
}
