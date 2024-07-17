namespace Skender.Stock.Indicators;

// ENDPOINT MOVING AVERAGE (SERIES)

public static partial class Indicator
{
    // calculate series
    private static List<EpmaResult> CalcEpma<T>(
        this List<T> source,
        int lookbackPeriods)
        where T : IReusable
    {
        // check parameter arguments
        Epma.Validate(lookbackPeriods);

        // initialize
        int length = source.Count;
        List<EpmaResult> results = new(length);

        IReadOnlyList<SlopeResult> slope
            = source.CalcSlope(lookbackPeriods);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            results.Add(new EpmaResult(
                Timestamp: slope[i].Timestamp,
                Epma: ((slope[i].Slope * (i + 1)) + slope[i].Intercept).NaN2Null()));
        }

        return results;
    }
}
