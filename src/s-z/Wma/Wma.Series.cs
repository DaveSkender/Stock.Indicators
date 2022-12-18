using System.Collections.ObjectModel;

namespace Skender.Stock.Indicators;

// WEIGHTED MOVING AVERAGE (SERIES)
public static partial class Indicator
{
    internal static Collection<WmaResult> CalcWma(
        this Collection<(DateTime, double)> tpColl,
        int lookbackPeriods)
    {
        // check parameter arguments
        ValidateWma(lookbackPeriods);

        // initialize
        Collection<WmaResult> results = new();
        double divisor = (double)lookbackPeriods * (lookbackPeriods + 1) / 2d;

        // roll through quotes
        for (int i = 0; i < tpColl.Count; i++)
        {
            (DateTime date, double _) = tpColl[i];

            WmaResult r = new(date);
            results.Add(r);

            if (i + 1 >= lookbackPeriods)
            {
                double wma = 0;
                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    (DateTime _, double pValue) = tpColl[p];
                    wma += pValue * (lookbackPeriods - (i + 1 - p - 1)) / divisor;
                }

                r.Wma = wma.NaN2Null();
            }
        }

        return results;
    }

    // parameter validation
    private static void ValidateWma(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for WMA.");
        }
    }
}
