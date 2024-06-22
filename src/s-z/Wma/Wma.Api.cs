namespace Skender.Stock.Indicators;

// WEIGHTED MOVING AVERAGE (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IEnumerable<WmaResult> GetWma<T>(
        this IEnumerable<T> results,
        int lookbackPeriods)
        where T : IReusableResult
        => results
            .ToTupleResult()
            .CalcWma(lookbackPeriods);

    // SERIES, from TUPLE
    public static IEnumerable<WmaResult> GetWma(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods) => priceTuples
            .ToSortedList()
            .CalcWma(lookbackPeriods);
}
