namespace Skender.Stock.Indicators;

// RATE OF CHANGE (ROC) WITH BANDS (SERIES)
public static partial class Indicator
{
    internal static List<RocWbResult> CalcRocWb(
        this List<(DateTime, double)> tpList,
        int lookbackPeriods,
        int emaPeriods,
        int stdDevPeriods)
    {
        // check parameter arguments
        RocWb.Validate(lookbackPeriods, emaPeriods, stdDevPeriods);

        // initialize
        List<RocWbResult> results = tpList
            .CalcRoc(lookbackPeriods, null)
            .Select(x => new RocWbResult(x.Date)
            {
                Roc = x.Roc
            })
            .ToList();

        double k = 2d / (emaPeriods + 1);
        double? lastEma = 0;

        int length = results.Count;

        if (length > lookbackPeriods)
        {
            int initPeriods = Math.Min(lookbackPeriods + emaPeriods, length);

            for (int i = lookbackPeriods; i < initPeriods; i++)
            {
                lastEma += results[i].Roc;
            }

            lastEma /= emaPeriods;
        }

        double?[] rocSq = results
            .Select(x => x.Roc * x.Roc)
            .ToArray();

        // roll through quotes
        for (int i = lookbackPeriods; i < length; i++)
        {
            RocWbResult r = results[i];

            // exponential moving average
            if (i + 1 > lookbackPeriods + emaPeriods)
            {
                r.RocEma = lastEma + (k * (r.Roc - lastEma));
                lastEma = r.RocEma;
            }
            else if (i + 1 == lookbackPeriods + emaPeriods)
            {
                r.RocEma = lastEma;
            }

            // ROC deviation
            if (i + 1 >= lookbackPeriods + stdDevPeriods)
            {
                double? sumSq = 0;
                for (int p = i - stdDevPeriods + 1; p <= i; p++)
                {
                    sumSq += rocSq[p];
                }

                if (sumSq is not null)
                {
                    double? rocDev = Math.Sqrt((double)sumSq / stdDevPeriods);

                    r.UpperBand = rocDev;
                    r.LowerBand = -rocDev;
                }
            }
        }

        return results;
    }
}
