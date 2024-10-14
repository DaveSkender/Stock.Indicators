namespace Skender.Stock.Indicators;

// SCHAFF TREND CYCLE (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IReadOnlyList<StcResult> GetStc<T>(
        this IReadOnlyList<T> results,
        int cyclePeriods = 10,
        int fastPeriods = 23,
        int slowPeriods = 50)
        where T : IReusable
        => results
            .ToSortedList()
            .CalcStc(cyclePeriods, fastPeriods, slowPeriods);
}
