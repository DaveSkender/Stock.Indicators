namespace Skender.Stock.Indicators;

/// <summary>
/// Provides utility methods for the Stochastic Oscillator.
/// </summary>
public static partial class Stoch
{
    /// <summary>
    /// Removes the recommended warmup periods from the results.
    /// </summary>
    /// <param name="results">The list of Stochastic Oscillator results.</param>
    /// <returns>A list of Stochastic Oscillator results with warmup periods removed.</returns>
    public static IReadOnlyList<StochResult> RemoveWarmupPeriods(
        this IReadOnlyList<StochResult> results)
    {
        int removePeriods = results
            .FindIndex(x => x.Oscillator != null);

        return results.Remove(removePeriods);
    }

    /// <summary>
    /// Validates the parameters for Stochastic Oscillator calculation.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods for the lookback.</param>
    /// <param name="signalPeriods">The number of periods for the signal line.</param>
    /// <param name="smoothPeriods">The number of periods for smoothing.</param>
    /// <param name="kFactor">The K factor for the Stochastic calculation.</param>
    /// <param name="dFactor">The D factor for the Stochastic calculation.</param>
    /// <param name="movingAverageType">The type of moving average to use.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when a parameter is out of range.</exception>
    internal static void Validate(
        int lookbackPeriods,
        int signalPeriods,
        int smoothPeriods,
        double kFactor,
        double dFactor,
        MaType movingAverageType)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Stochastic.");
        }

        if (signalPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(signalPeriods), signalPeriods,
                "Signal periods must be greater than 0 for Stochastic.");
        }

        if (smoothPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(smoothPeriods), smoothPeriods,
                "Smooth periods must be greater than 0 for Stochastic.");
        }

        if (kFactor <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(kFactor), kFactor,
                "kFactor must be greater than 0 for Stochastic.");
        }

        if (dFactor <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(dFactor), dFactor,
                "dFactor must be greater than 0 for Stochastic.");
        }

        if (movingAverageType is not MaType.SMA and not MaType.SMMA)
        {
            throw new ArgumentOutOfRangeException(nameof(dFactor), dFactor,
                "Stochastic only supports SMA and SMMA moving average types.");
        }
    }
}
