namespace Skender.Stock.Indicators;

// WILLIAM %R OSCILLATOR (SERIES)

public static partial class Indicator
{
    internal static List<WilliamsResult> CalcWilliamsR(
        this List<QuoteD> qdList,
        int lookbackPeriods)
    {
        // check parameter arguments
        WilliamsR.Validate(lookbackPeriods);

        // convert Fast Stochastic to William %R
        return qdList.CalcStoch(lookbackPeriods, 1, 1, 3, 2, MaType.SMA)
            .Select(s => new WilliamsResult
            {
                TickDate = s.TickDate,
                WilliamsR = s.Oscillator - 100
            })
            .ToList();
    }
}
