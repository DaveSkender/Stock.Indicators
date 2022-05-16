namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // EXPONENTIAL MOVING AVERAGE (SERIES)
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

    public static IEnumerable<EmaResult> GetEma(
        this IEnumerable<(DateTime, double)> quotes,
        int lookbackPeriods)
    {
        // convert quotes
        List<(DateTime, double)> tpList
            = quotes.ToTupleList();

        // calculate
        return tpList.CalcEma(lookbackPeriods);
    }

    public static IEnumerable<EmaResult> GetEma(
        this IEnumerable<IReusableResult> basicData,
        int lookbackPeriods)
    {
        // convert results
        List<(DateTime Date, double Value)> tpList
            = basicData.ToResultTuple();

        // calculate
        return CalcEma(tpList, lookbackPeriods);
    }

    // standard calculation
    private static List<EmaResult> CalcEma(
        this List<(DateTime Date, double Value)> tpList,
        int lookbackPeriods)
    {
        // check parameter arguments
        ValidateEma(lookbackPeriods);

        // initialize
        int length = tpList.Count;
        List<EmaResult> results = new(length);

        double lastEma = 0;
        double k = 2d / (lookbackPeriods + 1);
        int initPeriods = Math.Min(lookbackPeriods, length);

        for (int i = 0; i < initPeriods; i++)
        {
            lastEma += tpList[i].Value;
        }

        lastEma /= lookbackPeriods;

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            (DateTime date, double value) = tpList[i];
            EmaResult r = new() { Date = date };

            if (i + 1 > lookbackPeriods)
            {
                double ema = Ema.Increment(value, lastEma, k);
                r.Ema = ema;
                lastEma = ema;
            }
            else if (i + 1 == lookbackPeriods)
            {
                r.Ema = lastEma;
            }

            results.Add(r);
        }

        return results;
    }

    // parameter validation
    private static void ValidateEma(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for EMA.");
        }
    }
}
