namespace Skender.Stock.Indicators;

// TRIPLE EXPONENTIAL MOVING AVERAGE - TEMA (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<TemaResult> GetTema<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote => quotes
            .ToBasicTuple(CandlePart.Close)
            .CalcTema(lookbackPeriods);

    // SERIES, from CHAIN
    public static IEnumerable<TemaResult> GetTema(
        this IEnumerable<IReusableResult> results,
        int lookbackPeriods) => results
            .ToResultTuple()
            .CalcTema(lookbackPeriods)
            .SyncIndex(results, SyncType.Prepend);

    // SERIES, from TUPLE
    public static IEnumerable<TemaResult> GetTema(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods) => priceTuples
            .ToSortedList()
            .CalcTema(lookbackPeriods);
}