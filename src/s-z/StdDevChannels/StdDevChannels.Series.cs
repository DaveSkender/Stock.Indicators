namespace Skender.Stock.Indicators;

// STANDARD DEVIATION CHANNELS

public static partial class Indicator
{
    internal static List<StdDevChannelsResult> CalcStdDevChannels<T>(
        this List<T> source,
        int? lookbackPeriods,
        double stdDeviations)
        where T : IReusable
    {
        // assume whole quotes when lookback is null
        lookbackPeriods ??= source.Count;

        // check parameter arguments
        StdDevChannels.Validate(lookbackPeriods, stdDeviations);

        // initialize
        List<SlopeResult> slopeResults = source
            .CalcSlope((int)lookbackPeriods);

        int length = slopeResults.Count;
        List<StdDevChannelsResult> results = slopeResults
            .Select(x => new StdDevChannelsResult { Timestamp = x.Timestamp })
            .ToList();

        // roll through quotes in reverse
        for (int w = length - 1; w >= lookbackPeriods - 1; w -= (int)lookbackPeriods)
        {
            SlopeResult s = slopeResults[w];
            double? width = stdDeviations * s.StdDev;

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
}
