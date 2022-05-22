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
        where TQuote : IQuote
    {
        // convert quotes
        List<(DateTime, double)> tpList = quotes.ToBasicTuple();

        // calculate
        return tpList.CalcWma(lookbackPeriods);
    }

    // SERIES, from CHAIN
    public static IEnumerable<WmaResult> GetWma(
        this IEnumerable<IReusableResult> results,
        int lookbackPeriods)
    {
        // convert results
        List<(DateTime, double)> tpList
            = results.ToResultTuple();

        // calculate
        return tpList.CalcWma(lookbackPeriods);
    }

    // SERIES, from TUPLE
    public static IEnumerable<WmaResult> GetWma(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods)
    {
        // convert prices
        List<(DateTime, double)> tpList
            = priceTuples.ToTupleList();

        // calculate
        return tpList.CalcWma(lookbackPeriods);
    }
}
