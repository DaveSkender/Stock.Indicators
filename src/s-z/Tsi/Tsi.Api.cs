namespace Skender.Stock.Indicators;

// TRUE STRENGTH INDEX (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<TsiResult> GetTsi<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 25,
        int smoothPeriods = 13,
        int signalPeriods = 7)
        where TQuote : IQuote => quotes
            .ToBasicTuple(CandlePart.Close)
            .CalcTsi(lookbackPeriods, smoothPeriods, signalPeriods);

    // SERIES, from CHAIN
    public static IEnumerable<TsiResult> GetTsi(
        this IEnumerable<IReusableResult> results,
        int lookbackPeriods = 25,
        int smoothPeriods = 13,
        int signalPeriods = 7) => results
            .ToResultTuple()
            .CalcTsi(lookbackPeriods, smoothPeriods, signalPeriods)
            .SyncIndex(results, SyncType.Prepend);

    // SERIES, from TUPLE
    public static IEnumerable<TsiResult> GetTsi(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods = 25,
        int smoothPeriods = 13,
        int signalPeriods = 7) => priceTuples
            .ToSortedList()
            .CalcTsi(lookbackPeriods, smoothPeriods, signalPeriods);
}
