namespace Skender.Stock.Indicators;

// HULL MOVING AVERAGE (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IEnumerable<HmaResult> GetHma<T>(
        this IEnumerable<T> results,
        int lookbackPeriods)
        where T : IReusableResult
        => results
            .ToSortedList()
            .CalcHma(lookbackPeriods);
}
