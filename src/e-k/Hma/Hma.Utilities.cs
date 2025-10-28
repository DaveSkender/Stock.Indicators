namespace Skender.Stock.Indicators;

/// <summary>
/// Provides utility methods for Hull Moving Average (HMA) calculations.
/// </summary>
public static partial class Hma
{
    /// <summary>
    /// Validates the lookback periods for HMA calculations.
    /// </summary>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
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
}
