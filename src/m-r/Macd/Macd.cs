namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // MOVING AVERAGE CONVERGENCE/DIVERGENCE (MACD) OSCILLATOR
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<MacdResult> GetMacd<TQuote>(
        this IEnumerable<TQuote> quotes,
        int fastPeriods = 12,
        int slowPeriods = 26,
        int signalPeriods = 9,
        CandlePart candlePart = CandlePart.Close)
        where TQuote : IQuote
    {
        // convert quotes
        List<BasicData> bdList = quotes.ToBasicData(candlePart);

        // check parameter arguments
        ValidateMacd(fastPeriods, slowPeriods, signalPeriods);

        // initialize
        List<EmaResult> emaFast = CalcEma(bdList, fastPeriods);
        List<EmaResult> emaSlow = CalcEma(bdList, slowPeriods);

        int length = bdList.Count;
        List<BasicData> emaDiff = new();
        List<MacdResult> results = new(length);

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            BasicData h = bdList[i];
            EmaResult df = emaFast[i];
            EmaResult ds = emaSlow[i];

            MacdResult result = new()
            {
                Date = h.Date,
                FastEma = df.Ema,
                SlowEma = ds.Ema
            };

            if (df.Ema != null && ds.Ema != null)
            {
                double macd = (double)(df.Ema - ds.Ema);
                result.Macd = (decimal)macd;

                // temp data for interim EMA of macd
                BasicData diff = new()
                {
                    Date = h.Date,
                    Value = macd
                };

                emaDiff.Add(diff);
            }

            results.Add(result);
        }

        // add signal and histogram to result
        List<EmaResult> emaSignal = CalcEma(emaDiff, signalPeriods);

        for (int d = slowPeriods - 1; d < length; d++)
        {
            MacdResult r = results[d];
            EmaResult ds = emaSignal[d + 1 - slowPeriods];

            r.Signal = ds.Ema;
            r.Histogram = r.Macd - r.Signal;
        }

        return results;
    }

    // parameter validation
    private static void ValidateMacd(
        int fastPeriods,
        int slowPeriods,
        int signalPeriods)
    {
        // check parameter arguments
        if (fastPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(fastPeriods), fastPeriods,
                "Fast periods must be greater than 0 for MACD.");
        }

        if (signalPeriods < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(signalPeriods), signalPeriods,
                "Signal periods must be greater than or equal to 0 for MACD.");
        }

        if (slowPeriods <= fastPeriods)
        {
            throw new ArgumentOutOfRangeException(nameof(slowPeriods), slowPeriods,
                "Slow periods must be greater than the fast period for MACD.");
        }
    }
}
