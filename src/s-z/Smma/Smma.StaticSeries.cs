namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Smoothed Moving Average (SMMA) for a series of data.
/// </summary>
public static partial class Smma
{
    /// <summary>
    /// Calculates the Smoothed Moving Average (SMMA) for a series of data.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source list, which must implement <see cref="IReusable"/>.</typeparam>
    /// <param name="source">The source list of data.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the SMMA calculation.</param>
    /// <returns>A list of <see cref="SmmaResult"/> containing the SMMA values.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the source list is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are less than 1.</exception>
    [Series("SMMA", "Smoothed Moving Average", Category.MovingAverage, ChartType.Overlay)]
    public static IReadOnlyList<SmmaResult> ToSmma<T>(
        this IReadOnlyList<T> source,
        [ParamNum<int>("Lookback Periods", 1, 250, 20)]
        int lookbackPeriods)
        where T : IReusable
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(source);
        Validate(lookbackPeriods);

        // initialize
        int length = source.Count;
        List<SmmaResult> results = new(length);
        double prevSmma = double.NaN;

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            T s = source[i];

            // skip incalculable periods
            if (i < lookbackPeriods - 1)
            {
                results.Add(new(s.Timestamp));
                continue;
            }

            double smma;

            // when no prior SMMA, reset as SMA
            if (double.IsNaN(prevSmma))
            {
                double sum = 0;
                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    T ps = source[p];
                    sum += ps.Value;
                }

                smma = sum / lookbackPeriods;
            }

            // normal SMMA
            else
            {
                smma = ((prevSmma * (lookbackPeriods - 1)) + s.Value) / lookbackPeriods;
            }

            results.Add(new SmmaResult(
                Timestamp: s.Timestamp,
                Smma: smma.NaN2Null()));

            prevSmma = smma;
        }

        return results;
    }
}
