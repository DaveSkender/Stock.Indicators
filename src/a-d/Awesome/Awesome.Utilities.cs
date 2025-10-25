namespace Skender.Stock.Indicators;

public static partial class Awesome
{
    /// <summary>
    /// parameter validation
    /// </summary>
    /// <param name="fastPeriods"></param>
    /// <param name="slowPeriods"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    internal static void Validate(
        int fastPeriods,
        int slowPeriods)
    {
        // check parameter arguments
        if (fastPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(slowPeriods), slowPeriods,
                "Fast periods must be greater than 0 for Awesome Oscillator.");
        }

        if (slowPeriods <= fastPeriods)
        {
            throw new ArgumentOutOfRangeException(nameof(slowPeriods), slowPeriods,
                "Slow periods must be larger than Fast Periods for Awesome Oscillator.");
        }
    }
}
