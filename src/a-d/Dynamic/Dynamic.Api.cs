namespace Skender.Stock.Indicators;

// McGINLEY DYNAMIC
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<DynamicResult> GetDynamic<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods,
        double kFactor = 0.6)
        where TQuote : IQuote => quotes
            .ToTuple(CandlePart.Close)
            .CalcDynamic(lookbackPeriods, kFactor);

    // SERIES, from CHAIN
    public static IEnumerable<DynamicResult> GetDynamic(
        this IEnumerable<IReusableResult> results,
        int lookbackPeriods,
        double kFactor = 0.6) => results
            .ToTuple()
            .CalcDynamic(lookbackPeriods, kFactor)
            .SyncIndex(results, SyncType.Prepend);

    // SERIES, from TUPLE
    public static IEnumerable<DynamicResult> GetDynamic(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods,
        double kFactor = 0.6) => priceTuples
            .ToSortedList()
            .CalcDynamic(lookbackPeriods, kFactor);
}
