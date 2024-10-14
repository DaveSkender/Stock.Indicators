namespace Skender.Stock.Indicators;

// McGINLEY DYNAMIC (API)

public static partial class MgDynamic
{
    // SERIES, from CHAIN
    public static IReadOnlyList<DynamicResult> GetDynamic<T>(
        this IReadOnlyList<T> results,
        int lookbackPeriods,
        double kFactor = 0.6)
        where T : IReusable
        => results
            .ToSortedList()
            .CalcDynamic(lookbackPeriods, kFactor);
}
