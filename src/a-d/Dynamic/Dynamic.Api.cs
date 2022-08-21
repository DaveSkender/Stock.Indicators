namespace Skender.Stock.Indicators;

// McGINLEY DYNAMIC
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<DynamicResult> GetDynamic<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote => quotes
            .ToBasicTuple(CandlePart.Close)
            .CalcDynamic(lookbackPeriods);

    // SERIES, from CHAIN
    public static IEnumerable<DynamicResult> GetDynamic(
        this IEnumerable<IReusableResult> results,
        int lookbackPeriods) => results
            .ToResultTuple()
            .CalcDynamic(lookbackPeriods)
            .SyncIndex(results, SyncType.Prepend);

    // SERIES, from TUPLE
    public static IEnumerable<DynamicResult> GetDynamic(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods) => priceTuples
            .ToSortedList()
            .CalcDynamic(lookbackPeriods);
}
