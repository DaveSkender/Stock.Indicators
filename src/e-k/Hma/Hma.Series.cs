using System.Collections.ObjectModel;

namespace Skender.Stock.Indicators;

// HULL MOVING AVERAGE (SERIES)
public static partial class Indicator
{
    // calculate series
    internal static Collection<HmaResult> CalcHma(
        this Collection<(DateTime, double)> tpColl,
        int lookbackPeriods)
    {
        // check parameter arguments
        ValidateHma(lookbackPeriods);

        // initialize
        int shiftQty = lookbackPeriods - 1;
        Collection<(DateTime, double)> synthHistory = new();

        List<WmaResult> wmaN1 = tpColl.GetWma(lookbackPeriods).ToList();
        List<WmaResult> wmaN2 = tpColl.GetWma(lookbackPeriods / 2).ToList();

        // roll through quotes, to get interim synthetic quotes
        for (int i = 0; i < tpColl.Count; i++)
        {
            (DateTime date, double _) = tpColl[i];

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

        List<HmaResult> results = tpColl
            .Take(shiftQty)
            .Select(x => new HmaResult(x.Item1))
            .ToList();

        // calculate final HMA = WMA with period SQRT(n)
        List<HmaResult> hmaResults = synthHistory.CalcWma(sqN)
            .Select(x => new HmaResult(x.Date)
            {
                Hma = x.Wma
            })
            .ToList();

        // add WMA to results
        results
            .AddRange(hmaResults);

        return new Collection<HmaResult>(results);
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
