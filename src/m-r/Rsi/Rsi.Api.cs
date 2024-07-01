namespace Skender.Stock.Indicators;

// RELATIVE STRENGTH INDEX (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IEnumerable<RsiResult> GetRsi<T>(
        this IEnumerable<T> results,
        int lookbackPeriods = 14)
        where T : IReusable
        => results
            .ToSortedList()
            .CalcRsi(lookbackPeriods);
}
