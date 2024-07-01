namespace Skender.Stock.Indicators;

// RATE OF CHANGE (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IEnumerable<RocResult> GetRoc<T>(
        this IEnumerable<T> results,
        int lookbackPeriods)
        where T : IReusable
        => results
            .ToSortedList()
            .CalcRoc(lookbackPeriods);
}
