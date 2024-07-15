namespace Skender.Stock.Indicators;

// CHOPPINESS INDEX (SERIES)

public static partial class Indicator
{
    private static List<ChopResult> CalcChop(
        this List<QuoteD> qdList,
        int lookbackPeriods)
    {
        // check parameter arguments
        Chop.Validate(lookbackPeriods);

        // initialize
        int length = qdList.Count;
        List<ChopResult> results = new(length);
        double[] trueHigh = new double[length];
        double[] trueLow = new double[length];
        double[] trueRange = new double[length];

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            double? chop = null;

            if (i > 0)
            {
                trueHigh[i] = Math.Max(qdList[i].High, qdList[i - 1].Close);
                trueLow[i] = Math.Min(qdList[i].Low, qdList[i - 1].Close);
                trueRange[i] = trueHigh[i] - trueLow[i];

                // calculate CHOP

                if (i >= lookbackPeriods)
                {
                    // reset measurements
                    double sum = trueRange[i];
                    double high = trueHigh[i];
                    double low = trueLow[i];

                    // iterate over lookback window
                    for (int j = 1; j < lookbackPeriods; j++)
                    {
                        sum += trueRange[i - j];
                        high = Math.Max(high, trueHigh[i - j]);
                        low = Math.Min(low, trueLow[i - j]);
                    }

                    double range = high - low;

                    // calculate CHOP
                    if (range != 0)
                    {
                        chop = 100 * (Math.Log(sum / range) / Math.Log(lookbackPeriods));
                    }
                }
            }

            results.Add(new(
                Timestamp: qdList[i].Timestamp,
                Chop: chop));
        }

        return results;
    }
}
