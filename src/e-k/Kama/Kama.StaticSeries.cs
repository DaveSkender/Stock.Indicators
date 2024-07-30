namespace Skender.Stock.Indicators;

// KAUFMAN's ADAPTIVE MOVING AVERAGE (SERIES)

public static partial class Kama
{
    private static List<KamaResult> CalcKama<T>(
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
