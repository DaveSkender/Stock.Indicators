namespace Skender.Stock.Indicators;

// STANDARD DEVIATION CHANNELS (COMMON)

public static class StdDevChannels
{
    // parameter validation
    internal static void Validate(
        int? lookbackPeriods,
        double stdDeviations)
    {
        // check parameter arguments
        if (lookbackPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 1 for Standard Deviation Channels.");
        }

        if (stdDeviations <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(stdDeviations), stdDeviations,
                "Standard Deviations must be greater than 0 for Standard Deviation Channels.");
        }
    }

}
