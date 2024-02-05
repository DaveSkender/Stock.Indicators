namespace Skender.Stock.Indicators;

// AWESOME OSCILLATOR (SERIES)

public static partial class Indicator
{
    internal static List<AwesomeResult> CalcAwesome(
        this List<(DateTime, double)> tpList,
        int fastPeriods,
        int slowPeriods)
    {
        // check parameter arguments
        Awesome.Validate(fastPeriods, slowPeriods);

        // initialize
        int length = tpList.Count;
        List<AwesomeResult> results = new(length);
        double[] pr = new double[length];

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            (DateTime date, double value) = tpList[i];
            pr[i] = value;

            AwesomeResult r = new() { Timestamp = date };
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
}
