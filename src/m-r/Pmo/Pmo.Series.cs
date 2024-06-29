namespace Skender.Stock.Indicators;

// PRICE MOMENTUM OSCILLATOR (SERIES)

public static partial class Indicator
{
    internal static List<PmoResult> CalcPmo<T>(
        this List<T> source,
        int timePeriods,
        int smoothPeriods,
        int signalPeriods)
        where T : IReusable
    {
        // check parameter arguments
        Pmo.Validate(timePeriods, smoothPeriods, signalPeriods);

        // initialize
        int length = source.Count;
        List<PmoResult> results = new(length);
        double smoothingConstant1 = 2d / smoothPeriods;
        double smoothingConstant2 = 2d / timePeriods;
        double smoothingConstant3 = 2d / (signalPeriods + 1);

        double prevPrice = double.NaN;
        double prevPmo = double.NaN;
        double prevRocEma = double.NaN;
        double prevSignal = double.NaN;

        double[] rc = new double[length];  // roc
        double[] re = new double[length];  // roc ema
        double[] pm = new double[length];  // pmo

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            T s = source[i];

            // rate of change (ROC)
            rc[i] = prevPrice == 0 ? double.NaN : 100 * ((s.Value / prevPrice) - 1);
            prevPrice = s.Value;

            // ROC smoothed moving average
            double rocEma;

            if (double.IsNaN(prevRocEma) && i >= timePeriods)
            {
                double sum = 0;
                for (int p = i - timePeriods + 1; p <= i; p++)
                {
                    sum += rc[p];
                }
                rocEma = sum / timePeriods;
            }
            else
            {
                rocEma = prevRocEma + (smoothingConstant2 * (rc[i] - prevRocEma));
            }

            re[i] = rocEma * 10;
            prevRocEma = rocEma;

            // price momentum oscillator
            double pmo;

            if (double.IsNaN(prevPmo) && i >= smoothPeriods)
            {
                double sum = 0;
                for (int p = i - smoothPeriods + 1; p <= i; p++)
                {
                    sum += re[p];
                }
                pmo = sum / smoothPeriods;
            }
            else
            {
                pmo = prevPmo + (smoothingConstant1 * (re[i] - prevPmo));
            }

            prevPmo = pm[i] = pmo;

            // add signal (EMA of PMO)
            double signal;

            if (double.IsNaN(prevSignal) && i >= signalPeriods)
            {
                double sum = 0;
                for (int p = i - signalPeriods + 1; p <= i; p++)
                {
                    sum += pm[p];
                }

                signal = sum / signalPeriods;
            }
            else
            {
                signal = EmaUtilities.Increment(smoothingConstant3, prevSignal, pm[i]);
            }

            PmoResult r = new(
                Timestamp: s.Timestamp,
                Pmo: pmo.NaN2Null(),
                Signal: signal.NaN2Null());

            results.Add(r);

            prevSignal = signal;
        }

        return results;
    }
}
