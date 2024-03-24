namespace Skender.Stock.Indicators;

// SMOOTHED MOVING AVERAGE (SERIES)

public static partial class Indicator
{
    // calculate series
    internal static List<SmmaResult> CalcSmma(
        this List<(DateTime, double)> tpList,
        int lookbackPeriods)
    {
        // check parameter arguments
        Smma.Validate(lookbackPeriods);

        // initialize
        int length = tpList.Count;
        List<SmmaResult> results = new(length);
        double prevSmma = double.NaN;

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            (DateTime date, double value) = tpList[i];

            SmmaResult r = new() { Timestamp = date };
            results.Add(r);

            // skip incalculable periods
            if (i < lookbackPeriods - 1)
            {
                continue;
            }

            double smma;

            // when no prior SMMA, reset as SMA
            if (double.IsNaN(prevSmma))
            {
                double sum = 0;
                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    (DateTime _, double pValue) = tpList[p];
                    sum += pValue;
                }

                smma = sum / lookbackPeriods;
            }

            // normal SMMA
            else
            {
                smma = ((prevSmma * (lookbackPeriods - 1)) + value) / lookbackPeriods;
            }

            r.Smma = smma.NaN2Null();
            prevSmma = smma;
        }

        return results;
    }
}
