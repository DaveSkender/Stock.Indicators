namespace Skender.Stock.Indicators;

// HULL MOVING AVERAGE (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IReadOnlyList<HmaResult> GetHma<T>(
        this IReadOnlyList<T> results,
        int lookbackPeriods)
        where T : IReusable
        => results
            .ToSortedList()
            .CalcHma(lookbackPeriods);
}
