namespace Skender.Stock.Indicators;

// HULL MOVING AVERAGE (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<HmaResult> GetHma<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote => quotes
            .ToBasicTuple(CandlePart.Close)
            .CalcHma(lookbackPeriods);

    // SERIES, from CHAIN
    public static IEnumerable<HmaResult> GetHma(
        this IEnumerable<IReusableResult> results,
        int lookbackPeriods) => results
            .ToResultTuple()
            .CalcHma(lookbackPeriods)
            .SyncIndex(results, SyncType.Prepend);

    // SERIES, from TUPLE
    public static IEnumerable<HmaResult> GetHma(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods) => priceTuples
            .ToSortedList()
            .CalcHma(lookbackPeriods);
}