namespace Skender.Stock.Indicators;

// SLOPE AND LINEAR REGRESSION (API)
public static partial class Indicator
{
    //// SERIES, from TQuote
    ///// <include file='./info.xml' path='info/*' />
    /////
    //public static IEnumerable<SlopeResult> GetSlope<TQuote>(
    //    this IEnumerable<TQuote> quotes,
    //    int lookbackPeriods)
    //    where TQuote : IQuote => quotes
    //        .ToTuple(CandlePart.Close)
    //        .CalcSlope(lookbackPeriods);

    // SERIES, from CHAIN
    public static IEnumerable<SlopeResult> GetSlope<T>(
        this IEnumerable<T> results,
        int lookbackPeriods)
        where T : IReusableResult
        => results
            .ToTupleResult()
            .CalcSlope(lookbackPeriods);

    // SERIES, from TUPLE
    public static IEnumerable<SlopeResult> GetSlope(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods) => priceTuples
            .ToSortedList()
            .CalcSlope(lookbackPeriods);
}
