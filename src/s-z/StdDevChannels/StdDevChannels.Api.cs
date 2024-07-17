namespace Skender.Stock.Indicators;

// STANDARD DEVIATION CHANNELS (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IReadOnlyList<StdDevChannelsResult> GetStdDevChannels<T>(
        this IEnumerable<T> results,
        int? lookbackPeriods = 20,
        double stdDeviations = 2)
        where T : IReusable
        => results
            .ToSortedList()
            .CalcStdDevChannels(lookbackPeriods, stdDeviations);
}
