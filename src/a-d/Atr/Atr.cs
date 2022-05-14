namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // AVERAGE TRUE RANGE
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<AtrResult> GetAtr<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 14)
        where TQuote : IQuote
    {
        // sort quotes
        List<TQuote> quotesList = quotes.SortToList();

        // check parameter arguments
        ValidateAtr(lookbackPeriods);

        // initialize
        List<AtrResult> results = new(quotesList.Count);
        decimal? prevAtr = 0;
        decimal prevClose = 0;
        decimal highMinusPrevClose = 0;
        decimal lowMinusPrevClose = 0;
        decimal? sumTr = 0;

        // roll through quotes
        for (int i = 0; i < quotesList.Count; i++)
        {
            TQuote q = quotesList[i];

            AtrResult result = new()
            {
                Date = q.Date
            };

            if (i > 0)
            {
                highMinusPrevClose = Math.Abs(q.High - prevClose);
                lowMinusPrevClose = Math.Abs(q.Low - prevClose);
            }

            decimal? tr = Math.Max(q.High - q.Low, Math.Max(highMinusPrevClose, lowMinusPrevClose));
            result.Tr = tr;

            if (i + 1 > lookbackPeriods)
            {
                // calculate ATR
                result.Atr = ((prevAtr * (lookbackPeriods - 1)) + tr) / lookbackPeriods;
                result.Atrp = (q.Close == 0) ? null : result.Atr / q.Close * 100;
                prevAtr = result.Atr;
            }
            else if (i + 1 == lookbackPeriods)
            {
                // initialize ATR
                sumTr += tr;
                result.Atr = sumTr / lookbackPeriods;
                result.Atrp = (q.Close == 0) ? null : result.Atr / q.Close * 100;
                prevAtr = result.Atr;
            }
            else
            {
                // only used for periods before ATR initialization
                sumTr += tr;
            }

            results.Add(result);
            prevClose = q.Close;
        }

        return results;
    }

    // parameter validation
    private static void ValidateAtr(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 1 for Average True Range.");
        }
    }
}
