namespace Skender.Stock.Indicators;

// ARNAUD LEGOUX MOVING AVERAGE (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IEnumerable<AlmaResult> GetAlma<T>(
        this IEnumerable<T> results,
        int lookbackPeriods = 9,
        double offset = 0.85,
        double sigma = 6)
        where T: IReusableResult
        => results
            .ToTupleResult()
            .CalcAlma(lookbackPeriods, offset, sigma);

    // SERIES, from TUPLE
    public static IEnumerable<AlmaResult> GetAlma(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods = 9,
        double offset = 0.85,
        double sigma = 6) => priceTuples
            .ToSortedList()
            .CalcAlma(lookbackPeriods, offset, sigma);
}
