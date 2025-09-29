namespace Skender.Stock.Indicators;

/// <summary>
/// Provides utility methods for the WMA (Weighted Moving Average) indicator.
/// </summary>
public static partial class Wma
{
    // parameter validation
    /// <summary>
    /// Validates the parameters for the WMA calculation.
    /// </summary>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the lookback periods are less than or equal to 0.
    /// </exception>
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for WMA.");
        }
    }

    /// <summary>
    /// Updates the rolling buffer used for weighted moving average calculations.
    /// </summary>
    /// <param name="buffer">The buffer that stores recent values.</param>
    /// <param name="capacity">The maximum number of elements allowed in the buffer.</param>
    /// <param name="value">The new value to enqueue.</param>
    internal static void UpdateBuffer(Queue<double> buffer, int capacity, double value)
    {
        if (buffer.Count == capacity)
        {
            buffer.Dequeue();
        }

        buffer.Enqueue(value);
    }

    /// <summary>
    /// Calculates a weighted moving average from the supplied buffer.
    /// </summary>
    /// <param name="buffer">The buffer containing the values to average.</param>
    /// <param name="periods">The number of periods in the weighted average.</param>
    /// <param name="divisor">The divisor used to normalize the weighted sum.</param>
    /// <returns>The weighted moving average, or <c>null</c> if insufficient data is present.</returns>
    internal static double? ComputeWeightedMovingAverage(Queue<double> buffer, int periods, double divisor)
    {
        if (buffer.Count < periods)
        {
            return null;
        }

        double wma = 0d;
        int weight = 1;

        // Calculate exactly like static series: divide inside the loop for each term
        // This ensures precision matches the static series implementation
        foreach (double value in buffer)
        {
            wma += value * weight / divisor;
            weight++;

            if (weight > periods)
            {
                break;
            }
        }

        return wma;
    }
}
