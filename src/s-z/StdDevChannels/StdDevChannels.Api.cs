namespace Skender.Stock.Indicators;

// STANDARD DEVIATION CHANNELS (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IEnumerable<StdDevChannelsResult> GetStdDevChannels<T>(
        this IEnumerable<T> results,
        int? lookbackPeriods = 20,
        double stdDeviations = 2)
        where T: IReusableResult
        => results
            .ToTupleResult()
            .CalcStdDevChannels(lookbackPeriods, stdDeviations);

    // SERIES, from TUPLE
    public static IEnumerable<StdDevChannelsResult> GetStdDevChannels(
        this IEnumerable<(DateTime, double)> priceTuples,
        int? lookbackPeriods = 20,
        double stdDeviations = 2) => priceTuples
            .ToSortedList()
            .CalcStdDevChannels(lookbackPeriods, stdDeviations);
}
