namespace Skender.Stock.Indicators;

// WEIGHTED MOVING AVERAGE (SERIES)

public static partial class Indicator
{
    internal static List<WmaResult> CalcWma(
        this List<(DateTime, double)> tpList,
        int lookbackPeriods)
    {
        // check parameter arguments
        Wma.Validate(lookbackPeriods);

        // initialize
        List<WmaResult> results = new(tpList.Count);
        double divisor = (double)lookbackPeriods * (lookbackPeriods + 1) / 2d;

        // roll through quotes
        for (int i = 0; i < tpList.Count; i++)
        {
            (DateTime date, double _) = tpList[i];

            WmaResult r = new() { TickDate = date };
            results.Add(r);

            if (i + 1 >= lookbackPeriods)
            {
                double wma = 0;
                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    (DateTime _, double pValue) = tpList[p];
                    wma += pValue * (lookbackPeriods - (i + 1 - p - 1)) / divisor;
                }

                r.Wma = wma.NaN2Null();
            }
        }

        return results;
    }
}
