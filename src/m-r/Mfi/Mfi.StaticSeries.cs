namespace Skender.Stock.Indicators;

/// <summary>
/// Money Flow Index (MFI) for a series of quotes indicator.
/// </summary>
public static partial class Mfi
{
    /// <summary>
    /// Converts a list of quotes to Money Flow Index (MFI) results.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">The number of periods to use for the MFI calculation. Default is 14.</param>
    /// <returns>A list of <see cref="MfiResult"/> containing the MFI values.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quotes list is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are out of range.</exception>
    public static IReadOnlyList<MfiResult> ToMfi(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14)
        => quotes
            .ToQuoteDList()
            .CalcMfi(lookbackPeriods);

    /// <summary>
    /// Calculates the Money Flow Index (MFI) for a list of quotes.
    /// </summary>
    /// <param name="quotes">The source list of quotes.</param>
    /// <param name="lookbackPeriods">The number of periods to use for the MFI calculation.</param>
    /// <returns>A list of <see cref="MfiResult"/> containing the MFI values.</returns>
    private static List<MfiResult> CalcMfi(
        this List<QuoteD> quotes,
        int lookbackPeriods)
    {
        // check parameter arguments
        Validate(lookbackPeriods);

        // initialize
        int length = quotes.Count;
        List<MfiResult> results = new(length);

        double[] tp = new double[length];  // true price
        double[] mf = new double[length];  // raw MF value
        int[] direction = new int[length]; // direction

        double? prevTp = null;

        // roll through source values, to get preliminary data
        for (int i = 0; i < length; i++)
        {
            QuoteD q = quotes[i];
            double mfi;

            // true price
            tp[i] = (q.High + q.Low + q.Close) / 3;

            // raw money flow
            mf[i] = tp[i] * q.Volume;

            // direction
            if (prevTp == null || tp[i] == prevTp)
            {
                direction[i] = 0;
            }
            else if (tp[i] > prevTp)
            {
                direction[i] = 1;
            }
            else if (tp[i] < prevTp)
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

            results.Add(new(
                Timestamp: q.Timestamp,
                Mfi: mfi.NaN2Null()));

            prevTp = tp[i];
        }

        return results;
    }
}
