using System.Collections.ObjectModel;

namespace Skender.Stock.Indicators;

// ENDPOINT MOVING AVERAGE (SERIES)
public static partial class Indicator
{
    // calculate series
    internal static Collection<EpmaResult> CalcEpma(
        this Collection<(DateTime, double)> tpColl,
        int lookbackPeriods)
    {
        // check parameter arguments
        ValidateEpma(lookbackPeriods);

        // initialize
        Collection<SlopeResult> slopeResults = tpColl
            .CalcSlope(lookbackPeriods);

        int length = slopeResults.Count;
        Collection<EpmaResult> results = new();

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            SlopeResult s = slopeResults[i];

            EpmaResult r = new(s.Date)
            {
                Epma = ((s.Slope * (i + 1)) + s.Intercept).NaN2Null()
            };

            results.Add(r);
        }

        return results;
    }

    // parameter validation
    private static void ValidateEpma(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Epma.");
        }
    }
}
