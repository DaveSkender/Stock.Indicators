/// <summary>
/// Provides utility methods for the ALMA (Arnaud Legoux Moving Average) indicator.
/// </summary>
public static partial class Alma
{
    /// <summary>
    /// Validates the parameters for the ALMA calculation.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <param name="offset">The offset parameter for the ALMA calculation, must be between 0 and 1.</param>
    /// <param name="sigma">The sigma parameter for the ALMA calculation, must be greater than 0.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the lookback periods are less than or equal to 1,
    /// the offset is not between 0 and 1, or the sigma is less than or equal to 0.
    /// </exception>
    internal static void Validate(
        int lookbackPeriods,
        double offset,
        double sigma)
    {
        // check parameter arguments
        if (lookbackPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 1 for ALMA.");
        }

        if (offset is < 0 or > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), offset,
                "Offset must be between 0 and 1 for ALMA.");
        }

        if (sigma <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(sigma), sigma,
                "Sigma must be greater than 0 for ALMA.");
        }
    }
}
