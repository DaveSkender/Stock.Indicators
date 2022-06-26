namespace Skender.Stock.Indicators;

// FORCE INDEX (SERIES)
public static partial class Indicator
{
    internal static List<ForceIndexResult> CalcForceIndex(
        this List<QuoteD> qdList,
        int lookbackPeriods)
    {
        // check parameter arguments
        ValidateForceIndex(lookbackPeriods);

        // initialize
        int length = qdList.Count;
        List<ForceIndexResult> results = new(length);
        double? prevClose = null, prevFI = null, sumRawFI = 0;
        double k = 2d / (lookbackPeriods + 1);

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            QuoteD q = qdList[i];

            ForceIndexResult r = new(q.Date);
            results.Add(r);

            // skip first period
            if (i == 0)
            {
                prevClose = q.Close;
                continue;
            }

            // raw Force Index
            double? rawFI = q.Volume * (q.Close - prevClose);
            prevClose = q.Close;

            // calculate EMA
            if (i > lookbackPeriods)
            {
                r.ForceIndex = prevFI + (k * (rawFI - prevFI));
            }

            // initialization period
            else
            {
                sumRawFI += rawFI;

                // first EMA value
                if (i == lookbackPeriods)
                {
                    r.ForceIndex = sumRawFI / lookbackPeriods;
                }
            }

            prevFI = r.ForceIndex;
        }

        return results;
    }

    // parameter validation
    private static void ValidateForceIndex(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Force Index.");
        }
    }
}
