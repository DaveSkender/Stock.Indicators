namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // ACCUMULATION/DISTRIBUTION LINE
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<AdlResult> GetAdl<TQuote>(
        this IEnumerable<TQuote> quotes,
        int? smaPeriods = null)
        where TQuote : IQuote
    {

        // convert quotes
        List<QuoteD> quotesList = quotes.ConvertToList();

        // check parameter arguments
        ValidateAdl(quotes, smaPeriods);

        // initialize
        List<AdlResult> results = new(quotesList.Count);
        double prevAdl = 0;

        // roll through quotes
        for (int i = 0; i < quotesList.Count; i++)
        {
            QuoteD q = quotesList[i];
            int index = i + 1;

            double mfm = (q.High == q.Low) ? 0 : ((q.Close - q.Low) - (q.High - q.Close)) / (q.High - q.Low);
            double mfv = mfm * q.Volume;
            double adl = mfv + prevAdl;

            AdlResult result = new()
            {
                Date = q.Date,
                MoneyFlowMultiplier = mfm,
                MoneyFlowVolume = mfv,
                Adl = adl
            };
            results.Add(result);

            prevAdl = adl;

            // optional SMA
            if (smaPeriods != null && index >= smaPeriods)
            {
                double sumSma = 0;
                for (int p = index - (int)smaPeriods; p < index; p++)
                {
                    sumSma += results[p].Adl;
                }

                result.AdlSma = sumSma / smaPeriods;
            }
        }

        return results;
    }


    // convert to quotes
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Convert"]/*' />
    ///
    public static IEnumerable<Quote> ConvertToQuotes(
        this IEnumerable<AdlResult> results)
    {
        return results
          .Select(x => new Quote
          {
              Date = x.Date,
              Open = (decimal)x.Adl,
              High = (decimal)x.Adl,
              Low = (decimal)x.Adl,
              Close = (decimal)x.Adl,
              Volume = (decimal)x.Adl
          })
          .ToList();
    }


    // parameter validation
    private static void ValidateAdl<TQuote>(
        IEnumerable<TQuote> quotes,
        int? smaPeriods)
        where TQuote : IQuote
    {

        // check parameter arguments
        if (smaPeriods is not null and <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(smaPeriods), smaPeriods,
                "SMA periods must be greater than 0 for ADL.");
        }

        // check quotes
        int qtyHistory = quotes.Count();
        int minHistory = 2;
        if (qtyHistory < minHistory)
        {
            string message = string.Format(EnglishCulture,
                "Insufficient quotes provided for Accumulation/Distribution Line.  " +
                "You provided {0} periods of quotes when at least {1} are required.",
                qtyHistory, minHistory);

            throw new BadQuotesException(nameof(quotes), message);
        }
    }
}
