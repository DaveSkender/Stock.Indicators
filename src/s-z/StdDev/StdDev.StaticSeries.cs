namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating standard deviation on a series of data.
/// </summary>
public static partial class StdDev
{
    /// <summary>
    /// Calculates the standard deviation for a series of data.
    /// </summary>
    /// <typeparam name="T">The type of the data series, which must implement IReusable.</typeparam>
    /// <param name="source">The source data series.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <returns>A list of StdDevResult containing the standard deviation, mean, and z-score for each data point.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the source is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when lookbackPeriods is less than 1.</exception>
    [Series("STDEV", "Standard Deviation", Category.PriceCharacteristic, ChartType.Oscillator)]
    public static IReadOnlyList<StdDevResult> ToStdDev<T>(
        this IReadOnlyList<T> source,

        [Param("Lookback Periods", 1, 250, 14)]
        int lookbackPeriods)
        where T : IReusable
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(source);
        Validate(lookbackPeriods);

        // initialize
        int length = source.Count;
        List<StdDevResult> results = new(length);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            T s = source[i];

            double mean;
            double stdDev;
            double zScore;

            if (i >= lookbackPeriods - 1)
            {
                double[] values = new double[lookbackPeriods];
                double sum = 0;
                int n = 0;

                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    T ps = source[p];
                    values[n] = ps.Value;
                    sum += ps.Value;
                    n++;
                }

                mean = sum / lookbackPeriods;

                stdDev = values.StdDev();

                zScore = stdDev == 0 ? double.NaN
                    : (s.Value - mean) / stdDev;
            }
            else
            {
                mean = double.NaN;
                stdDev = double.NaN;
                zScore = double.NaN;
            }

            StdDevResult r = new(
                Timestamp: s.Timestamp,
                StdDev: stdDev.NaN2Null(),
                Mean: mean.NaN2Null(),
                ZScore: zScore.NaN2Null());

            results.Add(r);
        }

        return results;
    }
}
