/// <summary>
/// Provides utility methods for Slope and Linear Regression calculations.
/// </summary>
public static partial class Slope
{
    /// <summary>
    /// Validates the lookback periods parameter.
    /// </summary>
    /// <param name="lookbackPeriods">The number of lookback periods to validate.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are less than or equal to 1.</exception>
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 1 for Slope/Linear Regression.");
        }
    }
}
