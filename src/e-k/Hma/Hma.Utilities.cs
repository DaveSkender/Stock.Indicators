namespace Skender.Stock.Indicators;

/// <summary>
/// Provides utility methods for Hull Moving Average (HMA) calculations.
/// </summary>
public static partial class Hma
{
    /// <summary>
    /// Validates the lookback periods for HMA calculations.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the lookback periods are less than or equal to 1.
    /// </exception>
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 1 for HMA.");
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
        double[] values = buffer.ToArray();

        for (int i = 0; i < periods; i++)
        {
            wma += values[i] * (i + 1) / divisor;
        }

        return wma;
    }
}
