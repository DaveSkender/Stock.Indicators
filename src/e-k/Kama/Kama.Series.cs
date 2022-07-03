namespace Skender.Stock.Indicators;

// KAUFMAN's ADAPTIVE MOVING AVERAGE (SERIES)
public static partial class Indicator
{
    internal static List<KamaResult> CalcKama(
        this List<(DateTime, double)> tpList,
        int erPeriods,
        int fastPeriods,
        int slowPeriods)
    {
        // check parameter arguments
        ValidateKama(erPeriods, fastPeriods, slowPeriods);

        // initialize
        int length = tpList.Count;
        List<KamaResult> results = new(length);
        double scFast = 2d / (fastPeriods + 1);
        double scSlow = 2d / (slowPeriods + 1);

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            (DateTime date, double value) = tpList[i];

            KamaResult r = new(date);
            results.Add(r);

            if (i + 1 > erPeriods)
            {
                // ER period change
                double change = Math.Abs(value - tpList[i - erPeriods].Item2);

                // volatility
                double sumPV = 0;
                for (int p = i - erPeriods + 1; p <= i; p++)
                {
                    sumPV += Math.Abs(tpList[p].Item2 - tpList[p - 1].Item2);
                }

                if (sumPV != 0)
                {
                    // efficiency ratio
                    double er = change / sumPV;
                    r.ER = er.NaN2Null();

                    // smoothing constant
                    double sc = (er * (scFast - scSlow)) + scSlow;  // squared later

                    // kama calculation
                    double? pk = results[i - 1].Kama;  // prior KAMA
                    r.Kama = (pk + (sc * sc * (value - pk))).NaN2Null();
                }

                // handle flatline case
                else
                {
                    r.ER = 0;
                    r.Kama = value.NaN2Null();
                }
            }

            // initial value
            else if (i + 1 == erPeriods)
            {
                r.Kama = value.NaN2Null();
            }
        }

        return results;
    }

    // parameter validation
    private static void ValidateKama(
        int erPeriods,
        int fastPeriods,
        int slowPeriods)
    {
        // check parameter arguments
        if (erPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(erPeriods), erPeriods,
                "Efficiency Ratio periods must be greater than 0 for KAMA.");
        }

        if (fastPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(fastPeriods), fastPeriods,
                "Fast EMA periods must be greater than 0 for KAMA.");
        }

        if (slowPeriods <= fastPeriods)
        {
            throw new ArgumentOutOfRangeException(nameof(slowPeriods), slowPeriods,
                "Slow EMA periods must be greater than Fast EMA period for KAMA.");
        }
    }
}
