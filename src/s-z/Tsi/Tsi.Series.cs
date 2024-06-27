namespace Skender.Stock.Indicators;

// TRUE STRENGTH INDEX (SERIES)

public static partial class Indicator
{
    internal static List<TsiResult> CalcTsi<T>(
        this List<T> source,
        int lookbackPeriods,
        int smoothPeriods,
        int signalPeriods)
        where T : IReusable
    {
        // check parameter arguments
        Tsi.Validate(lookbackPeriods, smoothPeriods, signalPeriods);

        // initialize
        int length = source.Count;
        double mult1 = 2d / (lookbackPeriods + 1);
        double mult2 = 2d / (smoothPeriods + 1);
        double multS = 2d / (signalPeriods + 1);
        List<TsiResult> results = new(length);

        double[] c = new double[length]; // price change
        double[] cs1 = new double[length]; // smooth 1
        double[] cs2 = new double[length]; // smooth 2

        double[] a = new double[length]; // abs of price change
        double[] as1 = new double[length]; // smooth 1
        double[] as2 = new double[length]; // smooth 2

        double prevSignal = double.NaN;

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            var s = source[i];

            // initialize
            TsiResult r = new() { Timestamp = s.Timestamp };
            results.Add(r);

            // skip first period
            if (i == 0)
            {
                a[i] = double.NaN;
                c[i] = double.NaN;
                cs1[i] = double.NaN;
                as1[i] = double.NaN;
                cs2[i] = double.NaN;
                as2[i] = double.NaN;
                continue;
            }

            // price change
            c[i] = s.Value - source[i - 1].Value;
            a[i] = Math.Abs(c[i]);

            // re/initialize first smoothing
            if (double.IsNaN(cs1[i - 1]) && i >= lookbackPeriods)
            {
                double sumC = 0;
                double sumA = 0;

                for (int p = i - lookbackPeriods + 1; p <= i; p++)
                {
                    sumC += c[p];
                    sumA += a[p];
                }

                cs1[i] = sumC / lookbackPeriods;
                as1[i] = sumA / lookbackPeriods;
            }

            // normal first smoothing
            else
            {
                cs1[i] = ((c[i] - cs1[i - 1]) * mult1) + cs1[i - 1];
                as1[i] = ((a[i] - as1[i - 1]) * mult1) + as1[i - 1];
            }

            // re/initialize second smoothing
            if (double.IsNaN(cs2[i - 1]) && i >= smoothPeriods)
            {
                double sumCS = 0;
                double sumAS = 0;

                for (int p = i - smoothPeriods + 1; p <= i; p++)
                {
                    sumCS += cs1[p];
                    sumAS += as1[p];
                }

                cs2[i] = sumCS / smoothPeriods;
                as2[i] = sumAS / smoothPeriods;
            }

            // normal second smoothing
            else
            {
                cs2[i] = ((cs1[i] - cs2[i - 1]) * mult2) + cs2[i - 1];
                as2[i] = ((as1[i] - as2[i - 1]) * mult2) + as2[i - 1];
            }

            // true strength index
            double tsi = (as2[i] != 0) ? 100d * (cs2[i] / as2[i]) : double.NaN;
            r.Tsi = tsi.NaN2Null();

            // signal line
            if (signalPeriods > 1)
            {
                double signal;

                // re/initialize signal
                if (double.IsNaN(prevSignal) && i > signalPeriods)
                {
                    double sum = 0;
                    for (int p = i - signalPeriods + 1; p <= i; p++)
                    {
                        sum += results[p].Tsi.Null2NaN();
                    }

                    signal = sum / signalPeriods;
                }

                // normal signal
                else
                {
                    signal = ((tsi - prevSignal) * multS) + prevSignal;
                }

                r.Signal = signal.NaN2Null();
                prevSignal = signal;
            }
            else if (signalPeriods == 1)
            {
                r.Signal = r.Tsi;
            }
        }

        return results;
    }
}
