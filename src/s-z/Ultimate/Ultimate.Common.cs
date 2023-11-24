namespace Skender.Stock.Indicators;

// ULTIMATE OSCILLATOR (COMMON)

public static class Ultimate
{
    // parameter validation
    internal static void Validate(
        int shortPeriods,
        int middleAverage,
        int longPeriods)
    {
        // check parameter arguments
        if (shortPeriods <= 0 || middleAverage <= 0 || longPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(longPeriods), longPeriods,
                "Average periods must be greater than 0 for Ultimate Oscillator.");
        }

        if (shortPeriods >= middleAverage || middleAverage >= longPeriods)
        {
            throw new ArgumentOutOfRangeException(nameof(middleAverage), middleAverage,
                "Average periods must be increasingly larger than each other for Ultimate Oscillator.");
        }
    }

}
