namespace Skender.Stock.Indicators;

// BOLLINGER BANDS (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IEnumerable<BollingerBandsResult> GetBollingerBands<T>(
        this IEnumerable<T> results,
        int lookbackPeriods = 20,
        double standardDeviations = 2)
        where T : IReusableResult
        => results
            .ToTupleResult()
            .CalcBollingerBands(lookbackPeriods, standardDeviations);

    // SERIES, from TUPLE
    public static IEnumerable<BollingerBandsResult> GetBollingerBands(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods = 20,
        double standardDeviations = 2) => priceTuples
            .ToSortedList()
            .CalcBollingerBands(lookbackPeriods, standardDeviations);
}
