using System.Collections.ObjectModel;

namespace Skender.Stock.Indicators;

// MONEY FLOW INDEX (SERIES)
public static partial class Indicator
{
    internal static Collection<DynamicResult> CalcDynamic(
        this Collection<(DateTime, double)> tpColl,
        int lookbackPeriods,
        double kFactor)
    {
        // check parameter arguments
        ValidateDynamic(lookbackPeriods, kFactor);

        // initialize
        int iStart = 1;
        int length = tpColl.Count;
        Collection<DynamicResult> results = new();

        if (length == 0)
        {
            return results;
        }

        double prevMD = tpColl[0].Item2;

        // roll through quotes, to get preliminary data
        for (int i = 0; i < length; i++)
        {
            (DateTime date, double value) = tpColl[i];

            DynamicResult r = new(date);
            results.Add(r);

            // re-initialize if value is NaN
            if (double.IsNaN(value) || prevMD == 0)
            {
                prevMD = value;
                iStart = i + lookbackPeriods;
            }
            else
            {
                double md = prevMD + ((value - prevMD) /
                    (kFactor * lookbackPeriods * Math.Pow(value / prevMD, 4)));

                if (i >= iStart)
                {
                    r.Dynamic = md.NaN2Null();
                }

                prevMD = md;
            }
        }

        return results;
    }

    // parameter validation
    private static void ValidateDynamic(
        int lookbackPeriods,
        double kFactor)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for DYNAMIC.");
        }

        if (kFactor <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(kFactor), kFactor,
                "K-Factor range adjustment must be greater than 0 for DYNAMIC.");
        }
    }
}
