namespace Skender.Stock.Indicators;

// WILLIAMS ALLIGATOR (COMMON)

public partial class Alligator
{
    // string label
    public override string ToString()
        => $"ALLIGATOR({JawPeriods},{JawOffset},{TeethPeriods},{TeethOffset},{LipsPeriods},{LipsOffset})";

    // parameter validation
    internal static void Validate(
        int jawPeriods,
        int jawOffset,
        int teethPeriods,
        int teethOffset,
        int lipsPeriods,
        int lipsOffset)
    {
        // check parameter arguments
        if (jawPeriods <= teethPeriods)
        {
            throw new ArgumentOutOfRangeException(nameof(jawPeriods), jawPeriods,
                "Jaw lookback periods must be greater than Teeth lookback periods for Alligator.");
        }

        if (teethPeriods <= lipsPeriods)
        {
            throw new ArgumentOutOfRangeException(nameof(teethPeriods), teethPeriods,
                "Teeth lookback periods must be greater than Lips lookback periods for Alligator.");
        }

        if (lipsPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lipsPeriods), lipsPeriods,
                "Lips lookback periods must be greater than 0 for Alligator.");
        }

        if (jawOffset <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(jawOffset), jawOffset,
                "Jaw offset periods must be greater than 0 for Alligator.");
        }

        if (teethOffset <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(teethOffset), teethOffset,
                "Jaw offset periods must be greater than 0 for Alligator.");
        }

        if (lipsOffset <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lipsOffset), lipsOffset,
                "Jaw offset periods must be greater than 0 for Alligator.");
        }

        if (jawPeriods + jawOffset <= teethPeriods + teethOffset)
        {
            throw new ArgumentOutOfRangeException(nameof(jawPeriods), jawPeriods,
                "Jaw lookback + offset are too small for Alligator.");
        }

        if (teethPeriods + teethOffset <= lipsPeriods + lipsOffset)
        {
            throw new ArgumentOutOfRangeException(nameof(teethPeriods), teethPeriods,
                "Teeth lookback + offset are too small for Alligator.");
        }
    }
}
