namespace Skender.Stock.Indicators;

// SMOOTHED MOVING AVERAGE (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='indicator/type[@name="Main"]/*' />
    ///
    public static IEnumerable<SmmaResult> GetSmma<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote => quotes
            .ToBasicTuple()
            .CalcSmma(lookbackPeriods);

    // SERIES, from CHAIN
    public static IEnumerable<SmmaResult> GetSmma(
        this IEnumerable<IReusableResult> results,
        int lookbackPeriods) => results
            .ToResultTuple()
            .CalcSmma(lookbackPeriods);

    // SERIES, from TUPLE
    public static IEnumerable<SmmaResult> GetSmma(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods) => priceTuples
            .ToTupleList()
            .CalcSmma(lookbackPeriods);
}
