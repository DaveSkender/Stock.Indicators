namespace Skender.Stock.Indicators;

// RATE OF CHANGE (ROC) WITH BANDS (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IReadOnlyList<RocWbResult> ToRocWb<T>(
        this IReadOnlyList<T> results,
        int lookbackPeriods,
        int emaPeriods,
        int stdDevPeriods)
        where T : IReusable
        => results
            .ToSortedList()
            .CalcRocWb(lookbackPeriods, emaPeriods, stdDevPeriods);
}
