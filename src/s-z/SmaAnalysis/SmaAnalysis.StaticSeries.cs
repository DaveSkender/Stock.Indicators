namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating Simple Moving Average (SMA) with extended analysis.
/// </summary>
public static partial class SmaAnalysis
{
    /// <summary>
    /// Converts a source list to a list of SMA analysis results.
    /// </summary>
    /// /// <param name="source">The source list to analyze.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the SMA calculation.</param>
    /// <returns>A read-only list of SMA analysis results.</returns>
    public static IReadOnlyList<SmaAnalysisResult> ToSmaAnalysis(
        this IReadOnlyList<IReusable> source,
        int lookbackPeriods)
    {
        // initialize
        List<SmaAnalysisResult> results = source
            .ToSma(lookbackPeriods)
            .Select(static s => new SmaAnalysisResult(s.Timestamp, s.Sma))
            .ToList();

        // roll through source values
        for (int i = lookbackPeriods - 1; i < results.Count; i++)
        {
            SmaAnalysisResult r = results[i];
            double sma = r.Sma ?? double.NaN;

            double sumMad = 0;
            double sumMse = 0;
            double sumMape = 0;

            for (int p = i + 1 - lookbackPeriods; p <= i; p++)
            {
                IReusable s = source[p];

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
