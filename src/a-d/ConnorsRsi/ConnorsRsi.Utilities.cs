namespace Skender.Stock.Indicators;

// CONNORS RSI (UTILITIES)

public static partial class ConnorsRsi
{
    // parameter validation
    internal static void Validate(
        int rsiPeriods,
        int streakPeriods,
        int rankPeriods)
    {
        // check parameter arguments
        if (rsiPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(rsiPeriods), rsiPeriods,
                "RSI period for Close price must be greater than 1 for ConnorsRsi.");
        }

        if (streakPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(streakPeriods), streakPeriods,
                "RSI period for Streak must be greater than 1 for ConnorsRsi.");
        }

        if (rankPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(rankPeriods), rankPeriods,
                "Percent Rank periods must be greater than 1 for ConnorsRsi.");
        }
    }
}
