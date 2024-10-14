namespace Skender.Stock.Indicators;

// STANDARD DEVIATION (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IReadOnlyList<StdDevResult> GetStdDev<T>(
        this IReadOnlyList<T> results,
        int lookbackPeriods)
        where T : IReusable
        => results
            .ToSortedList()
            .CalcStdDev(lookbackPeriods);
}
