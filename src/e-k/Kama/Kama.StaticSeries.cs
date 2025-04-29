namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Kaufman's Adaptive Moving Average (KAMA) indicator.
/// </summary>
public static partial class Kama
{
    /// <summary>
    /// Converts a list of source values to KAMA (Kaufman's Adaptive Moving Average) results.
    /// </summary>
    /// <typeparam name="T">The type of the source values, which must implement <see cref="IReusable"/>.</typeparam>
    /// <param name="source">The list of source values to transform.</param>
    /// <param name="erPeriods">The number of periods for the Efficiency Ratio (ER). Default is 10.</param>
    /// <param name="fastPeriods">The number of periods for the fast EMA. Default is 2.</param>
    /// <param name="slowPeriods">The number of periods for the slow EMA. Default is 30.</param>
    /// <returns>A list of KAMA results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the source list is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the parameters are out of their valid range.</exception>
    [Series("KAMA", "Kaufman's Adaptive Moving Average", Category.MovingAverage, ChartType.Overlay)]
    public static IReadOnlyList<KamaResult> ToKama<T>(
        this IReadOnlyList<T> source,
        [ParamNum<int>("Lookback Periods", 2, 250, 10)]
        int erPeriods = 10,
        [ParamNum<int>("Fast Periods", 1, 50, 2)]
        int fastPeriods = 2,
        [ParamNum<int>("Slow Periods", 1, 250, 30)]
        int slowPeriods = 30)
        where T : IReusable
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(source);
        Validate(erPeriods, fastPeriods, slowPeriods);

        // initialize
        int length = source.Count;
        List<KamaResult> results = new(length);

        double scFast = 2d / (fastPeriods + 1);
        double scSlow = 2d / (slowPeriods + 1);

        double prevKama = double.NaN;

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            // skip incalculable periods
            if (i < erPeriods - 1)
            {
                results.Add(new(source[i].Timestamp));
                continue;
            }

            double er;
            double kama;

            if (results[i - 1].Kama is not null)
            {
                double newVal = source[i].Value;

                // ER period change
                double change = Math.Abs(newVal - source[i - erPeriods].Value);

                // volatility
                double sumPv = 0;
                for (int p = i - erPeriods + 1; p <= i; p++)
                {
                    sumPv += Math.Abs(source[p].Value - source[p - 1].Value);
                }

                if (sumPv != 0)
                {
                    // efficiency ratio
                    er = change / sumPv;

                    // smoothing constant
                    double sc = (er * (scFast - scSlow)) + scSlow;  // squared later

                    // kama calculation
                    kama = prevKama + (sc * sc * (newVal - prevKama));
                }

                // handle flatline case
                else
                {
                    er = 0;
                    kama = source[i].Value;
                }
            }

            // re/initialize
            else
            {
                er = double.NaN;
                kama = source[i].Value;
            }

            results.Add(new KamaResult(
                Timestamp: source[i].Timestamp,
                Er: er.NaN2Null(),
                Kama: kama.NaN2Null()));

            prevKama = kama;
        }

        return results;
    }
}
