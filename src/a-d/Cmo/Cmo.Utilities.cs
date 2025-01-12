/// <summary>
/// Provides utility methods for the CMO (Chande Momentum Oscillator) indicator.
/// </summary>
public static partial class Cmo
{
    /// <summary>
    /// Validates the parameters for the CMO calculation.
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
                "Lookback periods must be greater than 0 for CMO.");
        }
    }
}
