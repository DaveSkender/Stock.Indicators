namespace Skender.Stock.Indicators;

// WEIGHTED MOVING AVERAGE (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IReadOnlyList<WmaResult> GetWma<T>(
        this IEnumerable<T> results,
        int lookbackPeriods)
        where T : IReusable
        => results
            .ToSortedList()
            .CalcWma(lookbackPeriods);
}
