namespace Skender.Stock.Indicators;

// SMOOTHED MOVING AVERAGE (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IReadOnlyList<SmmaResult> GetSmma<T>(
        this IEnumerable<T> results,
        int lookbackPeriods)
        where T : IReusable
        => results
            .ToSortedList()
            .CalcSmma(lookbackPeriods);
}
