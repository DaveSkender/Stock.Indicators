namespace Skender.Stock.Indicators;

// DETRENDED PRICE OSCILLATOR (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<DpoResult> GetDpo<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote => quotes
            .ToBasicTuple()
            .CalcDpo(lookbackPeriods);

    // SERIES, from CHAIN
    public static IEnumerable<DpoResult> GetDpo(
        this IEnumerable<IReusableResult> results,
        int lookbackPeriods) => results
            .ToResultTuple()
            .CalcDpo(lookbackPeriods);

    // SERIES, from TUPLE
    public static IEnumerable<DpoResult> GetDpo(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods) => priceTuples
            .ToTupleList()
            .CalcDpo(lookbackPeriods);
}