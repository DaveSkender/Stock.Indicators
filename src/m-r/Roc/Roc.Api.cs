namespace Skender.Stock.Indicators;

// RATE OF CHANGE (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/type[@name="Main"]/*' />
    ///
    public static IEnumerable<RocResult> GetRoc<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods,
        int? smaPeriods = null)
        where TQuote : IQuote => quotes
            .ToBasicTuple(CandlePart.Close)
            .CalcRoc(lookbackPeriods, smaPeriods);

    // SERIES, from CHAIN
    public static IEnumerable<RocResult> GetRoc(
        this IEnumerable<IReusableResult> results,
        int lookbackPeriods,
        int? smaPeriods = null) => results
            .ToResultTuple()
            .CalcRoc(lookbackPeriods, smaPeriods)
            .SyncIndex(results, SyncType.Prepend);

    // SERIES, from TUPLE
    public static IEnumerable<RocResult> GetRoc(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods,
        int? smaPeriods = null) => priceTuples
            .ToSortedList()
            .CalcRoc(lookbackPeriods, smaPeriods);
}
