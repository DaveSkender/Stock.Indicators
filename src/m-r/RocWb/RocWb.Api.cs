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
            .ToTupleResult()
            .CalcRocWb(lookbackPeriods, emaPeriods, stdDevPeriods);

    // SERIES, from TUPLE
    public static IEnumerable<RocWbResult> GetRocWb(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods,
        int emaPeriods,
        int stdDevPeriods) => priceTuples
            .ToSortedList()
            .CalcRocWb(lookbackPeriods, emaPeriods, stdDevPeriods);
}
