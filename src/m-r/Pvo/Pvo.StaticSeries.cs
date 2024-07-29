namespace Skender.Stock.Indicators;

// PRICE VOLUME OSCILLATOR (SERIES)

public static partial class Indicator
{
    private static List<PvoResult> CalcPvo<T>(
        this List<T> source,  // volume
        int fastPeriods,
        int slowPeriods,
        int signalPeriods)
        where T : IReusable
    {
        // check parameter arguments
        Pvo.Validate(fastPeriods, slowPeriods, signalPeriods);

        // initialize
        int length = source.Count;
        List<PvoResult> results = new(length);

        double lastEmaFast = double.NaN;
        double lastEmaSlow = double.NaN;
        double lastEmaPvo = double.NaN;

        double kFast = 2d / (fastPeriods + 1);
        double kSlow = 2d / (slowPeriods + 1);
        double kPvo = 2d / (signalPeriods + 1);

        // roll through source values
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
                emaFast = Ema.Increment(kFast, lastEmaFast, s.Value);
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
                emaSlow = Ema.Increment(kSlow, lastEmaSlow, s.Value);
            }

            double pvo = emaSlow != 0 ?
                100 * ((emaFast - emaSlow) / emaSlow) : double.NaN;

            // re-initialize Signal EMA
            double signal;

            if (double.IsNaN(lastEmaPvo) && i >= signalPeriods + slowPeriods - 2)
            {
                double sum = pvo;
                for (int p = i - signalPeriods + 1; p < i; p++)
                {
                    sum += ((IReusable)results[p]).Value;
                }

                signal = sum / signalPeriods;
            }
            else
            {
                signal = Ema.Increment(kPvo, lastEmaPvo, pvo);
            }

            // write results
            results.Add(new(
                Timestamp: s.Timestamp,
                Pvo: pvo.NaN2Null(),
                Signal: signal.NaN2Null(),
                Histogram: (pvo - signal).NaN2Null()));

            lastEmaPvo = signal;
            lastEmaFast = emaFast;
            lastEmaSlow = emaSlow;
        }

        return results;
    }

    /* DESIGN NOTE: this is exactly like MACD, except for:
     *   a) it uses Volume instead of Price (see API)
     *   b) the PVO calculation slightly different     */
}
