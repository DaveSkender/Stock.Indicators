namespace Skender.Stock.Indicators;

// HULL MOVING AVERAGE (SERIES)
public static partial class Indicator
{
    // calculate series
    internal static IEnumerable<HmaResult> CalcHma(
        this List<(DateTime, double)> tpList,
        int lookbackPeriods)
    {
        // check parameter arguments
        ValidateHma(lookbackPeriods);

        // initialize
        List<Quote> synthHistory = new();

        List<WmaResult> wmaN1 = tpList.GetWma(lookbackPeriods).ToList();
        List<WmaResult> wmaN2 = tpList.GetWma(lookbackPeriods / 2).ToList();

        // roll through quotes, to get interim synthetic quotes
        for (int i = 0; i < tpList.Count; i++)
        {
            (DateTime date, double value) = tpList[i];

            Quote sh = new()
            {
                Date = date
            };

            WmaResult w1 = wmaN1[i];
            WmaResult w2 = wmaN2[i];

            if (i >= lookbackPeriods - 1 && w1.Wma != null && w2.Wma != null)
            {
                sh.Close = (decimal)((w2.Wma * 2d) - w1.Wma);
                synthHistory.Add(sh);
            }
        }

        // add back truncated null results
        int sqN = (int)Math.Sqrt(lookbackPeriods);
        int shiftQty = lookbackPeriods - 1;

        List<HmaResult> results = tpList
            .Take(shiftQty)
            .Select(x => new HmaResult
            {
                Date = x.Item1
            })
            .ToList();

        // calculate final HMA = WMA with period SQRT(n)
        List<HmaResult> hmaResults = synthHistory.GetWma(sqN)
            .Select(x => new HmaResult
            {
                Date = x.Date,
                Hma = x.Wma
            })
            .ToList();

        // add WMA to results
        results.AddRange(hmaResults);
        results = results.OrderBy(x => x.Date).ToList();

        return results;
    }

    // parameter validation
    private static void ValidateHma(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 1 for HMA.");
        }
    }
}
