namespace Skender.Stock.Indicators;

/// <summary>
/// Provides utility methods for Relative Strength Index (RSI) calculations.
/// </summary>
public static partial class Rsi
{
    /// <summary>
    /// Removes the recommended warmup periods from the RSI results.
    /// </summary>
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<RsiResult> RemoveWarmupPeriods(
        this IReadOnlyList<RsiResult> results)
    {
        ArgumentNullException.ThrowIfNull(results);

        int n = results
            .FindIndex(static x => x.Rsi != null);

        return results.Remove(10 * n);
    }

    /// <summary>
    /// Validates the parameters for RSI calculations.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the RSI calculation.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are less than 1.</exception>
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for RSI.");
        }
    }

    /// <summary>
    /// Computes the gain and loss between two values for RSI calculation.
    /// </summary>
    /// <param name="currentValue">The current value.</param>
    /// <param name="previousValue">The previous value.</param>
    /// <returns>A tuple containing the gain and loss values.</returns>
    internal static (double gain, double loss) ComputeGainLoss(double currentValue, double previousValue)
    {
        if (double.IsNaN(currentValue) || double.IsNaN(previousValue))
        {
            return (double.NaN, double.NaN);
        }

        double gain = currentValue > previousValue ? currentValue - previousValue : 0;
        double loss = currentValue < previousValue ? previousValue - currentValue : 0;
        return (gain, loss);
    }

    /// <summary>
    /// Calculates the RSI value from average gain and average loss.
    /// </summary>
    /// <param name="avgGain">The average gain.</param>
    /// <param name="avgLoss">The average loss.</param>
    /// <returns>The RSI value, or null if the calculation is not viable.</returns>
    internal static double? CalculateRsiValue(double avgGain, double avgLoss)
    {
        if (double.IsNaN(avgGain / avgLoss))
        {
            return null;
        }

        return avgLoss > 0 ? 100 - (100 / (1 + (avgGain / avgLoss))) : 100;
    }

    /// <summary>
    /// Applies Wilder's smoothing to update average gain and loss.
    /// </summary>
    /// <param name="avgGain">The current average gain.</param>
    /// <param name="avgLoss">The current average loss.</param>
    /// <param name="gain">The new gain value.</param>
    /// <param name="loss">The new loss value.</param>
    /// <param name="periods">The number of periods for smoothing.</param>
    /// <returns>A tuple containing the updated average gain and average loss.</returns>
    internal static (double avgGain, double avgLoss) ApplyWilderSmoothing(
        double avgGain,
        double avgLoss,
        double gain,
        double loss,
        int periods)
    {
        if (double.IsNaN(gain))
        {
            return (double.NaN, double.NaN);
        }

        double newAvgGain = ((avgGain * (periods - 1)) + gain) / periods;
        double newAvgLoss = ((avgLoss * (periods - 1)) + loss) / periods;
        return (newAvgGain, newAvgLoss);
    }
}
