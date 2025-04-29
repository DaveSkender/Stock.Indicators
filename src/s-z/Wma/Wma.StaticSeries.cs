namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the WMA (Weighted Moving Average) indicator.
/// </summary>
public static partial class Wma
{
    /// <summary>
    /// Calculates the WMA for a series of reusable data.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source list, which must implement IReusable.</typeparam>
    /// <param name="source">The source list of reusable data.</param>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
    /// <returns>A list of WmaResult containing the WMA values.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the source is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are less than or equal to 0.</exception>
    [Series("WMA", "Weighted Moving Average", Category.MovingAverage, ChartType.Overlay)]
    public static IReadOnlyList<WmaResult> ToWma<T>(
        this IReadOnlyList<T> source,
        [ParamNum<int>("Lookback Periods", 14, 1, 250)]
        int lookbackPeriods)
        where T : IReusable
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(source);
        Validate(lookbackPeriods);

        // initialize
        int length = source.Count;
        List<WmaResult> results = new(length);

        double divisor = (double)lookbackPeriods * (lookbackPeriods + 1) / 2d;

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            T s = source[i];

            double wma;

            if (i >= lookbackPeriods - 1)
            {
                wma = 0;
                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    T ps = source[p];
                    wma += ps.Value * (lookbackPeriods - (i + 1 - p - 1)) / divisor;
                }
            }
            else
            {
                wma = double.NaN;
            }

            results.Add(new(
                Timestamp: s.Timestamp,
                Wma: wma.NaN2Null()));
        }

        return results;
    }
}
