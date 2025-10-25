namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the EPMA (Endpoint Moving Average) indicator.
/// </summary>
public static partial class Epma
{
    /// <summary>
    /// Converts a list of source data to EPMA results.
    /// </summary>
    /// <param name="source">The list of source data.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <returns>A list of EPMA results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the source list is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static IReadOnlyList<EpmaResult> ToEpma(
        this IReadOnlyList<IReusable> source,
        int lookbackPeriods = 10)
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
