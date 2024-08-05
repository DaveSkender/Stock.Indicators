namespace Skender.Stock.Indicators;

// MOTHER of ADAPTIVE MOVING AVERAGES (COMMON)

public static class Mama
{
    // parameter validation
    internal static void Validate(
        double fastLimit,
        double slowLimit)
    {
        // check parameter arguments
        if (fastLimit <= slowLimit || fastLimit >= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(fastLimit), fastLimit,
                "Fast Limit must be greater than Slow Limit and less than 1 for MAMA.");
        }

        if (slowLimit <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(slowLimit), slowLimit,
                "Slow Limit must be greater than 0 for MAMA.");
        }
    }

}
