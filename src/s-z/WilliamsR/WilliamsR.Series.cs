namespace Skender.Stock.Indicators;

// WILLIAM %R OSCILLATOR (SERIES
public static partial class Indicator
{
    internal static List<WilliamsResult> CalcWilliamsR(
        this List<QuoteD> qdList,
        int lookbackPeriods)
    {
        // check parameter arguments
        ValidateWilliam(lookbackPeriods);

        // convert Fast Stochastic to William %R
        return qdList.CalcStoch(lookbackPeriods, 1, 1, 3, 2, MaType.SMA)
            .Select(s => new WilliamsResult(s.Date)
            {
                WilliamsR = s.Oscillator - 100
            })
            .ToList();
    }

    // parameter validation
    private static void ValidateWilliam(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for William %R.");
        }
    }
}
