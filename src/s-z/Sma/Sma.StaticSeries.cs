namespace Skender.Stock.Indicators;

/// <summary>
/// Simple Moving Average (SMA) indicator.
/// </summary>
public static partial class Sma
{
    /// <summary>
    /// Calculates the Simple Moving Average (SMA) for a given source list and lookback period.
    /// </summary>
    /// <param name="source">The source list to analyze.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the SMA calculation.</param>
    /// <returns>A read-only list of SMA results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback period is less than 1.</exception>
    public static IReadOnlyList<SmaResult> ToSma(
        this IReadOnlyList<IReusable> source,
        int lookbackPeriods)
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(source);
        Validate(lookbackPeriods);

        // initialize
        int length = source.Count;
        SmaResult[] results = new SmaResult[length];

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            IReusable s = source[i];

            if (i >= lookbackPeriods - 1)
            {
                double sum = 0;
                int start = i - lookbackPeriods + 1;

                for (int p = start; p <= i; p++)
                {
                    sum += source[p].Value;
                }

                results[i] = new SmaResult(
                    Timestamp: s.Timestamp,
                    Sma: (sum / lookbackPeriods).NaN2Null());
            }
            else
            {
                results[i] = new SmaResult(
                    Timestamp: s.Timestamp,
                    Sma: null);
            }
        }

        return results;
    }
}
