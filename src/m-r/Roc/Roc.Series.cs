namespace Skender.Stock.Indicators;

// RATE OF CHANGE (SERIES)

public static partial class Indicator
{
    internal static List<RocResult> CalcRoc(
        this List<(DateTime, double)> tpList,
        int lookbackPeriods)
    {
        // check parameter arguments
        Roc.Validate(lookbackPeriods);

        // initialize
        List<RocResult> results = new(tpList.Count);

        // roll through quotes
        for (int i = 0; i < tpList.Count; i++)
        {
            (DateTime date, double value) = tpList[i];

            RocResult r = new() { TickDate = date };
            results.Add(r);

            if (i + 1 > lookbackPeriods)
            {
                (DateTime _, double backValue) = tpList[i - lookbackPeriods];

                r.Momentum = (value - backValue).NaN2Null();
                r.Roc = (backValue == 0) ? null
                    : (100d * r.Momentum / backValue).NaN2Null();
            }
        }

        return results;
    }
}
