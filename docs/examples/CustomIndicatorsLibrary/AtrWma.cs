using System.Text.Json.Serialization;
using Skender.Stock.Indicators;

namespace Custom.Stock.Indicators;

// Custom results class
// This inherits many of the extension methods, like .RemoveWarmupPeriods()
public record AtrWmaResult
(
    DateTime Timestamp,
    double? AtrWma = null
) : IReusable
{
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => AtrWma.Null2NaN();
}

public static class CustomIndicators
{
    // Custom ATR WMA calculation
    public static IReadOnlyList<AtrWmaResult> GetAtrWma(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods)
    {
        // sort quotes and convert to list
        List<IQuote> quotesList = quotes
            .OrderBy(x => x.Timestamp)
            .ToList();

        // initialize results
        List<AtrWmaResult> results = new(quotesList.Count);

        // perform pre-requisite calculations to get ATR values
        List<AtrResult> atrResults = quotes
            .ToAtr(lookbackPeriods)
            .ToList();

        // roll through source values
        for (int i = 0; i < quotesList.Count; i++)
        {
            IQuote q = quotesList[i];
            double atrWma = double.NaN;

            // only do calculations after uncalculable periods
            if (i >= lookbackPeriods - 1)
            {
                double sumWma = 0;
                double sumAtr = 0;

                for (int p = i - lookbackPeriods + 1; p <= i; p++)
                {
                    double close = (double)quotesList[p].Close;
                    double atr = atrResults[p].Atr ?? double.NaN;

                    sumWma += atr * close;
                    sumAtr += atr;
                }

                atrWma = sumWma / sumAtr;
            }

            // add record to results
            results.Add(new AtrWmaResult(
                Timestamp: q.Timestamp,
                AtrWma: atrWma.NaN2Null()));
        }

        return results;
    }
}
