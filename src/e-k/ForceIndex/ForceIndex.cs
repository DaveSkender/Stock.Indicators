namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // FORCE INDEX
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<ForceIndexResult> GetForceIndex<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote
    {
        // convert quotes
        List<QuoteD> quotesList = quotes.ConvertToList();

        // check parameter arguments
        ValidateForceIndex(lookbackPeriods);

        // initialize
        int length = quotesList.Count;
        List<ForceIndexResult> results = new(length);
        double? prevClose = null, prevFI = null, sumRawFI = 0;
        double k = 2d / (lookbackPeriods + 1);

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            QuoteD q = quotesList[i];
            int index = i + 1;

            ForceIndexResult r = new()
            {
                Date = q.Date
            };
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
            if (index > lookbackPeriods + 1)
            {
                r.ForceIndex = prevFI + (k * (rawFI - prevFI));
            }

            // initialization period
            else
            {
                sumRawFI += rawFI;

                // first EMA value
                if (index == lookbackPeriods + 1)
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
