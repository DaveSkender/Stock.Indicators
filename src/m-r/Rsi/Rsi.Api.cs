namespace Skender.Stock.Indicators;

// RELATIVE STRENGTH INDEX (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IReadOnlyList<RsiResult> GetRsi<T>(
        this IReadOnlyList<T> results,
        int lookbackPeriods = 14)
        where T : IReusable
        => results
            .ToSortedList()
            .CalcRsi(lookbackPeriods);
}
