namespace Skender.Stock.Indicators;

// MONEY FLOW INDEX (SERIES)
public static partial class Indicator
{
    internal static List<MfiResult> CalcMfi(
        this List<QuoteD> qdList,
        int lookbackPeriods)
    {
        // check parameter arguments
        ValidateMfi(lookbackPeriods);

        // initialize
        int length = qdList.Count;
        List<MfiResult> results = new(length);
        double[] tp = new double[length];  // true price
        double[] mf = new double[length];  // raw MF value
        int[] direction = new int[length]; // direction

        double? prevTP = null;

        // roll through quotes, to get preliminary data
        for (int i = 0; i < qdList.Count; i++)
        {
            QuoteD q = qdList[i];

            MfiResult r = new(q.Date);
            results.Add(r);

            // true price
            tp[i] = (q.High + q.Low + q.Close) / 3;

            // raw money flow
            mf[i] = tp[i] * q.Volume;

            // direction
            if (prevTP == null || tp[i] == prevTP)
            {
                direction[i] = 0;
            }
            else if (tp[i] > prevTP)
            {
                direction[i] = 1;
            }
            else if (tp[i] < prevTP)
            {
                direction[i] = -1;
            }

            prevTP = tp[i];
        }

        // add money flow index
        for (int i = lookbackPeriods; i < results.Count; i++)
        {
            MfiResult r = results[i];

            double sumPosMFs = 0;
            double sumNegMFs = 0;

            for (int p = i + 1 - lookbackPeriods; p <= i; p++)
            {
                if (direction[p] == 1)
                {
                    sumPosMFs += mf[p];
                }
                else if (direction[p] == -1)
                {
                    sumNegMFs += mf[p];
                }
            }

            // calculate MFI normally
            if (sumNegMFs != 0)
            {
                double? mfRatio = sumPosMFs / sumNegMFs;
                r.Mfi = 100 - (100 / (1 + mfRatio));
            }

            // handle no negative case
            else
            {
                r.Mfi = 100;
            }
        }

        return results;
    }

    // parameter validation
    private static void ValidateMfi(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 1 for MFI.");
        }
    }
}
