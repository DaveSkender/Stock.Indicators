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
        List<QuoteD> quotesList = quotes.ToQuoteD();

        // check parameter arguments
        ValidateChandelier(lookbackPeriods, multiplier);

        // initialize
        List<ChandelierResult> results = new(quotesList.Count);
        List<AtrResult> atrResult = GetAtr(quotes, lookbackPeriods).ToList();  // uses ATR

        // roll through quotes
        for (int i = 0; i < quotesList.Count; i++)
        {
            QuoteD q = quotesList[i];

            ChandelierResult result = new()
            {
                Date = q.Date
            };

            // add exit values
            if (i + 1 >= lookbackPeriods)
            {
                double? atr = (double?)atrResult[i].Atr;

                switch (type)
                {
                    case ChandelierType.Long:

                        double maxHigh = 0;
                        for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                        {
                            QuoteD d = quotesList[p];
                            if (d.High > maxHigh)
                            {
                                maxHigh = d.High;
                            }
                        }

                        result.ChandelierExit = (decimal?)(maxHigh - (atr * multiplier));
                        break;

                    case ChandelierType.Short:

                        double minLow = double.MaxValue;
                        for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                        {
                            QuoteD d = quotesList[p];
                            if (d.Low < minLow)
                            {
                                minLow = d.Low;
                            }
                        }

                        result.ChandelierExit = (decimal?)(minLow + (atr * multiplier));
                        break;

                    default:
                        break;
                }
            }

            results.Add(result);
        }

        return results;
    }

    // parameter validation
    private static void ValidateChandelier(
        int lookbackPeriods,
        double multiplier)
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
    }
}
