namespace Skender.Stock.Indicators;

// RENKO CHART - STANDARD (UTILITIES)

public static partial class Renko
{
    // parameter validation
    internal static void Validate(
        decimal brickSize)
    {
        // check parameter arguments
        if (brickSize <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(brickSize), brickSize,
                "Brick size must be greater than 0 for Renko Charts.");
        }
    }

}
