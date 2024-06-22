namespace Skender.Stock.Indicators;

// KAUFMAN's ADAPTIVE MOVING AVERAGE (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IEnumerable<KamaResult> GetKama<T>(
        this IEnumerable<T> results,
        int erPeriods = 10,
        int fastPeriods = 2,
        int slowPeriods = 30)
        where T: IReusableResult
        => results
            .ToTupleResult()
            .CalcKama(erPeriods, fastPeriods, slowPeriods);

    // SERIES, from TUPLE
    public static IEnumerable<KamaResult> GetKama(
        this IEnumerable<(DateTime, double)> priceTuples,
        int erPeriods = 10,
        int fastPeriods = 2,
        int slowPeriods = 30) => priceTuples
            .ToSortedList()
            .CalcKama(erPeriods, fastPeriods, slowPeriods);
}
