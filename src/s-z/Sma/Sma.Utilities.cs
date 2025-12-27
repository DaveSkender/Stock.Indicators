using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Skender.Stock.Indicators;

/// <summary>
/// Provides utility methods for Simple Moving Average (SMA) calculations.
/// </summary>
public static partial class Sma
{
    /// <summary>
    /// Simple moving average calculation.
    /// </summary>
    /// <param name="values">List of chainable values.</param>
    /// <param name="lookbackPeriods">Window to evaluate, prior to 'endIndex'.</param>
    /// <param name="endIndex">Index position to evaluate or last position when <see langword="null"/>.</param>
    /// <returns>Simple moving average or <see langword="null"/> if incalculable <see langword="double.NaN"/> values are in range.</returns>
    public static double? Average(  // public API only
        this IReadOnlyList<IReusable> values,
        int lookbackPeriods,
        int? endIndex = null)
    {
        ArgumentNullException.ThrowIfNull(values);

        return Increment(
            values,
            lookbackPeriods,
            endIndex ?? (values.Count - 1))
           .NaN2Null();
    }

    /// <summary>
    /// Simple moving average calculation.
    /// </summary>
    /// <param name="source">List of chainable values.</param>
    /// <param name="lookbackPeriods">Window to evaluate, prior to 'endIndex'.</param>
    /// <param name="endIndex">Index position to evaluate.</param>
    /// <returns>Simple moving average or <see langword="double.NaN"/> when incalculable.</returns>
    internal static double Increment(
        IReadOnlyList<IReusable> source,
        int lookbackPeriods,
        int endIndex)
    {
        if (endIndex < lookbackPeriods - 1 || endIndex >= source.Count)
        {
            return double.NaN;
        }

        double sum = 0;
        for (int i = endIndex - lookbackPeriods + 1; i <= endIndex; i++)
        {
            sum += source[i].Value;
        }

        return sum / lookbackPeriods;

        // TODO: apply this SMA increment method more widely in other indicators (see EMA example)
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

        int startIndex = endIndex - lookbackPeriods + 1;
        Vector<double> sumVector = Vector<double>.Zero;
        int simdWidth = Vector<double>.Count;

        int j = 0;
        // SIMD loop for vectorized addition
        for (; j <= lookbackPeriods - simdWidth; j += simdWidth)
        {
            Vector<double> priceVector = new(source, startIndex + j);
            sumVector += priceVector;
        }

        // Scalar loop for remainder elements
        double sum = 0;
        for (; j < lookbackPeriods; j++)
        {
            sum += source[startIndex + j];
        }

        // Combine SIMD sum with scalar remainder
        sum += Vector.Dot(sumVector, Vector<double>.One);

        return sum / lookbackPeriods;
    }

    /// <summary>
    /// Validates the lookback periods parameter.
    /// </summary>
    /// <param name="lookbackPeriods">The number of lookback periods to validate.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are less than or equal to 0.</exception>
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for SMA.");
        }
    }
}
