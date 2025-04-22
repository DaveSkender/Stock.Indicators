namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the McGinley Dynamic indicator.
/// </summary>
public static partial class MgDynamic
{
    /// <summary>
    /// Converts a list of source data to McGinley Dynamic results.
    /// </summary>
    /// <typeparam name="T">The type of the source data.</typeparam>
    /// <param name="source">The list of source data.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <param name="kFactor">The smoothing factor for the calculation.</param>
    /// <returns>A list of McGinley Dynamic results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the source list is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods or kFactor are invalid.</exception>
    [Series("DYNAMIC", "McGinley Dynamic", Category.MovingAverage, ChartType.Overlay)]
    public static IReadOnlyList<DynamicResult> ToDynamic<T>(
        this IReadOnlyList<T> source,
        [Param("Lookback Periods", 1, 250, 10)]
        int lookbackPeriods,
        double kFactor = 0.6)
        where T : IReusable
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(source);
        Validate(lookbackPeriods, kFactor);

        // initialize
        int length = source.Count;
        List<DynamicResult> results = new(length);

        // skip first period
        if (length > 0)
        {
            results.Add(new(source[0].Timestamp, null));
        }

        // roll through source values
        for (int i = 1; i < length; i++)
        {
            double? dyn = Increment(
                lookbackPeriods,
                kFactor,
                newVal: source[i].Value,
                prevDyn: results[i - 1].Dynamic ?? source[i - 1].Value
                ).NaN2Null();

            results.Add(new DynamicResult(
                Timestamp: source[i].Timestamp,
                Dynamic: dyn));
        }

        return results;
    }
}
