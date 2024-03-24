namespace Skender.Stock.Indicators;

// ENDPOINT MOVING AVERAGE (SERIES)

public static partial class Indicator
{
    // calculate series
    internal static List<EpmaResult> CalcEpma(
        this List<(DateTime, double)> tpList,
        int lookbackPeriods)
    {
        // check parameter arguments
        Epma.Validate(lookbackPeriods);

        // initialize
        List<SlopeResult> slopeResults = tpList
            .CalcSlope(lookbackPeriods)
            .ToList();

        int length = slopeResults.Count;
        List<EpmaResult> results = new(length);

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            SlopeResult s = slopeResults[i];

            EpmaResult r = new() {
                Timestamp = s.Timestamp,
                Epma = ((s.Slope * (i + 1)) + s.Intercept).NaN2Null()
            };

            results.Add(r);
        }

        return results;
    }
}
