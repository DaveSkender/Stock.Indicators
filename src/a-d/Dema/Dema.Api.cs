namespace Skender.Stock.Indicators;

// DOUBLE EXPONENTIAL MOVING AVERAGE - DEMA (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IReadOnlyList<DemaResult> GetDema<T>(
        this IReadOnlyList<T> results,
        int lookbackPeriods)
        where T : IReusable
        => results
            .ToSortedList()
            .CalcDema(lookbackPeriods);
}
