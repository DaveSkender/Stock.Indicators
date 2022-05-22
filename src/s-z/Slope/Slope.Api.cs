namespace Skender.Stock.Indicators;

// SLOPE AND LINEAR REGRESSION (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<SlopeResult> GetSlope<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote => quotes
            .ToBasicTuple()
            .CalcSlope(lookbackPeriods);

    // SERIES, from CHAIN
    public static IEnumerable<SlopeResult> GetSlope(
        this IEnumerable<IReusableResult> results,
        int lookbackPeriods) => results
            .ToResultTuple()
            .CalcSlope(lookbackPeriods);

    // SERIES, from TUPLE
    public static IEnumerable<SlopeResult> GetSlope(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods) => priceTuples
            .ToTupleList()
            .CalcSlope(lookbackPeriods);
}
