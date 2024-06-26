namespace Skender.Stock.Indicators;

// RATE OF CHANGE (ROC) WITH BANDS (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IEnumerable<RocWbResult> GetRocWb<T>(
        this IEnumerable<T> results,
        int lookbackPeriods,
        int emaPeriods,
        int stdDevPeriods)
        where T : IReusableResult
        => results
            .ToSortedList()
            .CalcRocWb(lookbackPeriods, emaPeriods, stdDevPeriods);
}
