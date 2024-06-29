namespace Skender.Stock.Indicators;

// KAUFMAN's ADAPTIVE MOVING AVERAGE (SERIES)

public static partial class Indicator
{
    internal static List<KamaResult> CalcKama<T>(
        this List<T> source,
        int erPeriods,
        int fastPeriods,
        int slowPeriods)
        where T : IReusable
    {
        // check parameter arguments
        Kama.Validate(erPeriods, fastPeriods, slowPeriods);

        // initialize
        int length = source.Count;
        List<KamaResult> results = new(length);

        double scFast = 2d / (fastPeriods + 1);
        double scSlow = 2d / (slowPeriods + 1);

        double prevKama = double.NaN;

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            T s = source[i];

            // skip incalculable periods
            if (i < erPeriods - 1)
            {
                results.Add(new() { Timestamp = s.Timestamp });
                continue;
            }

            double er;
            double kama;

            if (double.IsNaN(prevKama))
            {
                er = double.NaN;
                kama = s.Value;
            }
            else
            {
                // ER period change
                double change = Math.Abs(s.Value - source[i - erPeriods].Value);

                // volatility
                double sumPV = 0;
                for (int p = i - erPeriods + 1; p <= i; p++)
                {
                    sumPV += Math.Abs(source[p].Value - source[p - 1].Value);
                }

                if (sumPV != 0)
                {
                    // efficiency ratio
                    er = change / sumPV;

                    // smoothing constant
                    double sc = (er * (scFast - scSlow)) + scSlow;  // squared later

                    // kama calculation
                    kama = prevKama + (sc * sc * (s.Value - prevKama));
                }

                // handle flatline case
                else
                {
                    er = 0;
                    kama = s.Value;
                }
            }

            results.Add(new KamaResult(
                Timestamp: s.Timestamp,
                ER: er.NaN2Null(),
                Kama: kama.NaN2Null()));

            prevKama = kama;
        }

        return results;
    }
}
