namespace Skender.Stock.Indicators;

// ROLLING PIVOT POINTS (COMMON)

public static class RollingPivots
{
    // parameter validation
    internal static void Validate(
        int windowPeriods,
        int offsetPeriods)
    {
        // check parameter arguments
        if (windowPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(windowPeriods), windowPeriods,
                "Window periods must be greater than 0 for Rolling Pivot Points.");
        }

        if (offsetPeriods < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offsetPeriods), offsetPeriods,
                "Offset periods must be greater than or equal to 0 for Rolling Pivot Points.");
        }
    }

}
