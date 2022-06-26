namespace Skender.Stock.Indicators;

// ARNAUD LEGOUX MOVING AVERAGE (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<AlmaResult> GetAlma<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 9,
        double offset = 0.85,
        double sigma = 6)
        where TQuote : IQuote => quotes
            .ToBasicTuple(CandlePart.Close)
            .CalcAlma(lookbackPeriods, offset, sigma);

    // SERIES, from CHAIN
    public static IEnumerable<AlmaResult> GetAlma(
        this IEnumerable<IReusableResult> results,
        int lookbackPeriods = 9,
        double offset = 0.85,
        double sigma = 6) => results
            .ToResultTuple()
            .CalcAlma(lookbackPeriods, offset, sigma)
            .SyncIndex(results, SyncType.Prepend);

    // SERIES, from TUPLE
    public static IEnumerable<AlmaResult> GetAlma(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods = 9,
        double offset = 0.85,
        double sigma = 6) => priceTuples
            .ToSortedList()
            .CalcAlma(lookbackPeriods, offset, sigma);
}
