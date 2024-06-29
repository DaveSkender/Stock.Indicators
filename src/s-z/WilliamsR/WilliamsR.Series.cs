namespace Skender.Stock.Indicators;

// WILLIAM %R OSCILLATOR (SERIES)

public static partial class Indicator
{
    private static List<WilliamsResult> CalcWilliamsR(
        this List<QuoteD> qdList,
        int lookbackPeriods)
    {
        // check parameter arguments
        WilliamsR.Validate(lookbackPeriods);

        // convert Fast Stochastic to William %R
        return qdList.CalcStoch(lookbackPeriods, 1, 1, 3, 2, MaType.Sma)
            .Select(s => new WilliamsResult {
                Timestamp = s.Timestamp,
                WilliamsR = s.Oscillator - 100
            })
            .ToList();
    }
}
