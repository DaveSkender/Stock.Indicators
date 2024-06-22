namespace Skender.Stock.Indicators;

// RATE OF CHANGE (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IEnumerable<RocResult> GetRoc<T>(
        this IEnumerable<T> results,
        int lookbackPeriods)
        where T : IReusableResult
        => results
            .ToTupleResult()
            .CalcRoc(lookbackPeriods);

    // SERIES, from TUPLE
    public static IEnumerable<RocResult> GetRoc(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods) => priceTuples
            .ToSortedList()
            .CalcRoc(lookbackPeriods);
}
