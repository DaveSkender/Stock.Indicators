namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // CHAIKIN OSCILLATOR
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<ChaikinOscResult> GetChaikinOsc<TQuote>(
        this IEnumerable<TQuote> quotes,
        int fastPeriods = 3,
        int slowPeriods = 10)
        where TQuote : IQuote
    {

        // check parameter arguments
        ValidateChaikinOsc(quotes, fastPeriods, slowPeriods);

        // money flow
        List<ChaikinOscResult> results = GetAdl(quotes)
            .Select(r => new ChaikinOscResult
            {
                Date = r.Date,
                MoneyFlowMultiplier = r.MoneyFlowMultiplier,
                MoneyFlowVolume = r.MoneyFlowVolume,
                Adl = r.Adl
            })
            .ToList();

        // EMA of ADL
        List<BasicD> bdAdl = results
            .Select(x => new BasicD { Date = x.Date, Value = x.Adl })
            .ToList();

        List<EmaResult> adlEmaSlow = CalcEma(bdAdl, slowPeriods);
        List<EmaResult> adlEmaFast = CalcEma(bdAdl, fastPeriods);

        // add Oscillator
        for (int i = slowPeriods - 1; i < results.Count; i++)
        {
            ChaikinOscResult r = results[i];

            EmaResult f = adlEmaFast[i];
            EmaResult s = adlEmaSlow[i];

            r.Oscillator = (double)(f.Ema - s.Ema);
        }

        return results;
    }


    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<ChaikinOscResult> RemoveWarmupPeriods(
        this IEnumerable<ChaikinOscResult> results)
    {
        int s = results
            .ToList()
            .FindIndex(x => x.Oscillator != null) + 1;

        return results.Remove(s + 100);
    }


    // parameter validation
    private static void ValidateChaikinOsc<TQuote>(
        IEnumerable<TQuote> quotes,
        int fastPeriods,
        int slowPeriods)
        where TQuote : IQuote
    {

        // check parameter arguments
        if (fastPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(fastPeriods), fastPeriods,
                "Fast lookback periods must be greater than 0 for Chaikin Oscillator.");
        }

        if (slowPeriods <= fastPeriods)
        {
            throw new ArgumentOutOfRangeException(nameof(slowPeriods), slowPeriods,
                "Slow lookback periods must be greater than Fast lookback period for Chaikin Oscillator.");
        }

        // check quotes
        int qtyHistory = quotes.Count();
        int minHistory = Math.Max(2 * slowPeriods, slowPeriods + 100);
        if (qtyHistory < minHistory)
        {
            string message = "Insufficient quotes provided for Chaikin Oscillator.  " +
                string.Format(EnglishCulture,
                "You provided {0} periods of quotes when at least {1} are required.  "
                + "Since this uses a smoothing technique, for a slow period of {2}, "
                + "we recommend you use at least {3} data points prior to the intended "
                + "usage date for better precision.",
                qtyHistory, minHistory, slowPeriods, slowPeriods + 250);

            throw new BadQuotesException(nameof(quotes), message);
        }
    }
}
