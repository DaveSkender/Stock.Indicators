namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // EXPONENTIAL MOVING AVERAGE (on CLOSE price)
    /// <include file='./info.xml' path='indicator/type[@name="Main"]/*' />
    ///
    public static IEnumerable<EmaResult> GetEma<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote
    {
        // convert quotes
        List<BasicD> bdList = quotes.ConvertToBasic(CandlePart.Close);

        // calculate
        return bdList.CalcEma(lookbackPeriods);
    }

    // EXPONENTIAL MOVING AVERAGE (on specified OHLCV part)
    /// <include file='./info.xml' path='indicator/type[@name="Custom"]/*' />
    ///
    public static IEnumerable<EmaResult> GetEma<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods,
        CandlePart candlePart)
        where TQuote : IQuote
    {
        // convert quotes
        List<BasicD> bdList = quotes.ConvertToBasic(candlePart);

        // calculate
        return bdList.CalcEma(lookbackPeriods);
    }

    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<EmaResult> RemoveWarmupPeriods(
        this IEnumerable<EmaResult> results)
    {
        int n = results
          .ToList()
          .FindIndex(x => x.Ema != null) + 1;

        return results.Remove(n + 100);
    }

    // standard calculation
    private static List<EmaResult> CalcEma(
        this List<BasicD> bdList, int lookbackPeriods)
    {
        // check parameter arguments
        ValidateEma(bdList, lookbackPeriods);

        // initialize
        int length = bdList.Count;
        List<EmaResult> results = new(length);

        double k = 2d / (lookbackPeriods + 1);
        double? lastEma = 0;
        int initPeriods = Math.Min(lookbackPeriods, length);

        for (int i = 0; i < initPeriods; i++)
        {
            lastEma += bdList[i].Value;
        }

        lastEma /= lookbackPeriods;

        // roll through quotes
        for (int i = 0; i < bdList.Count; i++)
        {
            BasicD h = bdList[i];
            int index = i + 1;

            EmaResult result = new()
            {
                Date = h.Date
            };
            results.Add(result);

            if (index > lookbackPeriods)
            {
                double? ema = lastEma + (k * (h.Value - lastEma));
                result.Ema = (decimal?)ema;
                lastEma = ema;
            }
            else if (index == lookbackPeriods)
            {
                result.Ema = (decimal?)lastEma;
            }
        }

        return results;
    }

    // parameter validation
    private static void ValidateEma(
        List<BasicD> quotes,
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for EMA.");
        }

        // check quotes
        int qtyHistory = quotes.Count;
        int minHistory = Math.Max(2 * lookbackPeriods, lookbackPeriods + 100);
        if (config.UseBadQuotesException && qtyHistory < minHistory)
        {
            string message = "Insufficient quotes provided for EMA.  " +
                string.Format(
                    EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.  "
                + "Since this uses a smoothing technique, for {2} lookback periods "
                + "we recommend you use at least {3} data points prior to the intended "
                + "usage date for better precision.",
                    qtyHistory, minHistory, lookbackPeriods, lookbackPeriods + 250);

            throw new BadQuotesException(nameof(quotes), message);
        }
    }
}
