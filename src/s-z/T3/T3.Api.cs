namespace Skender.Stock.Indicators;

// TILLSON T3 MOVING AVERAGE (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<T3Result> GetT3<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 5,
        double volumeFactor = 0.7)
        where TQuote : IQuote => quotes
            .ToBasicTuple(CandlePart.Close)
            .CalcT3(lookbackPeriods, volumeFactor);

    // SERIES, from CHAIN
    public static IEnumerable<T3Result> GetT3(
        this IEnumerable<IReusableResult> results,
        int lookbackPeriods = 5,
        double volumeFactor = 0.7) => results
            .ToResultTuple()
            .CalcT3(lookbackPeriods, volumeFactor)
            .SyncIndex(results, SyncType.Prepend);

    // SERIES, from TUPLE
    public static IEnumerable<T3Result> GetT3(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods = 5,
        double volumeFactor = 0.7) => priceTuples
            .ToSortedList()
            .CalcT3(lookbackPeriods, volumeFactor);
}
