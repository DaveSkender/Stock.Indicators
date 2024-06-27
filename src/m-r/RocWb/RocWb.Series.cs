namespace Skender.Stock.Indicators;

// RATE OF CHANGE (ROC) WITH BANDS (SERIES)
public static partial class Indicator
{
    internal static List<RocWbResult> CalcRocWb<T>(
        this List<T> source,
        int lookbackPeriods,
        int emaPeriods,
        int stdDevPeriods)
        where T : IReusable
    {
        // check parameter arguments
        RocWb.Validate(lookbackPeriods, emaPeriods, stdDevPeriods);

        // initialize
        int length = source.Count;
        List<RocWbResult> results = new(length);

        double k = 2d / (emaPeriods + 1);
        double prevEma = double.NaN;

        List<IReusable> ogRoc = source
            .CalcRoc(lookbackPeriods)
            .Cast<IReusable>()
            .ToSortedList();

        double[] rocSq = ogRoc
            .Select(x => x.Value * x.Value)
            .ToArray();

        double[] ema = new double[length];

        // roll through results
        for (int i = 0; i < length; i++)
        {
            IReusable roc = ogRoc[i];
            RocWbResult r = new() {
                Timestamp = roc.Timestamp,
                Roc = roc.Value.NaN2Null()
            };
            results.Add(r);

            // exponential moving average
            if (double.IsNaN(prevEma) && i >= emaPeriods)
            {
                double sum = 0;
                for (int p = i - emaPeriods + 1; p <= i; p++)
                {
                    sum += ogRoc[p].Value;
                }

                ema[i] = sum / emaPeriods;
            }

            // normal EMA
            else
            {
                ema[i] = EmaUtilities.Increment(k, prevEma, roc.Value);
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
