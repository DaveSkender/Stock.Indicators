namespace Skender.Stock.Indicators;

// MONEY FLOW INDEX (SERIES)

public static partial class Indicator
{
    internal static List<MfiResult> CalcMfi(
        this List<QuoteD> qdList,
        int lookbackPeriods)
    {
        // check parameter arguments
        Mfi.Validate(lookbackPeriods);

        // initialize
        int length = qdList.Count;
        List<MfiResult> results = new(length);

        double[] tp = new double[length];  // true price
        double[] mf = new double[length];  // raw MF value
        int[] direction = new int[length]; // direction

        double? prevTP = null;

        // roll through quotes, to get preliminary data
        for (int i = 0; i < length; i++)
        {
            QuoteD q = qdList[i];
            double mfi;

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

            // add money flow index
            if (i >= lookbackPeriods)
            {
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
                    double mfRatio = sumPosMFs / sumNegMFs;
                    mfi = 100 - (100 / (1 + mfRatio));
                }

                // handle no negative case
                else
                {
                    mfi = 100;
                }
            }
            else
            {
                mfi = double.NaN;
            }

            results.Add(new MfiResult(
                Timestamp: q.Timestamp,
                Mfi: mfi.NaN2Null()));

            prevTP = tp[i];
        }

        return results;
    }
}
