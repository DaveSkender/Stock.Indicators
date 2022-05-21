namespace Skender.Stock.Indicators;

// EXPONENTIAL MOVING AVERAGE (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/type[@name="standard"]/*' />
    ///
    public static IEnumerable<EmaResult> GetEma<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote
    {
        // convert quotes
        List<(DateTime, double)> tpList
            = quotes.ToBasicTuple(CandlePart.Close);

        // calculate
        return tpList.CalcEma(lookbackPeriods);
    }

    // SERIES, from CHAIN
    public static IEnumerable<EmaResult> GetEma(
        this IEnumerable<IReusableResult> results,
        int lookbackPeriods)
    {
        // convert results
        List<(DateTime Date, double Value)> tpList
            = results.ToResultTuple();

        // calculate
        return CalcEma(tpList, lookbackPeriods);
    }

    // SERIES, from TUPLE
    // TODO: preview, undocumented, for now
    public static IEnumerable<EmaResult> GetEma(
        this IEnumerable<(DateTime, double)> tpPrices,
        int lookbackPeriods)
    {
        // convert quotes
        List<(DateTime, double)> tpList
            = tpPrices.ToTupleList();

        // calculate
        return tpList.CalcEma(lookbackPeriods);
    }

    // STREAM INITIALIZATION, from TQuote
    /// <include file='./info.xml' path='info/type[@name="stream"]/*' />
    ///
    public static Ema InitEma<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote
    {
        // convert quotes
        List<(DateTime, double)> tpList
            = quotes.ToBasicTuple(CandlePart.Close);

        return new Ema(tpList, lookbackPeriods);
    }

    // STREAM INITIALIZATION, from CHAIN
    public static Ema InitEma(
        this IEnumerable<IReusableResult> results,
        int lookbackPeriods)
    {
        // convert results
        List<(DateTime, double)> tpList
            = results.ToResultTuple();

        return new Ema(tpList, lookbackPeriods);
    }
}
