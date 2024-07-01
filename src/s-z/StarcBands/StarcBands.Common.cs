namespace Skender.Stock.Indicators;

// STARC BANDS (COMMON)

public static class StarcBands
{
    // parameter validation
    internal static void Validate(
        int smaPeriods,
        double multiplier,
        int atrPeriods)
    {
        // check parameter arguments
        if (smaPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(smaPeriods), smaPeriods,
                "EMA periods must be greater than 1 for STARC Bands.");
        }

        if (atrPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(atrPeriods), atrPeriods,
                "ATR periods must be greater than 1 for STARC Bands.");
        }

        if (multiplier <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(multiplier), multiplier,
                "Multiplier must be greater than 0 for STARC Bands.");
        }
    }

}
