namespace Skender.Stock.Indicators;

// STANDARD DEVIATION (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<StdDevResult> GetStdDev<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods,
        int? smaPeriods = null)
        where TQuote : IQuote => quotes
            .ToBasicTuple(CandlePart.Close)
            .CalcStdDev(lookbackPeriods, smaPeriods);

    // SERIES, from CHAIN
    public static IEnumerable<StdDevResult> GetStdDev(
        this IEnumerable<IReusableResult> results,
        int lookbackPeriods,
        int? smaPeriods = null) => results
            .ToResultTuple()
            .CalcStdDev(lookbackPeriods, smaPeriods)
            .SyncIndex(results, SyncType.Prepend);

    // SERIES, from TUPLE
    public static IEnumerable<StdDevResult> GetStdDev(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods,
        int? smaPeriods = null) => priceTuples
            .ToSortedList()
            .CalcStdDev(lookbackPeriods, smaPeriods);
}
