namespace Skender.Stock.Indicators;

// FORCE INDEX (SERIES)

public static partial class Indicator
{
    private static List<ForceIndexResult> CalcForceIndex(
        this List<QuoteD> qdList,
        int lookbackPeriods)
    {
        // check parameter arguments
        ForceIndex.Validate(lookbackPeriods);

        // initialize
        int length = qdList.Count;
        List<ForceIndexResult> results = new(length);
        double? prevFi = null;
        double? sumRawFi = 0;
        double k = 2d / (lookbackPeriods + 1);

        // skip first period
        if (length > 0)
        {
            results.Add(new() { Timestamp = qdList[0].Timestamp });
        }

        // roll through quotes
        for (int i = 1; i < length; i++)
        {
            QuoteD q = qdList[i];
            double? fi = null;

            // raw Force Index
            double? rawFi = q.Volume * (q.Close - qdList[i - 1].Close);

            // calculate EMA
            if (i > lookbackPeriods)
            {
                fi = prevFi + k * (rawFi - prevFi);
            }

            // initialization period
            // TODO: update healing, without requiring specific indexing
            else
            {
                sumRawFi += rawFi;

                // first EMA value
                if (i == lookbackPeriods)
                {
                    fi = sumRawFi / lookbackPeriods;
                }
            }

            results.Add(new(
                Timestamp: q.Timestamp,
                ForceIndex: fi));

            prevFi = fi;
        }

        return results;
    }
}
