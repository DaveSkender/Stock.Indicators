namespace Skender.Stock.Indicators;

// RATE OF CHANGE (SERIES)
public static partial class Indicator
{
    internal static List<RocResult> CalcRoc(
        this List<(DateTime, double)> tpList,
        int lookbackPeriods,
        int? smaPeriods)
    {
        // check parameter arguments
        ValidateRoc(lookbackPeriods, smaPeriods);

        // initialize
        List<RocResult> results = new(tpList.Count);

        // roll through quotes
        for (int i = 0; i < tpList.Count; i++)
        {
            (DateTime date, double value) = tpList[i];

            RocResult r = new(date);
            results.Add(r);

            if (i + 1 > lookbackPeriods)
            {
                (DateTime _, double backValue) = tpList[i - lookbackPeriods];

                r.Roc = (backValue == 0) ? null
                    : (100d * (value - backValue) / backValue).NaN2Null();
            }

            // optional SMA
            if (smaPeriods != null && i + 1 >= lookbackPeriods + smaPeriods)
            {
                double? sumSma = 0;
                for (int p = i + 1 - (int)smaPeriods; p <= i; p++)
                {
                    sumSma += results[p].Roc;
                }

                r.RocSma = sumSma / smaPeriods;
            }
        }

        return results;
    }

    // parameter validation
    private static void ValidateRoc(
        int lookbackPeriods,
        int? smaPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for ROC.");
        }

        if (smaPeriods is not null and <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(smaPeriods), smaPeriods,
                "SMA periods must be greater than 0 for ROC.");
        }
    }
}
