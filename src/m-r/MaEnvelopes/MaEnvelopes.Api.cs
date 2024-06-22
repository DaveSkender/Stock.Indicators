namespace Skender.Stock.Indicators;

// MOVING AVERAGE ENVELOPES (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IEnumerable<MaEnvelopeResult> GetMaEnvelopes<T>(
        this IEnumerable<T> results,
        int lookbackPeriods,
        double percentOffset = 2.5,
        MaType movingAverageType = MaType.SMA)
        where T : IReusableResult
        => results
            .ToSortedList()
            .CalcMaEnvelopes(lookbackPeriods, percentOffset, movingAverageType);
}
