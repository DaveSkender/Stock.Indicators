namespace Skender.Stock.Indicators;

// MOVING AVERAGE ENVELOPES (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IReadOnlyList<MaEnvelopeResult> GetMaEnvelopes<T>(
        this IReadOnlyList<T> results,
        int lookbackPeriods,
        double percentOffset = 2.5,
        MaType movingAverageType = MaType.SMA)
        where T : IReusable
        => results
            .ToSortedList()
            .CalcMaEnvelopes(lookbackPeriods, percentOffset, movingAverageType);
}
