namespace Skender.Stock.Indicators;

// MOVING AVERAGE CONVERGENCE/DIVERGENCE (MACD) OSCILLATOR (SERIES)

public static partial class Indicator
{
    private static List<MacdResult> CalcMacd<T>(
        this List<T> source,
        int fastPeriods,
        int slowPeriods,
        int signalPeriods)
        where T : IReusable
    {
        // check parameter arguments
        Macd.Validate(fastPeriods, slowPeriods, signalPeriods);

        // initialize
        int length = source.Count;
        List<MacdResult> results = new(length);

        double lastEmaFast = double.NaN;
        double lastEmaSlow = double.NaN;
        double lastEmaMacd = double.NaN;

        double kFast = 2d / (fastPeriods + 1);
        double kSlow = 2d / (slowPeriods + 1);
        double kMacd = 2d / (signalPeriods + 1);

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            T s = source[i];

            // re-initialize Fast EMA
            double emaFast;

            if (double.IsNaN(lastEmaFast) && i >= fastPeriods - 1)
            {
                double sum = 0;
                for (int p = i - fastPeriods + 1; p <= i; p++)
                {
                    T ps = source[p];
                    sum += ps.Value;
                }

                emaFast = sum / fastPeriods;
            }
            else
            {
                emaFast = EmaUtilities
                    .Increment(kFast, lastEmaFast, s.Value);
            }

            // re-initialize Slow EMA
            double emaSlow;

            if (double.IsNaN(lastEmaSlow) && i >= slowPeriods - 1)
            {
                double sum = 0;
                for (int p = i - slowPeriods + 1; p <= i; p++)
                {
                    T ps = source[p];
                    sum += ps.Value;
                }

                emaSlow = sum / slowPeriods;
            }
            else
            {
                emaSlow = EmaUtilities
                    .Increment(kSlow, lastEmaSlow, s.Value);
            }

            double macd = emaFast - emaSlow;

            // re-initialize Signal EMA
            double signal;

            if (double.IsNaN(lastEmaMacd) && i >= signalPeriods + slowPeriods - 2)
            {
                double sum = macd;
                for (int p = i - signalPeriods + 1; p < i; p++)
                {
                    sum += ((IReusable)results[p]).Value;
                }

                signal = sum / signalPeriods;
            }
            else
            {
                signal = EmaUtilities
                    .Increment(kMacd, lastEmaMacd, macd);
            }

            // write results
            results.Add(new(
                Timestamp: s.Timestamp,
                Macd: macd.NaN2Null(),
                Signal: signal.NaN2Null(),
                Histogram: (macd - signal).NaN2Null(),
                FastEma: emaFast.NaN2Null(),
                SlowEma: emaSlow.NaN2Null()));

            lastEmaMacd = signal;
            lastEmaFast = emaFast;
            lastEmaSlow = emaSlow;
        }

        return results;
    }
}
