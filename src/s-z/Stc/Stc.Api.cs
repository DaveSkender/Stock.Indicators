namespace Skender.Stock.Indicators;

// SCHAFF TREND CYCLE (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IEnumerable<StcResult> GetStc<T>(
        this IEnumerable<T> results,
        int cyclePeriods = 10,
        int fastPeriods = 23,
        int slowPeriods = 50)
        where T: IReusableResult
        => results
            .ToTupleResult()
            .CalcStc(cyclePeriods, fastPeriods, slowPeriods);

    // SERIES, from TUPLE
    public static IEnumerable<StcResult> GetStc(
        this IEnumerable<(DateTime, double)> priceTuples,
        int cyclePeriods = 10,
        int fastPeriods = 23,
        int slowPeriods = 50) => priceTuples
            .ToSortedList()
            .CalcStc(cyclePeriods, fastPeriods, slowPeriods);
}
