namespace Skender.Stock.Indicators;

// PRICE MOMENTUM OSCILLATOR (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IReadOnlyList<PmoResult> GetPmo<T>(
        this IEnumerable<T> results,
        int timePeriods = 35,
        int smoothPeriods = 20,
        int signalPeriods = 10)
        where T : IReusable
        => results
            .ToSortedList()
            .CalcPmo(timePeriods, smoothPeriods, signalPeriods);
}
