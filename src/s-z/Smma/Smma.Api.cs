namespace Skender.Stock.Indicators;

// SMOOTHED MOVING AVERAGE (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IEnumerable<SmmaResult> GetSmma<T>(
        this IEnumerable<T> results,
        int lookbackPeriods)
        where T : IReusableResult
        => results
            .ToTupleResult()
            .CalcSmma(lookbackPeriods);

    // SERIES, from TUPLE
    public static IEnumerable<SmmaResult> GetSmma(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods) => priceTuples
            .ToSortedList()
            .CalcSmma(lookbackPeriods);
}
