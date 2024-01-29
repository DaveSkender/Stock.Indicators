namespace Skender.Stock.Indicators;

// KAUFMAN's ADAPTIVE MOVING AVERAGE (SERIES)

public static partial class Indicator
{
    internal static List<KamaResult> CalcKama(
        this List<(DateTime _, double Value)> tpList,
        int erPeriods,
        int fastPeriods,
        int slowPeriods)
    {
        // check parameter arguments
        Kama.Validate(erPeriods, fastPeriods, slowPeriods);

        // initialize
        int length = tpList.Count;
        List<KamaResult> results = new(length);

        double scFast = 2d / (fastPeriods + 1);
        double scSlow = 2d / (slowPeriods + 1);

        double prevKama = double.NaN;

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            (DateTime date, double value) = tpList[i];

            KamaResult r = new() { TickDate = date };
            results.Add(r);

            // skip incalculable periods
            if (i < erPeriods - 1)
            {
                continue;
            }

            double kama;

            if (double.IsNaN(prevKama))
            {
                kama = value;
            }
            else
            {
                // ER period change
                double change = Math.Abs(value - tpList[i - erPeriods].Value);

                // volatility
                double sumPV = 0;
                for (int p = i - erPeriods + 1; p <= i; p++)
                {
                    sumPV += Math.Abs(tpList[p].Value - tpList[p - 1].Value);
                }

                if (sumPV != 0)
                {
                    // efficiency ratio
                    double er = change / sumPV;
                    r.ER = er.NaN2Null();

                    // smoothing constant
                    double sc = (er * (scFast - scSlow)) + scSlow;  // squared later

                    // kama calculation
                    kama = prevKama + (sc * sc * (value - prevKama));
                }

                // handle flatline case
                else
                {
                    r.ER = 0;
                    kama = value;
                }
            }

            r.Kama = kama.NaN2Null();
            prevKama = kama;
        }

        return results;
    }
}
