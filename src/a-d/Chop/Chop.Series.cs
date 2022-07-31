namespace Skender.Stock.Indicators;

// CHOPPINESS INDEX (SERIES)
public static partial class Indicator
{
    internal static List<ChopResult> CalcChop(
        this List<QuoteD> qdList,
        int lookbackPeriods)
    {
        // check parameter arguments
        ValidateChop(lookbackPeriods);

        // initialize
        double sum;
        double high;
        double low;
        double range;

        int length = qdList.Count;
        List<ChopResult> results = new(length);
        double[] trueHigh = new double[length];
        double[] trueLow = new double[length];
        double[] trueRange = new double[length];

        // roll through quotes
        for (int i = 0; i < qdList.Count; i++)
        {
            ChopResult r = new(qdList[i].Date);
            results.Add(r);

            if (i > 0)
            {
                trueHigh[i] = Math.Max(qdList[i].High, qdList[i - 1].Close);
                trueLow[i] = Math.Min(qdList[i].Low, qdList[i - 1].Close);
                trueRange[i] = trueHigh[i] - trueLow[i];

                // calculate CHOP

                if (i >= lookbackPeriods)
                {
                    // reset measurements
                    sum = trueRange[i];
                    high = trueHigh[i];
                    low = trueLow[i];

                    // iterate over lookback window
                    for (int j = 1; j < lookbackPeriods; j++)
                    {
                        sum += trueRange[i - j];
                        high = Math.Max(high, trueHigh[i - j]);
                        low = Math.Min(low, trueLow[i - j]);
                    }

                    range = high - low;

                    // calculate CHOP
                    if (range != 0)
                    {
                        r.Chop = 100 * (Math.Log(sum / range) / Math.Log(lookbackPeriods));
                    }
                }
            }
        }

        return results;
    }

    // parameter validation
    private static void ValidateChop(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 1 for CHOP.");
        }
    }
}
