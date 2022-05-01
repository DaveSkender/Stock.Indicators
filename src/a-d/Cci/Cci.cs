namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // COMMODITY CHANNEL INDEX
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<CciResult> GetCci<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 20)
        where TQuote : IQuote
    {
        // convert quotes
        List<QuoteD> quotesList = quotes.ToQuoteD();

        // check parameter arguments
        ValidateCci(lookbackPeriods);

        // initialize
        int length = quotesList.Count;
        List<CciResult> results = new(length);
        double[] tp = new double[length];

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            QuoteD q = quotesList[i];
            tp[i] = (q.High + q.Low + q.Close) / 3d;

            CciResult result = new()
            {
                Date = q.Date
            };
            results.Add(result);

            if (i + 1 >= lookbackPeriods)
            {
                // average TP over lookback
                double avgTp = 0;
                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    avgTp += tp[p];
                }

                avgTp /= lookbackPeriods;

                // average Deviation over lookback
                double avgDv = 0;
                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    avgDv += Math.Abs(avgTp - tp[p]);
                }

                avgDv /= lookbackPeriods;

                result.Cci = (avgDv == 0) ? null
                    : (tp[i] - avgTp) / (0.015 * avgDv);
            }
        }

        return results;
    }

    // parameter validation
    private static void ValidateCci(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Commodity Channel Index.");
        }
    }
}
