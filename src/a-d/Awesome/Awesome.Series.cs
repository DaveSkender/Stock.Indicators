namespace Skender.Stock.Indicators;

// AWESOME OSCILLATOR (SERIES)
public static partial class Indicator
{
    internal static IEnumerable<AwesomeResult> CalcAwesome(
        this List<(DateTime, double)> tpList,
        int fastPeriods = 5,
        int slowPeriods = 34)
    {
        // check parameter arguments
        ValidateAwesome(fastPeriods, slowPeriods);

        // initialize
        int length = tpList.Count;
        List<AwesomeResult> results = new(length);
        double[] pr = new double[length];

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            (DateTime date, double value) = tpList[i];
            pr[i] = value;

            AwesomeResult r = new()
            {
                Date = date
            };

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

                r.Oscillator = (sumFast / fastPeriods) - (sumSlow / slowPeriods);
                r.Normalized = (pr[i] != 0) ? 100 * r.Oscillator / pr[i] : double.NaN;
            }

            results.Add(r);
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
