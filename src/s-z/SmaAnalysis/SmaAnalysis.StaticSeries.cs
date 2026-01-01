namespace Skender.Stock.Indicators;

/// <summary>
/// Simple Moving Average (SMA) with extended analysis indicator.
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
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(source);
        Sma.Validate(lookbackPeriods);

        // initialize
        int length = source.Count;
        List<SmaAnalysisResult> results = new(length);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            if (i >= lookbackPeriods - 1)
            {
                double sma = Sma.Increment(source, lookbackPeriods, i);
                double sumMad = 0;
                double sumMse = 0;
                double sumMape = 0;

                for (int p = i - lookbackPeriods + 1; p <= i; p++)
                {
                    IReusable s = source[p];

                    sumMad += Math.Abs(s.Value - sma);
                    sumMse += (s.Value - sma) * (s.Value - sma);

                    sumMape += s.Value == 0 ? double.NaN
                        : Math.Abs(s.Value - sma) / s.Value;
                }

                results.Add(new SmaAnalysisResult(
                    Timestamp: source[i].Timestamp,
                    Sma: sma.NaN2Null(),
                    Mad: (sumMad / lookbackPeriods).NaN2Null(),
                    Mse: (sumMse / lookbackPeriods).NaN2Null(),
                    Mape: (sumMape / lookbackPeriods).NaN2Null()));
            }
            else
            {
                results.Add(new SmaAnalysisResult(
                    Timestamp: source[i].Timestamp,
                    Sma: null,
                    Mad: null,
                    Mse: null,
                    Mape: null));
            }
        }

        return results;
    }
}
