namespace Skender.Stock.Indicators;

// MARUBOZU (UTILITIES)

public static partial class Marubozu
{
    // parameter validation
    internal static void Validate(
        double minBodyPercent)
    {
        // check parameter arguments
        if (minBodyPercent > 100)
        {
            throw new ArgumentOutOfRangeException(
                nameof(minBodyPercent), minBodyPercent,
                "Minimum Body Percent must be less than 100 " +
                "for Marubozu (<=100%).");
        }

        if (minBodyPercent < 80)
        {
            throw new ArgumentOutOfRangeException(
                nameof(minBodyPercent), minBodyPercent,
                "Minimum Body Percent must at least 80 (80%) " +
                "for Marubozu and is usually greater than 90 (90%).");
        }
    }
}
