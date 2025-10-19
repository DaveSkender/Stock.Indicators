namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating Standard Deviation Channels.
/// </summary>
public static partial class StdDevChannels
{
    /// <summary>
    /// Converts a series of quotes to Standard Deviation Channels.
    /// </summary>
    /// <param name="source">The source series of quotes.</param>
    /// <param name="lookbackPeriods">
    /// The number of periods for the lookback. Default is 20.</param>
    /// <param name="stdDeviations">The number of standard deviations for the channel width. Default is 2.</param>
    /// <returns>A list of <see cref="StdDevChannelsResult"/> containing the Standard Deviation Channels values.</returns>
    public static IReadOnlyList<StdDevChannelsResult> ToStdDevChannels(
        this IReadOnlyList<IReusable> source,
        int lookbackPeriods = 20,
        double stdDeviations = 2)
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(source);
        Validate(lookbackPeriods, stdDeviations);

        // initialize
        int length = source.Count;

        IReadOnlyList<SlopeResult> slopeResults = source
            .ToSlope(lookbackPeriods);

        List<StdDevChannelsResult> results = slopeResults
            .Select(x => new StdDevChannelsResult(x.Timestamp))
            .ToList();

        // roll through source values in reverse
        for (int i = length - 1; i >= lookbackPeriods - 1; i -= lookbackPeriods)
        {
            SlopeResult s = slopeResults[i];
            double? width = stdDeviations * s.StdDev;

            // add regression line (y = mx + b) and channels
            for (int p = i - lookbackPeriods + 1; p <= i; p++)
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
