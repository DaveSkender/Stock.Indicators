namespace Skender.Stock.Indicators;

// ULCER INDEX (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IReadOnlyList<UlcerIndexResult> GetUlcerIndex<T>(
        this IEnumerable<T> results,
        int lookbackPeriods = 14)
        where T : IReusable
        => results
            .ToSortedList()
            .CalcUlcerIndex(lookbackPeriods);
}
