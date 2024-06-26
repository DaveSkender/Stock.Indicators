namespace Skender.Stock.Indicators;

// STANDARD DEVIATION (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IEnumerable<StdDevResult> GetStdDev<T>(
        this IEnumerable<T> results,
        int lookbackPeriods)
        where T : IReusableResult
        => results
            .ToSortedList()
            .CalcStdDev(lookbackPeriods);
}
