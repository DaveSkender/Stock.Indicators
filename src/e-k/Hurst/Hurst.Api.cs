namespace Skender.Stock.Indicators;

// HURST EXPONENT (API)
public static partial class Indicator
{
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<HurstResult> GetHurst<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 100)
        where TQuote : IQuote => quotes
            .ToBasicTuple(CandlePart.Close)
            .CalcHurst(lookbackPeriods);

    // SERIES, from CHAIN
    public static IEnumerable<HurstResult> GetHurst(
        this IEnumerable<IReusableResult> results,
        int lookbackPeriods) => results
            .ToResultTuple()
            .CalcHurst(lookbackPeriods)
            .SyncIndex(results, SyncType.Prepend);

    // SERIES, from TUPLE
    public static IEnumerable<HurstResult> GetHurst(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods) => priceTuples
            .ToSortedList()
            .CalcHurst(lookbackPeriods);
}
