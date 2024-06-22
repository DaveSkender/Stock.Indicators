namespace Skender.Stock.Indicators;

// McGINLEY DYNAMIC
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IEnumerable<DynamicResult> GetDynamic<T>(
        this IEnumerable<T> results,
        int lookbackPeriods,
        double kFactor = 0.6)
        where T: IReusableResult
        => results
            .ToTupleResult()
            .CalcDynamic(lookbackPeriods, kFactor);

    // SERIES, from TUPLE
    public static IEnumerable<DynamicResult> GetDynamic(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods,
        double kFactor = 0.6) => priceTuples
            .ToSortedList()
            .CalcDynamic(lookbackPeriods, kFactor);
}
