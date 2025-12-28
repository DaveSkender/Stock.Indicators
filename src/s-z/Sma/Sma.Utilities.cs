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
    }

    /// <summary>
    /// Calculates the sum of all values in a Queue buffer.
    /// Optimized for Queue&lt;double&gt; usage in BufferList implementations.
    /// </summary>
    /// <param name="buffer">The queue buffer containing values to sum.</param>
    /// <returns>Sum of all values in the buffer.</returns>
    internal static double Sum(this Queue<double> buffer)
    {
        double sum = 0;
        foreach (double val in buffer)
        {
            sum += val;
        }

        return sum;
    }

    /// <summary>
    /// Calculates the average of all values in a Queue buffer.
    /// Uses the buffer's count as the denominator for improved conciseness.
    /// </summary>
    /// <param name="buffer">The queue buffer containing values to average.</param>
    /// <returns>Average of all values in the buffer, or NaN if buffer is empty.</returns>
    internal static double Average(this Queue<double> buffer)
        => buffer.Count > 0 ? buffer.Sum() / buffer.Count : double.NaN;

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
