namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating Simple Moving Average (SMA).
/// </summary>
public static partial class Sma
{
    /// <summary>
    /// Calculates the Simple Moving Average (SMA) for a given source list and lookback period.
    /// </summary>
    /// <typeparam name="T">The type of the source items, must implement IReusable.</typeparam>
    /// <param name="source">The source list to analyze.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the SMA calculation.</param>
    /// <returns>A read-only list of SMA results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the source list is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback period is less than 1.</exception>
    public static IReadOnlyList<SmaResult> ToSma<T>(
        this IReadOnlyList<T> source,
        int lookbackPeriods)
        where T : IReusable
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(source);
        Validate(lookbackPeriods);

        // initialize
        int length = source.Count;
        SmaResult[] results = new SmaResult[length];
        double[] values = new double[length];

        for (int i = 0; i < length; i++)
        {
            values[i] = source[i].Value;
        }

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            T s = source[i];

            double sma;

            if (i >= lookbackPeriods - 1)
            {
                double sum = 0;
                int end = i + 1;
                int start = end - lookbackPeriods;

                for (int p = start; p < end; p++)
                {
                    sum += source[p].Value;
                }

                sma = sum / lookbackPeriods;
            }
            else
            {
                sma = double.NaN;
            }

            results[i] = new SmaResult(
                Timestamp: s.Timestamp,
                Sma: sma.NaN2Null());
        }

        return new List<SmaResult>(results);
    }
}
