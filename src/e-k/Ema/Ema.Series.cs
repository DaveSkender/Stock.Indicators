using System.Collections.ObjectModel;

namespace Skender.Stock.Indicators;

// EXPONENTIAL MOVING AVERAGE (SERIES)
public static partial class Indicator
{
    internal static Collection<EmaResult> CalcEma(
        this Collection<(DateTime, double)> tpColl,
        int lookbackPeriods)
    {
        // check parameter arguments
        EmaBase.Validate(lookbackPeriods);

        // initialize
        int length = tpColl.Count;
        Collection<EmaResult> results = new();

        double lastEma = 0;
        double k = 2d / (lookbackPeriods + 1);
        int initPeriods = Math.Min(lookbackPeriods, length);

        for (int i = 0; i < initPeriods; i++)
        {
            (DateTime _, double value) = tpColl[i];
            lastEma += value;
        }

        lastEma /= lookbackPeriods;

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            (DateTime date, double value) = tpColl[i];
            EmaResult r = new(date);
            results.Add(r);

            if (i + 1 > lookbackPeriods)
            {
                double ema = EmaBase.Increment(value, lastEma, k);
                r.Ema = ema.NaN2Null();
                lastEma = ema;
            }
            else if (i == lookbackPeriods - 1)
            {
                r.Ema = lastEma.NaN2Null();
            }
        }

        return results;
    }
}
