namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating Simple Moving Average (SMA).
/// </summary>
public static partial class Sma
{
    /// <summary>
    /// Calculates the Simple Moving Average (SMA) for a given source list and lookback period.
    /// </summary>
    /// <param name="source">The source list to analyze.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the SMA calculation.</param>
    /// <returns>A read-only list of SMA results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the source list is null.</exception>
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

        // convert to array of values
        double[] values = new double[length];
        for (int i = 0; i < length; i++)
        {
            values[i] = source[i].Value;
        }

        // calculate using array-based method
        double[] smaValues = values.ToSma(lookbackPeriods);

        // convert back to result objects
        SmaResult[] results = new SmaResult[length];
        for (int i = 0; i < length; i++)
        {
            results[i] = new SmaResult(
                Timestamp: source[i].Timestamp,
                Sma: smaValues[i].NaN2Null());
        }

        return new List<SmaResult>(results);
    }

    /// <summary>
    /// Calculates the Simple Moving Average (SMA) for a given array of values and lookback period.
    /// This is an experimental high-performance method that operates on arrays.
    /// </summary>
    /// <param name="source">The source array of values to analyze.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the SMA calculation.</param>
    /// <returns>An array of SMA values with the same length as the input, where values before the lookback period are NaN.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the source array is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback period is less than 1.</exception>
    public static double[] ToSma(
        this double[] source,
        int lookbackPeriods)
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(source);
        Validate(lookbackPeriods);

        // initialize
        int length = source.Length;
        double[] results = new double[length];

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            if (i >= lookbackPeriods - 1)
            {
                double sum = 0;
                int end = i + 1;
                int start = end - lookbackPeriods;

                for (int p = start; p < end; p++)
                {
                    sum += source[p];
                }

                results[i] = sum / lookbackPeriods;
            }
            else
            {
                results[i] = double.NaN;
            }
        }

        return results;
    }
}

