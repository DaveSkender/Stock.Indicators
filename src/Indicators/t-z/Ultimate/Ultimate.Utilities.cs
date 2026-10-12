namespace FacioQuo.Stock.Indicators;

/// <summary>
/// Provides utility methods for the Ultimate Oscillator indicator.
/// </summary>
public static partial class Ultimate
{
    // parameter validation
    /// <summary>
    /// Validates the parameters for the Ultimate Oscillator calculation.
    /// </summary>
    /// <param name="shortPeriods">Number of short lookback periods.</param>
    /// <param name="middlePeriods">Number of middle lookback periods.</param>
    /// <param name="longPeriods">Number of long lookback periods.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when any of the periods are less than or equal to 0, or when the periods are not in increasing order.
    /// </exception>
    internal static void Validate(
        int shortPeriods,
        int middlePeriods,
        int longPeriods)
    {
        // check parameter arguments
        if (shortPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(shortPeriods), shortPeriods,
                "Short periods must be greater than 0 for Ultimate Oscillator.");
        }

        if (middlePeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(middlePeriods), middlePeriods,
                "Middle periods must be greater than 0 for Ultimate Oscillator.");
        }

        if (longPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(longPeriods), longPeriods,
                "Long periods must be greater than 0 for Ultimate Oscillator.");
        }

        if (shortPeriods >= middlePeriods || middlePeriods >= longPeriods)
        {
            throw new ArgumentOutOfRangeException(nameof(middlePeriods), middlePeriods,
                "Short, middle, and long periods must be increasingly larger for Ultimate Oscillator.");
        }
    }
}
