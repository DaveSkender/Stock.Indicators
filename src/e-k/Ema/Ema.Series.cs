namespace Skender.Stock.Indicators;

// EXPONENTIAL MOVING AVERAGE (SERIES)
public static partial class Indicator
{
    internal static List<EmaResult> CalcEma(
        this List<(DateTime, double)> tpList,
        int lookbackPeriods)
    {
        // check parameter arguments
        EmaBase.Validate(lookbackPeriods);

        // initialize
        int length = tpList.Count;
        List<EmaResult> results = new(length);

        double lastEma = 0;
        double k = 2d / (lookbackPeriods + 1);
        int initPeriods = Math.Min(lookbackPeriods, length);

        for (int i = 0; i < initPeriods; i++)
        {
            (DateTime _, double value) = tpList[i];
            lastEma += value;
        }

        lastEma /= lookbackPeriods;

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            (DateTime date, double value) = tpList[i];
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
