namespace Skender.Stock.Indicators;

// MOVING AVERAGE ENVELOPES (COMMON)

public static class MaEnvelopes
{
    // parameter validation
    internal static void Validate(
        double percentOffset)
    {
        // check parameter arguments
        if (percentOffset <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(percentOffset), percentOffset,
                "Percent Offset must be greater than 0 for Moving Average Envelopes.");
        }
    }

}
