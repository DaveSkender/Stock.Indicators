namespace Skender.Stock.Indicators;

// ULCER INDEX (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IEnumerable<UlcerIndexResult> GetUlcerIndex<T>(
        this IEnumerable<T> results,
        int lookbackPeriods = 14)
        where T: IReusableResult
        => results
            .ToTupleResult()
            .CalcUlcerIndex(lookbackPeriods);

    // SERIES, from TUPLE
    public static IEnumerable<UlcerIndexResult> GetUlcerIndex(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods) => priceTuples
            .ToSortedList()
            .CalcUlcerIndex(lookbackPeriods);
}
