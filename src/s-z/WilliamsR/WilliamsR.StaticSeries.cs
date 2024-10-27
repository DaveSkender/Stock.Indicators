namespace Skender.Stock.Indicators;

// WILLIAM %R OSCILLATOR (SERIES)

public static partial class WilliamsR
{
    public static IReadOnlyList<WilliamsResult> ToWilliamsR<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods = 14)
        where TQuote : IQuote => quotes
            .ToQuoteDList()
            .CalcWilliamsR(lookbackPeriods);

    private static List<WilliamsResult> CalcWilliamsR(
        this IReadOnlyList<QuoteD> source,
        int lookbackPeriods)
    {
        // check parameter arguments
        Validate(lookbackPeriods);

        // convert Fast Stochastic to William %R
        return source.CalcStoch(lookbackPeriods, 1, 1, 3, 2, MaType.SMA)
            .Select(s => new WilliamsResult(
                Timestamp: s.Timestamp,
                WilliamsR: s.Oscillator - 100
             ))
            .ToList();
    }
}
