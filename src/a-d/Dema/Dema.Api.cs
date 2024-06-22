namespace Skender.Stock.Indicators;

// DOUBLE EXPONENTIAL MOVING AVERAGE - DEMA (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IEnumerable<DemaResult> GetDema<T>(
        this IEnumerable<T> results,
        int lookbackPeriods)
        where T:IReusableResult
        => results
            .ToTupleResult()
            .CalcDema(lookbackPeriods);

    // SERIES, from TUPLE
    public static IEnumerable<DemaResult> GetDema(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods) => priceTuples
            .ToSortedList()
            .CalcDema(lookbackPeriods);
}
