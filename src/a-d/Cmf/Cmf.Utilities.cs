/// <summary>
/// Provides utility methods for the CMF (Chaikin Money Flow) indicator.
/// </summary>
public static partial class Cmf
{
    /// <summary>
    /// Validates the parameters for the CMF calculation.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are less than or equal to 0.</exception>
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Chaikin Money Flow.");
        }
    }
}
