namespace Skender.Stock.Indicators;

// STANDARD DEVIATION (SERIES)

public static partial class Indicator
{
    internal static List<StdDevResult> CalcStdDev(
        this List<(DateTime, double)> tpList,
        int lookbackPeriods)
    {
        // check parameter arguments
        StdDev.Validate(lookbackPeriods);

        // initialize
        int length = tpList.Count;
        List<StdDevResult> results = new(length);

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            (DateTime date, double value) = tpList[i];

            StdDevResult r = new(date);
            results.Add(r);

            if (i + 1 >= lookbackPeriods)
            {
                double[] values = new double[lookbackPeriods];
                double sum = 0;
                int n = 0;

                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    (DateTime _, double v) = tpList[p];
                    values[n] = v;
                    sum += v;
                    n++;
                }

                double avg = sum / lookbackPeriods;

                r.StdDev = values.StdDev().NaN2Null();
                r.Mean = avg.NaN2Null();

                r.ZScore = (r.StdDev == 0) ? null
                    : (value - avg) / r.StdDev;
            }
        }

        return results;
    }
}
