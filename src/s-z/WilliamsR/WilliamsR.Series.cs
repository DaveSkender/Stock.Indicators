using System.Collections.ObjectModel;

namespace Skender.Stock.Indicators;

// WILLIAM %R OSCILLATOR (SERIES
public static partial class Indicator
{
    internal static Collection<WilliamsResult> CalcWilliamsR(
        this Collection<QuoteD> qdList,
        int lookbackPeriods)
    {
        // check parameter arguments
        ValidateWilliam(lookbackPeriods);

        // convert Fast Stochastic to William %R
        Collection<WilliamsResult> results = new();

        Collection<StochResult> stoch = qdList
            .CalcStoch(lookbackPeriods, 1, 1, 3, 2, MaType.SMA);

        foreach (StochResult s in stoch)
        {
            results.Add(new WilliamsResult(s.Date)
            {
                WilliamsR = s.Oscillator - 100
            });
        }

        return results;
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
