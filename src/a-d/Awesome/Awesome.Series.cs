using System.Collections.ObjectModel;

namespace Skender.Stock.Indicators;

// AWESOME OSCILLATOR (SERIES)
public static partial class Indicator
{
    internal static Collection<AwesomeResult> CalcAwesome(
        this Collection<(DateTime, double)> tpColl,
        int fastPeriods,
        int slowPeriods)
    {
        // check parameter arguments
        ValidateAwesome(fastPeriods, slowPeriods);

        // initialize
        int length = tpColl.Count;
        Collection<AwesomeResult> results = new();
        double[] pr = new double[length];

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            (DateTime date, double value) = tpColl[i];
            pr[i] = value;

            AwesomeResult r = new(date);
            results.Add(r);

            if (i + 1 >= slowPeriods)
            {
                double sumSlow = 0;
                double sumFast = 0;

                for (int p = i + 1 - slowPeriods; p <= i; p++)
                {
                    sumSlow += pr[p];

                    if (p >= i + 1 - fastPeriods)
                    {
                        sumFast += pr[p];
                    }
                }

                r.Oscillator = ((sumFast / fastPeriods) - (sumSlow / slowPeriods)).NaN2Null();
                r.Normalized = (pr[i] != 0) ? 100 * r.Oscillator / pr[i] : null;
            }
        }

        return results;
    }

    // parameter validation
    private static void ValidateAwesome(
        int fastPeriods,
        int slowPeriods)
    {
        // check parameter arguments
        if (fastPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(slowPeriods), slowPeriods,
                "Fast periods must be greater than 0 for Awesome Oscillator.");
        }

        if (slowPeriods <= fastPeriods)
        {
            throw new ArgumentOutOfRangeException(nameof(slowPeriods), slowPeriods,
                "Slow periods must be larger than Fast Periods for Awesome Oscillator.");
        }
    }
}
