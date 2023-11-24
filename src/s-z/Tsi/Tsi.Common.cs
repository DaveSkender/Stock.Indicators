namespace Skender.Stock.Indicators;

// TRUE STRENGTH INDEX (COMMON)

public static class Tsi
{
    // parameter validation
    internal static void Validate(
        int lookbackPeriods,
        int smoothPeriods,
        int signalPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for TSI.");
        }

        if (smoothPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(smoothPeriods), smoothPeriods,
                "Smoothing periods must be greater than 0 for TSI.");
        }

        if (signalPeriods < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(signalPeriods), signalPeriods,
                "Signal periods must be greater than or equal to 0 for TSI.");
        }
    }

}
