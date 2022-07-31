using Skender.Stock.Indicators;

namespace Custom.Stock.Indicators;

// Custom results class
// This inherits many of the extension methods, like .RemoveWarmupPeriods()
public class AtrWmaResult : ResultBase
{
    public double? AtrWma { get; set; }
}

public static class CustomIndicators
{
    // Custom ATR WMA calculation
    public static IEnumerable<AtrWmaResult> GetAtrWma<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote
    {
        // sort quotes and convert to list
        List<TQuote> quotesList = quotes
            .OrderBy(x => x.Date)
            .ToList();

        // initialize results
        List<AtrWmaResult> results = new(quotesList.Count);

        // perform pre-requisite calculations to get ATR values
        List<AtrResult> atrResults = quotes
            .GetAtr(lookbackPeriods)
            .ToList();

        // roll through quotes
        for (int i = 0; i < quotesList.Count; i++)
        {
            TQuote q = quotesList[i];

            AtrWmaResult r = new()
            {
                Date = q.Date
            };

            // only do calculations after uncalculable periods
            if (i >= lookbackPeriods - 1)
            {
                double? sumWma = 0;
                double? sumAtr = 0;

                for (int p = i - lookbackPeriods + 1; p <= i; p++)
                {
                    double close = (double)quotesList[p].Close;
                    double? atr = atrResults[p]?.Atr;

                    sumWma += atr * close;
                    sumAtr += atr;
                }

                r.AtrWma = sumWma / sumAtr;
            }

            // add record to results
            results.Add(r);
        }

        return results;
    }
}
