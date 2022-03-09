namespace Skender.Stock.Indicators;
#nullable disable

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
        List<QuoteD> quotesList = quotes.ConvertToList();

        // check parameter arguments
        ValidateCci(lookbackPeriods);

        // initialize
        List<CciResult> results = new(quotesList.Count);

        // roll through quotes
        for (int i = 0; i < quotesList.Count; i++)
        {
            QuoteD q = quotesList[i];
            int index = i + 1;

            CciResult result = new()
            {
                Date = q.Date,
                Tp = (q.High + q.Low + q.Close) / 3
            };
            results.Add(result);

            if (index >= lookbackPeriods)
            {
                // average TP over lookback
                double avgTp = 0;
                for (int p = index - lookbackPeriods; p < index; p++)
                {
                    CciResult d = results[p];
                    avgTp += (double)d.Tp;
                }

                avgTp /= lookbackPeriods;

                // average Deviation over lookback
                double avgDv = 0;
                for (int p = index - lookbackPeriods; p < index; p++)
                {
                    CciResult d = results[p];
                    avgDv += Math.Abs(avgTp - (double)d.Tp);
                }

                avgDv /= lookbackPeriods;

                result.Cci = (avgDv == 0) ? null
                    : (result.Tp - avgTp) / (0.015 * avgDv);
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
