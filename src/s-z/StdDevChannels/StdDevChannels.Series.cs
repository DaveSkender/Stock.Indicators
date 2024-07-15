namespace Skender.Stock.Indicators;

// STANDARD DEVIATION CHANNELS

public static partial class Indicator
{
    private static List<StdDevChannelsResult> CalcStdDevChannels<T>(
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
        int length = source.Count;

        List<SlopeResult> slopeResults = source
            .CalcSlope((int)lookbackPeriods);

        List<StdDevChannelsResult> results = slopeResults
            .Select(x => new StdDevChannelsResult(x.Timestamp))
            .ToList();

        // roll through source values in reverse
        for (int i = length - 1; i >= lookbackPeriods - 1; i -= (int)lookbackPeriods)
        {
            SlopeResult s = slopeResults[i];
            double? width = stdDeviations * s.StdDev;

            // add regression line (y = mx + b) and channels
            for (int p = i - (int)lookbackPeriods + 1; p <= i; p++)
            {
                if (p < 0)
                {
                    continue;
                }

                StdDevChannelsResult d = results[p];

                double? c = (s.Slope * (p + 1)) + s.Intercept;

                // re-write record
                results[p] = d with {
                    Centerline = c,
                    UpperChannel = c + width,
                    LowerChannel = c - width,
                    BreakPoint = p == i - lookbackPeriods + 1
                };
            }
        }

        return results;
    }
}
