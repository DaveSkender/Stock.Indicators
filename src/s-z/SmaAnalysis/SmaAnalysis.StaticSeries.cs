namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating Simple Moving Average (SMA) with extended analysis.
/// </summary>
public static partial class Sma
{
    /// <summary>
    /// Converts a source list to a list of SMA analysis results.
    /// </summary>
    /// <typeparam name="T">The type of the source items, must implement IReusable.</typeparam>
    /// <param name="source">The source list to analyze.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the SMA calculation.</param>
    /// <returns>A read-only list of SMA analysis results.</returns>
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
