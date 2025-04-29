namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating Standard Deviation Channels.
/// </summary>
public static partial class StdDevChannels
{
    /// <summary>
    /// Converts a series of quotes to Standard Deviation Channels.
    /// </summary>
    /// <typeparam name="T">The type of the quote, which must implement <see cref="IReusable"/>.</typeparam>
    /// <param name="source">The source series of quotes.</param>
    /// <param name="lookbackPeriods">
    /// The number of periods for the lookback. Default is 20.
    /// Spans all provided prices when <see langword="null"/>.</param>
    /// <param name="stdDeviations">The number of standard deviations for the channel width. Default is 2.</param>
    /// <returns>A list of <see cref="StdDevChannelsResult"/> containing the Standard Deviation Channels values.</returns>
    [Series("STDEV-CHANNELS", "Standard Deviation Channels", Category.PriceChannel, ChartType.Overlay)]
    public static IReadOnlyList<StdDevChannelsResult> ToStdDevChannels<T>(
        this IReadOnlyList<T> source,
        [ParamNum<int>("Lookback Periods", 20, 1, 250)]
        int? lookbackPeriods = 20,
        [ParamNum<double>("Standard Deviations", 2, 0.01, 10)]
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
