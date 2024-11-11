namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the True Strength Index (TSI) indicator.
/// </summary>
public static partial class Tsi
{
    /// <summary>
    /// Calculates the True Strength Index (TSI) for a given source of data.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source list, which must implement IReusable.</typeparam>
    /// <param name="source">The source list of data.</param>
    /// <param name="lookbackPeriods">The number of periods for the lookback calculation. Default is 25.</param>
    /// <param name="smoothPeriods">The number of periods for the smoothing calculation. Default is 13.</param>
    /// <param name="signalPeriods">The number of periods for the signal calculation. Default is 7.</param>
    /// <returns>A list of TsiResult containing the TSI and signal values.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the source is null.</exception>
    public static IReadOnlyList<TsiResult> ToTsi<T>(
        this IReadOnlyList<T> source,
        int lookbackPeriods = 25,
        int smoothPeriods = 13,
        int signalPeriods = 7)
        where T : IReusable
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(source);
        Validate(lookbackPeriods, smoothPeriods, signalPeriods);

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

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            T s = source[i];

            // skip first period
            if (i == 0)
            {
                a[i] = double.NaN;
                c[i] = double.NaN;
                cs1[i] = double.NaN;
                as1[i] = double.NaN;
                cs2[i] = double.NaN;
                as2[i] = double.NaN;

                results.Add(new(s.Timestamp));
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
                double sumCs = 0;
                double sumAs = 0;

                for (int p = i - smoothPeriods + 1; p <= i; p++)
                {
                    sumCs += cs1[p];
                    sumAs += as1[p];
                }

                cs2[i] = sumCs / smoothPeriods;
                as2[i] = sumAs / smoothPeriods;
            }

            // normal second smoothing
            else
            {
                cs2[i] = ((cs1[i] - cs2[i - 1]) * mult2) + cs2[i - 1];
                as2[i] = ((as1[i] - as2[i - 1]) * mult2) + as2[i - 1];
            }

            // true strength index
            double tsi = as2[i] != 0
                ? 100d * (cs2[i] / as2[i])
                : double.NaN;

            // signal line
            double signal;

            if (signalPeriods > 1)
            {
                // re/initialize signal
                if (double.IsNaN(prevSignal) && i > signalPeriods)
                {
                    double sum = tsi;
                    for (int p = i - signalPeriods + 1; p < i; p++)
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
            }
            else
            {
                signal = signalPeriods == 1
                    ? tsi
                    : double.NaN;
            }

            results.Add(new TsiResult(
                Timestamp: s.Timestamp,
                Tsi: tsi.NaN2Null(),
                Signal: signal.NaN2Null()));

            prevSignal = signal;
        }

        return results;
    }
}
