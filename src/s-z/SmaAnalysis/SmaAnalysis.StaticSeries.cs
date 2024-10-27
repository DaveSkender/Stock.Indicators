namespace Skender.Stock.Indicators;

// SIMPLE MOVING AVERAGE (ANALYSIS)

public static partial class Sma
{
    public static IReadOnlyList<SmaAnalysis> ToSmaAnalysis<T>(
        this IReadOnlyList<T> source,
        int lookbackPeriods)
        where T : IReusable
    {
        // initialize
        List<SmaAnalysis> results = source
            .ToSma(lookbackPeriods)
            .Select(s => new SmaAnalysis(s.Timestamp, s.Sma))
            .ToList();

        // roll through source values
        for (int i = lookbackPeriods - 1; i < results.Count; i++)
        {
            SmaAnalysis r = results[i];
            double sma = r.Sma ?? double.NaN;

            double sumMad = 0;
            double sumMse = 0;
            double sumMape = 0;

            for (int p = i + 1 - lookbackPeriods; p <= i; p++)
            {
                T s = source[p];

                sumMad += Math.Abs(s.Value - sma);
                sumMse += (s.Value - sma) * (s.Value - sma);

                sumMape += s.Value == 0 ? double.NaN
                    : Math.Abs(s.Value - sma) / s.Value;
            }

            results[i] = r with {

                // mean absolute deviation
                Mad = (sumMad / lookbackPeriods).NaN2Null(),

                // mean squared error
                Mse = (sumMse / lookbackPeriods).NaN2Null(),

                // mean absolute percent error
                Mape = (sumMape / lookbackPeriods).NaN2Null()
            };
        }

        return results;
    }
}
