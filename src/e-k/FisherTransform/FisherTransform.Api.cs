namespace Skender.Stock.Indicators;

// FISHER TRANSFORM (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IReadOnlyList<FisherTransformResult> GetFisherTransform<T>(
        this IReadOnlyList<T> results,
        int lookbackPeriods = 10)
        where T : IReusable
        => results
            .ToSortedList(CandlePart.HL2)
            .CalcFisherTransform(lookbackPeriods);
}
