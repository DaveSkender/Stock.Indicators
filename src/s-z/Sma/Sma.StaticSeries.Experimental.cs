using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Skender.Stock.Indicators;

// EXPERIMENTAL METHODS
public static partial class Sma
{
    /// <remarks>
    /// EXPERIMENTAL: Internal conversion to/from array looping method.
    /// </remarks>
    /// <inheritdoc cref="ToSma(IReadOnlyList{IReusable}, int)"/>
    public static IReadOnlyList<SmaResult> ToSmaArray(
        this IReadOnlyList<IReusable> source,
        int lookbackPeriods)
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(source);
        Validate(lookbackPeriods);

        // initialize
        int length = source.Count;

        // calculate using array-based method
        double[] smaValues = source
            .ToValuesArray()
            .ToSmaArrayLoop(lookbackPeriods);

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

    /// <remarks>
    /// EXPERIMENTAL: Array I/O with looping sum.
    /// </remarks>
    /// <inheritdoc cref="ToSma(IReadOnlyList{IReusable}, int)"/>
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

    /// <remarks>
    /// EXPERIMENTAL: Array I/O with rolling sum.
    /// </remarks>
    /// <inheritdoc cref="ToSma(IReadOnlyList{IReusable}, int)"/>
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

    /// <remarks>
    /// EXPERIMENTAL: Array I/O with SIMD sum.
    /// </remarks>
    /// <inheritdoc cref="ToSma(IReadOnlyList{IReusable}, int)"/>
    public static double[] ToSmaArraySimd(
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
            results[i] = Sma.IncrementSimd(source, lookbackPeriods, i);
        }

        return results;
    }

    /// <summary>
    /// Simple moving average calculation.
    /// </summary>
    /// <param name="source">Array of values.</param>
    /// <param name="lookbackPeriods">Window to evaluate, prior to 'endIndex'.</param>
    /// <param name="endIndex">Index position to evaluate.</param>
    /// <returns>Simple moving average or <see langword="double.NaN"/> when incalculable.</returns>
    internal static double Increment(
        double[] source,
        int lookbackPeriods,
        int endIndex)
    {
        if (endIndex < lookbackPeriods - 1 || endIndex >= source.Length)
        {
            return double.NaN;
        }

        double sum = 0;
        for (int i = endIndex - lookbackPeriods + 1; i <= endIndex; i++)
        {
            sum += source[i];
        }

        return sum / lookbackPeriods;
    }

    /// <summary>
    /// Simple moving average calculation with rolling sum optimization.
    /// </summary>
    /// <param name="source">Array of values.</param>
    /// <param name="lookbackPeriods">Window to evaluate, prior to 'endIndex'.</param>
    /// <param name="endIndex">Index position to evaluate.</param>
    /// <param name="priorSum">Previous sum of the window, or <see langword="double.NaN"/> to trigger full sum calculation.</param>
    /// <param name="dropValue">Optional value being dropped from the rolling window. If null, calculated as source[endIndex - lookbackPeriods].</param>
    /// <returns>Tuple containing the simple moving average (or <see langword="double.NaN"/> when incalculable), the new sum, and the drop value used.</returns>
    internal static (double average, double newSum, double dropValue) Increment(
        double[] source,
        int lookbackPeriods,
        int endIndex,
        double priorSum,
        double? dropValue = null)
    {
        if (endIndex < lookbackPeriods - 1 || endIndex >= source.Length)
        {
            return (double.NaN, double.NaN, double.NaN);
        }

        // calculate drop value if not provided, handling initial case
        double actualDropValue = dropValue
            ?? (endIndex >= lookbackPeriods ? source[endIndex - lookbackPeriods] : 0);

        double newSum;

        // fallback to brute force if prior sum is not available
        if (double.IsNaN(priorSum))
        {
            newSum = 0;
            for (int i = endIndex - lookbackPeriods + 1; i <= endIndex; i++)
            {
                newSum += source[i];
            }
        }
        else
        {
            // use rolling sum optimization
            newSum = priorSum - actualDropValue + source[endIndex];
        }

        return (newSum / lookbackPeriods, newSum, actualDropValue);
    }

    /// <summary>
    /// Simple moving average calculation using SIMD.
    /// </summary>
    /// <param name="source">Array of prices.</param>
    /// <param name="lookbackPeriods">Period to evaluate.</param>
    /// <param name="endIndex">Index position to evaluate.</param>
    /// <returns>Simple moving average or <see langword="double.NaN"/> when incalculable.</returns>
    /// <remarks>Caution: this experimental method may have rounding differences.</remarks>
    [ExcludeFromCodeCoverage]  // experimental SIMD code
    internal static double IncrementSimd(
        double[] source,
        int lookbackPeriods,
        int endIndex)
    {
        if (endIndex < lookbackPeriods - 1 || endIndex >= source.Length)
        {
            return double.NaN;
        }

        double sum = 0;
        int start = endIndex - lookbackPeriods + 1;
        int end = endIndex + 1;

        // SIMD optimization
        int vectorSize = Vector<double>.Count;
        int simdEnd = start + ((end - start) / vectorSize * vectorSize);

        for (int p = start; p < simdEnd; p += vectorSize)
        {
            Vector<double> vector = new(source, p);
            sum += Vector.Sum(vector);
        }

        for (int p = simdEnd; p < end; p++)
        {
            sum += source[p];
        }

        return sum / lookbackPeriods;
    }
}
