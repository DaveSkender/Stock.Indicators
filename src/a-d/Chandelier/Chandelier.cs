namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // CHANDELIER EXIT
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<ChandelierResult> GetChandelier<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 22,
        double multiplier = 3,
        ChandelierType type = ChandelierType.Long)
        where TQuote : IQuote
    {

        // convert quotes
        List<QuoteD> quotesList = quotes.ConvertToList();

        // check parameter arguments
        ValidateChandelier(quotes, lookbackPeriods, multiplier);

        // initialize
        List<ChandelierResult> results = new(quotesList.Count);
        List<AtrResult> atrResult = GetAtr(quotes, lookbackPeriods).ToList();  // uses ATR

        // roll through quotes
        for (int i = 0; i < quotesList.Count; i++)
        {
            QuoteD q = quotesList[i];
            int index = i + 1;

            ChandelierResult result = new()
            {
                Date = q.Date
            };

            // add exit values
            if (index >= lookbackPeriods)
            {

                double? atr = (double?)atrResult[i].Atr;

                switch (type)
                {
                    case ChandelierType.Long:

                        double maxHigh = 0;
                        for (int p = index - lookbackPeriods; p < index; p++)
                        {
                            QuoteD d = quotesList[p];
                            if (d.High > maxHigh)
                            {
                                maxHigh = d.High;
                            }
                        }

                        result.ChandelierExit = (decimal?)(maxHigh - atr * multiplier);
                        break;

                    case ChandelierType.Short:

                        double minLow = double.MaxValue;
                        for (int p = index - lookbackPeriods; p < index; p++)
                        {
                            QuoteD d = quotesList[p];
                            if (d.Low < minLow)
                            {
                                minLow = d.Low;
                            }
                        }

                        result.ChandelierExit = (decimal?)(minLow + atr * multiplier);
                        break;

                    default:
                        break;
                }
            }

            results.Add(result);
        }

        return results;
    }


    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<ChandelierResult> RemoveWarmupPeriods(
        this IEnumerable<ChandelierResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.ChandelierExit != null);

        return results.Remove(removePeriods);
    }


    // parameter validation
    private static void ValidateChandelier<TQuote>(
        IEnumerable<TQuote> quotes,
        int lookbackPeriods,
        double multiplier)
        where TQuote : IQuote
    {

        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Chandelier Exit.");
        }

        if (multiplier <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(multiplier), multiplier,
                "Multiplier must be greater than 0 for Chandelier Exit.");
        }

        // check quotes
        int qtyHistory = quotes.Count();
        int minHistory = lookbackPeriods + 1;
        if (qtyHistory < minHistory)
        {
            string message = "Insufficient quotes provided for Chandelier Exit.  " +
                string.Format(EnglishCulture,
                "You provided {0} periods of quotes when at least {1} are required.",
                qtyHistory, minHistory);

            throw new BadQuotesException(nameof(quotes), message);
        }
    }
}
