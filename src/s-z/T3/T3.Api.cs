namespace Skender.Stock.Indicators;

// TILLSON T3 MOVING AVERAGE (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IEnumerable<T3Result> GetT3<T>(
        this IEnumerable<T> results,
        int lookbackPeriods = 5,
        double volumeFactor = 0.7)
        where T:IReusableResult
        => results
            .ToTupleResult()
            .CalcT3(lookbackPeriods, volumeFactor);

    // SERIES, from TUPLE
    public static IEnumerable<T3Result> GetT3(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods = 5,
        double volumeFactor = 0.7) => priceTuples
            .ToSortedList()
            .CalcT3(lookbackPeriods, volumeFactor);
}
