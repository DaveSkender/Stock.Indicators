namespace Skender.Stock.Indicators;

// RATE OF CHANGE (ROC) WITH BANDS (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/type[@name="WithBands"]/*' />
    ///
    public static IEnumerable<RocWbResult> GetRocWb<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods,
        int emaPeriods,
        int stdDevPeriods)
        where TQuote : IQuote => quotes
            .ToBasicTuple(CandlePart.Close)
            .CalcRocWb(lookbackPeriods, emaPeriods, stdDevPeriods);

    // SERIES, from CHAIN
    public static IEnumerable<RocWbResult> GetRocWb(
        this IEnumerable<IReusableResult> results,
        int lookbackPeriods,
        int emaPeriods,
        int stdDevPeriods) => results
            .ToResultTuple()
            .CalcRocWb(lookbackPeriods, emaPeriods, stdDevPeriods)
            .SyncIndex(results, SyncType.Prepend);

    // SERIES, from TUPLE
    public static IEnumerable<RocWbResult> GetRocWb(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods,
        int emaPeriods,
        int stdDevPeriods) => priceTuples
            .ToSortedList()
            .CalcRocWb(lookbackPeriods, emaPeriods, stdDevPeriods);
}
