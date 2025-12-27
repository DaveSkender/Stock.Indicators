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
        SmaResult[] results = new SmaResult[length];

        for (int i = 0; i < length; i++)
        {
            IReusable s = source[i];

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
                Sma: sma.NaN2Null()); results[i] = new SmaResult(
                Timestamp: source[i].Timestamp,
                Sma: sma.NaN2Null());
        }

        return results;
    }

    public static IReadOnlyList<SmaResult> ToSmaArray(
        this IReadOnlyList<IReusable> source,
        int lookbackPeriods)
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(source);
        Validate(lookbackPeriods);

        // initialize
        int length = source.Count;

        // convert to array of values
        double[] srcValues = source.ToValueArray();

        // calculate using array-based method
        double[] smaValues = srcValues.ToSmaArrayLoop(lookbackPeriods);

        // convert back to result objects
        SmaResult[] results = new SmaResult[length];

        for (int i = 0; i < length; i++)
        {
            results[i] = new SmaResult(
                Timestamp: source[i].Timestamp,
                Sma: smaValues[i].NaN2Null());
        }

        return results;
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
    public static double[] ToSmaArrayLoop(
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
            results[i] = Sma.Increment(source, lookbackPeriods, i);
        }

        return results;
    }

    public static double[] ToSmaArrayRoll(
        this double[] source,
        int lookbackPeriods)
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(source);
        Validate(lookbackPeriods);

        // initialize
        int length = source.Length;
        double[] results = new double[length];
        double sum = double.NaN;

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            (results[i], sum, _) = Increment(source, lookbackPeriods, i, sum);
        }

        return results;
    }

}
