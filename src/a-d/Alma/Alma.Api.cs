namespace Skender.Stock.Indicators;

// ARNAUD LEGOUX MOVING AVERAGE (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    public static IReadOnlyList<AlmaResult> GetAlma<T>(
        this IReadOnlyList<T> results,
        int lookbackPeriods = 9,
        double offset = 0.85,
        double sigma = 6)
        where T : IReusable
        => results
            .ToSortedList()
            .CalcAlma(lookbackPeriods, offset, sigma);
}
