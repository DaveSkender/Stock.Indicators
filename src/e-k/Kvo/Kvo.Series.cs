namespace Skender.Stock.Indicators;

// KLINGER VOLUME OSCILLATOR (SERIES)

public static partial class Indicator
{
    private static List<KvoResult> CalcKvo(
        this List<QuoteD> qdList,
        int fastPeriods,
        int slowPeriods,
        int signalPeriods)
    {
        // check parameter arguments
        Kvo.Validate(fastPeriods, slowPeriods, signalPeriods);

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

            double? kvo = null;
            double? sig = null;

            // trend basis comparator
            hlc[i] = q.High + q.Low + q.Close;

            // daily measurement
            dm[i] = q.High - q.Low;

            if (i <= 0)
            {
                results.Add(new() { Timestamp = q.Timestamp });
                continue;
            }

            // trend direction
            t[i] = hlc[i] > hlc[i - 1] ? 1 : -1;

            if (i <= 1)
            {
                cm[i] = 0;
                results.Add(new() { Timestamp = q.Timestamp });
                continue;
            }

            // cumulative measurement
            cm[i] = t[i] == t[i - 1] ?
                    cm[i - 1] + dm[i] : dm[i - 1] + dm[i];

            // volume force (VF)
            vf[i] = dm[i] == cm[i] || q.Volume == 0 ? 0
                : dm[i] == 0 ? q.Volume * 2d * t[i] * 100d
                : cm[i] != 0 ? q.Volume * Math.Abs(2d * (dm[i] / cm[i] - 1)) * t[i] * 100d
                : vf[i - 1];

            // fast-period EMA of VF
            if (i > fastPeriods + 1)
            {
                vfFastEma[i] = vf[i] * kFast + vfFastEma[i - 1] * (1 - kFast);
            }

            // TODO: update healing, without requiring specific indexing
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
                vfSlowEma[i] = vf[i] * kSlow + vfSlowEma[i - 1] * (1 - kSlow);
            }

            // TODO: update healing, without requiring specific indexing
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
                kvo = vfFastEma[i] - vfSlowEma[i];

                // Signal
                if (i > slowPeriods + signalPeriods)
                {
                    sig = kvo * kSignal
                        + results[i - 1].Signal * (1 - kSignal);
                }

                // TODO: update healing, without requiring specific indexing
                else if (i == slowPeriods + signalPeriods)
                {
                    double? sum = kvo;
                    for (int p = slowPeriods + 1; p < i; p++)
                    {
                        sum += results[p].Oscillator;
                    }

                    sig = sum / signalPeriods;
                }
            }

            results.Add(new(
                Timestamp: q.Timestamp,
                Oscillator: kvo,
                Signal: sig));
        }

        return results;
    }
}
