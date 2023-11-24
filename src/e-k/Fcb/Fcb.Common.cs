namespace Skender.Stock.Indicators;

// FRACTAL CHAOS BANDS (COMMON)

public static class Fcb
{
    // parameter validation
    internal static void Validate(
        int windowSpan)
    {
        // check parameter arguments
        if (windowSpan < 2)
        {
            throw new ArgumentOutOfRangeException(nameof(windowSpan), windowSpan,
                "Window span must be at least 2 for FCB.");
        }
    }

}
