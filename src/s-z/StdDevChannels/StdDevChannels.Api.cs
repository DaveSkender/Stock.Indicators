namespace Skender.Stock.Indicators;

// STANDARD DEVIATION CHANNELS (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<StdDevChannelsResult> GetStdDevChannels<TQuote>(
        this IEnumerable<TQuote> quotes,
        int? lookbackPeriods = 20,
        double stdDeviations = 2)
        where TQuote : IQuote => quotes
            .ToTuple(CandlePart.Close)
            .CalcStdDevChannels(lookbackPeriods, stdDeviations);

    // SERIES, from CHAIN
    public static IEnumerable<StdDevChannelsResult> GetStdDevChannels(
        this IEnumerable<IReusableResult> results,
        int? lookbackPeriods = 20,
        double stdDeviations = 2) => results
            .ToTuple()
            .CalcStdDevChannels(lookbackPeriods, stdDeviations)
            .SyncIndex(results, SyncType.Prepend);

    // SERIES, from TUPLE
    public static IEnumerable<StdDevChannelsResult> GetStdDevChannels(
        this IEnumerable<(DateTime, double)> priceTuples,
        int? lookbackPeriods = 20,
        double stdDeviations = 2) => priceTuples
            .ToSortedList()
            .CalcStdDevChannels(lookbackPeriods, stdDeviations);
}
