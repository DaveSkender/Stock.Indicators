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
        List<QuoteD> quotesList = quotes.ToQuoteD();

        // check parameter arguments
        ValidateAdl(smaPeriods);

        // initialize
        List<AdlResult> results = new(quotesList.Count);
        double prevAdl = 0;

        // roll through quotes
        for (int i = 0; i < quotesList.Count; i++)
        {
            QuoteD q = quotesList[i];
            int index = i + 1;

            double mfm = (q.High == q.Low) ? 0 : (q.Close - q.Low - (q.High - q.Close)) / (q.High - q.Low);
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

    // parameter validation
    private static void ValidateAdl(
        int? smaPeriods)
    {
        // check parameter arguments
        if (smaPeriods is not null and <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(smaPeriods), smaPeriods,
                "SMA periods must be greater than 0 for ADL.");
        }
    }
}
