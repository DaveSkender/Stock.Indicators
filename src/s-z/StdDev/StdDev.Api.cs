namespace Skender.Stock.Indicators;

// STANDARD DEVIATION (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IEnumerable<StdDevResult> GetStdDev<T>(
        this IEnumerable<T> results,
        int lookbackPeriods)
        where T: IReusableResult
        => results
            .ToTupleResult()
            .CalcStdDev(lookbackPeriods);

    // SERIES, from TUPLE
    public static IEnumerable<StdDevResult> GetStdDev(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods) => priceTuples
            .ToSortedList()
            .CalcStdDev(lookbackPeriods);
}
