namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // MONEY FLOW INDEX
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<MfiResult> GetMfi<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 14)
        where TQuote : IQuote
    {
        // convert quotes
        List<QuoteD> quotesList = quotes.ToQuoteD();

        // check parameter arguments
        ValidateMfi(lookbackPeriods);

        // initialize
        int length = quotesList.Count;
        List<MfiResult> results = new(length);
        double[] tp = new double[length];  // true price
        double[] mf = new double[length];  // raw MF value
        int[] direction = new int[length];   // direction

        double? prevTP = null;

        // roll through quotes, to get preliminary data
        for (int i = 0; i < quotesList.Count; i++)
        {
            QuoteD q = quotesList[i];

            MfiResult result = new()
            {
                Date = q.Date
            };

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

            results.Add(result);

            prevTP = tp[i];
        }

        // add money flow index
        for (int i = lookbackPeriods; i < results.Count; i++)
        {
            MfiResult r = results[i];
            int index = i + 1;

            double sumPosMFs = 0;
            double sumNegMFs = 0;

            for (int p = index - lookbackPeriods; p < index; p++)
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

            // handle no negative case
            if (sumNegMFs == 0)
            {
                r.Mfi = 100;
                continue;
            }

            // calculate MFI normally
            decimal mfRatio = (decimal)(sumPosMFs / sumNegMFs);

            r.Mfi = 100 - (100 / (1 + mfRatio));
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
