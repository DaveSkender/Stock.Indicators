namespace Skender.Stock.Indicators;

// BOLLINGER BANDS (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IReadOnlyList<BollingerBandsResult> GetBollingerBands<T>(
        this IEnumerable<T> results,
        int lookbackPeriods = 20,
        double standardDeviations = 2)
        where T : IReusable
        => results
            .ToSortedList()
            .CalcBollingerBands(lookbackPeriods, standardDeviations);
}
