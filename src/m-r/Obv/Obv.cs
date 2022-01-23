namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // ON-BALANCE VOLUME
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<ObvResult> GetObv<TQuote>(
        this IEnumerable<TQuote> quotes,
        int? smaPeriods = null)
        where TQuote : IQuote
    {
        // convert quotes
        List<QuoteD> quotesList = quotes.ConvertToList();

        // check parameter arguments
        ValidateObv(smaPeriods);

        // initialize
        List<ObvResult> results = new(quotesList.Count);

        double? prevClose = null;
        double obv = 0;

        // roll through quotes
        for (int i = 0; i < quotesList.Count; i++)
        {
            QuoteD q = quotesList[i];
            int index = i + 1;

            if (prevClose == null || q.Close == prevClose)
            {
                // no change to OBV
            }
            else if (q.Close > prevClose)
            {
                obv += q.Volume;
            }
            else if (q.Close < prevClose)
            {
                obv -= q.Volume;
            }

            ObvResult result = new()
            {
                Date = q.Date,
                Obv = obv
            };
            results.Add(result);

            prevClose = q.Close;

            // optional SMA
            if (smaPeriods != null && index > smaPeriods)
            {
                double sumSma = 0;
                for (int p = index - (int)smaPeriods; p < index; p++)
                {
                    sumSma += results[p].Obv;
                }

                result.ObvSma = sumSma / smaPeriods;
            }
        }

        return results;
    }

    // convert to quotes
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Convert"]/*' />
    ///
    public static IEnumerable<Quote> ConvertToQuotes(
        this IEnumerable<ObvResult> results)
    {
        return results
          .Select(x => new Quote
          {
              Date = x.Date,
              Open = (decimal)x.Obv,
              High = (decimal)x.Obv,
              Low = (decimal)x.Obv,
              Close = (decimal)x.Obv,
              Volume = (decimal)x.Obv
          })
          .ToList();
    }

    // parameter validation
    private static void ValidateObv(
        int? smaPeriods)
    {
        // check parameter arguments
        if (smaPeriods is not null and <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(smaPeriods), smaPeriods,
                "SMA periods must be greater than 0 for OBV.");
        }
    }
}
