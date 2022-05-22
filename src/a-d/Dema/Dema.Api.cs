namespace Skender.Stock.Indicators;

// DOUBLE EXPONENTIAL MOVING AVERAGE - DEMA (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<DemaResult> GetDema<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote
    {
        // convert quotes
        List<(DateTime, double)> tpList
            = quotes.ToBasicTuple();

        // calculate
        return tpList.CalcDema(lookbackPeriods);
    }

    // SERIES, from CHAIN
    public static IEnumerable<DemaResult> GetDema(
        this IEnumerable<IReusableResult> results,
        int lookbackPeriods)
    {
        // convert results
        List<(DateTime Date, double Value)> tpList
            = results.ToResultTuple();

        // calculate
        return tpList.CalcDema(lookbackPeriods);
    }

    // SERIES, from TUPLE
    public static IEnumerable<DemaResult> GetDema(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods)
    {
        // convert prices
        List<(DateTime, double)> tpList
            = priceTuples.ToTupleList();

        // calculate
        return tpList.CalcDema(lookbackPeriods);
    }
}
