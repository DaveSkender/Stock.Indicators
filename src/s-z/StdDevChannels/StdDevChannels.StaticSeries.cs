namespace Skender.Stock.Indicators;

// STANDARD DEVIATION CHANNELS (SERIES)

public static partial class StdDevChannels
{
    public static IReadOnlyList<StdDevChannelsResult> ToStdDevChannels<T>(
        this IReadOnlyList<T> source,
        int? lookbackPeriods = 20,
        double stdDeviations = 2)
        where T : IReusable
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(source);
        Validate(lookbackPeriods, stdDeviations);

        // initialize
        lookbackPeriods ??= source.Count; // assume whole quotes when null
        int length = source.Count;

        IReadOnlyList<SlopeResult> slopeResults = source
            .ToSlope((int)lookbackPeriods);

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
