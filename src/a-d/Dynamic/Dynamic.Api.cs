namespace Skender.Stock.Indicators;

// McGINLEY DYNAMIC
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IEnumerable<DynamicResult> GetDynamic<T>(
        this IEnumerable<T> results,
        int lookbackPeriods,
        double kFactor = 0.6)
        where T : IReusableResult
        => results
            .ToSortedList()
            .CalcDynamic(lookbackPeriods, kFactor);
}
