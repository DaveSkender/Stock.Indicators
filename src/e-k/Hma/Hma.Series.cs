namespace Skender.Stock.Indicators;

// HULL MOVING AVERAGE (SERIES)

public static partial class Indicator
{
    // calculate series
    internal static List<HmaResult> CalcHma(
        this List<(DateTime Timestamp, double _)> tpList,
        int lookbackPeriods)
    {
        // check parameter arguments
        Hma.Validate(lookbackPeriods);

        // initialize
        int shiftQty = lookbackPeriods - 1;
        List<(DateTime, double)> synthHistory = [];

        List<WmaResult> wmaN1 = tpList.GetWma(lookbackPeriods).ToList();
        List<WmaResult> wmaN2 = tpList.GetWma(lookbackPeriods / 2).ToList();

        // roll through quotes, to get interim synthetic quotes
        for (int i = 0; i < tpList.Count; i++)
        {
            (DateTime date, double _) = tpList[i];

            WmaResult w1 = wmaN1[i];
            WmaResult w2 = wmaN2[i];

            if (i >= shiftQty)
            {
                (DateTime, double) sh
                    = new(date, (w2.Wma.Null2NaN() * 2d) - w1.Wma.Null2NaN());

                synthHistory.Add(sh);
            }
        }

        // add back truncated null results
        int sqN = (int)Math.Sqrt(lookbackPeriods);

        List<HmaResult> results = tpList
            .Take(shiftQty)
            .Select(x => new HmaResult { Timestamp = x.Timestamp })
            .ToList();

        // calculate final HMA = WMA with period SQRT(n)
        List<HmaResult> hmaResults = synthHistory.CalcWma(sqN)
            .Select(x => new HmaResult {
                Timestamp = x.Timestamp,
                Hma = x.Wma
            })
            .ToList();

        // add WMA to results
        results.AddRange(hmaResults);

        return results.ToSortedList();
    }
}
