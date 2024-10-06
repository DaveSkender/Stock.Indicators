namespace Skender.Stock.Indicators;

// WILLIAMS FRACTAL (COMMON)

public static class Fractal
{
    // parameter validation
    internal static void Validate(
        int windowSpan)
    {
        // check parameter arguments
        if (windowSpan < 2)
        {
            throw new ArgumentOutOfRangeException(nameof(windowSpan), windowSpan,
                "Window span must be at least 2 for Fractal.");
        }
    }

}
