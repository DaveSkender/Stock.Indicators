namespace Skender.Stock.Indicators;

// HULL MOVING AVERAGE (SERIES)

public static partial class Indicator
{
    // calculate series
    private static List<HmaResult> CalcHma<T>(
        this List<T> source,
        int lookbackPeriods)
        where T : IReusable
    {
        // check parameter arguments
        Hma.Validate(lookbackPeriods);

        // initialize
        int length = source.Count;
        int shiftQty = lookbackPeriods - 1;
        List<IReusable> synthHistory = [];

        IReadOnlyList<WmaResult> wmaN1
            = source.CalcWma(lookbackPeriods);

        IReadOnlyList<WmaResult> wmaN2
            = source.CalcWma(lookbackPeriods / 2);

        // roll through source values, to get interim synthetic quotes
        for (int i = 0; i < length; i++)
        {
            T s = source[i];

            WmaResult w1 = wmaN1[i];
            WmaResult w2 = wmaN2[i];

            if (i < shiftQty)
            {
                continue;
            }

            QuotePart sh = new(
                s.Timestamp,
                (w2.Wma.Null2NaN() * 2d) - w1.Wma.Null2NaN());

            synthHistory.Add(sh);
        }

        // add back truncated null results
        int sqN = (int)Math.Sqrt(lookbackPeriods);

        List<HmaResult> results = source
            .Take(shiftQty)
            .Select(x => new HmaResult(x.Timestamp))
            .ToList();

        // calculate final HMA = WMA with period SQRT(n)
        List<HmaResult> hmaResults = synthHistory.CalcWma(sqN)
            .Select(x => new HmaResult(
                Timestamp: x.Timestamp,
                Hma: x.Wma
             ))
            .ToList();

        // add WMA to results
        results.AddRange(hmaResults);

        return results.ToSortedList();
    }
}
