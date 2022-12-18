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
        ValidateSmma(lookbackPeriods);

        // initialize
        int length = tpList.Count;
        List<SmmaResult> results = new(length);
        double prevValue = double.NaN;

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            double smma = double.NaN;
            (DateTime date, double value) = tpList[i];

            SmmaResult r = new(date);
            results.Add(r);

            // calculate SMMA
            if (i + 1 > lookbackPeriods)
            {
                smma = ((prevValue * (lookbackPeriods - 1)) + value) / lookbackPeriods;
                r.Smma = smma.NaN2Null();
            }

            // first SMMA calculated as simple SMA
            else if (i + 1 == lookbackPeriods)
            {
                double sumClose = 0;
                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    (DateTime _, double pValue) = tpList[p];
                    sumClose += pValue;
                }

                smma = sumClose / lookbackPeriods;
                r.Smma = smma.NaN2Null();
            }

            prevValue = smma;
        }

        return results;
    }

    // parameter validation
    private static void ValidateSmma(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for SMMA.");
        }
    }
}
