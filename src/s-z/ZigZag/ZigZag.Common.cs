namespace Skender.Stock.Indicators;

// ZIG ZAG (COMMON)

public static class ZigZag
{
    // parameter validation
    internal static void Validate(
        decimal percentChange)
    {
        // check parameter arguments
        if (percentChange <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(percentChange), percentChange,
                "Percent change must be greater than 0 for ZIGZAG.");
        }
    }

}
