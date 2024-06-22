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
        int length = tpList.Count;
        List<RocWbResult> results = new(length);

        double k = 2d / (emaPeriods + 1);
        double prevEma = double.NaN;

        List<(DateTime _, double Roc)> tpRoc = tpList
            .CalcRoc(lookbackPeriods)
            .ToTupleResult();

        double[] rocSq = tpRoc
            .Select(x => x.Roc * x.Roc)
            .ToArray();

        double[] ema = new double[length];

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            (DateTime date, double roc) = tpRoc[i];
            RocWbResult r = new() {
                Timestamp = date,
                Roc = roc.NaN2Null()
            };
            results.Add(r);

            // exponential moving average
            if (double.IsNaN(prevEma) && i >= emaPeriods)
            {
                double sum = 0;
                for (int p = i - emaPeriods + 1; p <= i; p++)
                {
                    sum += tpRoc[p].Roc;
                }

                ema[i] = sum / emaPeriods;
            }

            // normal EMA
            else
            {
                ema[i] = EmaUtilities.Increment(k, prevEma, roc);
            }

            r.RocEma = ema[i].NaN2Null();
            prevEma = ema[i];

            // ROC deviation
            if (i >= stdDevPeriods)
            {
                double sum = 0;
                for (int p = i - stdDevPeriods + 1; p <= i; p++)
                {
                    sum += rocSq[p];
                }

                double? rocDev = Math.Sqrt(sum / stdDevPeriods).NaN2Null();

                r.UpperBand = rocDev;
                r.LowerBand = -rocDev;
            }
        }

        return results;
    }
}
