namespace Skender.Stock.Indicators;

// BALANCE OF POWER (COMMON)

public static class Bop
{
    // parameter validation
    internal static void Validate(
        int smoothPeriods)
    {
        // check parameter arguments
        if (smoothPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(smoothPeriods), smoothPeriods,
                "Smoothing periods must be greater than 0 for BOP.");
        }
    }

}
