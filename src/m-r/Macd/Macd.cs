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
        List<(DateTime Date, double Value)> tpList = quotes.ToBasicTuple(candlePart);

        // check parameter arguments
        ValidateMacd(fastPeriods, slowPeriods, signalPeriods);

        // initialize
        List<EmaResult> emaFast = CalcEma(tpList, fastPeriods);
        List<EmaResult> emaSlow = CalcEma(tpList, slowPeriods);

        int length = tpList.Count;
        List<(DateTime, double)> emaDiff = new();
        List<MacdResult> results = new(length);

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            (DateTime date, double value) = tpList[i];
            EmaResult df = emaFast[i];
            EmaResult ds = emaSlow[i];

            MacdResult result = new()
            {
                Date = date,
                FastEma = (decimal?)df.Ema,
                SlowEma = (decimal?)ds.Ema
            };

            if (i >= slowPeriods - 1
                && df.Ema is not null
                && ds.Ema is not null)
            {
                double macd = (double)(df.Ema - ds.Ema);
                result.Macd = macd;

                // temp data for interim EMA of macd
                (DateTime, double) diff = (date, macd);

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

            r.Signal = (double?)ds.Ema;
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
