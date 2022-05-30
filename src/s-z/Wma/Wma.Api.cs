namespace Skender.Stock.Indicators;

// WEIGHTED MOVING AVERAGE (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<WmaResult> GetWma<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote => quotes
            .ToBasicTuple(CandlePart.Close)
            .CalcWma(lookbackPeriods);

    // SERIES, from CHAIN
    public static IEnumerable<WmaResult> GetWma(
        this IEnumerable<IReusableResult> results,
        int lookbackPeriods) => results
            .ToResultTuple()
            .CalcWma(lookbackPeriods);

    // SERIES, from TUPLE
    public static IEnumerable<WmaResult> GetWma(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods) => priceTuples
            .ToSortedList()
            .CalcWma(lookbackPeriods);
}
