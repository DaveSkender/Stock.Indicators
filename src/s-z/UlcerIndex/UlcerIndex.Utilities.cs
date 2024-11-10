namespace Skender.Stock.Indicators;

/// <summary>
/// Provides utility methods for the Ulcer Index indicator.
/// </summary>
public static partial class UlcerIndex
{
    /// <summary>
    /// Validates the parameters for the Ulcer Index calculation.
    /// </summary>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are less than or equal to 0.</exception>
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Ulcer Index.");
        }
    }
}
