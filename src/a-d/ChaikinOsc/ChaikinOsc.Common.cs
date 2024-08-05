namespace Skender.Stock.Indicators;

// CHAIKIN OSCILLATOR (COMMON)

public static class ChaikinOsc
{
    // parameter validation
    internal static void Validate(
        int fastPeriods,
        int slowPeriods)
    {
        // check parameter arguments
        if (fastPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(fastPeriods), fastPeriods,
                "Fast lookback periods must be greater than 0 for Chaikin Oscillator.");
        }

        if (slowPeriods <= fastPeriods)
        {
            throw new ArgumentOutOfRangeException(nameof(slowPeriods), slowPeriods,
                "Slow lookback periods must be greater than Fast lookback period for Chaikin Oscillator.");
        }
    }

}
