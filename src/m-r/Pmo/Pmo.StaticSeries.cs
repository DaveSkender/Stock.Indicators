namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Price Momentum Oscillator (PMO) series.
/// </summary>
public static partial class Pmo
{
    /// <summary>
    /// Converts a list of source values to a list of PMO results.
    /// </summary>
    /// <typeparam name="T">The type of the source values.</typeparam>
    /// <param name="source">The list of source values.</param>
    /// <param name="timePeriods">The number of periods for the time span.</param>
    /// <param name="smoothPeriods">The number of periods for smoothing.</param>
    /// <param name="signalPeriods">The number of periods for the signal line.</param>
    /// <returns>A list of PMO results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the source list is null.</exception>
    [Series("PMO", "Price Momentum Oscillator", Category.Oscillator, ChartType.Oscillator)]
    public static IReadOnlyList<PmoResult> ToPmo<T>(
        this IReadOnlyList<T> source,
        [ParamNum<int>("Time Periods", 1, 250, 35)]
        int timePeriods = 35,
        [ParamNum<int>("Smooth Periods", 1, 250, 20)]
        int smoothPeriods = 20,
        [ParamNum<int>("Signal Periods", 1, 50, 10)]
        int signalPeriods = 10)
        where T : IReusable
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(source);
        Validate(timePeriods, smoothPeriods, signalPeriods);

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

        // roll through source values
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
                signal = Ema.Increment(smoothingConstant3, prevSignal, pm[i]);
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
