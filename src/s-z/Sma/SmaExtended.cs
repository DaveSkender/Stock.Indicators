namespace Skender.Stock.Indicators;
#nullable disable

public static partial class Indicator
{
    // SIMPLE MOVING AVERAGE (EXTENDED VERSION)
    /// <include file='./info.xml' path='indicator/type[@name="Extended"]/*' />
    ///
    public static IEnumerable<SmaExtendedResult> GetSmaExtended<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote
    {
        // convert quotes
        List<BaseQuote> quotesList = quotes.ToBaseQuote(CandlePart.Close);

        // initialize
        List<SmaExtendedResult> results = GetSma(quotes, lookbackPeriods)
            .Select(x => new SmaExtendedResult { Date = x.Date, Sma = x.Sma })
            .ToList();

        // roll through quotes
        for (int i = lookbackPeriods - 1; i < results.Count; i++)
        {
            SmaExtendedResult r = results[i];
            double sma = (double)r.Sma;

            double sumMad = 0;
            double sumMse = 0;
            double? sumMape = 0;

            for (int p = i + 1 - lookbackPeriods; p <= i; p++)
            {
                BaseQuote d = quotesList[p];
                double close = d.Value;

                sumMad += Math.Abs(close - sma);
                sumMse += (close - sma) * (close - sma);

                sumMape += (close == 0) ? null
                    : Math.Abs(close - sma) / close;
            }

            // mean absolute deviation
            r.Mad = sumMad / lookbackPeriods;

            // mean squared error
            r.Mse = sumMse / lookbackPeriods;

            // mean absolute percent error
            r.Mape = sumMape / lookbackPeriods;
        }

        return results;
    }
}
