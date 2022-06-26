namespace Skender.Stock.Indicators;

// KLINGER VOLUME OSCILLATOR (SERIES)
public static partial class Indicator
{
    internal static List<KvoResult> CalcKvo(
        this List<QuoteD> qdList,
        int fastPeriods,
        int slowPeriods,
        int signalPeriods)
    {
        // check parameter arguments
        ValidateKlinger(fastPeriods, slowPeriods, signalPeriods);

        // initialize
        int length = qdList.Count;
        List<KvoResult> results = new(length);

        double[] t = new double[length];          // trend direction
        double[] hlc = new double[length];        // trend basis
        double[] dm = new double[length];         // daily measurement
        double[] cm = new double[length];         // cumulative measurement
        double[] vf = new double[length];         // volume force (VF)
        double[] vfFastEma = new double[length];  // EMA of VF (short-term)
        double[] vfSlowEma = new double[length];  // EMA of VP (long-term)

        // EMA multipliers
        double kFast = 2d / (fastPeriods + 1);
        double kSlow = 2d / (slowPeriods + 1);
        double kSignal = 2d / (signalPeriods + 1);

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            QuoteD q = qdList[i];

            KvoResult r = new(q.Date);
            results.Add(r);

            // trend basis comparator
            hlc[i] = q.High + q.Low + q.Close;

            // daily measurement
            dm[i] = q.High - q.Low;

            if (i <= 0)
            {
                continue;
            }

            // trend direction
            t[i] = (hlc[i] > hlc[i - 1]) ? 1 : -1;

            if (i <= 1)
            {
                cm[i] = 0;
                continue;
            }

            // cumulative measurement
            cm[i] = (t[i] == t[i - 1]) ?
                    (cm[i - 1] + dm[i]) : (dm[i - 1] + dm[i]);

            // volume force (VF)
            vf[i] = (dm[i] == cm[i] || q.Volume == 0) ? 0
                : (dm[i] == 0) ? q.Volume * 2d * t[i] * 100d
                : (cm[i] != 0) ? q.Volume * Math.Abs(2d * ((dm[i] / cm[i]) - 1)) * t[i] * 100d
                : vf[i - 1];

            // fast-period EMA of VF
            if (i > fastPeriods + 1)
            {
                vfFastEma[i] = (vf[i] * kFast) + (vfFastEma[i - 1] * (1 - kFast));
            }
            else if (i == fastPeriods + 1)
            {
                double sum = 0;
                for (int p = 2; p <= i; p++)
                {
                    sum += vf[p];
                }

                vfFastEma[i] = sum / fastPeriods;
            }

            // slow-period EMA of VF
            if (i > slowPeriods + 1)
            {
                vfSlowEma[i] = (vf[i] * kSlow) + (vfSlowEma[i - 1] * (1 - kSlow));
            }
            else if (i == slowPeriods + 1)
            {
                double sum = 0;
                for (int p = 2; p <= i; p++)
                {
                    sum += vf[p];
                }

                vfSlowEma[i] = sum / slowPeriods;
            }

            // Klinger Oscillator
            if (i >= slowPeriods + 1)
            {
                r.Oscillator = vfFastEma[i] - vfSlowEma[i];

                // Signal
                if (i > slowPeriods + signalPeriods)
                {
                    r.Signal = (r.Oscillator * kSignal)
                        + (results[i - 1].Signal * (1 - kSignal));
                }
                else if (i == slowPeriods + signalPeriods)
                {
                    double? sum = 0;
                    for (int p = slowPeriods + 1; p <= i; p++)
                    {
                        sum += results[p].Oscillator;
                    }

                    r.Signal = sum / signalPeriods;
                }
            }
        }

        return results;
    }

    // parameter validation
    private static void ValidateKlinger(
        int fastPeriods,
        int slowPeriods,
        int signalPeriods)
    {
        // check parameter arguments
        if (fastPeriods <= 2)
        {
            throw new ArgumentOutOfRangeException(nameof(fastPeriods), fastPeriods,
                "Fast (short) Periods must be greater than 2 for Klinger Oscillator.");
        }

        if (slowPeriods <= fastPeriods)
        {
            throw new ArgumentOutOfRangeException(nameof(slowPeriods), slowPeriods,
                "Slow (long) Periods must be greater than Fast Periods for Klinger Oscillator.");
        }

        if (signalPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(signalPeriods), signalPeriods,
                "Signal Periods must be greater than 0 for Klinger Oscillator.");
        }
    }
}
