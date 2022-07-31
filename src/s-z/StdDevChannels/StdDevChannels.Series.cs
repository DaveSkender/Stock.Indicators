namespace Skender.Stock.Indicators;

// STANDARD DEVIATION CHANNELS
public static partial class Indicator
{
    internal static List<StdDevChannelsResult> CalcStdDevChannels(
        this List<(DateTime, double)> tpList,
        int? lookbackPeriods,
        double standardDeviations)
    {
        // assume whole quotes when lookback is null
        if (lookbackPeriods is null)
        {
            lookbackPeriods = tpList.Count;
        }

        // check parameter arguments
        ValidateStdDevChannels(lookbackPeriods, standardDeviations);

        // initialize
        List<SlopeResult> slopeResults = tpList
            .CalcSlope((int)lookbackPeriods);

        int length = slopeResults.Count;
        List<StdDevChannelsResult> results = slopeResults
            .Select(x => new StdDevChannelsResult(x.Date))
            .ToList();

        // roll through quotes in reverse
        for (int w = length - 1; w >= lookbackPeriods - 1; w -= (int)lookbackPeriods)
        {
            SlopeResult s = slopeResults[w];
            double? width = standardDeviations * s.StdDev;

            // add regression line (y = mx + b) and channels
            for (int p = w - (int)lookbackPeriods + 1; p <= w; p++)
            {
                if (p >= 0)
                {
                    StdDevChannelsResult d = results[p];
                    d.Centerline = (s.Slope * (p + 1)) + s.Intercept;
                    d.UpperChannel = d.Centerline + width;
                    d.LowerChannel = d.Centerline - width;

                    d.BreakPoint = p == w - lookbackPeriods + 1;
                }
            }
        }

        return results;
    }

    // parameter validation
    private static void ValidateStdDevChannels(
        int? lookbackPeriods,
        double standardDeviations)
    {
        // check parameter arguments
        if (lookbackPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 1 for Standard Deviation Channels.");
        }

        if (standardDeviations <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(standardDeviations), standardDeviations,
                "Standard Deviations must be greater than 0 for Standard Deviation Channels.");
        }
    }
}
