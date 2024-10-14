namespace Skender.Stock.Indicators;

// ENDPOINT MOVING AVERAGE (SERIES)

public static partial class Epma
{
    public static IReadOnlyList<EpmaResult> ToEpma<T>(
        this IReadOnlyList<T> source,
        int lookbackPeriods)
        where T : IReusable
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(source);
        Validate(lookbackPeriods);

        // initialize
        int length = source.Count;
        List<EpmaResult> results = new(length);

        IReadOnlyList<SlopeResult> slope
            = source.ToSlope(lookbackPeriods);

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
