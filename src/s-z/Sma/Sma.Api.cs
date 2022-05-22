namespace Skender.Stock.Indicators;

// SIMPLE MOVING AVERAGE (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='indicator/type[@name="Main"]/*' />
    ///
    public static IEnumerable<SmaResult> GetSma<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote
    {
        // initialize
        List<(DateTime, double)> tpList = quotes.ToBasicTuple();

        // calculate
        return tpList.CalcSma(lookbackPeriods);
    }

    // SERIES, from CHAIN
    public static IEnumerable<SmaResult> GetSma(
        this IEnumerable<IReusableResult> results,
        int lookbackPeriods)
    {
        // convert results
        List<(DateTime Date, double Value)> tpList
            = results.ToResultTuple();

        // calculate
        return tpList.CalcSma(lookbackPeriods);
    }

    // SERIES, from TUPLE
    public static IEnumerable<SmaResult> GetSma(
        this IEnumerable<(DateTime, double)> tpPrices,
        int lookbackPeriods)
    {
        // convert quotes
        List<(DateTime, double)> tpList
            = tpPrices.ToTupleList();

        // calculate
        return tpList.CalcSma(lookbackPeriods);
    }
}
